## InterfaceMetadata class

Represent interface information

### Declaration
```csharp
internal class InterfaceMetadata
{
    private readonly Dictionary<ulong, PropertyMetadata> tblHashNames;

    public Guid Id { get; set; }
    public Type InterfaceType { get; set; }
    public string Name { get; set; }
    public Type Concrete { get; set; }
    public List<PropertyMetadata> Properties { get; set; }
    public List<InterfaceMetadata> Parents { get; set; }

    public PropertyMetadata GetProperty(string name, bool ignoreCase);
    public void UpdateMetadata();
}
```

### Methdos
```csharp
public PropertyMetadata GetProperty(string name, bool ignoreCase);
```
*Description*: retrieve property information by name.

`name`: property name.

`ignoreCase`: ignore character casing when searching for property.

```csharp
public void UpdateMetadata();
```
*Description*: this function populate `tblHashNames` field with all properties and their name hash for fast access.

### Properties
```csharp
public Guid Id { get; set; }
```
*Description*: interface id, from [`Type.GUID`](https://msdn.microsoft.com/en-us/library/system.type.guid(v=vs.110).aspx).

```csharp
public Type InterfaceType { get; set; }
```
*Description*: [`Type`](https://msdn.microsoft.com/en-us/library/system.type(v=vs.110).aspx) class for this interface.

```csharp
public string Name { get; set; }
```
*Description*: interface name.

```csharp
public Type Concrete { get; set; }
```
*Description*: concrete class [`type`](https://msdn.microsoft.com/en-us/library/system.type(v=vs.110).aspx) for this interface.

```csharp
public List<PropertyMetadata> Properties { get; set; }
```
*Description*: list of properties in this interface.

```csharp
public List<InterfaceMetadata> Parents { get; set; }
```
*Description*: list of interfaces that this interface implements.

## Fields
```csharp
private readonly Dictionary<ulong, PropertyMetadata> tblHashNames;
```
*Description*: provide fast access to properties using their name hash.


## Remarks

After the interface collect all information required then the function `UpdateMetadata` gets called to fill the hash table `tblHashNames` with properties twice, one with provided name, and another using lower case version.

The hash function is `CalculateHash` in `DataMap`.
