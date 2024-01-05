using ParkingGPT.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ParkingGPT.Model;
using CommunityToolkit.Mvvm.Input;
using System.Diagnostics;

namespace ParkingGPT.ViewModel
{
    

    public partial class SettingsViewModel : BaseViewModel
    {
        SettingsService settingsService;

        [ObservableProperty]
        Settings settings;

        public SettingsViewModel(SettingsService settingsService)
        {
            Title = "Settings";
            this.settingsService = settingsService;
            Settings = this.settingsService.GetSettingsFromStorage();
        }

        [RelayCommand]
        async Task SavePreferencesAsync(Settings settings)
        {

            try
            {
                settingsService.SetSettingsToStorage(settings);
                await Shell.Current.DisplayAlert("Success!", "Your changes have been saved.", "OK");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"SavePreferencesAsync: {ex.Message}");
                await Shell.Current.DisplayAlert("Error!", ex.Message, "OK");
            }
        }
    }
}
