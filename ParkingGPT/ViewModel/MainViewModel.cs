using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ParkingGPT.Helpers;
using ParkingGPT.Model;
using ParkingGPT.Services;
using ParkingGPT.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkingGPT.ViewModel
{
    public partial class MainViewModel : BaseViewModel
    {
        [ObservableProperty]
        string imageSource;

        [ObservableProperty]
        Parking parking;

        GPTVisionService visionService;

        public MainViewModel(GPTVisionService gPTVisionService)
        {
            visionService = gPTVisionService;
            
        }

        [RelayCommand]
        void ShowPopup()
        {
            if (Parking != null)
            {
                var popup = new PopupView(this);
                Shell.Current.CurrentPage.ShowPopup(popup);
            }
        }

        [RelayCommand]
        async Task CapturePhoto()
        {
            if (MediaPicker.Default.IsCaptureSupported)
            {
               
                try
                {
                    FileResult photo = await MediaPicker.Default.CapturePhotoAsync();

                    if (photo != null)
                    {
                        IsBusy = true;

                        // save the file into local storage
                        string localFilePath = Path.Combine(FileSystem.CacheDirectory, photo.FileName);
                        ImageSource = localFilePath;
                        using Stream sourceStream = await photo.OpenReadAsync();
                        using FileStream localFileStream = File.OpenWrite(localFilePath);
                        await sourceStream.CopyToAsync(localFileStream);

                        var byteArrayImage = UtilityHelper.GetImageStreamAsBytes(sourceStream);
                        // string base64image = Convert.ToBase64String(byteArrayImage);
                        Parking = await visionService.GetParkingResult(byteArrayImage);
                        IsBusy = false;
                        ShowPopup();
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"CapturePhoto: {ex.Message}");
                    await Shell.Current.DisplayAlert("Error!", ex.Message, "OK");
                }
            }
        }

        [RelayCommand]
        async Task PickPhoto()
        {
            try
            {
                var result = await MediaPicker.Default.PickPhotoAsync();

                if (result != null)
                {
                    IsBusy = true;
                    byte[] arrayImage = UtilityHelper.GetByteArrayofImage(result.FullPath);
                    Parking = await visionService.GetParkingResult(arrayImage);
                    IsBusy = false;
                    ShowPopup();
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"PickPhoto: {ex.Message}");
                await Shell.Current.DisplayAlert("Error!", ex.Message, "OK");
            }    
        }

    }
}
