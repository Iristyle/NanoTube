﻿namespace NanoTube
{
	using System;
	using System.Collections.Concurrent;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Globalization;
	using System.Linq;
	using Collections;
	using Configuration;
	using Core;
	using Net;

	/// <summary>	A metric publishing client that supports pushing over Udp to either a StatsD or StatSite listener. </summary>
	/// <remarks>
	/// The client has two modes of operation.  There are some static methods that use object pools for efficiency / performance behind the
	/// scenes and put no responsibility on the user to clean-up resources. 
	/// 
	/// If more fine-grained control of resources are needed, a new Client may
	/// be created that will generate a new UdpMessenger class, and therefore must be disposed of after use.
	/// </remarks>
	public class MetricClient : IDisposable
	{
		private readonly static ConcurrentDictionary<string, SimpleObjectPool<UdpMessenger>> _messengerPool
			= new ConcurrentDictionary<string, SimpleObjectPool<UdpMessenger>>();

		private readonly UdpMessenger _messenger;
		private readonly IMetricPublishingConfiguration _configuration;
		private bool _disposed;

		/// <summary>	Initializes a new instance of the MetricClient class. </summary>
		/// <exception cref="ArgumentNullException">	Thrown when the configuration object is null. </exception>
		/// <param name="configuration">	The configuration. </param>
		public MetricClient(IMetricPublishingConfiguration configuration)
		{
			if (null == configuration) { throw new ArgumentNullException("configuration"); }
			_configuration = configuration;

			if (string.IsNullOrEmpty(configuration.HostNameOrAddress)) { throw new ArgumentException("HostNameOrAddress cannot be null or empty", "configuration"); }
			if (!configuration.PrefixKey.IsValidKey()) { throw new ArgumentException("PrefixKey contains invalid characters", "configuration"); }

			_messenger = new UdpMessenger(configuration.HostNameOrAddress, configuration.Port, configuration.ThrowExceptions);
		}

		/// <summary>	Initializes a new instance of the MetricClient class. </summary>
		/// <exception cref="ArgumentException">	Thrown when the hostNameOrAddress is null or empty OR the key contains invalid characters. </exception>
		/// <param name="hostNameOrAddress">	The DNS hostName or IPv4 or IPv6 address of the server. </param>
		/// <param name="port">					The port. </param>
		/// <param name="format">				Describes the metric format to use. </param>
		/// <param name="key">					The optional key to prefix metrics with. </param>
		/// <param name="throwExceptions">  	true to throw exceptions. </param>
		public MetricClient(string hostNameOrAddress, int port, MetricFormat format, string key, bool throwExceptions)
		{
			if (string.IsNullOrEmpty(hostNameOrAddress)) { throw new ArgumentException("cannot be null or empty", "hostNameOrAddress"); }
			if (!key.IsValidKey()) { throw new ArgumentException("contains invalid characters", "key"); }

			_configuration = new MetricPublishingConfiguration() { HostNameOrAddress = hostNameOrAddress, Port = port, Format = format, PrefixKey = key };
			_messenger = new UdpMessenger(hostNameOrAddress, port, throwExceptions);
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
				if (null != _messenger)
				{
					_messenger.Dispose();
				}
				this._disposed = true;
			}
		}

		/// <summary>
		/// Will send the given metrics in the specified format.  The IEnumerable will be materialized, and all data will be sent together
		/// asynchronously.  This call is not appropriate if the IEnumerable is infinite.
		/// </summary>
		/// <exception cref="ArgumentNullException">	Thrown when the configuration or metrics are null. </exception>
		/// <exception cref="ArgumentException">		Thrown when the hostNameOrAddress is null or empty OR the key contains invalid characters. </exception>
		/// <exception cref="ArgumentNullException">	Thrown when the metrics are null. </exception>
		/// <param name="configuration">	The configuration. </param>
		/// <param name="metrics">			The metrics. </param>
		public static void Send(IMetricPublishingConfiguration configuration, IEnumerable<IMetric> metrics)
		{
			if (null == configuration) { throw new ArgumentNullException("configuration"); }

			if (string.IsNullOrEmpty(configuration.HostNameOrAddress)) { throw new ArgumentException("HostNameOrAddress cannot be null or empty", "configuration"); }
			if (!configuration.PrefixKey.IsValidKey()) { throw new ArgumentException("PrefixKey contains invalid characters", "configuration"); }

			if (null == metrics) { throw new ArgumentNullException("metrics"); }

			SendToServer(configuration.HostNameOrAddress, configuration.Port, configuration.ThrowExceptions, metrics.ToStrings(configuration.PrefixKey, configuration.Format), false);
		}

		/// <summary>
		/// Will stream the given metrics in the specified format, breaking up the metrics into 10 packets at a time, where multiple metrics may
		/// comprise a single packet.  This call is appropriate for infinite IEnumerables.
		/// </summary>
		/// <exception cref="ArgumentNullException">	Thrown when the configuration or metrics are null. </exception>
		/// <exception cref="ArgumentException">		Thrown when the hostNameOrAddress is null or empty OR the key contains invalid characters. </exception>
		/// <exception cref="ArgumentNullException">	Thrown when the metrics are null. </exception>
		/// <param name="configuration">	The configuration. </param>
		/// <param name="metrics">			The metrics. </param>
		public static void Stream(IMetricPublishingConfiguration configuration, IEnumerable<IMetric> metrics)
		{
			if (null == configuration) { throw new ArgumentNullException("configuration"); }
			if (null == metrics) { throw new ArgumentNullException("metrics"); }

			if (string.IsNullOrEmpty(configuration.HostNameOrAddress)) { throw new ArgumentException("HostNameOrAddress cannot be null or empty", "configuration"); }
			if (!configuration.PrefixKey.IsValidKey()) { throw new ArgumentException("PrefixKey contains invalid characters", "configuration"); }
			if (null == metrics) { throw new ArgumentNullException("metrics"); }

			SendToServer(configuration.HostNameOrAddress, configuration.Port, configuration.ThrowExceptions, metrics.ToStrings(configuration.PrefixKey, configuration.Format), true);
		}

		/// <summary>	Times a given Action and reports it as a Timing metrics to the server. </summary>
		/// <remarks>	Exceptions generated by the Action are not handled. </remarks>
		/// <exception cref="ArgumentNullException">	Thrown when the configuration or action are null. </exception>
		/// <exception cref="ArgumentException">		Thrown when the hostNameOrAddress is null or empty OR the key contains invalid characters. </exception>
		/// <exception cref="ArgumentNullException">	Thrown when the action is null. </exception>
		/// <param name="configuration">	The configuration. </param>
		/// <param name="action">			The action. </param>
		public static void Time(IMetricPublishingConfiguration configuration, Action action)
		{
			if (null == configuration) { throw new ArgumentNullException("configuration"); }


			if (null == action) { throw new ArgumentNullException("action"); }

			if (string.IsNullOrEmpty(configuration.HostNameOrAddress)) { throw new ArgumentException("HostNameOrAddress cannot be null or empty", "configuration"); }
			if (!configuration.PrefixKey.IsValidKey()) { throw new ArgumentException("PrefixKey contains invalid characters", "configuration"); }
			if (null == action) { throw new ArgumentNullException("action"); }

			Stopwatch timer = null;
			try
			{
				timer = new Stopwatch();
				timer.Start();
				action();
			}
			finally
			{
				if (null != timer)
				{
					timer.Stop();
					SendToServer(configuration.HostNameOrAddress, configuration.Port, configuration.ThrowExceptions, new[] { Metric.Timing(configuration.PrefixKey, timer.Elapsed.TotalSeconds).ToString(null, configuration.Format) }, false);
				}
			}
		}

		private static void SendToServer(string server, int port, bool throwExceptions, IEnumerable<string> metrics, bool stream)
		{
			UdpMessenger messenger = null;
			SimpleObjectPool<UdpMessenger> serverPool = null;

			try
			{
				serverPool = _messengerPool.GetOrAdd(string.Format(CultureInfo.InvariantCulture, "{0}:{1}", server, port),
					new SimpleObjectPool<UdpMessenger>(3, pool => new UdpMessenger(server, port, throwExceptions)));
				messenger = serverPool.Pop();

				//all used up, sorry!
				if (null == messenger) { return; }

				if (stream)
				{
					messenger.StreamMetrics(metrics);
				}
				else
				{
					messenger.SendMetrics(metrics);
				}
			}
			catch
			{
				if (throwExceptions) { throw; }
			}
			finally
			{
				if (null != serverPool && null != messenger) { serverPool.Push(messenger); }
			}
		}

		/// <summary>	Will send the given metric in the specified format. </summary>
		/// <exception cref="ArgumentNullException">	Thrown when the metrics is null, only if ThrowExceptions is true. </exception>
		/// <param name="metric">	The metric. </param>
		public void Send(IMetric metric)
		{
			if (_configuration.ThrowExceptions && null == metric) { throw new ArgumentNullException("metric"); }

			Send(new [] { metric });
		}

		/// <summary>
		/// Will send the given metrics in the specified format.  The IEnumerable will be materialized, and all data will be sent together
		/// asynchronously.  This call is not appropriate if the IEnumerable is infinite.
		/// </summary>
		/// <exception cref="ArgumentNullException">	Thrown when the metrics are null, only if ThrowExceptions is true. </exception>
		/// <param name="metrics">	The metrics. </param>
		public void Send(IEnumerable<IMetric> metrics)
		{
			if (_configuration.ThrowExceptions && null == metrics) { throw new ArgumentNullException("metrics"); }

			try
			{
				_messenger.SendMetrics(metrics.ToStrings(_configuration.PrefixKey, _configuration.Format));
			}
			catch
			{
				if (_configuration.ThrowExceptions) { throw; }
			}
		}

		/// <summary>
		/// Will stream the given metrics in the specified format, breaking up the metrics into 10 packets at a time, where multiple metrics may
		/// comprise a single packet.  This call is appropriate for infinite IEnumerables.
		/// </summary>
		/// <exception cref="ArgumentNullException">	Thrown when the metrics are null. </exception>
		/// <param name="metrics">	The metrics. </param>
		public void Stream(IEnumerable<IMetric> metrics)
		{
			if (null == metrics) { throw new ArgumentNullException("metrics"); }

			_messenger.StreamMetrics(metrics.ToStrings(_configuration.PrefixKey, _configuration.Format));
		}
	}
}