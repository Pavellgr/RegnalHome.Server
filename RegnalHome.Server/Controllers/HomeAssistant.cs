using Microsoft.AspNetCore.Mvc;
using RegnalHome.Server.Services;

namespace RegnalHome.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HomeAssistant : Controller
    {
        private readonly HomeAssistantService _homeAssistantService;

        public HomeAssistant(HomeAssistantService homeAssistantService)
        {
            _homeAssistantService = homeAssistantService;
        }

        //[Authorize(Constants.Egd)]
        [HttpGet(nameof(GetVirtualBatteryStatus))]
        public async Task<IActionResult> GetVirtualBatteryStatus(int? year = null)
        {
            return Ok( await _homeAssistantService.GetVirtualBatteryStatus(year, HttpContext.RequestAborted));
        }
    }
}
