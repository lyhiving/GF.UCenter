using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UCenter.Common
{
    [Export]
    public class Settings
    {
        public string DatabaseConnectionString { get; set; }
    }
}
