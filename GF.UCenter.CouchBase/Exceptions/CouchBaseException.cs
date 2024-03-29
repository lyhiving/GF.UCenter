﻿namespace GF.UCenter.CouchBase
{
    using System;
    using Couchbase;

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