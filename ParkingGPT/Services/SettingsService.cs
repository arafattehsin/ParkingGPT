using ParkingGPT.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkingGPT.Services
{
    public class SettingsService
    {
        public SettingsService() { }

        public Settings GetSettingsFromStorage()
        {
            Settings settings = new();
            settings.EndpointKey = Preferences.Default.Get(nameof(settings.EndpointKey), string.Empty);
            settings.IsUseOpenAI = Preferences.Default.Get(nameof(settings.IsUseOpenAI), false);
            return settings;
        }

        public string GetSpecificSettingFromStorage(string settingName)
        {
            return Preferences.Default.Get(settingName, string.Empty);
        }

        public bool SetSettingsToStorage(Settings settings)
        {
            Preferences.Default.Set(nameof(settings.EndpointKey), settings.EndpointKey);
            Preferences.Default.Set(nameof(settings.IsUseOpenAI), settings.IsUseOpenAI);
            return true;
        }

        public bool IsConfigRequired()
        {
            return !(string.IsNullOrEmpty(this.GetSpecificSettingFromStorage("EndpointKey")))
                           ? false : true;
        }
    }
}
