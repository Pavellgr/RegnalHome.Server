using RegnalHome.App.ViewModels;

namespace RegnalHome.App.Pages;

public partial class Irrigation : ContentPage
{
  public Irrigation()
  {
    InitializeComponent();
    BindingContext = ServiceResolver.GetViewModel<IrrigationViewModel>();
  }
}