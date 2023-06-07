using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RegnalHome.App.ViewModels;

public abstract class BaseViewModel : INotifyPropertyChanged
{
  public BaseViewModel()
  {
    InitData();
  }

  public abstract void InitData();

  #region INotifyPropertyChanged

  public event PropertyChangedEventHandler PropertyChanged;

  protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
  {
    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
  }

  protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
  {
    if (EqualityComparer<T>.Default.Equals(field, value)) return false;
    field = value;
    OnPropertyChanged(propertyName);
    return true;
  }

  #endregion
}