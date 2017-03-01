using System;

namespace MapLess
{
    /// <summary>
    /// mark interface to be implemented by user-defined concrete class or automatic concrete class
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface, Inherited = false, AllowMultiple = false)]
    public sealed class ConcreteAttribute : Attribute
    {
        /// <summary>
        /// initialize new instance with automatic concrete type implementation
        /// </summary>
        public ConcreteAttribute()
        {
        }

        /// <summary>
        /// initialize new instance user-defined type implementation
        /// </summary>
        /// <param name="type">user-defined type implementation</param>
        public ConcreteAttribute(Type type)
        {
            if (type == null) throw new ArgumentNullException();
            Type = type;
        }

        /// <summary>
        /// get type implementation
        /// </summary>
        public Type Type
        {
            get;
            private set;
        }
    }
}
