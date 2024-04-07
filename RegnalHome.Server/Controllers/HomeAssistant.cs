using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RegnalHome.Server.Extensions;
using RegnalHome.Server.Http.HttpClients;
using RegnalHome.Server.Http.Options;
using System.ComponentModel.DataAnnotations;

namespace RegnalHome.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HomeAssistant : Controller
    {
        private readonly IEgdHttpClient _egdHttpClient;
        private readonly EgdOptions _egdOptions;

        public HomeAssistant(IEgdHttpClient egdHttpClient,
            [FromKeyedServices(Constants.Egd)] IOAuthOptions egdOptions)
        {
            _egdHttpClient = egdHttpClient;
            _egdOptions = (EgdOptions)egdOptions;
        }

        //[Authorize(Constants.Egd)]
        [HttpGet(nameof(GetVirtualBatteryStatus))]
        public async Task<IActionResult> GetVirtualBatteryStatus(CancellationToken cancellationToken, [FromHeader] string scope = "RegnalHome.HomeAssistant", [FromHeader] string client_id = "RegnalHome.HomeAssistant", [FromHeader] string client_secret = RegnalHome.Common.RegnalIdentity.Configuration.IdentityServer.Clients.RegnalHome.HomeAssistant.ClientSecret, int? year = null)
        {
            var today = DateTime.Today;
            var dateFrom = new DateTime(year ?? DateTime.Now.Year, _egdOptions.BeginDateTime.Month, _egdOptions.BeginDateTime.Day);
            var dateTo = year == null
                            ? today.AddSeconds(-1)
                            : new DateTime((int)year, today.Month, today.Day);

            var production = await _egdHttpClient.GetProduction(dateFrom, dateTo, cancellationToken);
            var consumption = await _egdHttpClient.GetConsumption(dateFrom, dateTo, cancellationToken);

            var consumptionTotal = consumption.Sum();
            var productionTotal = production.Sum();

            return Ok(productionTotal - consumptionTotal);
        }
    }
}
