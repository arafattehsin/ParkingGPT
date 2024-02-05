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
            settings.EndpointURL = Preferences.Default.Get(nameof(settings.EndpointURL), string.Empty);
            settings.DeploymentModel = Preferences.Default.Get(nameof(settings.DeploymentModel), string.Empty);
            return settings;
        }

        public string GetSpecificSettingFromStorage(string settingName)
        {
            return Preferences.Default.Get(settingName, string.Empty);
        }

        public bool SetSettingsToStorage(Settings settings)
        {
            Preferences.Default.Set(nameof(settings.EndpointKey), settings.EndpointKey);
            Preferences.Default.Set(nameof(settings.EndpointURL), settings.EndpointURL);
            Preferences.Default.Set(nameof(settings.DeploymentModel), settings.DeploymentModel);
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
