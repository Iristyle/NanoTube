namespace NanoTube.Core
{
	using System;

	/// <summary>	A sample metric that represents a counter or timing that has been sampled at the given frequency. </summary>
	/// <remarks>
	/// In StatSite, this is sent as an elapsed timer value and a sample rate.  In StatsD, this is considered a counter sample and a sample
	/// rate.
	/// </remarks>
	public interface ISample : IMetric
	{
		/// <summary>
		/// Gets the value.  In the case of StatSite this will be treated as a Duration value.  In the case of StatsD this will be
		/// treated as a Counter Adjustment value.
		/// </summary>
		/// <value>	The value. </value>
		double Value { get; }

		/// <summary>	Gets the frequency.  For instance, 0.1 means that only 1/10th of the time the value is sampled. </summary>
		/// <value>	The frequency. </value>
		double Frequency { get; }
	}
}