using System;
using Couchbase;

namespace GF.UCenter.CouchBase
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
