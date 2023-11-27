using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Pointauc.Api
{
	/// <summary>
	/// Provides a <see cref="IPointaucClient"/> implementation for interacting with <see href="https://pointauc.com"/> API.
	/// </summary>
	public class PointaucClient : IPointaucClient, IDisposable
	{
		#region Fields

		private readonly HttpClient httpClient;
		private readonly JsonSerializerOptions options;

		#endregion Fields

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="PointaucClient"/> class with an access token.
		/// </summary>
		/// <param name="token">Token to authorize with one of the existing integrations to create an account.</param>
		public PointaucClient(string token)
		{
			httpClient = new HttpClient();
			httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

			options = new JsonSerializerOptions()
			{
				DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
				PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
			};
		}

		#endregion Constructors

		#region Methods

		/// <exception cref="ArgumentNullException"></exception>
		public async Task Bids(CancellationToken cancellationToken = default, params Bid[] bids)
		{
			if (bids is null)
			{
				throw new ArgumentNullException(nameof(bids));
			}

			using (var stream = new MemoryStream())
			{
				await JsonSerializer.SerializeAsync(stream, new { Bids = bids }, options, cancellationToken).ConfigureAwait(false);
				stream.Seek(0, SeekOrigin.Begin);

				using (var streamContent = new StreamContent(stream))
				{
					streamContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

					using (var request = new HttpRequestMessage(HttpMethod.Post, "https://pointauc.com/api/oshino/bids") { Content = streamContent })
					{
						using (var response = await httpClient.SendAsync(request).ConfigureAwait(false))
						{
							response.EnsureSuccessStatusCode();
						}
					}
				}
			}
		}

		/// <exception cref="ArgumentNullException"></exception>
		public Task Bids(params Bid[] bids)
		{
			return Bids(default, bids);
		}

		/// <inheritdoc cref="IPointaucClient.GetAllLots(CancellationToken)"/>
		public async Task<List<Lot>> GetAllLots(CancellationToken cancellationToken = default)
		{
			using (var request = new HttpRequestMessage(HttpMethod.Get, "https://pointauc.com/api/oshino/lot/getAll"))
			{
				using (var response = await httpClient.SendAsync(request).ConfigureAwait(false))
				{
					response.EnsureSuccessStatusCode();

					var utf8Json = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
					var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

					return await JsonSerializer.DeserializeAsync<List<Lot>>(utf8Json, options, cancellationToken).ConfigureAwait(false);
				}
			}
		}

		/// <exception cref="ArgumentNullException"></exception>
		public async Task ChangeLot(string lotId, string investorId, Lot lot, CancellationToken cancellationToken = default)
		{
			if (string.IsNullOrEmpty(lotId) && string.IsNullOrEmpty(investorId))
			{
				throw new ArgumentException($"'{nameof(lotId)}' and '{nameof(investorId)}' cannot be null or empty.", nameof(investorId));
			}

			if (lot is null)
			{
				throw new ArgumentNullException(nameof(lot));
			}

			using (var stream = new MemoryStream())
			{
				await JsonSerializer.SerializeAsync(stream, new { Query = new { Id = lotId, InvestorId = investorId }, Lot = lot }, options, cancellationToken).ConfigureAwait(false);
				stream.Seek(0, SeekOrigin.Begin);

				using (var streamContent = new StreamContent(stream))
				{
					streamContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

					using (var request = new HttpRequestMessage(HttpMethod.Put, "https://pointauc.com/api/oshino/lot") { Content = streamContent })
					{
						using (var response = await httpClient.SendAsync(request).ConfigureAwait(false))
						{
							response.EnsureSuccessStatusCode();
						}
					}
				}
			}
		}

		/// <exception cref="ArgumentNullException"></exception>
		public Task ChangeLot(string lotId, string investorId, Lot lot)
		{
			return ChangeLot(lotId, investorId, lot, default);
		}

		/// <inheritdoc cref="IDisposable.Dispose"/>
		public void Dispose()
		{
			httpClient.Dispose();
		}

		#endregion Methods
	}
}