using ParkingGPT.Services;
using ParkingGPT.ViewModel;
using ParkingGPT.Views;
using Microsoft.Extensions.Logging;

namespace ParkingGPT
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif
            // Services
            builder.Services.AddSingleton<SettingsService>();

            // View Models
            builder.Services.AddSingleton<MainViewModel>();
            builder.Services.AddTransient<SettingsViewModel>();
            
            // Views
            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<SettingsPage>();
            return builder.Build();
        }
    }
}
