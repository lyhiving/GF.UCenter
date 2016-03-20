using System;

namespace UCenter.CouchBase.Expressions
{
    internal class QueryParameter
    {
        public string Name { get; set; }

        public object Value { get; set; }

        public Type DataType { get; set; }

        public TypeCode TypeCode { get; set; }

    }
}
