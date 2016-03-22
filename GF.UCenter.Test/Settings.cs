using System.ComponentModel;
using System.ComponentModel.Composition;

namespace GF.UCenter.Test
{
    [Export]
    public class Settings
    {
        [DefaultValue("localhost")]
        public string ServerHost { get; set; }

        [DefaultValue(8888)]
        public int ServerPort { get; set; }
    }
}
