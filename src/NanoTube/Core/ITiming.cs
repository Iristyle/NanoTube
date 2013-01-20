namespace NanoTube.Core
{
	using System;

	/// <summary>	A timing metric.  </summary>
	public interface ITiming : IMetric
	{
		/// <summary>	Gets the duration time value.  Units are not specified. </summary>
		/// <value>	The elapsed. </value>
		double Duration { get; }
	}
}