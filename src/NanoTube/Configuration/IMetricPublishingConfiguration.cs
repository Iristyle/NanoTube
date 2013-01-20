namespace NanoTube.Configuration
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	/// <summary>	Interface for metric publishing configuration.  </summary>
	public interface IMetricPublishingConfiguration
	{
		/// <summary>	Gets the server DNS hostName or IPv4 or IPv6 address. </summary>
		/// <value>	The server hostName or address. </value>
		string HostNameOrAddress { get; }
		
		/// <summary>	Gets the port. </summary>
		/// <value>	The port. </value>
		int Port { get; }
		
		/// <summary>	Gets the prefix key. </summary>
		/// <value>	The prefix key. </value>
		string PrefixKey { get; }

		/// <summary>	Gets the format to use. </summary>
		/// <value>	The format. </value>
		MetricFormat Format { get; }

		/// <summary>	Gets a value indicating whether exceptions will be thrown from the internals. </summary>
		/// <value>	true to throw exceptions, false if not. </value>
		bool ThrowExceptions { get; }
	}
}