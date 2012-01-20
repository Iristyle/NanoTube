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

		/// <summary>	Returns the hash code for this instance. </summary>
		/// <returns>	A 32-bit signed integer that is the hash code for this instance. </returns>
		public override int GetHashCode()
		{
			return Key.GetHashCode() ^ Duration.GetHashCode();
		}

		/// <summary>	Indicates whether this instance and a specified object are equal. </summary>
		/// <param name="obj">	The object to compare to this object. </param>
		/// <returns>
		/// true if <paramref name="obj" /> and this instance are the same type and represent the same value; otherwise, false.
		/// </returns>
		public override bool Equals(object obj)
		{
			if (!(obj is Timing))
				return false;

			return Equals((Timing)obj);
		}

		/// <summary>	Indicates whether this instance and a specified object are equal. </summary>
		/// <param name="other">	The counter to compare to this object. </param>
		/// <returns>
		/// true if <paramref name="other" /> and this instance are the same type and represent the same value; otherwise, false.
		/// </returns>
		public bool Equals(Timing other)
		{
			return Key == other.Key && Duration == other.Duration;
		}

		/// <summary>	Equality operator. </summary>
		/// <param name="timing1">	The first instance to compare. </param>
		/// <param name="timing2">	The second instance to compare. </param>
		/// <returns>	true if the parameters are considered equivalent. </returns>
		public static bool operator ==(Timing timing1, Timing timing2)
		{
			return timing1.Equals(timing2);
		}

		/// <summary>	Inequality operator. </summary>
		/// <param name="timing1">	The first instance to compare. </param>
		/// <param name="timing2">	The second instance to compare. </param>
		/// <returns>	true if the parameters are not considered equivalent. </returns>
		public static bool operator !=(Timing timing1, Timing timing2)
		{
			return !timing1.Equals(timing2);
		}
	}
}