## Concrete Attribute

Mark interface to be implemented by user-defined concrete class or automatic concrete class

### Declaration
```csharp
[AttributeUsage(AttributeTargets.Interface, Inherited = false, AllowMultiple = false)]
public sealed class ConcreteAttribute : Attribute
{
    public ConcreteAttribute();
    public ConcreteAttribute(Type type);

    public Type Type { get; }
}
```

### Methdos
```csharp
public ConcreteAttribute();
public ConcreteAttribute(Type type);
```
*Description*: the constructor will mark the interface to be used by MapLess, However the default constructor will implement the interface dynamically, and the other constructor thw user will provide the concrete implementation.

`type`: user-defined type implementation for marker interface. If *type* is null `ArgumentNullException` will throw.

### Properties
```csharp
public Type Type { get; }
```
*Description*: get user-defined type implementation.

## Example

```csharp
[Concrete]
public interface IUserId
{
    int UserId { get; set; }
}
```

In above example we declare interface `IUserId` which can be used by MapLess and will be implemented at runtime.

```csharp
internal class User : IUser
{
    public int UserId { get; set; }
    public string FirstName { get; set; }
}

[Concrete(typeof(User))]
public interface IUser
{
    int UserId { get; set; }
    string FirstName { get; set; }
}
```

In above example we declare interface `IUser` and corresponding concrete class `User`, the interface can be used by MapLess and at runtime the `User` class will be used for `IUser`.
