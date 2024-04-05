using RegnalHome.Server.Http.Responses;

namespace RegnalHome.Server.Extensions
{
    public static class EgdDataResponseExtensions
    {
        public static double Sum(this EgdDataResponse response)
            => response?.Items?.Sum(p => p.data?.Where(q => q.IsValid()).Sum(q => q.value) ?? 0) ?? 0;

        public static bool IsValid(this EgdDataResponseItemData data)
            => data.status == "IU012";
    }
}
