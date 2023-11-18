using ParkingGPT.ViewModel;

namespace ParkingGPT.Views;

public partial class SettingsPage : ContentPage
{
	public SettingsPage(SettingsViewModel settingsViewModel)
	{
		InitializeComponent();
		BindingContext = settingsViewModel;
	}
}