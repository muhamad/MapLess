using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace MapLess.Internal
{
    /// <summary>
    /// cache-enabled data-to-type mapper
    /// </summary>
    internal static class DataMap
    {
        #region static fields

        // cached items
        private static readonly Dictionary<Guid, InterfaceMetadata> interfaceCache = new Dictionary<Guid, InterfaceMetadata>();

        // assembly builder for dynamic types
        private static readonly AssemblyBuilder assembly = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName("DynamicContainers"), AssemblyBuilderAccess.Run);

        // module builder for dynamic classes
        private static readonly ModuleBuilder module = assembly.DefineDynamicModule("DynamicTypes");

        #endregion

        #region cache interfaces

        /// <summary>
        /// cache assembly interfaces
        /// </summary>
        /// <param name="assembly">assembly to cache</param>
        internal static void CacheAssembly(Assembly assembly)
        {
            // collect all db interfaces in assembly and create concrete
            // types for them then store all required information
            // for fast access when needed

            Type[] interfaces = assembly.GetTypes().Where(t => t.IsInterface).ToArray();

            // stage 1: load basic information for all types and their dependencies
            foreach (var item in interfaces)
            {
                if (interfaceCache.ContainsKey(item.GUID))
                {
                    if (interfaceCache[item.GUID].InterfaceType.AssemblyQualifiedName == item.AssemblyQualifiedName)
                        continue;

                    throw new InvalidOperationException($"Internal Error, interface {item.Name} have GUID similar to another loaded interface from other assembly");
                }

                InterfaceMetadata itemMeta = GetInterfaceBasicInformation(item);
                if (itemMeta == null) continue;

                interfaceCache.Add(item.GUID, itemMeta);

                // load type dependencies
                foreach (var parent in item.GetInterfaces())
                {
                    InterfaceMetadata meta = null;
                    if (interfaceCache.ContainsKey(parent.GUID))
                        meta = interfaceCache[parent.GUID];
                    else
                    {
                        meta = GetInterfaceBasicInformation(parent);

                        if (meta == null)
                            throw new TypeLoadException($"Interface '{parent.Name}' is used by other interfaces and don't marked by ConcreteAttribute");

                        interfaceCache.Add(parent.GUID, meta);
                    }

                    itemMeta.Parents.Add(meta);
                }
            }

            // stage 2: create dynamic types
            foreach (var item in interfaceCache.Values.Where(e => e.Concrete == null))
                item.Concrete = ImplementInterface(item);
        }

        /// <summary>
        /// get interface basic meta data
        /// </summary>
        /// <param name="interfaceType">interface to get metadata</param>
        /// <returns>return basic metadata for interface</returns>
        private static InterfaceMetadata GetInterfaceBasicInformation(Type interfaceType)
        {
            var concreteAttrib = (ConcreteAttribute[]) interfaceType.GetCustomAttributes(typeof(ConcreteAttribute), false);

            if (concreteAttrib.Length == 0) return null;

            var meta = new InterfaceMetadata
            {
                Id = interfaceType.GUID,
                Name = interfaceType.Name,
                InterfaceType = interfaceType,
                Properties = GetInterfaceProperties(interfaceType, concreteAttrib[0].Type == null),
                Parents = new List<InterfaceMetadata>(),
                Concrete = concreteAttrib[0].Type
            };

            meta.UpdateMetadata();

            return meta;
        }

        /// <summary>
        /// get type properties
        /// </summary>
        /// <param name="interfaceType">interface to enumerate properties</param>
        /// <param name="isAutoImplemented">is interface auto implemented</param>
        /// <returns>return properties of input type</returns>
        private static List<PropertyMetadata> GetInterfaceProperties(Type interfaceType, bool isAutoImplemented)
        {
            var result = new List<PropertyMetadata>();
            PropertyInfo[] properties = interfaceType.GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(p => (p.GetMethod != null && p.SetMethod != null && (p.GetMethod.GetParameters().Length + p.SetMethod.GetParameters().Length) == 1)).ToArray();

            if (properties.Length == 0)
                throw new ArgumentException($"Interface '{interfaceType.Name}' should have at least one property");

            if (isAutoImplemented && interfaceType.GetMembers().Length != (properties.Length + properties.Length * 2))
                throw new ArgumentException($"Interface '{interfaceType.Name}' should only contains properties with get/set methods and no indexers");

            foreach (var property in properties)
            {
                var aliases = (AliasAttribute[]) interfaceType.GetCustomAttributes(typeof(AliasAttribute), false);

                var item = new PropertyMetadata
                {
                    Name = property.Name,
                    Type = property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) ? Nullable.GetUnderlyingType(property.PropertyType) : property.PropertyType,
                    Alias = aliases.Length != 0 ? aliases[0].GetNames() : new List<string>(),
                    GetMethod = property.GetMethod,
                    SetMethod = property.SetMethod
                };

                item.CreateFastGetSetMethod(interfaceType);

                result.Add(item);
            }

            return result;
        }

        /// <summary>
        /// implement interface and its dependencies into class
        /// </summary>
        /// <param name="interfaceMeta">interface metadata</param>
        /// <returns>return generated class that implement input interface</returns>
        private static Type ImplementInterface(InterfaceMetadata interfaceMeta)
        {
            TypeBuilder resultType = module.DefineType($"{interfaceMeta.Name}_{Guid.NewGuid().ToString("N")}", TypeAttributes.Class | TypeAttributes.Public, typeof(object), null);

            var interfaces = new List<InterfaceMetadata>(interfaceMeta.Parents);
            interfaces.Insert(0, interfaceMeta);

            foreach (var item in interfaces)
            {
                resultType.AddInterfaceImplementation(interfaceMeta.InterfaceType);

                foreach (var property in item.Properties)
                {
                    FieldBuilder field = resultType.DefineField($"m{property.Name}", property.Type, FieldAttributes.Private);
                    PropertyBuilder prop = resultType.DefineProperty($"{interfaceMeta.InterfaceType.FullName}.{property.Name}",
                        System.Reflection.PropertyAttributes.None, CallingConventions.HasThis, property.Type, Type.EmptyTypes);

                    MethodBuilder getMethod = resultType.DefineMethod($"{interfaceMeta.InterfaceType.FullName}.get_{property.Name}",
                        MethodAttributes.Private | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.SpecialName |
                        MethodAttributes.Virtual | MethodAttributes.Final, property.Type, Type.EmptyTypes);

                    ILGenerator getGen = getMethod.GetILGenerator();
                    getGen.Emit(OpCodes.Ldarg_0);
                    getGen.Emit(OpCodes.Ldfld, field);
                    getGen.Emit(OpCodes.Ret);

                    MethodBuilder setMethod = resultType.DefineMethod($"{interfaceMeta.InterfaceType.FullName}.set_{property.Name}",
                        MethodAttributes.Private | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.SpecialName |
                        MethodAttributes.Virtual | MethodAttributes.Final, null, new[] { property.Type });

                    ILGenerator setGen = setMethod.GetILGenerator();
                    setGen.Emit(OpCodes.Ldarg_0);
                    setGen.Emit(OpCodes.Ldarg_1);
                    setGen.Emit(OpCodes.Stfld, field);
                    setGen.Emit(OpCodes.Ret);

                    prop.SetGetMethod(getMethod);
                    resultType.DefineMethodOverride(getMethod, property.GetMethod);
                    prop.SetSetMethod(setMethod);
                    resultType.DefineMethodOverride(setMethod, property.SetMethod);
                }
            }

            return resultType.CreateType();
        }


        #endregion

        #region access cache

        /// <summary>
        /// get interface metadata
        /// </summary>
        /// <param name="interfaceType">type to get its metadata</param>
        /// <returns>return type metadata for input type; null for invalid type</returns>
        /// <remarks>an invalid type is an non-interface type.</remarks>
        public static InterfaceMetadata GetInterface(Type interfaceType)
        {
            if (interfaceCache.ContainsKey(interfaceType.GUID)) return interfaceCache[interfaceType.GUID];

            if (!interfaceType.IsInterface) throw new ArgumentException($"Type {interfaceType.Name} is not an interface");

            InterfaceMetadata itemMeta = GetInterfaceBasicInformation(interfaceType);
            if (itemMeta == null)
                throw new TypeLoadException($"Interface '{interfaceType.Name}' is not marked with ConcreteAttribute");

            interfaceCache.Add(interfaceType.GUID, itemMeta);

            Type[] interfaces = interfaceType.GetInterfaces();

            // load type dependencies
            foreach (var parent in interfaces)
            {
                InterfaceMetadata meta = null;
                if (interfaceCache.ContainsKey(parent.GUID))
                    meta = interfaceCache[parent.GUID];
                else
                {
                    meta = GetInterfaceBasicInformation(parent);

                    if (meta == null)
                        throw new TypeLoadException($"Interface '{parent.Name}' is used by other interfaces and don't marked by ConcreteAttribute");

                    interfaceCache.Add(parent.GUID, meta);
                }

                itemMeta.Parents.Add(meta);
            }

            if (itemMeta.Concrete == null)
                itemMeta.Concrete = ImplementInterface(itemMeta);

            foreach (var item in itemMeta.Parents.Where(e => e.Concrete == null))
                item.Concrete = ImplementInterface(item);

            return itemMeta;
        }

        #endregion

        #region mapping methods

        /// <summary>
        /// get reader columns list
        /// </summary>
        /// <param name="reader">data reader</param>
        /// <returns>return columns list</returns>
        internal static string[] GetColumns(IDataReader reader)
        {
            var names = new string[reader.FieldCount];

            for (int index = 0; index < reader.FieldCount; ++index)
                names[index] = reader.GetName(index);

            return names;
        }

        /// <summary>
        /// read current row from reader
        /// </summary>
        /// <param name="reader">data reader to read from</param>
        /// <param name="columns">data reader columns</param>
        /// <param name="instance">object instance to set data in</param>
        /// <param name="metaData">instance type metadata</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void ReadCurrentRow(IDataReader reader, string[] columns, object instance, InterfaceMetadata metaData)
        {
            for (int i = 0; i < columns.Length; ++i)
            {
                object value = reader.GetValue(i);
                if (value is DBNull) continue;

                PropertyMetadata property = metaData.GetProperty(columns[i], true);
                property?.SetValue(instance, ConvertType(value, property.Type));
            }
        }

        /// <summary>
        /// get entire result set from reader
        /// </summary>
        /// <param name="reader">data reader to read from</param>
        /// <param name="columns">data reader columns</param>
        /// <param name="list">list to save objects into</param>
        /// <param name="metaData">metadata for items in list</param>
        internal static void ReadSingleResultset(IDataReader reader, string[] columns, IList list, InterfaceMetadata metaData)
        {
            while (reader.Read())
            {
                object instance = Activator.CreateInstance(metaData.Concrete);

                ReadCurrentRow(reader, columns, instance, metaData);

                list.Add(instance);
            }
        }

        /// <summary>
        /// read multiple result set into object
        /// </summary>
        /// <param name="reader">data reader to read from</param>
        /// <param name="instance">object instance to set data in</param>
        /// <param name="metaData">instance type metadata</param>
        /// <param name="properties">array of perties in instance to map result sets according</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void ReadMultiResultset(IDataReader reader, object instance, InterfaceMetadata metaData, string[] properties)
        {
            for (int index = 0; ; ++index)
            {
                if (properties.Length <= index)
                    throw new ArgumentException("input properties are fewer than number of result set");

                PropertyMetadata property = metaData.GetProperty(properties[index], true);

                // check if result rest is a IEnumerable (only IList is supported)
                if (property.Type.IsGenericType && (property.Type.GetGenericTypeDefinition() == typeof(IList<>)))
                {
                    Type listGenericType = property.Type.GetGenericArguments()[0];

                    var list = Activator.CreateInstance(typeof(List<>).MakeGenericType(listGenericType)) as IList;

                    if (list == null)
                        throw new ArgumentException($"property '{property.Name}' should be of type IList or IList<>");

                    string[] columns = GetColumns(reader);
                    InterfaceMetadata propertyTypeMetadata = GetInterface(listGenericType);

                    ReadSingleResultset(reader, columns, list, propertyTypeMetadata);

                    property.SetValue(instance, list);
                }

                // if property type is interface then get one record from result set
                else if (property.Type.IsInterface && !property.Type.IsGenericType)
                {
                    if (reader.Read())
                    {
                        InterfaceMetadata interfaceType = GetInterface(property.Type);

                        object value = Activator.CreateInstance(interfaceType.Concrete);
                        string[] columns = GetColumns(reader);

                        ReadCurrentRow(reader, columns, value, interfaceType);

                        property.SetValue(instance, value);
                    }
                }
                // this case require a plain data type to work correctly
                else
                {
                    if (reader.Read())
                        property.SetValue(instance, ConvertType(reader[0], property.Type));
                }

                if (!reader.NextResult()) break;
            }
        }

#endregion

        #region helper methods

        /// <summary>
        /// Knuth hash function for distribution in hash tables
        /// </summary>
        /// <param name="value">string to hash</param>
        /// <returns>return hash value for input string</returns>
        public static ulong CalculateHash(string value)
        {
            ulong hashedValue = 3074457345618258791ul;

            for (int i = 0; i < value.Length; ++i)
            {
                hashedValue += value[i];
                hashedValue *= 3074457345618258799ul;
            }
            return hashedValue;
        }

        /// <summary>
        /// convert input value to type if necessary
        /// </summary>
        /// <param name="value">value to convert</param>
        /// <param name="type">type to convert to</param>
        /// <returns>return value converted to type</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object ConvertType(object value, Type type)
        {
            if (value.GetType() != type && MapManager.ConvertColumnTypeToPropertyTypeIfPossible)
            {
                if ((value as IConvertible) == null)
                    value = Convert.ChangeType(value, type);
                else
                    value = (value as IConvertible).ToType(type, null);
            }
            return value;
        }

        #endregion
    }
}
