namespace NanoTube.Net
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics.CodeAnalysis;
	using System.Linq;
	using System.Net;
	using System.Net.Sockets;
	using Collections;
	using Linq;

	/// <summary>
	/// A class that performs Udp communications with a remote host.  It can either Send metrics data all in one shot, or it can stream a
	/// specific number of packets at a time over an infinite IEnumerable.
	/// </summary>
	public class UdpMessenger : IDisposable
	{		
		private readonly static SimpleObjectPool<SocketAsyncEventArgs> _eventArgsPool 
			= new SimpleObjectPool<SocketAsyncEventArgs>(30, pool => new PoolAwareSocketAsyncEventArgs(pool));
		private readonly string _hostNameOrAddress;
		private readonly int _port;
		private readonly bool _throwExceptions;
		private readonly UdpClient _client;
		private bool _disposed;
		private readonly IPEndPoint _ipBasedEndpoint;

		/// <summary>	Initializes a new instance of the UdpMessenger class. </summary>
		/// <remarks>
		/// Will not throw if the host name is not a valid host or IP address. Will not throw if the metrics being sent through Send are null, or
		/// there are other problems parsing said metrics.  Will still throw if the metrics given to Stream are null.
		/// </remarks>
		/// <param name="hostNameOrAddress">	The DNS hostName or IPv4 or IPv6 address of the server. </param>
		/// <param name="port">					The server port. </param>
		/// <exception cref="ArgumentException">	Thrown when the hostNameOrAddress is null or whitespace. </exception>
		public UdpMessenger(string hostNameOrAddress, int port)
			: this(hostNameOrAddress, port, false)
		{ }

		/// <summary>	Initializes a new instance of the UdpMessenger class. </summary>
		/// <exception cref="ArgumentException">	Thrown when the hostNameOrAddress is null or whitespace. </exception>
		/// <param name="hostNameOrAddress">	The DNS hostName or IPv4 or IPv6 address of the server. </param>
		/// <param name="port">					The server port. </param>
		/// <param name="throwExceptions">  	true to throw exceptions if the given hostName cannot be resolved, if the metrics being passed to
		/// 									the Send operation are invalid, or if there are other problems parsing said metrics. </param>
		/// <exception cref="SocketException">	Thrown when the hostNameOrAddress is not an IP address and cannot be resolved, only if
		/// 										throwExceptions is set to true. </exception>
		public UdpMessenger(string hostNameOrAddress, int port, bool throwExceptions)
		{
			if (string.IsNullOrWhiteSpace(hostNameOrAddress)) { throw new ArgumentException("cannot be null or whitespace", "hostNameOrAddress"); }

			_hostNameOrAddress = hostNameOrAddress;
			_port = port;
			_throwExceptions = throwExceptions;

			_client = new UdpClient();
			_client.Client.SendBufferSize = 0;

			//if we were given an IP instead of a hostName, we can happily cache it off
			IPAddress address;
			if (IPAddress.TryParse(hostNameOrAddress, out address))
			{
				_ipBasedEndpoint = new IPEndPoint(address, port);
			}
			else if (throwExceptions)
			{
				//this will throw on bad input - fail fast
				Dns.GetHostAddresses(hostNameOrAddress);
			}
		}

		/// <summary>	Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources. </summary>
		public void Dispose()
		{
			if (!this._disposed)
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}
		}

		/// <summary>	Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources. </summary>
		/// <param name="disposing">	true if resources should be disposed, false if not. </param>
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (null != _client)
				{
					_client.Close();
				}
				this._disposed = true;
			}
		}

		/// <summary>	Sends all of the given metrics in the IEnumerable. </summary>
		/// <remarks>
		/// It's critical to realize that this method will materialize the entire set of metrics up-front, and then return. This method is *not*
		/// not appropriate for use in streaming from an infinite IEnumerable, unless the stream is first batched up using .Chunk().  Use
		/// StreamMetrics to let the messenger Chunk() on your behalf.
		/// </remarks>
		/// <exception cref="ArgumentNullException">	Thrown when the metrics are null, only if throwExceptions was set in the constructor. </exception>
		/// <exception cref="SocketException">	Thrown when the hostNameOrAddress is not an IP address and cannot be resolved, only if
		/// 										throwExceptions is set to true. </exception>
		/// <param name="metrics">	The metrics. </param>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification="This is one of the rare cases where eating exceptions is OK")]
		public void SendMetrics(IEnumerable<string> metrics)
		{
			if (_throwExceptions && null == metrics) { throw new ArgumentNullException("metrics"); }

			var data = _eventArgsPool.Pop();
			//firehose alert! -- keep it moving!
			if (null == data) { return; }

			try
			{
				data.RemoteEndPoint = _ipBasedEndpoint ?? new IPEndPoint(Dns.GetHostAddresses(_hostNameOrAddress)[0], _port); //only DNS resolve if we were given a hostname
				data.SendPacketsElements = metrics.ToMaximumBytePackets()
					.Select(bytes => new SendPacketsElement(bytes, 0, bytes.Length, true))
					.ToArray();

				//_client.Client.NoDelay = true;
				_client.Client.SendPacketsAsync(data);

				//Write-Debug "Wrote $(byteBlock.length) bytes to $server:$port"
			}
			//fire and forget, so just eat intermittent failures / exceptions
			catch
			{ 
				if (_throwExceptions) { throw; }
			}
		}

		/// <summary>	Streams the given metrics in the IEnumerable, terminating when the IEnumerable does. </summary>
		/// <remarks>
		/// This method will not materialize the entire set of metrics up-front, but will instead fill up to the full 512 bytes of a UDP packet,
		/// and will create 10 packets before sending.  It can run infinitely if the IEnumerable passed in is infinite. If there are no available
		/// SocketAsyncEventArgs in the pool, it will discard the current number of specified packets. This method is *not* appropriate if all
		/// metrics should be sent immediately in one shot. Use SendMetrics for that use case.
		/// </remarks>
		/// <exception cref="ArgumentNullException">	Thrown when the metrics are null. </exception>
		/// <exception cref="SocketException">	Thrown when the hostNameOrAddress is not an IP address and cannot be resolved, only if
		/// 										throwExceptions is set to true. </exception>
		/// <param name="metrics">	The metrics. </param>
		public void StreamMetrics(IEnumerable<string> metrics)
		{
			if (null == metrics) { throw new ArgumentNullException("metrics"); }

			StreamMetrics(metrics, 10);
		}

		/// <summary>	Streams the given metrics in the IEnumerable, terminating when the IEnumerable does. </summary>
		/// <remarks>
		/// This method will not materialize the entire set of metrics up-front, but will instead fill up to the full 512 bytes of a UDP packet,
		/// and will create the given number of packets specified before sending.  It can run infinitely if the IEnumerable passed in is
		/// infinite. If there are no available SocketAsyncEventArgs in the pool, it will discard the current number of specified packets. This
		/// method is *not* appropriate if all metrics should be sent immediately in one shot. Use SendMetrics for that use case.
		/// </remarks>
		/// <exception cref="ArgumentNullException">	Thrown when the metrics are null. </exception>
		/// <exception cref="SocketException">	Thrown when the hostNameOrAddress is not an IP address and cannot be resolved, only if
		/// 										throwExceptions is set to true. </exception>
		/// <param name="metrics">		 	The metrics. </param>
		/// <param name="packetsPerSend">	Size of the chunk. </param>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "This is one of the rare cases where eating exceptions is OK")]
		public void StreamMetrics(IEnumerable<string> metrics, int packetsPerSend)
		{
			if (null == metrics) { throw new ArgumentNullException("metrics"); }

			try
			{
				foreach (var chunk in metrics.ToMaximumBytePackets().Chunk(packetsPerSend))
				{
					var data = _eventArgsPool.Pop();
					//firehose alert! -- keep it moving!
					if (null == data) { continue; }

					try
					{
						data.RemoteEndPoint = _ipBasedEndpoint ?? new IPEndPoint(Dns.GetHostAddresses(_hostNameOrAddress)[0], _port);
						data.SendPacketsElements = chunk
							.Select(bytes => new SendPacketsElement(bytes, 0, bytes.Length, true))
							.ToArray();

						_client.Client.SendPacketsAsync(data);

						//Write-Debug "Wrote $(byteBlock.length) bytes to $server:$port"
					}
					//fire and forget, so just eat intermittent send / dns resolution failures if applicable OR other exceptions, unless instructed to throw
					catch
					{
						if (_throwExceptions) { throw; }
					}
				}
			}
			catch
			{
				//problems with the current chunk
				if (_throwExceptions) { throw; }
			}
		}
	}
}