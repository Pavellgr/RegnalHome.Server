using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RegnalHome.Common.Models;

namespace RegnalHome.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class IrrigationController : ControllerBase
{
  private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;

  public IrrigationController(IDbContextFactory<ApplicationDbContext> dbContextFactory)
  {
    _dbContextFactory = dbContextFactory;
  }

  [HttpGet]
  public JsonResult GetAll()
  {
    var modules = new List<IrrigationModule>
    {
      new(),
      new()
    };

    return new JsonResult(modules);
  }
}