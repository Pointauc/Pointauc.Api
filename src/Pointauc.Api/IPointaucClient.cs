using System.Collections.Generic;
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
		Task<List<string>> Bids(CancellationToken cancellationToken = default, params Bid[] bids);

		/// <inheritdoc cref="Bids(CancellationToken, Bid[])"/>
		Task<List<string>> Bids(params Bid[] bids);

		/// <summary>
		/// Updates existed lot on the client side.
		/// </summary>
		/// <param name="lotId">Will search lot by its id.</param>
		/// <param name="investorId">Will search first lot which contain this investor.</param>
		/// <param name="lot">Lot to update.</param>
		/// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to cancel the write operation.</param>
		/// <returns></returns>
		Task ChangeLot(string lotId, string investorId, Lot lot, CancellationToken cancellationToken = default);

		/// <summary>
		/// Returns a list of all lots from the client.
		/// </summary>
		/// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to cancel the write operation.</param>
		/// <returns></returns>
		Task<LotsResponse> GetAllLots(CancellationToken cancellationToken = default);

		/// <summary>
		/// Will return the bid status by its id.
		/// </summary>
		/// <param name="bidId">Bid id.</param>
		/// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to cancel the write operation.</param>
		/// <returns></returns>
		Task<StatusResponse> GetBidStatus(string bidId, CancellationToken cancellationToken = default);
	}
}