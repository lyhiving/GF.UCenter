using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UCenter.Common.Database
{
    [Export]
    public class DatabaseContext
    {
        private readonly Settings settings;

        [ImportingConstructor]
        public DatabaseContext(Settings settings)
        {
            this.settings = settings;
            this.ConnectionString = settings.DatabaseConnectionString;
        }

        public string ConnectionString { get; private set; }
    }
}
