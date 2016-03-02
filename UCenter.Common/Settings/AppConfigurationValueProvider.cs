using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.Hosting;

namespace UCenter.Common
{
    public class AppConfigurationValueProvider : ISettingsValueProvider
    {
        private readonly Configuration configuration;
        private readonly List<SettingsValuePair> settingValues = new List<SettingsValuePair>();

        private static Lazy<AppConfigurationValueProvider> defaultProvider = new Lazy<AppConfigurationValueProvider>(
            () =>
            {
                Configuration configuration = null;
                if (HostingEnvironment.IsHosted)
                {
                    configuration = WebConfigurationManager.OpenWebConfiguration("~/");
                }
                else
                {
                    configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                }

                return new AppConfigurationValueProvider(configuration);
            },
            LazyThreadSafetyMode.PublicationOnly);

        public AppConfigurationValueProvider(Configuration configuration)
        {
            this.configuration = configuration;
            var appSettings = this.configuration.AppSettings.Settings;
            var settings = appSettings.OfType<KeyValueConfigurationElement>().ToDictionary(
                element => element.Key,
                element => element.Value,
                StringComparer.Ordinal);

            foreach (var kv in settings)
            {
                this.settingValues.Add(new SettingsValuePair() { Name = kv.Key, Value = kv.Value });
            }
        }

        public ICollection<SettingsValuePair> SettingValues
        {
            get
            {
                return this.settingValues;
            }
        }

        public static AppConfigurationValueProvider Default
        {
            get
            {
                return defaultProvider.Value;
            }
        }
    }
}
