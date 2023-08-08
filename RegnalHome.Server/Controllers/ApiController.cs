using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace RegnalHome.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ApiController : ControllerBase
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;

        public ApiController(IDbContextFactory<ApplicationDbContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }
        [HttpGet]
        public async Task<JsonResult> Get(CancellationToken cancellationToken)
        {
            using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

            var module = await dbContext.IrrigationModules.FirstOrDefaultAsync(cancellationToken);

            return new JsonResult(new
            {
                irrigationModule = module
            });
        }
    }
}
