using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UCenter.Test
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
