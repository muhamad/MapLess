## PropertyMetadata class

Encapsulate property information

### Declaration
```csharp
internal class PropertyMetadata
{
    public string Name { get; set; }
    public Type Type { get; set; }
    public IList<string> Alias { get; set; }
    public MethodInfo SetMethod { get; set; }
    public MethodInfo GetMethod { get; set; }
    public Action<object, object> SetValue { get; set; }
    public Func<object, object> GetValue { get; set; }

    internal void CreateFastGetSetMethod(Type interfaceType);
}
```

### Methdos
```csharp
internal void CreateFastGetSetMethod(Type interfaceType);
```
*Description*: create getter/setter functions for input interface and assign them to property `GetValue` and `SetValue`.

`interfaceType`: interface to create property setter and getter.

### Properties
```csharp
public string Name { get; set; }
```
*Description*: property name

```csharp
public Type Type { get; set; }
```
*Description*: property type

```csharp
public IList<string> Alias { get; set; }
```
*Description*: alias list used with property

```csharp
public MethodInfo SetMethod { get; set; }
```
*Description*: set method as returned from [`PropertyInfo.GetSetMethod()`](https://msdn.microsoft.com/en-us/library/scfx0019(v=vs.110).aspx)

```csharp
public MethodInfo GetMethod { get; set; }
```
*Description*: get method as returned from [`PropertyInfo.GetGetMethod()`](https://msdn.microsoft.com/en-us/library/e17dw503(v=vs.110).aspx)

```csharp
public Action<object, object> SetValue { get; set; }
```
*Description*: property setter created by `CreateFastGetSetMethod`.

```csharp
public Func<object, object> GetValue { get; set; }
```
*Description*: property getter created by `CreateFastGetSetMethod`.
