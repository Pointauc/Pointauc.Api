using System.Threading;
using System.Threading.Tasks;

namespace Pointauc.Api
{
	public interface IPointaucClient
	{
		/// <summary>
		/// Sends bids to the currently connected auction clients. This bid will appear as regular card underneath the timer.
		/// </summary>
		/// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to cancel the write operation.</param>
		/// <param name="bids">Bids to send.</param>
		/// <returns></returns>
		Task Bids(CancellationToken cancellationToken = default, params Bid[] bids);

		/// <summary>
		/// Sends bids to the currently connected auction clients. This bid will appear as regular card underneath the timer.
		/// </summary>
		/// <param name="bids">Bids to send.</param>
		/// <returns></returns>
		Task Bids(params Bid[] bids);
	}
}