using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UCenter.Common
{
    public interface ISettingsValueProvider
    {
        ICollection<SettingsValuePair> SettingValues { get; }
    }

    public class SettingsValuePair
    {
        public string Name { get; set; }

        public object Value { get; set; }
    }
}
