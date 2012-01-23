namespace NanoTube.Core
{
	using System;

	/// <summary>	Represents a Key/Value pair that is passed directly to Graphite. This has a very specific mapping in StatSite, but
	/// 			in StatsD is treated similarly to a Timing.  Timestamp has no mapping in StatsD.</summary>
	public interface IKeyValue : IMetric
	{
		/// <summary>	Gets the value. </summary>
		/// <value>	The value. </value>
		double Value { get; }

		/// <summary>	Gets an optional Timestamp.  Meaningless to StatsD, but used by StatSite. </summary>
		/// <value>	The time stamp. </value>
		DateTime? Timestamp { get; }
	}
}