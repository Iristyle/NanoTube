namespace NanoTube.Core
{
	using System;
	
	/// <summary>	Interface for a metric - mostly a marker interface.  </summary>
	public interface IMetric
	{
		/// <summary>	Gets or sets the key name. </summary>
		/// <value>	The key. </value>
		string Key { get; set; }
	}
}