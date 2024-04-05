using RegnalHome.Server.Http.Responses;

namespace RegnalHome.Server.Http.HttpClients
{
    public interface IEgdHttpClient
    {
        Task<EgdDataResponse> GetConsumption(DateTime dateFrom, DateTime dateTo, CancellationToken cancellationToken);

        Task<EgdDataResponse> GetProduction(DateTime dateFrom, DateTime dateTo, CancellationToken cancellationToken);
    }
}