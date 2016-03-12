using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Couchbase;
using Couchbase.N1QL;

namespace UCenter.Common.Exceptions
{
    public class CouchBaseException : Exception
    {
        public readonly IResult Result;

        public CouchBaseException(IResult result)
            : base(result.Message, result.Exception)
        {
            this.Result = result;
        }
    }
}
