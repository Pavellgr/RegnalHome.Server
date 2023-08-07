using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RegnalHome.Common.Models;
using System.Diagnostics;

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
        HttpContext.Request.Headers.TryGetValue("Host", out var host);
        Console.WriteLine($"Get IrrigationModule Host: {host}, RemoteIpAddress: {HttpContext.Features.Get<IHttpConnectionFeature>()?.RemoteIpAddress}");

        using (var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken))
        {
            var module = await dbContext.IrrigationModules.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
            if (module != null)
            {
                if (host == nameof(Common.RegnalIdentity.Configuration.IdentityServer.Clients.RegnalHome.Irrigation.ClientName))
                {
                    module.LastCommunication = DateTime.Now;
                    await dbContext.SaveChangesAsync(cancellationToken);
                }

                return module;
            }

            return null;
        }
    }
}