using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using UCenter.Common.Expressions;
using UCenter.Common;

namespace UCenter.Common.Database.Couch
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
