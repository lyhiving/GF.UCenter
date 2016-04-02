namespace GF.UCenter.Common.Settings
{
    using System.Collections.Generic;

    public interface ISettingsValueProvider
    {
        ICollection<SettingsValuePair> SettingValues { get; }
    }
}