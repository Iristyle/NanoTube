namespace NanoTube
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics.CodeAnalysis;
	using System.Linq;
	using System.Text;
	using System.Text.RegularExpressions;
	using NanoTube.Collections;

	/// <summary>	Performs some simple validation on given key values, and provides a method for sanitizing bad keys. </summary>
	public static class KeyHelper
	{
		private const string _badChars = "! \0\t;:/\\#%$^*";
		private readonly static SimpleObjectPool<StringBuilder> _builderPool 
			= new SimpleObjectPool<StringBuilder>(1, pool => new StringBuilder(200));
		private readonly static Regex _validKey = new Regex(@"^[^!\s;:/\\#%\$\^\*]+$", RegexOptions.Compiled);

		/// <summary>	A string extension method that query if 'key' is valid key. </summary>
		/// <param name="key">	The key to act on. </param>
		/// <returns>	true if valid key, false if not. </returns>
		public static bool IsValidKey(this string key)
		{
			return string.IsNullOrEmpty(key) || _validKey.IsMatch(key);
		}

		/// <summary>	A string extension method that will sanitize a key name so that it may be used with a metric. </summary>
		/// <param name="key">	The key to act on. </param>
		/// <returns>	A new string that has been sanitized if necessary, otherwise the original key . </returns>
		[SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "IsValidKey is validating key")]
		public static string Sanitize(this string key)
		{
			if (null == key) { return string.Empty; }
			if (IsValidKey(key)) { return key; }

			StringBuilder sanitized = null;

			try
			{
				sanitized = _builderPool.Pop();
				if (null != sanitized)
				{
					sanitized.Clear();
					sanitized.Append(key);
				}
				//autogrow pool as necessary
				else { sanitized = new StringBuilder(key); }

				for (int i = 0; i < key.Length; ++i)
				{
					if (_badChars.Contains(key[i])) { sanitized[i] = '_'; }
				}

				return sanitized.ToString();
			}
			finally
			{
				if (null != sanitized)
				{
					_builderPool.Push(sanitized);
				}
			}
		}
	}
}