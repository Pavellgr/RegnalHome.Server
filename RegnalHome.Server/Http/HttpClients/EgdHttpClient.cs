using Azure.Core;
using Microsoft.AspNetCore.WebUtilities;
using RegnalHome.Server.Http.Endpoints;
using RegnalHome.Server.Http.Options;
using RegnalHome.Server.Http.Responses;
using RegnalHome.Server.Http.Settings;
using System;

namespace RegnalHome.Server.Http.HttpClients
{
    public class EgdHttpClient : OAuthHttpClient<EgdSettings, EgdOptions, EgdEndpoints>, IEgdHttpClient
    {
        public EgdHttpClient(IHttpClientFactory httpClientFactory,
                             [FromKeyedServices(Constants.Egd)] ITokenSettings settings,
                             [FromKeyedServices(Constants.Egd)] IOAuthOptions options,
                             [FromKeyedServices(Constants.Egd)] IOAuthEndpoints endpoints)
            : base(httpClientFactory, (EgdSettings)settings, (EgdOptions)options, (EgdEndpoints)endpoints)
        {
        }
        public async Task<EgdDataResponse> GetProduction(DateTime dateFrom, DateTime dateTo, CancellationToken cancellationToken)
            => await WithAuthenticate(ct => GetProductionInner(dateFrom, dateTo, ct), cancellationToken);

        public async Task<EgdDataResponse> GetConsumption(DateTime dateFrom, DateTime dateTo, CancellationToken cancellationToken)
            => await WithAuthenticate(ct => GetConsumptionInner(dateFrom, dateTo, ct), cancellationToken);


        private async Task<EgdDataResponse> GetConsumptionInner(DateTime dateFrom, DateTime dateTo, CancellationToken cancellationToken)
        {
            var query = new Dictionary<string, string?>
            {
                { "ean", _options.Ean },
                { "profile", "ICH1" },
                { "from", dateFrom.ToString("s") },
                { "to", dateTo.ToString("s") }
            };

            var result = await Get(QueryHelpers.AddQueryString(_endpoints.DataEndpoint, query),
                             isApiCall: true,
                             responseChecks: (EgdDataResponse response) => response != null,
                             cancellationToken: cancellationToken);

            if (result.Items.First().data.Count() >= Constants.EgdPageSize)
            {
                var nextPage = await GetConsumptionInner(result.Items.First().data.Max(p => p.timestamp), dateTo, cancellationToken);

                foreach (var item in nextPage.Items)
                {
                    result.Items.Add(item);
                }
            }

            return result;
        }
        private async Task<EgdDataResponse> GetProductionInner(DateTime dateFrom, DateTime dateTo, CancellationToken cancellationToken)
        {
            var query = new Dictionary<string, string?>
            {
                { "ean", _options.Ean },
                { "profile", "ISH1" },
                { "from", dateFrom.ToString("s") },
                { "to", dateTo.ToString("s") }
            };

            var result = await Get(QueryHelpers.AddQueryString(_endpoints.DataEndpoint, query),
                             isApiCall: true,
                             responseChecks: (EgdDataResponse response) => response != null,
                             cancellationToken: cancellationToken);

            if (result.Items.First().data.Count() >= Constants.EgdPageSize)
            {
                var nextPage = await GetProductionInner(result.Items.First().data.Max(p=>p.timestamp), dateTo, cancellationToken);

                foreach (var item in nextPage.Items)
                {
                    result.Items.Add(item);
                }
            }

            return result;
        }
    }
}
