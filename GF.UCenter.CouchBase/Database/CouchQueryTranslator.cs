using System;
using System.Linq.Expressions;
using GF.UCenter.Common;

namespace GF.UCenter.CouchBase
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
