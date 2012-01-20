namespace NanoTube
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	/// <summary>	A set of options for how metrics should be represented when stringified.  </summary>
	public enum MetricFormat
	{
		/// <summary> Using the Etsy StatsD format <a href="https://github.com/etsy/statsd" />.  </summary>
		StatsD,

		/// <summary> Using the StatSite format <a href="https://github.com/kiip/statsite" />.  </summary>
		StatSite
	}
}