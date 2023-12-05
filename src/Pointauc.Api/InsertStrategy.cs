namespace Pointauc.Api
{
	/// <summary>
	/// Describes how bid would be handled.
	/// </summary>
	public enum InsertStrategy
	{
		/// <summary>
		/// Bid will be handled according to the site settings.
		/// </summary>
		Auto,

		/// <summary>
		/// Bid will create a new lot if there are no matching lot message.
		/// </summary>
		Force,

		/// <summary>
		/// Bid will be added to the lot with the same message or short key.
		/// </summary>
		Match,

		/// <summary>
		/// Bid stays in a cards queue in all cases.
		/// </summary>
		None,
	}
}