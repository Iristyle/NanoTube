namespace NanoTube.Configuration.Tests
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using Xunit;

	public class MetricPublishingConfigurationTests
	{
		[Fact]
		public void Constructor_Foos()
		{
			var config = new MetricPublishingConfiguration();
			config.Format = MetricFormat.StatsD;
		}
	}
}
