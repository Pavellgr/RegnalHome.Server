using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using RegnalHome.Common.Grpc;
using RegnalHome.Server.Extensions;
using RegnalHome.Irrigation.Grpc;

namespace RegnalHome.Server.Services;

public class IrrigationService : RegnalHome.Irrigation.Grpc.Irrigation.IrrigationBase
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;

    public IrrigationService(
        IDbContextFactory<ApplicationDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public override Task<IrrigationModules> GetIrrigationModules(Empty request, ServerCallContext context)
    {
        using (var dbContext = _dbContextFactory.CreateDbContext())
        {
            var modules = dbContext.IrrigationModules;

            return Task.FromResult(new IrrigationModules
            {
                Modules =
                {
                    modules.Select(p => p.ToGrpc<RegnalHome.Irrigation.Grpc.IrrigationModule>())
                }
            });
        }
    }

    public override async Task<IrrigationModule> GetIrrigationModule(Id request, ServerCallContext context)
    {
        using (var dbContext = _dbContextFactory.CreateDbContext())
        {
            if (Guid.TryParse(request.Id_, out var id))
            {
                var module = await dbContext.IrrigationModules.FirstOrDefaultAsync(p => p.Id == id);
                if (module != null)
                {
                    return module.ToGrpc<RegnalHome.Irrigation.Grpc.IrrigationModule>();
                }
            }
        }

        throw new ArgumentException($"Module for id '{request.Id_}' not found.");
    }
}