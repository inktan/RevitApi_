using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WpfMvvm.Infrastruct
{
    public static class PropertySupport
    {
        public static string ExtractPropertyName<T>(Expression<Func<T>> propertyExpression)
        {
            if (propertyExpression == null)
            {
                throw new ArgumentNullException("propertyExpression");
            }

            var member = propertyExpression.Body as MemberExpression;

            if (member == null)
            {
                throw new ArgumentException("The expression is not a member access expression.", "propertyExpression");
            }

            var property = member.Member as PropertyInfo;

            if (property == null)
            {
                throw new ArgumentException("The member access expression does not access a property.",
                    "propertyExpression");
            }

            MethodInfo method = property.GetGetMethod(true);

            if (method.IsStatic)
            {
                throw new ArgumentException("The referenced property is a static property", "propertyExpression");
            }

            return member.Member.Name;
        }
    }
}
