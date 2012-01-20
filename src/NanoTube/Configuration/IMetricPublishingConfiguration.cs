namespace NanoTube.Configuration
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	/// <summary>	Interface for metric publishing configuration.  </summary>
	public interface IMetricPublishingConfiguration
	{
		/// <summary>	Gets the server hostName. </summary>
		/// <value>	The server hostName. </value>
		string HostName { get; }
		
		/// <summary>	Gets the port. </summary>
		/// <value>	The port. </value>
		int Port { get; }
		
		/// <summary>	Gets the prefix key. </summary>
		/// <value>	The prefix key. </value>
		string PrefixKey { get; }

		/// <summary>	Gets the format to use. </summary>
		/// <value>	The format. </value>
		MetricFormat Format { get; }
	}
}