namespace Pointauc.Api
{
	public class StatusResponse
	{
		public Status Status { get; set; }

		/// <summary>
		/// The lot this bid was added to. Will be returned only for the processed status.
		/// </summary>
		public Lot Lot { get; set; }
	}
}