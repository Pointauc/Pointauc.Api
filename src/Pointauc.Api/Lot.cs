using System.Collections.Generic;

namespace Pointauc.Api
{
	public class Lot
	{
		/// <summary>
		/// Lot shortcut which can be used in a bid message to instantly add bid to this lot - `#{fastId}`.
		/// </summary>
		public int? FastId { get; set; }

		/// <summary>
		/// Unique identifier of this lot.
		/// </summary>
		public string Id { get; set; }

		public string Name { get; set; }
		public int? Amount { get; set; }

		/// <summary>
		/// List of people who send at least one bid to this lot. read more in the "Reference" section.
		/// </summary>
		public IEnumerable<string> Investors { get; set; }
	}
}