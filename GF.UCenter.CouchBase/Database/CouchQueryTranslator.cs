namespace GF.UCenter.CouchBase
{
    using System;
    using System.Linq.Expressions;
    using Common;

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