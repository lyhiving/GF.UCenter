using System.Collections.Generic;

namespace GF.UCenter.Common
{
    public interface ISettingsValueProvider
    {
        ICollection<SettingsValuePair> SettingValues { get; }
    }
}
