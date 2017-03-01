using System;
using System.Collections.Generic;

namespace MapLess
{
    /// <summary>
    /// represent one or more alternate name for property
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class AliasAttribute : Attribute
    {
        private readonly List<string> names = new List<string>();

        /// <summary>
        /// initialize new instance
        /// </summary>
        /// <param name="name">alternate name</param>
        /// <param name="names">alternate names</param>
        public AliasAttribute(string name, params string[] names)
        {
            this.names.Add(name);
            this.names.AddRange(names);
        }

        /// <summary>
        /// get alternate names
        /// </summary>
        /// <returns>get alternate names</returns>
        public IList<string> GetNames()
        {
            return names;
        }
    }
}
