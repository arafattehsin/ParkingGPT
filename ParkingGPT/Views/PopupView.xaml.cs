using CommunityToolkit.Maui.Views;
using ParkingGPT.ViewModel;

namespace ParkingGPT.Views;

public partial class PopupView : Popup
{
	public PopupView(MainViewModel mainViewModel)
	{
		InitializeComponent();
		BindingContext = mainViewModel;
	}


}