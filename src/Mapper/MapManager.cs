using System.Reflection;

namespace MapLess
{
    /// <summary>
    /// manage MapLess configuration
    /// </summary>
    public static class MapManager
    {
        /// <summary>
        /// Cache interfaces of input assembly for fast access
        /// </summary>
        /// <param name="assembly">assembly to be cached</param>
        /// <remarks>
        /// if you intend to use interfaces that are visible only to your assembly - i.e. internal -
        /// then you need to mark you assembly with the attribute InternalsVisibleTo("DynamicContainers")]
        /// so dynamic types can be bound at runtime
        /// </remarks>
        public static void CacheAssembly(Assembly assembly)
        {
            Internal.DataMap.CacheAssembly(assembly);
        }

        /// <summary>
        /// get/set value to indicate if conversion may happened when mapping column from database to property
        /// </summary>
        public static bool ConvertColumnTypeToPropertyTypeIfPossible
        {
            get; set;
        }
    }
}
