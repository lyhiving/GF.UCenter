using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UCenter.Common.Exceptions
{
    public class DatabaseException : Exception
    {
        public DatabaseException(Exception innerException)
            : base($"[DatabaseException]:{innerException.Message}", innerException)
        {
            if (innerException is MySqlException)
            {
                this.Number = (innerException as MySqlException).Number;
            }
            // todo: add other database(no sql for example support)
        }

        public int Number { get; private set; }
    }
}
