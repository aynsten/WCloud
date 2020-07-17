using System;
using System.Linq.Expressions;

namespace WCloud.Framework.Database.Dapper.Expressions
{
    public static class Extensions
    {
        /// <summary>
        /// 处理
        /// where(x=>x.name==null)
        /// where(x=>x.name.contains(""))
        /// where(x=>(x.age/20)>9)
        /// order(x=>x.xx)
        /// select(x=>x.name)
        /// select(x=>new {x.name,x.age})
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static string Transform(Expression expr, ISqlDialect db, ref ExpressionParsedResult model)
        {
            switch (expr.NodeType)
            {
                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                case ExpressionType.ArrayIndex:
                case ExpressionType.Assign:
                case ExpressionType.Coalesce:
                case ExpressionType.Divide:
                case ExpressionType.Equal:
                case ExpressionType.ExclusiveOr:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.LeftShift:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.Modulo:
                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                case ExpressionType.NotEqual:
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                case ExpressionType.Power:
                case ExpressionType.RightShift:
                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                case ExpressionType.AddAssign:
                case ExpressionType.AndAssign:
                case ExpressionType.DivideAssign:
                case ExpressionType.ExclusiveOrAssign:
                case ExpressionType.LeftShiftAssign:
                case ExpressionType.ModuloAssign:
                case ExpressionType.MultiplyAssign:
                case ExpressionType.OrAssign:
                case ExpressionType.PowerAssign:
                case ExpressionType.RightShiftAssign:
                case ExpressionType.SubtractAssign:
                case ExpressionType.AddAssignChecked:
                case ExpressionType.MultiplyAssignChecked:
                case ExpressionType.SubtractAssignChecked: return TransformX((BinaryExpression)expr, db, ref model);

                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                case ExpressionType.Negate:
                case ExpressionType.NegateChecked:
                case ExpressionType.Not:
                case ExpressionType.Quote:
                case ExpressionType.TypeAs:
                case ExpressionType.UnaryPlus:
                case ExpressionType.Decrement:
                case ExpressionType.Increment:
                case ExpressionType.IsFalse:
                case ExpressionType.IsTrue:
                case ExpressionType.Throw:
                case ExpressionType.Unbox:
                case ExpressionType.OnesComplement: return TransformX((UnaryExpression)expr, db, ref model);

                case ExpressionType.Call: return TransformX((MethodCallExpression)expr, db, ref model);
                case ExpressionType.Lambda: return TransformX((LambdaExpression)expr, db, ref model);
                case ExpressionType.ListInit: return TransformX((ListInitExpression)expr, db, ref model);
                case ExpressionType.MemberAccess: return TransformX((MemberExpression)expr, db, ref model);
                case ExpressionType.MemberInit: return TransformX((MemberInitExpression)expr, db, ref model);

                case ExpressionType.New: return TransformX((NewExpression)expr, db, ref model);

                case ExpressionType.NewArrayBounds: return TransformX((NewArrayExpression)expr, db, ref model);
                case ExpressionType.NewArrayInit: return TransformX((NewArrayExpression)expr, db, ref model);
                case ExpressionType.TypeEqual:
                case ExpressionType.ArrayLength:
                case ExpressionType.PreIncrementAssign:
                case ExpressionType.PreDecrementAssign:
                case ExpressionType.PostIncrementAssign:
                case ExpressionType.PostDecrementAssign:
                case ExpressionType.Conditional:
                case ExpressionType.Invoke:
                case ExpressionType.TypeIs:
                case ExpressionType.Block:
                case ExpressionType.DebugInfo:
                case ExpressionType.Default:
                case ExpressionType.Constant:
                case ExpressionType.Parameter:
                case ExpressionType.Dynamic:
                case ExpressionType.Goto:
                case ExpressionType.Index:
                case ExpressionType.Label:
                case ExpressionType.RuntimeVariables:
                case ExpressionType.Loop:
                case ExpressionType.Switch:
                case ExpressionType.Try:
                case ExpressionType.Extension:
                    break;
            }

            throw new NotSupportedException($"不支持的表达式类型：{expr.NodeType}");
        }

        static string TransformX(Expression e, ISqlDialect db, ref ExpressionParsedResult nn)
        {
            throw new NotImplementedException();
        }
    }
}
