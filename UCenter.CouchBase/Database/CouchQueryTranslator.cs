using System;
using System.Linq.Expressions;
using UCenter.Common;
using UCenter.CouchBase.Expressions;

namespace UCenter.CouchBase.Database
{
    internal class CouchQueryTranslator : QueryTranslator
    {
        protected override Expression VisitMember(MemberExpression node)
        {
            this.Write(node.Member.Name.FirstCharacterToLower());
            return node;
        }

        protected override void WriteWithParameter(TypeCode code, object value)
        {
            base.WriteWithParameter("$", code, value);
        }
    }
}
