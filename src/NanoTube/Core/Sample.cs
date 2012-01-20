namespace NanoTube.Core
{
	using System;

	/// <summary>	A sample metric that represents a counter or timing that has been sampled at the given frequency. </summary>
	/// <remarks>
	/// In StatSite, this is sent as an elapsed timer value and a sample rate.  In StatsD, this is considered a counter sample and a sample
	/// rate.
	/// </remarks>
	public struct Sample : IMetric
	{
		/// <summary>	Gets or sets the key name. </summary>
		/// <value>	The key. </value>
		public string Key { get; set; }
		
		/// <summary>	Gets or sets the value. </summary>
		/// <value>	The value. </value>
		public double Value { get; set; }
		
		/// <summary>	Gets or sets the frequency.  For instance, 0.1 means that only 1/10th of the time the value is sampled. </summary>
		/// <value>	The frequency. </value>
		public double Frequency { get; set; }
	}
}