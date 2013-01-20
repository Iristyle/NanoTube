namespace NanoTube.Core
{
	using System;

	/// <summary>	A Counter metric adjustment.  </summary>
	public interface ICounter : IMetric
	{
		/// <summary>	Gets the value by which the named counter should be adjusted. </summary>
		/// <value>	The adjustment. </value>
		int Adjustment { get; }
	}
}