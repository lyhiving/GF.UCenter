using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UCenter.Common.Expressions
{
    internal class QueryCommand
    {
        public QueryCommand(string command, ICollection<QueryParameter> parameters)
        {
            this.Command = command;
            this.Parameters = parameters;
        }

        public string Command { get; private set; }

        public ICollection<QueryParameter> Parameters { get; private set; }
    }
}
