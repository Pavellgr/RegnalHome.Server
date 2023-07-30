using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RegnalHome.Common.Models;

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
        using (var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken))
        {
            var module = await dbContext.IrrigationModules.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
            if (module != null)
            {
                module.LastCommunication = DateTime.Now;
                await dbContext.SaveChangesAsync(cancellationToken);

                return module;
            }

            return null;
        }
    }
}