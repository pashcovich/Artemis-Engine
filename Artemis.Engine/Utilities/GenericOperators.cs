using System;
using System.Linq.Expressions;

namespace Artemis.Engine.Utilities
{
    public static class GenericOperators
    {

        /// <summary>
        /// Add two objects of generic type. This is a workaround to the issue of
        /// C# having no generic constraint requiring a type to implement the + 
        /// operator.
        /// 
        /// WARNING: Though the JIT compiler does a pretty good job of optimizing
        /// this method, it's clearly not as fast as a compiled + operator. Please 
        /// use with care.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static T Add<T>(T a, T b)
        {
            ParameterExpression paramA = Expression.Parameter(typeof(T), "a"),
                                paramB = Expression.Parameter(typeof(T), "b");

            BinaryExpression body = Expression.Add(paramA, paramB);
            Func<T, T, T> add = Expression.Lambda<Func<T, T, T>>(body, paramA, paramB).Compile();
            return add(a, b); 
        }

        /// <summary>
        /// Subtract two objects of generic type. This is a workaround to the issue of
        /// C# having no generic constraint requiring a type to implement the -
        /// operator. This will perform <code>a-b</code>
        /// 
        /// WARNING: Though the JIT compiler does a pretty good job of optimizing
        /// this method, it's clearly not as fast as a compiled - operator. Please 
        /// use with care.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static T Sub<T>(T a, T b)
        {
            ParameterExpression paramA = Expression.Parameter(typeof(T), "a"),
                                paramB = Expression.Parameter(typeof(T), "b");

            BinaryExpression body = Expression.Subtract(paramA, paramB);
            Func<T, T, T> add = Expression.Lambda<Func<T, T, T>>(body, paramA, paramB).Compile();
            return add(a, b);
        }

        /// <summary>
        /// Multiply two objects of generic type. This is a workaround to the issue of
        /// C# having no generic constraint requiring a type to implement the * 
        /// operator.
        /// 
        /// WARNING: Though the JIT compiler does a pretty good job of optimizing
        /// this method, it's clearly not as fast as a compiled * operator. Please 
        /// use with care.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static T Mul<T>(T a, T b)
        {
            ParameterExpression paramA = Expression.Parameter(typeof(T), "a"),
                                paramB = Expression.Parameter(typeof(T), "b");

            BinaryExpression body = Expression.Multiply(paramA, paramB);
            Func<T, T, T> add = Expression.Lambda<Func<T, T, T>>(body, paramA, paramB).Compile();
            return add(a, b);
        }

        /// <summary>
        /// Divide two objects of generic type. This is a workaround to the issue of
        /// C# having no generic constraint requiring a type to implement the / 
        /// operator. This will perform <code>a/b</code>.
        /// 
        /// WARNING: Though the JIT compiler does a pretty good job of optimizing
        /// this method, it's clearly not as fast as a compiled / operator. Please 
        /// use with care.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static T Div<T>(T a, T b)
        {
            ParameterExpression paramA = Expression.Parameter(typeof(T), "a"),
                                paramB = Expression.Parameter(typeof(T), "b");

            BinaryExpression body = Expression.Divide(paramA, paramB);
            Func<T, T, T> add = Expression.Lambda<Func<T, T, T>>(body, paramA, paramB).Compile();
            return add(a, b);
        }

        /// <summary>
        /// Return the average of two generic types.
        /// 
        /// WARNING: This method is not fast, please use with care.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static T Average<T>(T a, T b)
        {
            ParameterExpression paramA = Expression.Parameter(typeof(T), "a"),
                                paramB = Expression.Parameter(typeof(T), "b"),
                                paramC = Expression.Parameter(typeof(double));
            BinaryExpression body = Expression.Divide(Expression.Add(paramA, paramB), paramC);

            Func<T, T, double, T> average
                = Expression.Lambda<Func<T, T, double, T>>(
                    body, paramA, paramB, paramC).Compile();

            return average(a, b, 2.0);
        }
    }
}
