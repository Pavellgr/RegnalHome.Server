using System.Collections.ObjectModel;
using RegnalHome.Common.Models;

namespace RegnalHome.App.ViewModels;

internal class IrrigationViewModel : BaseViewModel
{
  public ObservableCollection<IrrigationModule> Modules { get; set; }

  public override void InitData()
  {
    var serverClient = ServiceResolver.GetService<ServerClient>();
  }
}