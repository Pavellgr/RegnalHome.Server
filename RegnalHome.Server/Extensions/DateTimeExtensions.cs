using DateTime = RegnalHome.Common.Grpc.DateTime;

namespace RegnalHome.Server.Extensions;

public static class DateTimeExtensions
{
    public static DateTime ToGrpc(this System.DateTime dateTime)
    {
        return new DateTime
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
}