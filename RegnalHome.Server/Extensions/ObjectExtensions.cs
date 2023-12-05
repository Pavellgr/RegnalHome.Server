using RegnalHome.Common.Models;

namespace RegnalHome.Server.Extensions
{
    public static class ObjectExtensions
    {
        #region FromGrpc
        public static IrrigationModule FromGrpc(this RegnalHome.Irrigation.Grpc.IrrigationModule irrigationModule)
        {
            return new IrrigationModule
            {
                Id = Guid.Parse(irrigationModule.Id),
                Name = irrigationModule.Name,
                IrrigationTime = irrigationModule.IrrigationTime.FromGrpc(),
                IrrigationLengthMinutes = irrigationModule.IrrigationLengthMinutes,
                LastCommunication = irrigationModule.LastCommunication.FromGrpc()
            };
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
        #endregion

        #region ToGrpc
        public static Common.Grpc.DateTime ToGrpc(this DateTime dateTime)
        {
            return new Common.Grpc.DateTime
            {
                Date = new Common.Grpc.Date
                {
                    Day = dateTime.Day,
                    Month = dateTime.Month,
                    Year = dateTime.Year
                },
                Time = new Common.Grpc.Time
                {
                    Hour = dateTime.Hour,
                    Minute = dateTime.Minute,
                    Second = dateTime.Second
                }
            };
        }

        public static RegnalHome.Irrigation.Grpc.IrrigationModule ToGrpc(this IrrigationModule irrigationModule)
        {
            return new RegnalHome.Irrigation.Grpc.IrrigationModule
            {
                Id = irrigationModule.Id.ToString(),
                Name = irrigationModule.Name,
                IrrigationTime = irrigationModule.IrrigationTime.ToGrpc(),
                IrrigationLengthMinutes = irrigationModule.IrrigationLengthMinutes,
                LastCommunication = irrigationModule.LastCommunication.ToGrpc()
            };
        }
        #endregion
    }
}
