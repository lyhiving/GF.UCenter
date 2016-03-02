using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UCenter.Common
{
    public class SettingsDefaultValueProvider<TSettings> : ISettingsValueProvider
    {
        private static Lazy<SettingsDefaultValueProvider<TSettings>> defaultProvider = new Lazy<SettingsDefaultValueProvider<TSettings>>(
            () =>
            {
                return new SettingsDefaultValueProvider<TSettings>();
            },
            LazyThreadSafetyMode.PublicationOnly);

        private ICollection<SettingsValuePair> settingValues;

        public SettingsDefaultValueProvider()
        {
            this.settingValues = typeof(TSettings).GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(p => p.GetCustomAttribute<DefaultValueAttribute>() != null)
                .AsParallel()
                .Select(p => new SettingsValuePair() { Name = p.Name, Value = p.GetCustomAttribute<DefaultValueAttribute>().Value })
                .ToList();
        }


        public ICollection<SettingsValuePair> SettingValues
        {
            get
            {
                return this.settingValues;
            }
        }

        public static SettingsDefaultValueProvider<TSettings> Default
        {
            get
            {
                return defaultProvider.Value;
            }
        }
    }
}
