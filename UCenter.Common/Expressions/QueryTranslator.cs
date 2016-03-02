using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace UCenter.Common.Expressions
{
    public abstract class QueryTranslator : ExpressionVisitor
    {
        private readonly StringBuilder sb;
        private readonly List<QueryParameter> parameters;
        private int parameterIndexer = 0;

        internal QueryTranslator()
        {
            this.sb = new StringBuilder();
            this.parameters = new List<QueryParameter>();
        }

        internal QueryCommand Translate(Expression expression)
        {
            this.Clear();
            this.Visit(Evaluator.PartialEval(expression));
            return new QueryCommand(this.sb.ToString(), this.parameters);
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Method.DeclaringType == typeof(Queryable) && node.Method.Name == "Where")
            {
                this.Write("SELECT * FROM (");
                this.Visit(node.Arguments[0]);
                this.Write(") AS T WHERE ");
                LambdaExpression lambda = (LambdaExpression)StripQuotes(node.Arguments[1]);
                this.Visit(lambda.Body);
                return node;
            }
            throw new NotSupportedException(string.Format("The method '{0}' is not supported", node.Method.Name));
        }

        protected override Expression VisitUnary(UnaryExpression node)
        {
            switch (node.NodeType)
            {
                case ExpressionType.Not:
                    this.Write(" NOT ");
                    this.Visit(node.Operand);
                    break;
                default:
                    throw new NotSupportedException(string.Format("The unary operator '{0}' is not supported", node.NodeType));
            }
            return node;
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            switch (node.NodeType)
            {
                case ExpressionType.ArrayIndex:
                    this.Visit(node.Left);
                    this.Write("[");
                    this.Visit(node.Right);
                    this.Write("]");
                    break;
                case ExpressionType.Power:
                    this.Write("POW(");
                    this.Visit(node.Left);
                    this.Write(", ");
                    this.Visit(node.Right);
                    this.Write(")");
                    break;
                default:
                    this.Visit(node.Left);
                    this.Write(" ");
                    this.Write(GetOperator(node.NodeType));
                    this.Write(" ");
                    this.Visit(node.Right);
                    break;
            }

            return node;
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            IQueryable q = node.Value as IQueryable;
            if (q != null)
            {
                // assume constant nodes w/ IQueryables are table references
                this.Write("SELECT * FROM ");
                this.Write(q.ElementType.Name);
            }
            else if (node.Value == null)
            {
                this.Write("NULL");
            }
            else {
                var typeCode = Type.GetTypeCode(node.Value.GetType());
                switch (typeCode)
                {
                    case TypeCode.Boolean:
                        this.Write(((bool)node.Value) ? 1 : 0);
                        break;
                    case TypeCode.String:
                    case TypeCode.DateTime:
                        this.WriteWithParameter(typeCode, node.Value);
                        break;
                    case TypeCode.Object:
                        throw new NotSupportedException(string.Format("The constant for '{0}' is not supported", node.Value));
                    default:
                        this.Write(node.Value);
                        break;
                }
            }
            return node;
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            this.Write(node.Member.Name);
            return node;
        }

        protected virtual string GetOperator(ExpressionType type)
        {
            switch (type)
            {
                case ExpressionType.Not:
                    return "!";
                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                    return "+";
                case ExpressionType.Negate:
                case ExpressionType.NegateChecked:
                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                    return "-";
                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                    return "*";
                case ExpressionType.Divide:
                    return "/";
                case ExpressionType.Modulo:
                    return "%";
                case ExpressionType.And:
                    return "&";
                case ExpressionType.AndAlso:
                    return " AND ";
                case ExpressionType.Or:
                    return "|";
                case ExpressionType.OrElse:
                    return " OR ";
                case ExpressionType.LessThan:
                    return "<";
                case ExpressionType.LessThanOrEqual:
                    return "<=";
                case ExpressionType.GreaterThan:
                    return ">";
                case ExpressionType.GreaterThanOrEqual:
                    return ">=";
                case ExpressionType.Equal:
                    return "=";
                case ExpressionType.NotEqual:
                    return "!=";
                case ExpressionType.Coalesce:
                    return "??";
                case ExpressionType.RightShift:
                    return ">>";
                case ExpressionType.LeftShift:
                    return "<<";
                case ExpressionType.ExclusiveOr:
                    return "^";
                default:
                    return null;
            }
        }


        private void Write(string text)
        {
            this.sb.Append(text);
        }

        private void Write(object text)
        {
            this.Write(Convert.ToString(text));
        }

        private void WriteWithParameter(TypeCode code, object value)
        {
            string name = $"@p{this.parameterIndexer++}";
            this.Write(name);
            this.parameters.Add(new QueryParameter() { Name = name, Value = value, TypeCode = code });
        }

        private void Clear()
        {
            this.parameterIndexer = 0;
            this.sb.Clear();
            this.parameters.Clear();
        }

        private static Expression StripQuotes(Expression node)
        {
            while (node.NodeType == ExpressionType.Quote)
            {
                node = ((UnaryExpression)node).Operand;
            }
            return node;
        }

    }
}
