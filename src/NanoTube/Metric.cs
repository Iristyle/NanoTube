namespace NanoTube
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Core;

	public static class Metric
	{
		/// <summary>	Generates a Counter metric. </summary>
		/// <exception cref="ArgumentException">	Thrown when the key contains invalid characters (essentially characters that are invalid on Linux file systems). </exception>
		/// <param name="key">   	The counter name. </param>
		/// <param name="adjustment">	The amount to add to or subtract from the counter value. </param>
		/// <returns>	A new Counter object. </returns>
		public static Counter Counter(string key, int adjustment)
		{
			if (!key.IsValidKey()) { throw new ArgumentException("Key contains invalid characters", "key"); }

			return new Counter() { Key = key, Adjustment = adjustment };
		}

		/// <summary>	Increments the given named counter by 1. </summary>
		/// <exception cref="ArgumentException">	Thrown when the key contains invalid characters (essentially characters that are invalid on Linux file systems). </exception>
		/// <param name="key">   	The counter name. </param>
		/// <returns>	A new Counter object. </returns>
		public static Counter Increment(string key)
		{
			if (!key.IsValidKey()) { throw new ArgumentException("Key contains invalid characters", "key"); }

			return new Counter() { Key = key, Adjustment = 1 };
		}

		/// <summary>	Decrements the given named counter by 1. </summary>
		/// <exception cref="ArgumentException">	Thrown when the key contains invalid characters (essentially characters that are invalid on Linux file systems). </exception>
		/// <param name="key">   	The counter name. </param>
		/// <returns>	A new Counter object. </returns>
		public static Counter Decrement(string key)
		{
			if (!key.IsValidKey()) { throw new ArgumentException("Key contains invalid characters", "key"); }

			return new Counter() { Key = key, Adjustment = -1 };
		}

		/// <summary>	Creates a Sample metric. </summary>
		/// <exception cref="ArgumentException">		  	Thrown when the key contains invalid characters (essentially characters that are
		/// 												invalid on Linux file systems). </exception>
		/// <exception cref="ArgumentOutOfRangeException">	Thrown when the sampling rate is greater than 1. </exception>
		/// <param name="key">			The key. </param>
		/// <param name="@value">   	The value. </param>
		/// <param name="frequency">	The frequency of the sampling that must be less than 1.  0.1 represents the value being sampled at 1/10th
		/// 							the rate for instance. </param>
		/// <returns>	A new Sample object. </returns>
		public static Sample Sample(string key, int @value, double frequency)
		{
			if (!key.IsValidKey()) { throw new ArgumentException("Key contains invalid characters", "key"); }
			if (frequency > 1) { throw new ArgumentOutOfRangeException("frequency", "Frequency must be < 1"); }

			return new Sample() { Key = key, Value = @value, Frequency = frequency };
		}

		/// <summary>	Creates a Timing metric. </summary>
		/// <exception cref="ArgumentException">	Thrown when the key contains invalid characters (essentially characters that are invalid on
		/// 										Linux file systems). </exception>
		/// <param name="key">	  	The key. </param>
		/// <param name="elapsed">	The elapsed time, which is not unit specific. </param>
		/// <returns>	A new Timing object. </returns>
		public static Timing Timing(string key, double elapsed)
		{
			if (!key.IsValidKey()) { throw new ArgumentException("Key contains invalid characters", "key"); }
			
			return new Timing() { Key = key, Elapsed = elapsed };
		}

		/// <summary>	Creates a KeyValue metric. </summary>
		/// <exception cref="ArgumentException">	Thrown when the key contains invalid characters (essentially characters that are invalid on
		/// 										Linux file systems). </exception>
		/// <param name="key">			The key. </param>
		/// <param name="@value">   	The value. </param>
		/// <param name="timeStamp">	The time at which the value was read.  Note that this has no bearing on StatsD and is only used by Statsite. </param>
		/// <returns>	A new KeyValue object. </returns>
		public static KeyValue KeyValue(string key, double @value, DateTime timeStamp)
		{
			if (!key.IsValidKey()) { throw new ArgumentException("Key contains invalid characters", "key"); }

			return new KeyValue() { Key = key, Value = @value, TimeStamp = timeStamp };
		}
	}
}