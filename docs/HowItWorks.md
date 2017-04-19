## How it Works

There are two stages:
* Caching interfaces.
* Mapping data to objects.

### Caching interfaces

This is the kick start stage where you first call `MapManager.CacheAssembly` which in turn calls the internal function `DataMap.CacheAssembly`.

The `DataMap.CacheAssembly` collects all interfaces that have `ConcreteAttribute` then it build a dependencies tree for each interface. when collecting all interfaces we get its basic information then if that interface have user-defined concrete it will be used otherwise a concrete type is created for this interface and all its dependencies flatten together in the target class.

The concrete class that gets created have explicit implementation for interfaces, which avoids name clashes when more than one interface have elements with same name.

The last step is adding the interface to the cache (actually this step happens earlier to avoid adding it more than once when resolving dependencies).

The functions related to this step in `DataMap` are `CacheAssembly`, `GetInterfaceBasicInformation`, `GetInterfaceProperties` and `ImplementInterface`.

### Mapping data to objects

After caching is complete; the interface `TypeMap` cache the interface metadata on first access in the field `metaData` which save the result of `DataMap.GetInterface`.

Now for each function in `TypeMap` there is a corresponding function in `DataMap` that do the actual work.

The `metaData` property is of type `InterfaceMetadata` which have interface information.

The `InterfaceMetadata` have a list of `PropertyMetadata` class which define information about interface properties.

The functions related to this step in `DataMap` are `GetInterface`, `ReadCurrentRow`, `ReadSingleResultset` and `ReadMultiResultset`
