using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RegnalHome.Common.Models;
using System.Diagnostics;
using System.Globalization;

namespace RegnalHome.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class IrrigationController : ControllerBase
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;

    public IrrigationController(IDbContextFactory<ApplicationDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    [HttpGet]
    public async Task<IrrigationModule?> Get(Guid id, CancellationToken cancellationToken)
    {
        HttpContext.Request.Headers.TryGetValue("User-Agent", out var userAgent);
        Console.WriteLine($"Get IrrigationModule User-Agent: {userAgent}, RemoteIpAddress: {HttpContext.Features.Get<IHttpConnectionFeature>()?.RemoteIpAddress}");

        using (var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken))
        {
            var module = await dbContext.IrrigationModules.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
            if (module != null)
            {
                if (userAgent != "ESP8266HTTPClient")
                {
                    var timeZone = TimeZoneInfo.GetSystemTimeZones().FirstOrDefault(p => p.DisplayName.Contains("Praha"));
                    module.LastCommunication = TimeZoneInfo.ConvertTime(DateTime.UtcNow, timeZone);
                    await dbContext.SaveChangesAsync(cancellationToken);
                }

                return module;
            }

            return null;
        }
    }
}