## MapManager class

This class contains function and properties that affect the behavior of [Mapless](https://github.com/muh00mad/MapLess).

### Declaration
```csharp
public static class MapManager
{
    public static void CacheAssembly(Assembly assembly);
    public static bool ConvertColumnTypeToPropertyTypeIfPossible { get; set; }
}
```

### Methdos

```csharp
public static void CacheAssembly(Assembly assembly);
```
*Description*: Search assembly for interfaces with [`ConcreteAttribute`](Ref.Concrete.md) and save them in cache.

### Properties

```csharp
public static bool ConvertColumnTypeToPropertyTypeIfPossible { get; set; }
```
*Description*: Indicate that type conversion may happened when mapping column value from database to property.

### Hints
If you want to use internal interfaces with Mapless then you need to give it permission to use them; by adding the following line to your `Assembly.cs`
```csharp
[assembly: InternalsVisibleTo("DynamicContainers")]
```
