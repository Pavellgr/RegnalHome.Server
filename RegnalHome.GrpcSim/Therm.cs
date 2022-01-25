using RegnalHome.Common.Dtos;

namespace RegnalHome.GrpcSim
{
    public class Therm:ThermDto
    {
        public new int? TargetTemperature { get; set; }
    }
}
