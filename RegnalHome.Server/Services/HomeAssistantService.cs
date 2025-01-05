using RegnalHome.Server.Extensions;
using RegnalHome.Server.Http.HttpClients;
using RegnalHome.Server.Http.Options;

namespace RegnalHome.Server.Services
{
    public class HomeAssistantService
    {
        private readonly IEgdHttpClient _egdHttpClient;
        private readonly EgdOptions _egdOptions;

        public HomeAssistantService(IEgdHttpClient egdHttpClient,
            [FromKeyedServices(Constants.Egd)] IOAuthOptions egdOptions)
        {
            _egdHttpClient = egdHttpClient;
            _egdOptions = (EgdOptions)egdOptions;
        }

        public async Task<double> GetVirtualBatteryStatus(int? year, CancellationToken cancellationToken)
        {
            var (dateFrom, dateTo) = GetDateFromTo(year);

            var production = await _egdHttpClient.GetProduction(dateFrom, dateTo, cancellationToken);
            var consumption = await _egdHttpClient.GetConsumption(dateFrom, dateTo, cancellationToken);

            var consumptionTotal = consumption.Sum();
            var productionTotal = production.Sum();

            consumptionTotal = consumptionTotal / 4;
            productionTotal = productionTotal / 4;

            var total = productionTotal - consumptionTotal;

            return total;
        }

        public (DateTime DateFrom, DateTime DateTo) GetDateFromTo(int? year, DateTime? today = null )
        {
            today ??= DateTime.Today;

            var dateFrom = new DateTime(year ?? (today >= new DateTime(today.Value.Year,
                                                                      _egdOptions.BeginDateTime.Month,
                                                                      _egdOptions.BeginDateTime.Day)
                                                 ? today.Value.Year
                                                 : today.Value.Year - 1),
                                        _egdOptions.BeginDateTime.Month,
                                        _egdOptions.BeginDateTime.Day);
            var dateTo = year == null
                            ? today.Value.AddSeconds(-1)
                            : new DateTime((int)(year + 1),
                                           _egdOptions.BeginDateTime.Month,
                                           _egdOptions.BeginDateTime.Day);

            return (dateFrom, dateTo);
        }
    }
}
