namespace NanoTube.Core
{
	using System;
	
	/// <summary>	A sample metric that represents a counter or timing that has been sampled at the given frequency. </summary>
	/// <remarks>
	/// In StatSite, this is sent as an elapsed timer value and a sample rate.  In StatsD, this is considered a counter sample and a sample
	/// rate.
	/// </remarks>
	internal struct Sample : ISample
	{
		/// <summary>	Gets or sets the key name. </summary>
		/// <value>	The key. </value>
		public string Key { get; set; }

		/// <summary>
		/// Gets or sets the value.  In the case of StatSite this will be treated as a Duration value.  In the case of StatsD this will be
		/// treated as a Counter Adjustment value.
		/// </summary>
		/// <value>	The value. </value>
		public double Value { get; set; }

		/// <summary>	Gets or sets the frequency.  For instance, 0.1 means that only 1/10th of the time the value is sampled. </summary>
		/// <value>	The frequency. </value>
		public double Frequency { get; set; }

		/// <summary>	Returns the hash code for this instance. </summary>
		/// <returns>	A 32-bit signed integer that is the hash code for this instance. </returns>
		public override int GetHashCode()
		{
			return Key.GetHashCode() ^ Value.GetHashCode() ^ Frequency.GetHashCode();
		}

		/// <summary>	Indicates whether this instance and a specified object are equal. </summary>
		/// <param name="obj">	The object to compare to this object. </param>
		/// <returns>
		/// true if <paramref name="obj" /> and this instance are the same type and represent the same value; otherwise, false.
		/// </returns>
		public override bool Equals(object obj)
		{
			if (!(obj is Sample))
				return false;

			return Equals((Sample)obj);
		}

		/// <summary>	Indicates whether this instance and a specified object are equal. </summary>
		/// <param name="other">	The counter to compare to this object. </param>
		/// <returns>
		/// true if <paramref name="other" /> and this instance are the same type and represent the same value; otherwise, false.
		/// </returns>
		public bool Equals(Sample other)
		{
			return Key == other.Key && Value == other.Value && Frequency == other.Frequency;
		}

		/// <summary>	Equality operator. </summary>
		/// <param name="sample1">	The first instance to compare. </param>
		/// <param name="sample2">	The second instance to compare. </param>
		/// <returns>	true if the parameters are considered equivalent. </returns>
		public static bool operator ==(Sample sample1, Sample sample2)
		{
			return sample1.Equals(sample2);
		}

		/// <summary>	Inequality operator. </summary>
		/// <param name="sample1">	The first instance to compare. </param>
		/// <param name="sample2">	The second instance to compare. </param>
		/// <returns>	true if the parameters are not considered equivalent. </returns>
		public static bool operator !=(Sample sample1, Sample sample2)
		{
			return !sample1.Equals(sample2);
		}
	}
}