using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
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
		public async Task Bids_WithDefaultParam_ShouldReturnId()
		{
			// Arrange
			var client = new PointaucClient(config.GetSection("Token").Value);

			var bid = new Bid()
			{
				Cost = 100,
				Message = nameof(Bids_WithDefaultParam_ShouldReturnId),
				InsertStrategy = InsertStrategy.None
			};

			// Act
			var result = await client.Bids(bid);

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(1, result.Count);
			CollectionAssert.AllItemsAreNotNull(result);
			CollectionAssert.DoesNotContain(result, string.Empty);

			client.Dispose();
		}

		[TestMethod]
		[TestCategory(nameof(PointaucClient))]
		[TestCategory(nameof(PointaucClient.Bids))]
		public async Task Bids_WithDefaultParams_ShouldReturnId()
		{
			// Arrange
			var client = new PointaucClient(config.GetSection("Token").Value);

			var bid1 = new Bid()
			{
				Cost = 101,
				Message = nameof(Bids_WithDefaultParams_ShouldReturnId),
				InsertStrategy = InsertStrategy.None,
			};

			var bid2 = new Bid()
			{
				Cost = 102,
				Message = nameof(Bids_WithDefaultParams_ShouldReturnId),
				InsertStrategy = InsertStrategy.None,
			};

			// Act
			var result = await client.Bids(bid1, bid2);

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(2, result.Count);
			CollectionAssert.AllItemsAreNotNull(result);
			CollectionAssert.DoesNotContain(result, string.Empty);

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
		public async Task Bids_WithEmpty_ShouldNotReturnId()
		{
			// Arrange
			var client = new PointaucClient(config.GetSection("Token").Value);

			// Act
			var result = await client.Bids(Array.Empty<Bid>());

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(0, result.Count);

			client.Dispose();
		}

		#endregion Bids

		#region GetAllLots

		[TestMethod]
		[TestCategory(nameof(PointaucClient))]
		[TestCategory(nameof(PointaucClient.GetAllLots))]
		public async Task GetAllLots_WithActualLots_ShouldReturnLots()
		{
			// Arrange
			var client = new PointaucClient(config.GetSection("Token").Value);

			var bid = new Bid()
			{
				Cost = 100,
				Message = nameof(GetAllLots_WithActualLots_ShouldReturnLots),
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

		#region GetBidStatus

		[TestMethod]
		[TestCategory(nameof(PointaucClient))]
		[TestCategory(nameof(PointaucClient.GetBidStatus))]
		public async Task GetBidStatus_WithoutLot_ShouldReturnOnlyStatus()
		{
			// Arrange
			var client = new PointaucClient(config.GetSection("Token").Value);

			var bid = new Bid()
			{
				Cost = 100,
				Message = nameof(GetBidStatus_WithoutLot_ShouldReturnOnlyStatus),
				InsertStrategy = InsertStrategy.None
			};

			// Act
			var result = await client.Bids(bid);
			var status = await client.GetBidStatus(result.First());

			// Assert
			Assert.IsNotNull(status);
			Assert.AreEqual(Status.Pending, status.Status);
			Assert.IsNull(status.Lot);

			client.Dispose();
		}

		[TestMethod]
		[TestCategory(nameof(PointaucClient))]
		[TestCategory(nameof(PointaucClient.GetBidStatus))]
		public async Task GetBidStatus_WithLot_ShouldReturnStatusWithLot()
		{
			// Arrange
			var client = new PointaucClient(config.GetSection("Token").Value);

			var bid = new Bid()
			{
				Cost = 100,
				Message = nameof(GetBidStatus_WithLot_ShouldReturnStatusWithLot),
				InsertStrategy = InsertStrategy.Force
			};

			// Act
			var result = await client.Bids(bid);
			var status = await client.GetBidStatus(result.First());

			// Assert
			Assert.IsNotNull(status);
			Assert.AreEqual(Status.Pending, status.Status);
			Assert.IsNotNull(status.Lot);
			Assert.IsNotNull(status.Lot.Id);
			Assert.AreNotEqual(string.Empty, status.Lot.Id);

			client.Dispose();
		}

		#endregion GetBidStatus
	}
}