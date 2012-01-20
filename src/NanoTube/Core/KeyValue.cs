namespace NanoTube.Core
{
	using System;

	/// <summary>	Represents a Key/Value pair that is passed directly to Graphite. This has a very specific mapping in StatSite, but
	/// 			in StatsD is treated similarly to a Timing.  Timestamp has no mapping in StatsD.</summary>
	public struct KeyValue : IMetric
	{
		/// <summary>	Gets or sets the key name. </summary>
		/// <value>	The key. </value>
		public string Key { get; set; }
		
		/// <summary>	Gets or sets the value. </summary>
		/// <value>	The value. </value>
		public double Value { get; set; }
		
		/// <summary>	Gets or sets an optional Timestamp.  Meaningless to StatsD, but used by StatSite. </summary>
		/// <value>	The time stamp. </value>
		public DateTime? Timestamp { get; set; }

		/// <summary>	Returns the hash code for this instance. </summary>
		/// <returns>	A 32-bit signed integer that is the hash code for this instance. </returns>
		public override int GetHashCode()
		{
			return Key.GetHashCode() ^ Value.GetHashCode() ^ Timestamp.GetHashCode();
		}

		/// <summary>	Indicates whether this instance and a specified object are equal. </summary>
		/// <param name="obj">	The object to compare to this object. </param>
		/// <returns>
		/// true if <paramref name="obj" /> and this instance are the same type and represent the same value; otherwise, false.
		/// </returns>
		public override bool Equals(object obj)
		{
			if (!(obj is KeyValue))
				return false;

			return Equals((KeyValue)obj);
		}

		/// <summary>	Indicates whether this instance and a specified object are equal. </summary>
		/// <param name="other">	The counter to compare to this object. </param>
		/// <returns>
		/// true if <paramref name="other" /> and this instance are the same type and represent the same value; otherwise, false.
		/// </returns>
		public bool Equals(KeyValue other)
		{
			return Key == other.Key && Value == other.Value && Timestamp == other.Timestamp;
		}

		/// <summary>	Equality operator. </summary>
		/// <param name="keyValue1">	The first instance to compare. </param>
		/// <param name="keyValue2">	The second instance to compare. </param>
		/// <returns>	true if the parameters are considered equivalent. </returns>
		public static bool operator ==(KeyValue keyValue1, KeyValue keyValue2)
		{
			return keyValue1.Equals(keyValue2);
		}

		/// <summary>	Inequality operator. </summary>
		/// <param name="keyValue1">	The first instance to compare. </param>
		/// <param name="keyValue2">	The second instance to compare. </param>
		/// <returns>	true if the parameters are not considered equivalent. </returns>
		public static bool operator !=(KeyValue keyValue1, KeyValue keyValue2)
		{
			return !keyValue1.Equals(keyValue2);
		}
	}
}