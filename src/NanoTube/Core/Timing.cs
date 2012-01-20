namespace NanoTube.Core
{
	using System;

	/// <summary>	Represents a timing metric.  </summary>
	public struct Timing : IMetric
	{
		/// <summary>	Gets or sets the key name. </summary>
		/// <value>	The key. </value>
		public string Key { get; set; }
		
		/// <summary>	Gets or sets the duration time value.  Units are not specified. </summary>
		/// <value>	The elapsed. </value>
		public double Duration { get; set; }
	}
}