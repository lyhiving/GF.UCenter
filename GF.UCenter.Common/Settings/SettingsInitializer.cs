using System;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Reflection;

namespace GF.UCenter.Common
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
                            property.SetValue(settings, Convert.ChangeType(pair.Value, property.PropertyType));
                        }
                    });
            }

            return settings;
        }
    }
}
