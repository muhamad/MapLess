using System;
using System.Collections.Generic;

namespace MapLess.Internal
{
    using Table = Dictionary<ulong, PropertyMetadata>;

    /// <summary>
    /// represent type metadata
    /// </summary>
    internal class InterfaceMetadata
    {
        // properties names are hashed and saved for fast access
        private readonly Table tblHashNames = new Table();


        /// <summary>type identifier</summary>
        public Guid Id { get; set; }

        /// <summary>interface type</summary>
        public Type InterfaceType { get; set; }

        /// <summary>type name</summary>
        public string Name { get; set; }

        /// <summary>concrete type</summary>
        public Type Concrete { get; set; }

        /// <summary>type properties</summary>
        public List<PropertyMetadata> Properties { get; set; }

        /// <summary>interfaces that inherited by this interface</summary>
        public List<InterfaceMetadata> Parents { get; set; }

        /// <summary>
        /// get property which have name/alias same as input name
        /// </summary>
        /// <param name="name">name to search for</param>
        /// <param name="ignoreCase">specify if to ignore character case during search</param>
        /// <returns>return property object or null if nothing found</returns>
        /// <remarks>
        /// if multiple properties matches input name the first one returned.
        /// search begins with Properties list for this type then Properties list for each type in BaseTypes
        /// </remarks>
        public PropertyMetadata GetProperty(string name, bool ignoreCase = false)
        {
            ulong hashCode = DataMap.CalculateHash(ignoreCase ? name.ToLower() : name);

            if (tblHashNames.ContainsKey(hashCode)) return tblHashNames[hashCode];

            for (int i = 0; i < Parents.Count; ++i)
            {
                PropertyMetadata property = Parents[i].GetProperty(name, ignoreCase);
                if (property != null) return property;
            }

            return null;
        }

        /// <summary>
        /// update names in metadata
        /// </summary>
        public void UpdateMetadata()
        {
            tblHashNames.Clear();

            for (int i = 0; i < Properties.Count; ++i)
            {
                tblHashNames.Add(DataMap.CalculateHash(Properties[i].Name), Properties[i]);
                tblHashNames.Add(DataMap.CalculateHash(Properties[i].Name.ToLower()), Properties[i]);
            }
        }
    }
}
