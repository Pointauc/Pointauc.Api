namespace Pointauc.Api
{
	public class Bid
	{
		/// <summary>
		/// Card color, only work when isDonation=false. Accepts only HEX format.
		/// </summary>
		public string Color { get; set; }

		public int Cost { get; set; }

		/// <summary>
		/// Unique identifier.
		/// </summary>
		public string Id { get; set; }

		/// <summary>
		/// Unique user identifier who made this bid. It will be stored in a lot data and can be used later for search and filtering.
		/// </summary>
		public string InvestorId { get; set; }

		/// <summary>
		/// Adds donation styles to the card. Cost will be multiplied by currency/points ratio.
		/// </summary>
		public bool? IsDonation { get; set; }

		public string Message { get; set; }

		/// <summary>
		/// Time at which bid was made. If not passed - sends current time to the client.
		/// </summary>
		public string Timestamp { get; set; }

		public string Username { get; set; }
	}
}