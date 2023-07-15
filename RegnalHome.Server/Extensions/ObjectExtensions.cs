using RegnalHome.Common.Models;

namespace RegnalHome.Server.Extensions
{
    public static class ObjectExtensions
    {
        public static T ToGrpc<T>(this object obj) where T : class
        {
            if (obj is DateTime dateTime &&
                typeof(T) == typeof(Common.Grpc.DateTime))
            {
                return new Common.Grpc.DateTime
                {
                    Date = new Common.Grpc.Date {
                        Day = dateTime.Day,
                        Month = dateTime.Month,
                        Year = dateTime.Year
                    },
                    Time = new Common.Grpc.Time{
                        Hour = dateTime.Hour,
                        Minute = dateTime.Minute,
                        Second = dateTime.Second
                    }
                } as T;
            }
            else if (obj is IrrigationModule irrigationModule &&
                typeof(T) == typeof(RegnalHome.Irrigation.Grpc.IrrigationModule))
            {
                return new RegnalHome.Irrigation.Grpc.IrrigationModule
                {
                    Id = irrigationModule.Id.ToString(),
                    Name = irrigationModule.Name,
                    IrrigationTime = irrigationModule.IrrigationTime.ToGrpc<Common.Grpc.DateTime>(),
                    IrrigationLengthMinutes = irrigationModule.IrrigationLengthMinutes,
                    LastCommunication = irrigationModule.LastCommunication.ToGrpc<Common.Grpc.DateTime>()
                } as T;
            }

            throw new NotImplementedException($"Conversion from type {obj.GetType()} to type {typeof(T)} is not implemented.");
        }

        public static T FromGrpc<T>(this object obj) where T : class
        {
            if (obj is RegnalHome.Irrigation.Grpc.IrrigationModule irrigationModule &&
                typeof(T) == typeof(IrrigationModule))
            {
                return new IrrigationModule
                {
                    Id = Guid.Parse(irrigationModule.Id),
                    Name = irrigationModule.Name,
                    IrrigationTime = irrigationModule.IrrigationTime.FromGrpc(),
                    IrrigationLengthMinutes = irrigationModule.IrrigationLengthMinutes,
                    LastCommunication = irrigationModule.LastCommunication.FromGrpc()
                } as T;
            }

            throw new NotImplementedException($"Conversion from type {obj.GetType()} to type {typeof(T)} is not implemented.");
        }

        public static DateTime FromGrpc(this Common.Grpc.DateTime dateTime)
        {
            return new DateTime(
                dateTime.Date.Year,
                dateTime.Date.Month,
                dateTime.Date.Day,
                dateTime.Time.Hour,
                dateTime.Time.Minute,
                dateTime.Time.Second);
        }
    }
}
