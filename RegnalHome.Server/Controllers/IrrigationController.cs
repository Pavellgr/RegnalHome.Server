using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RegnalHome.Common.Models;

namespace RegnalHome.Server.Controllers;

[Route("[controller]")]
[ApiController]
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
      return await dbContext.IrrigationModules.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }
  }
}