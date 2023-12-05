using System;
using System.Collections.Generic;
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
	public partial class PointaucClient : IPointaucClient, IDisposable
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
#if NET8_0_OR_GREATER
				TypeInfoResolver = /*JsonSerializer.IsReflectionEnabledByDefault ? new System.Text.Json.Serialization.Metadata.DefaultJsonTypeInfoResolver() : */SourceGenerationContext.Default
#endif
			};
		}

		#endregion Constructors

		#region Methods

		/// <inheritdoc/>
		/// <exception cref="ArgumentNullException"></exception>
		public async Task Bids(CancellationToken cancellationToken = default, params Bid[] bids)
		{
			if (bids is null)
			{
				throw new ArgumentNullException(nameof(bids));
			}

#if NET8_0_OR_GREATER
			
			BidsRequest bidsRequest = new BidsRequest { bids = bids };
			var jsonString = JsonSerializer.Serialize(bidsRequest, SourceGenerationContext.Default.BidsRequest);
#else
			var jsonString = JsonSerializer.Serialize(new BidsRequest { bids = bids }, options);

#endif

			using (var streamContent = new StringContent(jsonString))
			{
				streamContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

				using (var request = new HttpRequestMessage(HttpMethod.Post, "https://pointauc.com/api/oshino/bids") { Content = streamContent })
				{
					using (var response = await httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false))
					{
						System.Diagnostics.Debug.WriteLine(response.RequestMessage.Content.ReadAsStringAsync().Result);

						response.EnsureSuccessStatusCode();
					}
				}
			}
		}

		/// <inheritdoc/>
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
				using (var response = await httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false))
				{
					response.EnsureSuccessStatusCode();

#if NETSTANDARD2_0
					var utf8Json = await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false);

					return JsonSerializer.Deserialize<List<Lot>>(new ReadOnlySpan<byte>(utf8Json), options);
#else
					using (var utf8Json = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false))
					{
						return JsonSerializer.Deserialize<List<Lot>>(utf8Json, options);
					}
#endif
				}
			}
		}

		/// <inheritdoc/>
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

			var jsonString = JsonSerializer.Serialize(new ChangeLotRequst { Query = new Query { Id = lotId, InvestorId = investorId }, Lot = lot }, options);

			using (var streamContent = new StringContent(jsonString))
			{
				streamContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

				using (var request = new HttpRequestMessage(HttpMethod.Put, "https://pointauc.com/api/oshino/lot") { Content = streamContent })
				{
					using (var response = await httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false))
					{
						System.Diagnostics.Debug.WriteLine(response.RequestMessage.Content.ReadAsStringAsync().Result);
						response.EnsureSuccessStatusCode();
					}
				}
			}
		}

		/// <inheritdoc/>
		/// <exception cref="ArgumentNullException"></exception>
		public Task ChangeLot(string lotId, string investorId, Lot lot)
		{
			return ChangeLot(lotId, investorId, lot, default);
		}

		/// <inheritdoc cref="IDisposable.Dispose"/>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Frees unmanaged resources. In the overload, the disposing parameter is a Boolean that indicates whether the method call comes from a Dispose method (its value is <see langword="true"/>) or from a finalizer (its value is <see langword="false"/>).
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