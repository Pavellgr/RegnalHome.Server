using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RegnalHome.App.Services
{
    internal abstract class ServiceBase: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}