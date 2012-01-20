namespace NanoTube.Core
{
	using System;

	/// <summary>	Represents a Key/Value pair that is passed directly to Graphite. This has a very specific mapping in StatSite, but
	/// 			in StatsD is treated similarly to a Timing.  TimeStamp has no mapping in StatsD.</summary>
	public struct KeyValue : IMetric
	{
		/// <summary>	Gets or sets the key name. </summary>
		/// <value>	The key. </value>
		public string Key { get; set; }
		
		/// <summary>	Gets or sets the value. </summary>
		/// <value>	The value. </value>
		public double Value { get; set; }
		
		/// <summary>	Gets or sets an optional TimeStamp.  Meaningless to StatsD, but used by StatSite. </summary>
		/// <value>	The time stamp. </value>
		public DateTime? TimeStamp { get; set; }
	}
}