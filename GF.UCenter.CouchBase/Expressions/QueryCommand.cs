using System.Collections.Generic;

namespace GF.UCenter.CouchBase
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
