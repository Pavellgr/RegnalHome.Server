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
                    modules.Select(p => new IrrigationModule
                    {
                        Id = p.Id.ToString(),
                        Name = p.Name,
                        IrrigationTime = p.IrrigationTime.ToGrpc(),
                        IrrigationLengthMinutes = p.IrrigationLengthMinutes,
                        LastCommunication = System.DateTime.Now.ToGrpc()
                    })
                }
            });
        }
    }
}