using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace Pointauc.Api.Tests
{
	[TestClass]
	public class PointaucClientTests
	{
		private readonly IConfigurationRoot config;

		public PointaucClientTests()
		{
			config = new ConfigurationBuilder().AddUserSecrets<PointaucClientTests>().Build();
		}

		#region Bids

		[TestMethod]
		[TestCategory(nameof(PointaucClient))]
		[TestCategory(nameof(PointaucClient.Bids))]
		public async Task Bids_WithDefaultParam_ShouldSuccess()
		{
			// Arrange
			var client = new PointaucClient(config.GetSection("Token").Value);

			var bid = new Bid()
			{
				Cost = 100,
				Message = nameof(Bids_WithDefaultParams_ShouldSuccess),
			};

			// Act
			// Assert
			await client.Bids(bid);
			client.Dispose();
		}

		[TestMethod]
		[TestCategory(nameof(PointaucClient))]
		[TestCategory(nameof(PointaucClient.Bids))]
		public async Task Bids_WithDefaultParams_ShouldSuccess()
		{
			// Arrange
			var client = new PointaucClient(config.GetSection("Token").Value);

			var bid1 = new Bid()
			{
				Cost = 100,
				Message = nameof(Bids_WithDefaultParams_ShouldSuccess),
			};

			var bid2 = new Bid()
			{
				Cost = 100,
				Message = nameof(Bids_WithDefaultParams_ShouldSuccess),
			};

			// Act
			// Assert
			await client.Bids(bid1, bid2);
			client.Dispose();
		}

		[TestMethod]
		[TestCategory(nameof(PointaucClient))]
		[TestCategory(nameof(PointaucClient.Bids))]
		public async Task Bids_WithNull_ShouldThrow()
		{
			// Arrange
			var client = new PointaucClient(config.GetSection("Token").Value);

			// Act
			// Assert
			await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => client.Bids(null));
			client.Dispose();
		}

		[TestMethod]
		[TestCategory(nameof(PointaucClient))]
		[TestCategory(nameof(PointaucClient.Bids))]
		public async Task Bids_WithEmpty_ShouldThrow()
		{
			// Arrange
			var client = new PointaucClient(config.GetSection("Token").Value);

			// Act
			// Assert
			await client.Bids(Array.Empty<Bid>());
			client.Dispose();
		}

		#endregion Bids
	}
}