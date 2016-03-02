using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace UCenter.Common
{
    public static class SettingsInitializer
    {
        public static TSettings Initialize<TSettings>(ExportProvider exportProvider, params ISettingsValueProvider[] providers)
        {
            var settings = exportProvider.GetExportedValue<TSettings>();
            var properties = typeof(TSettings).GetProperties(BindingFlags.Instance | BindingFlags.Public);
            foreach (var provider in providers)
            {
                provider.SettingValues.AsParallel()
                    .ForAll(pair =>
                    {
                        var property = properties.Where(p => p.Name == pair.Name).FirstOrDefault();
                        if (property != null)
                        {
                            property.SetValue(settings, pair.Value);
                        }
                    });
            }

            return settings;
        }
    }
}
