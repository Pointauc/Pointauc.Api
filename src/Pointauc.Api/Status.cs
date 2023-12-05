namespace Pointauc.Api
{
	/// <summary>
	/// Possible bid statuses.
	/// </summary>
	public enum Status
	{
		/// <summary>
		/// This bid doesn't exist inside auction.
		/// </summary>
		NotFound,

		/// <summary>
		/// Streamer deleted this bid.
		/// </summary>
		Pending,

		/// <summary>
		/// The bid was added to some lot.
		/// </summary>
		Processed,

		/// <summary>
		/// Streamer didn't handle this bid and it's still waiting in the queue.
		/// </summary>
		Rejected,
	}
}