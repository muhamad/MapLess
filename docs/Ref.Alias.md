## Alias Attribute

Represent one or more alternate name for property

### Declaration
```csharp
[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
public sealed class AliasAttribute : Attribute
{
    public AliasAttribute(string name, params string[] names);

    public IList<string> GetNames();
}
```

### Methdos
```csharp
public AliasAttribute(string name, params string[] names);
```
*Description*: The constructor used to initialize the attribute which requires at least one alternate name to be specified

`name`: alternate name, must be non-empty

`names`: list of other names

```csharp
public IList<string> GetNames();
```
*Description*: get list of alternate names

## Example

You apply this attribute on properties to define other names to bind with, for example:
```csharp
public interface IUser
{
    [Alias("CustomerId", "SalesPersonId")]
    int UserId { get; set; }
}
```
Here the property `UserId` will bind to result set column with name `UserId`, `CustomerId` or `SalesPersonId`.
