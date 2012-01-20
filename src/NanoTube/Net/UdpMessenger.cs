namespace NanoTube.Net
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Net.Sockets;
	using System.Net;
	using Collections;
	using Linq;

	/// <summary>
	/// A class that performs Udp communications with a remote host.  It can either Send metrics data all in one shot, or it can stream a
	/// specific number of packets at a time over an infinite IEnumerable.
	/// </summary>
	/// <remarks>	1/19/2012. </remarks>
	public class UdpMessenger : IDisposable
	{		
		private readonly static SimpleObjectPool<SocketAsyncEventArgs> _eventArgsPool 
			= new SimpleObjectPool<SocketAsyncEventArgs>(30, pool => new PoolAwareSocketAsyncEventArgs(pool));
		private readonly int _port;
		private readonly string _hostname;
		private readonly UdpClient _client;
		private bool _disposed;

		/// <summary>
		/// Initializes a new instance of the UdpMessenger class.
		/// </summary>
		/// <param name="server"></param>
		/// <param name="port"></param>
		public UdpMessenger(string hostname, int port)
		{
			_hostname = hostname;
			_port = port;
			_client = new UdpClient();
			_client.Client.SendBufferSize = 0;
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
		/// <param name="metrics">	The metrics. </param>
		public void SendMetrics(IEnumerable<string> metrics)
		{
			var data = _eventArgsPool.Pop();
			//firehose alert! -- keep it moving!
			if (null == data) { return; }

			try
			{
				data.RemoteEndPoint = new IPEndPoint(Dns.GetHostAddresses(_hostname)[0], _port);
				data.SendPacketsElements = metrics.ToMaximumBytePackets()
					.Select(bytes => new SendPacketsElement(bytes, 0, bytes.Length, true))
					.ToArray();

				//_client.Client.NoDelay = true;
				_client.Client.SendPacketsAsync(data);

				//Write-Debug "Wrote $(byteBlock.length) bytes to $server:$port"
			}
			//fire and forget, so just eat intermittent failures / exceptions
			catch
			{ }
		}

		/// <summary>	Streams the given metrics in the IEnumerable, terminating when the IEnumerable does. </summary>
		/// <remarks>
		/// This method will not materialize the entire set of metrics up-front, but will instead fill up to the full 512 bytes of a UDP packet,
		/// and will create 10 packets before sending.  It can run infinitely if the IEnumerable passed in is infinite. If there are no available
		/// SocketAsyncEventArgs in the pool, it will discard the current number of specified packets. This method is *not* appropriate if all
		/// metrics should be sent immediately in one shot. Use SendMetrics for that use case.
		/// </remarks>
		/// <param name="metrics">	The metrics. </param>
		public void StreamMetrics(IEnumerable<string> metrics)
		{
			StreamMetrics(metrics, 10);
		}

		/// <summary>	Streams the given metrics in the IEnumerable, terminating when the IEnumerable does. </summary>
		/// <remarks>
		/// This method will not materialize the entire set of metrics up-front, but will instead fill up to the full 512 bytes of a UDP packet,
		/// and will create the given number of packets specified before sending.  It can run infinitely if the IEnumerable passed in is
		/// infinite. If there are no available SocketAsyncEventArgs in the pool, it will discard the current number of specified packets. This
		/// method is *not* appropriate if all metrics should be sent immediately in one shot. Use SendMetrics for that use case.
		/// </remarks>
		/// <param name="metrics">		 	The metrics. </param>
		/// <param name="packetsPerSend">	Size of the chunk. </param>
		public void StreamMetrics(IEnumerable<string> metrics, int packetsPerSend)
		{
			foreach (var chunk in metrics.ToMaximumBytePackets().Chunk(packetsPerSend))
			{
				var data = _eventArgsPool.Pop();
				//firehose alert! -- keep it moving!
				if (null == data) { continue; }

				try
				{
					data.RemoteEndPoint = new IPEndPoint(Dns.GetHostAddresses(_hostname)[0], _port);
					data.SendPacketsElements = chunk
						.Select(bytes => new SendPacketsElement(bytes, 0, bytes.Length, true))
						.ToArray();

					_client.Client.SendPacketsAsync(data);

					//Write-Debug "Wrote $(byteBlock.length) bytes to $server:$port"
				}
				//fire and forget, so just eat intermittent failures / exceptions
				catch
				{ }
			}
		}
	}
}