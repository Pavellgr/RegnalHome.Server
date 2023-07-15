namespace RegnalHome.Web.Services
{
    public interface ITranslationService
    {
        string Get(string label, int? cultureLCID = null);
    }
}