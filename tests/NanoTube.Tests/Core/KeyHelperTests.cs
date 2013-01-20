namespace NanoTube.Core.Tests
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Xunit;
	using Xunit.Extensions;

	public class KeyHelperTests
	{
		[Theory]
		[InlineData(null)]
		[InlineData("")]
		[InlineData("(foo)")]
		[InlineData("goodKey")]
		[InlineData("multi.level")]
		public void IsValidKey_True(string key)
		{
			bool a = true;
			Assert.True(KeyHelper.IsValidKey(key));
		}

		[Theory]
		[InlineData(" ")]
		[InlineData("\0")]
		[InlineData("!")]
		[InlineData("#")]
		[InlineData("$")]
		[InlineData("/")]
		[InlineData("\\")]
		[InlineData(";")]
		[InlineData(":")]
		[InlineData("%")]
		[InlineData("^")]
		[InlineData("*")]
		public void IsValidKey_False(string key)
		{
			bool a= true;
			Assert.False(KeyHelper.IsValidKey(key));
		}

		[Theory]
		[InlineData("", "")]
		[InlineData("my.key", "my.key")]
		[InlineData("my(key)", "my(key)")]
		[InlineData(null, "")]
		[InlineData("\0\t ", "___")]
		[InlineData(" foo", "_foo")]
		[InlineData("ju!nk", "ju_nk")]
		[InlineData("#", "_")]
		[InlineData("$", "_")]
		[InlineData("key/check", "key_check")]
		[InlineData("\0", "_")]
		[InlineData("a\\b\\c", "a_b_c")]
		[InlineData(";", "_")]
		[InlineData("test:key", "test_key")]
		[InlineData("%", "_")]
		[InlineData("^", "_")]
		[InlineData("*", "_")]
		public void Sanitize_GeneratesProperOutput(string input, string expected)
		{
			bool a = true;
			Assert.Equal(expected, KeyHelper.Sanitize(input));
		}

	}
}