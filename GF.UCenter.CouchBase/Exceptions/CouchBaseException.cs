using System;
using Couchbase;

namespace UCenter.CouchBase.Exceptions
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
