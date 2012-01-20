namespace NanoTube.Core
{
	using System;

	/// <summary>	Represents a Counter metric adjustment.  </summary>
	public struct Counter : IMetric
	{
		/// <summary>	Gets or sets the key name. </summary>
		/// <value>	The key. </value>
		public string Key { get; set; }

		/// <summary>	Gets or sets the value by which the named counter should be adjusted. </summary>
		/// <value>	The adjustment. </value>
		public int Adjustment { get; set; }
	}
}