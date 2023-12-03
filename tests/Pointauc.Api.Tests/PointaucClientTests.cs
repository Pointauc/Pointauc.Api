using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
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
#if NET8_0_OR_GREATER

		[TestMethod]
		public void MyTestMethod()
		{
			var bid = new BidsRequest() { bids = new List<Bid> { new Bid { Cost = 123, Message = "test" } } };

			var options = new JsonSerializerOptions
			{
				TypeInfoResolver = SourceGenerationContext.Default
			};

			var result = JsonSerializer.Serialize(bid, typeof(BidsRequest), options);
			Console.WriteLine(result);
		}
#endif
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
				Cost = 101,
				Message = nameof(Bids_WithDefaultParams_ShouldSuccess),
			};

			var bid2 = new Bid()
			{
				Cost = 102,
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
		public async Task Bids_WithEmpty_ShouldSuccess()
		{
			// Arrange
			var client = new PointaucClient(config.GetSection("Token").Value);

			// Act
			// Assert
			await client.Bids(Array.Empty<Bid>());
			client.Dispose();
		}

		#endregion Bids

		#region GetAllLots

		[TestMethod]
		[TestCategory(nameof(PointaucClient))]
		[TestCategory(nameof(PointaucClient.GetAllLots))]
		public async Task GetAllLots_WithActualLots_ShouldSuccess()
		{
			// Arrange
			var client = new PointaucClient(config.GetSection("Token").Value);

			var bid = new Bid()
			{
				Cost = 100,
				Message = nameof(GetAllLots_WithActualLots_ShouldSuccess),
			};

			// Act
			await client.Bids(bid);

			// The data is cached and updated every 5 seconds, so be aware that it may not always represent real-time data.
			await Task.Delay(TimeSpan.FromSeconds(5));

			var result = await client.GetAllLots();

			// Assert
			Assert.IsNotNull(result);
			Assert.AreNotEqual(0, result.Count);
			Assert.IsNotNull(result.FirstOrDefault(l => l.Name == bid.Message));

			client.Dispose();
		}

		#endregion GetAllLots

		#region ChangeLot

		[TestMethod]
		[TestCategory(nameof(PointaucClient))]
		[TestCategory(nameof(PointaucClient.ChangeLot))]
		public async Task ChangeLot_ByInvestorId_ShouldChangeLot()
		{
			// Arrange
			var client = new PointaucClient(config.GetSection("Token").Value);

			var bid = new Bid()
			{
				Cost = 100,
				Message = nameof(ChangeLot_ByInvestorId_ShouldChangeLot),
				InvestorId = nameof(ChangeLot_ByInvestorId_ShouldChangeLot),
			};

			// Act
			await client.Bids(bid);
			await client.ChangeLot(default, bid.InvestorId, new Lot() { Amount = 123, Name = $"{nameof(ChangeLot_ByInvestorId_ShouldChangeLot)}Changed" });

			// The data is cached and updated every 5 seconds, so be aware that it may not always represent real-time data.
			await Task.Delay(TimeSpan.FromSeconds(5));

			var result = await client.GetAllLots();

			// Assert
			Assert.IsNotNull(result);
			Assert.AreNotEqual(0, result.Count);
			Assert.IsNotNull(result.Where(l => l.Investors != null && l.Investors.Any(i => i == bid.InvestorId)).FirstOrDefault());
			Assert.AreEqual(123, result.Where(l => l.Investors != null && l.Investors.Any(i => i == bid.InvestorId)).FirstOrDefault().Amount);
			Assert.AreEqual($"{nameof(ChangeLot_ByInvestorId_ShouldChangeLot)}Changed", result.Where(l => l.Investors != null && l.Investors.Any(i => i == bid.InvestorId)).FirstOrDefault().Name);

			client.Dispose();
		}

		[TestMethod]
		[TestCategory(nameof(PointaucClient))]
		[TestCategory(nameof(PointaucClient.ChangeLot))]
		public async Task ChangeLot_WithNullQuery_ShouldThrow()
		{
			// Arrange
			var client = new PointaucClient(config.GetSection("Token").Value);

			// Act
			// Assert
			await Assert.ThrowsExceptionAsync<ArgumentException>(() => client.ChangeLot(default, default, new Lot()));
			client.Dispose();
		}

		[TestMethod]
		[TestCategory(nameof(PointaucClient))]
		[TestCategory(nameof(PointaucClient.ChangeLot))]
		public async Task ChangeLot_WithNullLot_ShouldThrow()
		{
			// Arrange
			var client = new PointaucClient(config.GetSection("Token").Value);

			// Act
			// Assert
			await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => client.ChangeLot(nameof(ChangeLot_WithNullLot_ShouldThrow), nameof(ChangeLot_WithNullLot_ShouldThrow), default));
			client.Dispose();
		}

		#endregion ChangeLot
	}
}