namespace NanoTube
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Core;

	/// <summary>	A simple factory for building Metric instances.  </summary>
	public static class Metric
	{
		/// <summary>	Generates a Counter metric. </summary>
		/// <remarks>	Keys with invalid characters (essentially characters that are invalid on Linux file systems) will be sanitized. </remarks>
		/// <exception cref="ArgumentNullException">	Thrown when key is null. </exception>
		/// <param name="key">		 	The counter name. </param>
		/// <param name="adjustment">	The amount to add to or subtract from the counter value. </param>
		/// <returns>	A new Counter object. </returns>
		public static ICounter Counter(string key, int adjustment)
		{
			if (null == key) { throw new ArgumentNullException("key"); }

			return new Counter() { Key = key.Sanitize(), Adjustment = adjustment };
		}

		/// <summary>	Increments the given named counter by 1. </summary>
		/// <remarks>	Keys with invalid characters (essentially characters that are invalid on Linux file systems) will be sanitized. </remarks>
		/// <exception cref="ArgumentNullException">	Thrown when key is null. </exception>
		/// <param name="key">   	The counter name. </param>
		/// <returns>	A new Counter object. </returns>
		public static ICounter Increment(string key)
		{
			if (null == key) { throw new ArgumentNullException("key"); }

			return new Counter() { Key = key.Sanitize(), Adjustment = 1 };
		}

		/// <summary>	Decrements the given named counter by 1. </summary>
		/// <remarks>	Keys with invalid characters (essentially characters that are invalid on Linux file systems) will be sanitized. </remarks>
		/// <exception cref="ArgumentNullException">	Thrown when key is null. </exception>
		/// <param name="key">   	The counter name. </param>
		/// <returns>	A new Counter object. </returns>
		public static ICounter Decrement(string key)
		{
			if (null == key) { throw new ArgumentNullException("key"); }

			return new Counter() { Key = key.Sanitize(), Adjustment = -1 };
		}

		/// <summary>	Creates a Sample metric. </summary>
		/// <remarks>	Keys with invalid characters (essentially characters that are invalid on Linux file systems) will be sanitized. </remarks>
		/// <exception cref="ArgumentNullException">	Thrown when key is null. </exception>
		/// <exception cref="ArgumentOutOfRangeException">	Thrown when the sampling rate is greater than 1. </exception>
		/// <param name="key">			The key. </param>
		/// <param name="value">   	The value. </param>
		/// <param name="frequency">	The frequency of the sampling that must be less than 1.  0.1 represents the value being sampled at 1/10th
		/// 							the rate for instance. </param>
		/// <returns>	A new Sample object. </returns>
		public static ISample Sample(string key, int @value, double frequency)
		{
			if (null == key) { throw new ArgumentNullException("key"); }
			if (frequency > 1) { throw new ArgumentOutOfRangeException("frequency", "Frequency must be < 1"); }

			return new Sample() { Key = key.Sanitize(), Value = @value, Frequency = frequency };
		}

		/// <summary>	Creates a Timing metric. </summary>
		/// <remarks>	Keys with invalid characters (essentially characters that are invalid on Linux file systems) will be sanitized. </remarks>
		/// <exception cref="ArgumentNullException">	Thrown when key is null. </exception>
		/// <param name="key">	  	The key. </param>
		/// <param name="elapsed">	The elapsed time, which is not unit specific. </param>
		/// <returns>	A new Timing object. </returns>
		public static ITiming Timing(string key, double elapsed)
		{
			if (null == key) { throw new ArgumentNullException("key"); }
			
			return new Timing() { Key = key.Sanitize(), Duration = elapsed };
		}

		/// <summary>	Creates a KeyValue metric. </summary>
		/// <remarks>	Keys with invalid characters (essentially characters that are invalid on Linux file systems) will be sanitized. </remarks>
		/// <exception cref="ArgumentNullException">	Thrown when key is null. </exception>
		/// <param name="key">			The key. </param>
		/// <param name="value">   	The value. </param>
		/// <param name="timestamp">	The time at which the value was read.  Note that this has no bearing on StatsD and is only used by Statsite. </param>
		/// <returns>	A new KeyValue object. </returns>
		public static IKeyValue KeyValue(string key, double @value, DateTime timestamp)
		{
			if (null == key) { throw new ArgumentNullException("key"); }

			return new KeyValue() { Key = key.Sanitize(), Value = @value, Timestamp = timestamp };
		}
	}
}