namespace NanoTube.Net.Tests
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Xunit;

	public class PacketBuilderTests
	{
		[Fact]
		public void ToMaximumBytePackets_AdheresToMaximum()
		{
			var bytes = PacketBuilder.ToMaximumBytePackets(new [] { Enumerable.Repeat("a", 512).ToString() }).ToArray();
			Assert.InRange(bytes[0].Length, 1, 512);
		}
	}
}
