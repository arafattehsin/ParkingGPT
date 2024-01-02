using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ParkingGPT.Helpers;
using ParkingGPT.Model;
using ParkingGPT.Services;
using Plugin.Media;
using Plugin.Media.Abstractions;
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

        IPopupService popupService;

        public MainViewModel(GPTVisionService gPTVisionService, IPopupService popupService)
        {
            visionService = gPTVisionService;
            this.popupService = popupService;
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
                        string base64image = Convert.ToBase64String(byteArrayImage);
                        // Parking = await visionService.GetParkingResult(base64image);
                        IsBusy = false;
                        popupService.ShowPopup<MainViewModel>();
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
                    ImageSource = result.FullPath;

                    IsBusy = true;
                    string base64image = UtilityHelper.GetBase64Image(ImageSource);
                    // Parking = await visionService.GetParkingResult(base64image);
                    IsBusy = false;
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
