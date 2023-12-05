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

    public async override Task<IrrigationModules> GetIrrigationModules(Empty request, ServerCallContext context)
    {
        using (var dbContext = await _dbContextFactory.CreateDbContextAsync())
        {
            var modules = await dbContext.IrrigationModules.ToListAsync();

            return new IrrigationModules
            {
                Modules =
                {
                    modules.Select(p => p.ToGrpc())
                }
            };
        }
    }

    public override async Task<IrrigationModule> GetIrrigationModule(Id request, ServerCallContext context)
    {
        using (var dbContext = await _dbContextFactory.CreateDbContextAsync())
        {
            if (Guid.TryParse(request.Id_, out var id))
            {
                var module = await dbContext.IrrigationModules.FirstOrDefaultAsync(p => p.Id == id);
                if (module != null)
                {
                    return module.ToGrpc();
                }
            }
        }

        throw new ArgumentException($"Module for id '{request.Id_}' not found.");
    }

    public override async Task<Empty> UpdateIrrigationModule(IrrigationModule request, ServerCallContext context)
    {
        using (var dbContext = await _dbContextFactory.CreateDbContextAsync())
        {
            if (Guid.TryParse(request.Id, out var id))
            {
                var module = await dbContext.IrrigationModules.FirstOrDefaultAsync(p => p.Id == id);
                if (module != null)
                {
                    module.IrrigationLengthMinutes = request.IrrigationLengthMinutes;
                    module.IrrigationTime = request.IrrigationTime.FromGrpc();
                    module.Name = request.Name;
                }

                await dbContext.SaveChangesAsync();
            }
        }

        return await Task.FromResult(new Empty());
    }

    public async override Task<Empty> IrrigationLog(Log request, ServerCallContext context)
    {
#pragma warning disable CS4014 // Intentional Fire&Forget
        Task.Run(async () =>
        {
            using (var dbContext = await _dbContextFactory.CreateDbContextAsync())
            {
                dbContext.Add(new Common.Models.Log
                {
                    Component = Common.Enums.Components.Irrigation
                });
                await dbContext.SaveChangesAsync();
            }
        });
#pragma warning restore CS4014 

        return await Task.FromResult(new Empty());
    }
}