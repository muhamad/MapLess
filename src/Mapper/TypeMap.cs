using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using MapLess.Internal;

namespace MapLess
{
    /// <summary>
    /// map between data-reader and input interface properties
    /// </summary>
    /// <typeparam name="T">input type</typeparam>
    public class TypeMap<T>
    {
        private readonly static InterfaceMetadata metaData = DataMap.GetInterface(typeof(T));

        #region public Methods

        /// <summary>
        /// get new instance of concrete type
        /// </summary>
        /// <returns>return new instance of concrete type of input interface</returns>
        public static T CreateConcrete()
        {
            return (T) Activator.CreateInstance(metaData.Concrete);
        }

        /// <summary>
        /// read current object from data reader
        /// </summary>
        /// <param name="reader">data reader</param>
        /// <returns>return object from data reader</returns>
        public static T ReadCurrentRow(IDataReader reader)
        {
            T instance = CreateConcrete();
            string[] columns = DataMap.GetColumns(reader);

            DataMap.ReadCurrentRow(reader, columns, instance, metaData);

            return instance;
        }

        /// <summary>
        /// get all rows from one result from reader
        /// </summary>
        /// <param name="reader">data reader</param>
        /// <returns>return list of rows</returns>
        /// <remarks>this method is optimized for single result set</remarks>
        public static IList<T> ReadSingleResultset(IDataReader reader)
        {
            var list = new List<T>();
            string[] columns = DataMap.GetColumns(reader);

            DataMap.ReadSingleResultset(reader, columns, list, metaData);

            return list;
        }

        /// <summary>
        /// get all result sets from data reader
        /// </summary>
        /// <param name="reader">data reader</param>
        /// <param name="properties">properties ordered to map result set into it</param>
        /// <returns>return object with mapped results</returns>
        /// <remarks>this method is optimized for multiple result sets</remarks>
        public static T ReadMultiResultset(IDataReader reader, params string[] properties)
        {
            if (properties.Length == 0)
                throw new ArgumentException("properties array is empty, you should provide at least one property to fill");

            if (properties.Distinct().Count() != properties.Length)
                throw new ArgumentException("properties array contains duplicate names");

            T instance = CreateConcrete();

            DataMap.ReadMultiResultset(reader, instance, metaData, properties);

            return instance;
        }

        #endregion
    }
}
