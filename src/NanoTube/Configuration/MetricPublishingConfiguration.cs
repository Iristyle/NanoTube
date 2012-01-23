namespace NanoTube.Configuration
{
	using System;
	using System.Collections.Generic;
	using System.Configuration;
	using System.Linq;

	/// <summary>	Metric publishing configuration.  </summary>
	public class MetricPublishingConfiguration : ConfigurationSection, IMetricPublishingConfiguration
	{
		/// <summary>	Reads from the default configuration section named nanoTubePublishing. </summary>
		/// <returns>	An IMetricPublishingConfiguration instance. </returns>
		public static IMetricPublishingConfiguration FromConfig()
		{
			return (MetricPublishingConfiguration)ConfigurationManager.GetSection("nanoTubePublishing");
		}

		/// <summary>	Reads from the a custom named configuration section. </summary>
		/// <param name="section">	The section name. </param>
		/// <returns>	An IMetricPublishingConfiguration instance. </returns>
		public static IMetricPublishingConfiguration FromConfig(string section)
		{
			return (MetricPublishingConfiguration)ConfigurationManager.GetSection(section);
		}

		/// <summary>	Gets or sets the server DNS hostName or IPv4 or IPv6 address. </summary>
		/// <value>	The server hostName or address. </value>
		[ConfigurationProperty("hostNameOrAddress", IsRequired = true)]
		public string HostNameOrAddress 
		{
			get { return (string)this["hostNameOrAddress"]; }
			set { this["hostNameOrAddress"] = value; }
		}

		/// <summary>	Gets or sets the port. </summary>
		/// <value>	The port. </value>
		[ConfigurationProperty("port", DefaultValue = 8125, IsRequired = false)]
		public int Port 
		{
			get { return (int)this["port"]; }
			set { this["port"] = value; }
		}

		/// <summary>	Gets or sets the prefix key. </summary>
		/// <value>	The prefix key. </value>
		[ConfigurationProperty("prefixKey", DefaultValue = "", IsRequired = false)]
		[RegexStringValidator(@"^[^!\s;:/\.\(\)\\#%\$\^]+$|^$")]
		public string PrefixKey 
		{
			get { return (string)this["prefixKey"]; }
			set { this["prefixKey"] = value; }
		}

		/// <summary>	Gets or sets the format to use. </summary>
		/// <value>	The format. </value>
		[ConfigurationProperty("format", IsRequired = true)]
		public MetricFormat Format
		{
			get { return (MetricFormat)this["format"]; }
			set { this["format"] = value; }
		}

		/// <summary>	Gets a value indicating whether exceptions will be thrown from the internals. </summary>
		/// <value>	true to throw exceptions, false if not. </value>
		[ConfigurationProperty("throwExceptions", IsRequired = false, DefaultValue = false)]
		public bool ThrowExceptions
		{
			get { return (bool)this["throwExceptions"]; }
			set { this["throwExceptions"] = value; }
		}
	}
}