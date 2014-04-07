using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace SquaredInfinity.Foundation.Extensions
{
    public static class ExpressionExtensions
    {
        /// <summary>
        /// Gets the name of a property or field passed as an expression
        /// </summary>
        /// <param name="me">Me.</param>
        /// <returns></returns>
        public static string GetAccessedMemberName(this Expression<Func<object>> me)
        {
            object value = null;

            return me.GetAccessedMemberName(out value);
        }

        /// <summary>
        /// Gets the name and value of a property or field passed as an expression
        /// </summary>
        /// <param name="me">Me.</param>
        /// <returns></returns>
        public static string GetAccessedMemberName(this Expression<Func<object>> me, out object value)
        {
            var lambda = me as LambdaExpression;

            var unaryExpression = lambda.Body as UnaryExpression; // value type
            var memberExpression = lambda.Body as MemberExpression; // reference type

            if (unaryExpression == null && memberExpression == null)
                throw new ArgumentException("Expression has to be a member access expression (i.e. () => this.Count)");

            if (unaryExpression != null)
                memberExpression = unaryExpression.Operand as MemberExpression;

            if (memberExpression == null)
                throw new ArgumentException("Expression has to be a member access expression (i.e. () => this.Count)");

            var propertyInfo = memberExpression.Member as MemberInfo;

            var constantExpression = memberExpression.Expression as ConstantExpression;

            if (constantExpression != null)
                value = constantExpression.Value;
            else
                value = null;

            return propertyInfo.Name;
        }
    }
}
