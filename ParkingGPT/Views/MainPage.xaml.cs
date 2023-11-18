using ParkingGPT.ViewModel;

namespace ParkingGPT.Views;

public partial class MainPage : ContentPage
{
    MainViewModel viewModel;
    public MainPage(MainViewModel viewModel)
    {
        InitializeComponent();
        this.viewModel = viewModel;
        BindingContext = this.viewModel;
    }
}