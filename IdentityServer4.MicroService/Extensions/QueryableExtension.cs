using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace OAuthApp
{
    public static class QueryableExtension
    {
        public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> query, string propertyName)
        {
            return _OrderBy<T>(query, propertyName, false);
        }
        public static IOrderedQueryable<T> OrderByDescending<T>(this IQueryable<T> query, string propertyName)
        {
            return _OrderBy<T>(query, propertyName, true);
        }

        static IOrderedQueryable<T> _OrderBy<T>(IQueryable<T> query, string propertyName, bool isDesc)
        {
            propertyName = propertyName.Substring(0, 1).ToUpper() + propertyName.Substring(1);

            var memberProp = typeof(T).GetProperty(propertyName);

            if (memberProp == null)
            {
                memberProp = typeof(T).GetProperties()[0];
            }

            var methodname = (isDesc) ? "OrderByDescendingInternal" : "OrderByInternal";

            var method = typeof(QueryableExtension).GetMethod(methodname)
                                       .MakeGenericMethod(typeof(T), memberProp.PropertyType);

            return (IOrderedQueryable<T>)method.Invoke(null, new object[] { query, memberProp });
        }
        public static IOrderedQueryable<T> OrderByInternal<T, TProp>(IQueryable<T> query, PropertyInfo memberProperty)
        {
            return query.OrderBy(_GetLamba<T, TProp>(memberProperty));
        }
        public static IOrderedQueryable<T> OrderByDescendingInternal<T, TProp>(IQueryable<T> query, PropertyInfo memberProperty)
        {
            return query.OrderByDescending(_GetLamba<T, TProp>(memberProperty));
        }
        static Expression<Func<T, TProp>> _GetLamba<T, TProp>(PropertyInfo memberProperty)
        {
            if (memberProperty.PropertyType != typeof(TProp)) throw new Exception();

            var thisArg = Expression.Parameter(typeof(T));
            var lamba = Expression.Lambda<Func<T, TProp>>(Expression.Property(thisArg, memberProperty), thisArg);

            return lamba;
        }
    }
}
