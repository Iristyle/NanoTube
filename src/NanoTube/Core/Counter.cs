namespace NanoTube.Core
{
	using System;
	/// <summary>	Represents a Counter metric adjustment.  </summary>
	internal struct Counter : ICounter
	{
		/// <summary>	Gets or sets the key name. </summary>
		/// <value>	The key. </value>
		public string Key { get; set; }

		/// <summary>	Gets or sets the value by which the named counter should be adjusted. </summary>
		/// <value>	The adjustment. </value>
		public int Adjustment { get; set; }

		/// <summary>	Returns the hash code for this instance. </summary>
		/// <returns>	A 32-bit signed integer that is the hash code for this instance. </returns>
		public override int GetHashCode()
		{
			return Key.GetHashCode() ^ Adjustment;
		}

		/// <summary>	Indicates whether this instance and a specified object are equal. </summary>
		/// <param name="obj">	Another object to compare to. </param>
		/// <returns>
		/// true if <paramref name="obj" /> and this instance are the same type and represent the same value; otherwise, false.
		/// </returns>
		public override bool Equals(object obj)
		{
			if (!(obj is Counter))
				return false;

			return Equals((Counter)obj);
		}

		/// <summary>	Indicates whether this instance and a specified object are equal. </summary>
		/// <param name="other">	The counter to compare to this object. </param>
		/// <returns>
		/// true if <paramref name="other" /> and this instance are the same type and represent the same value; otherwise, false.
		/// </returns>
		public bool Equals(Counter other)
		{
			return Key == other.Key && Adjustment == other.Adjustment;
		}

		/// <summary>	Equality operator. </summary>
		/// <param name="counter1">	The first instance to compare. </param>
		/// <param name="counter2">	The second instance to compare. </param>
		/// <returns>	true if the parameters are considered equivalent. </returns>
		public static bool operator ==(Counter counter1, Counter counter2)
		{
			return counter1.Equals(counter2);
		}

		/// <summary>	Inequality operator. </summary>
		/// <param name="counter1">	The first instance to compare. </param>
		/// <param name="counter2">	The second instance to compare. </param>
		/// <returns>	true if the parameters are not considered equivalent. </returns>
		public static bool operator !=(Counter counter1, Counter counter2)
		{
			return !counter1.Equals(counter2);
		}
	}
}