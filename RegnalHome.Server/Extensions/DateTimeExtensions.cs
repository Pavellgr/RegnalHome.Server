using DateTime = RegnalHome.Common.Grpc.DateTime;

namespace RegnalHome.Server.Extensions;

public static class DateTimeExtensions
{
    public static DateTime ToGrpc(this System.DateTime dateTime)
    {
        return new DateTime
        {
            Date =
            {
                Day = dateTime.Day,
                Month = dateTime.Month,
                Year = dateTime.Year
            },
            Time =
            {
                Hour = dateTime.Hour,
                Minute = dateTime.Minute,
                Second = dateTime.Second
            }
        };
    }
}