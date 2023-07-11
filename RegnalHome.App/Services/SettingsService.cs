namespace RegnalHome.App.Services
{
    class SettingsService : ServiceBase, ISettingsService
    {
        private bool isAuthenticated;

        public bool IsAuthenticated
        {
            get => isAuthenticated;
            set
            {
                isAuthenticated = value;
                OnPropertyChanged();
            }
        }

    }
}
