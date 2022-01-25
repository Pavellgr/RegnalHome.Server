using AutoMapper;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RegnalHome.Common.Dtos;
using Mapper = RegnalHome.Common.Mapper;

namespace RegnalHome.Server.Data;

public class ApplicationDbContext : IdentityDbContext
{
    private readonly Mapper _mapper;

    public DbSet<ThermSensor> ThermSensors { get; set; }

    public DbSet<ThermCu> ThermCus { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options,
        Mapper mapper)
        : base(options)
    {
        _mapper = mapper;
    }

    public async Task AddUpdateSensor(ThermDto sensor, CancellationToken cancellationToken = default)
    {
        var dbSensor = await GetThermSensor(sensor.Id, cancellationToken);
        var thermSensor = _mapper.Map<ThermSensor>(sensor);

        if (dbSensor != null)
        {
            var res = ThermSensors.Attach(thermSensor);

            res.State = EntityState.Modified;

            await SaveChangesAsync(cancellationToken);
        }
        else
        {
            var res = await ThermSensors.AddAsync(thermSensor, cancellationToken);

            if (res.State == EntityState.Added)
            {
                await SaveChangesAsync(cancellationToken);
            }
        }
    }

    public async Task<ThermDto?> GetThermSensor(Guid id, CancellationToken cancellationToken = default)
    {
        return await ThermSensors.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }
    public async Task<ThermDto?> GetThermSensor(string address, CancellationToken cancellationToken = default)
    {
        return await ThermSensors.FirstOrDefaultAsync(p => p.Address == address, cancellationToken);
    }

    public Task<List<ThermDto>> GetThermSensors(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_mapper.Map<ThermDto>(ThermSensors, cancellationToken).ToList());
    }

    public async Task<ThermDto?> GetThermCu(Guid id, CancellationToken cancellationToken = default)
    {
        return await ThermCus.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public Task<List<ThermDto>> GetThermCus(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_mapper.Map<ThermDto>(ThermCus, cancellationToken).ToList());
    }
}

[AutoMap(typeof(ThermDto), ReverseMap = true)]
public class ThermSensor : ThermDto
{ }

[AutoMap(typeof(ThermDto), ReverseMap = true)]
public class ThermCu : ThermDto
{ }