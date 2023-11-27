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
		Task Bids(CancellationToken cancellationToken = default, params Bid[] bids);

		/// <inheritdoc cref="Bids(CancellationToken, Bid[])"/>
		Task Bids(params Bid[] bids);

		/// <summary>
		/// Updates existed lot on the client side.
		/// </summary>
		/// <param name="lotId">Will search lot by its id.</param>
		/// <param name="investorId">Will search first lot which contain this investor.</param>
		/// <param name="lot">Lot to update.</param>
		/// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to cancel the write operation.</param>
		/// <returns></returns>
		Task ChangeLot(string lotId, string investorId, Lot lot, CancellationToken cancellationToken = default);

		/// <inheritdoc cref="ChangeLot(string, string, Lot, CancellationToken)"/>>
		Task ChangeLot(string lotId, string investorId, Lot lot);

		/// <summary>
		/// Returns a list of all lots from the client.
		/// </summary>
		/// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to cancel the write operation.</param>
		/// <returns></returns>
		Task<List<Lot>> GetAllLots(CancellationToken cancellationToken = default);
	}
}