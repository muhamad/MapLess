using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace MapLess.Internal
{
    /// <summary>
    /// represent property metadata
    /// </summary>
    internal class PropertyMetadata
    {
        /// <summary>property name</summary>
        public string Name { get; set; }

        /// <summary>property type</summary>
        public Type Type { get; set; }

        /// <summary>alias names</summary>
        public IList<string> Alias { get; set; }

        /// <summary>property setter</summary>
        public MethodInfo SetMethod { get; set; }

        /// <summary>property getter</summary>
        public MethodInfo GetMethod { get; set; }

        /// <summary>set property value</summary>
        public Action<object, object> SetValue { get; set; }

        /// <summary>get property value</summary>
        public Func<object, object> GetValue { get; set; }

        /// <summary>
        /// create set value function
        /// </summary>
        internal void CreateFastGetSetMethod(Type interfaceType)
        {
            ParameterExpression instance = Expression.Parameter(typeof(object), "instance");
            ParameterExpression argument = Expression.Parameter(typeof(object), "argument");

            MethodCallExpression setMethod = Expression.Call(Expression.Convert(instance, interfaceType), SetMethod, Expression.Convert(argument, Type));
            SetValue = (Action<object, object>) Expression.Lambda(setMethod, instance, argument).Compile();


            MethodCallExpression getMethod = Expression.Call(Expression.Convert(instance, interfaceType), GetMethod);
            GetValue = (Func<object, object>) Expression.Lambda( Expression.Convert(getMethod, typeof(object)), instance).Compile();
        }
    }
}
