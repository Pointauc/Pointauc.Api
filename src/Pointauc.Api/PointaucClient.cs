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
		private bool disposed;

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

			options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
		}

		#endregion Constructors

		#region Methods

		/// <inheritdoc/>
		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="HttpRequestException"></exception>
		public async Task<List<string>> Bids(CancellationToken cancellationToken = default, params Bid[] bids)
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
						using (var response = await httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false))
						{
							response.EnsureSuccessStatusCode();
#if NETSTANDARD2_0
							using (var utf8Json = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
#else
							using (var utf8Json = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false))
#endif
							{
								response.EnsureSuccessStatusCode();
								return await JsonSerializer.DeserializeAsync<List<string>>(utf8Json, options, cancellationToken).ConfigureAwait(false);
							}
						}
					}
				}
			}
		}

		/// <inheritdoc cref="Bids(CancellationToken, Bid[])"/>
		public Task<List<string>> Bids(params Bid[] bids)
		{
			return Bids(default, bids);
		}

		/// <inheritdoc/>
		/// <exception cref="HttpRequestException"></exception>
		public async Task<List<Lot>> GetAllLots(CancellationToken cancellationToken = default)
		{
			using (var request = new HttpRequestMessage(HttpMethod.Get, "https://pointauc.com/api/oshino/lot/getAll"))
			{
				using (var response = await httpClient.SendAsync(request).ConfigureAwait(false))
				{
					response.EnsureSuccessStatusCode();

#if NETSTANDARD2_0
					using (var utf8Json = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
#else
					using (var utf8Json = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false))
#endif
					{
						response.EnsureSuccessStatusCode();
						return await JsonSerializer.DeserializeAsync<List<Lot>>(utf8Json, options, cancellationToken).ConfigureAwait(false);
					}
				}
			}
		}

		/// <inheritdoc/>
		/// <exception cref="ArgumentException"></exception>
		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="HttpRequestException"></exception>
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
						using (var response = await httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false))
						{
							response.EnsureSuccessStatusCode();
						}
					}
				}
			}
		}

		/// <inheritdoc/>
		/// <exception cref="ArgumentException"></exception>
		/// <exception cref="HttpRequestException"></exception>
		public async Task<StatusResponse> GetBidStatus(string bidId, CancellationToken cancellationToken = default)
		{
			if (string.IsNullOrEmpty(bidId))
			{
				throw new ArgumentException($"'{nameof(bidId)}' cannot be null or empty.", nameof(bidId));
			}

			using (var request = new HttpRequestMessage(HttpMethod.Get, $"https://pointauc.com/api/oshino/bid/status?id={bidId}"))
			{
				using (var response = await httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false))
				{
					response.EnsureSuccessStatusCode();
#if NETSTANDARD2_0
					using (var utf8Json = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
#else
					using (var utf8Json = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false))
#endif
					{
						response.EnsureSuccessStatusCode();
						return await JsonSerializer.DeserializeAsync<StatusResponse>(utf8Json, options, cancellationToken).ConfigureAwait(false);
					}
				}
			}
		}

		/// <inheritdoc/>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Frees unmanaged resources. In the overload, the disposing parameter is a <see cref="bool"/> that indicates whether the method call comes from a <see cref="Dispose()"/> method (its value is <see langword="true"/>) or from a finalizer (its value is <see langword="false"/>).
		/// </summary>
		/// <param name="disposing"><see langword="true"/> to indicate object should free unmanaged resources; otherwise, <see langword="false"/>.</param>
		protected virtual void Dispose(bool disposing)
		{
			if (disposing && !disposed)
			{
				disposed = true;

				if (httpClient != null)
				{
					httpClient.Dispose();
				}
			}
		}

		#endregion Methods
	}
}