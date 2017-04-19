## TypeMap class

This class help mapping [`IDataReader`](https://msdn.microsoft.com/en-us/library/system.data.idatareader(v=vs.110).aspx) content to interface marked with [`ConcreteAttribute`](Ref.Concrete.md).

### Declaration
```csharp
public class TypeMap<T>
{
    public static T CreateConcrete();
    public static T ReadCurrentRow(IDataReader reader);
    public static IList<T> ReadSingleResultset(IDataReader reader);
    public static T ReadMultiResultset(IDataReader reader, params string[] properties);
}
```

### Generic parameters
```csharp
T
```
*Description*: `T` is an interface that marked with [`ConcreteAttribute`](Ref.Concrete.md).

### Methdos
```csharp
public static T CreateConcrete();
```
*Description*: create object that implement the interface.


```csharp
public static T ReadCurrentRow(IDataReader reader);
```
*Description*: read row at current position from input data reader and return active object.

`reader`: data reader with [`Read`](https://msdn.microsoft.com/en-us/library/system.data.idatareader.read(v=vs.110).aspx) function called before this function.


```csharp
public static IList<T> ReadSingleResultset(IDataReader reader);
```
*Description*: read all rows from input data reader and return list of rows.

`reader`: active [`IDataReader`](https://msdn.microsoft.com/en-us/library/system.data.idatareader(v=vs.110).aspx) to call its [`Read`](https://msdn.microsoft.com/en-us/library/system.data.idatareader.read(v=vs.110).aspx) function to pull the data.


```csharp
public static T ReadMultiResultset(IDataReader reader, params string[] properties);
```
*Description*: read all result sets from input data reader and bound them to object.

`reader`: [`IDataReader`](https://msdn.microsoft.com/en-us/library/system.data.idatareader(v=vs.110).aspx) to read data.
`properties`: list of properties to bound the resultset.


Please refer to Examples section in [Documentation](ReadMe.md#examples) page.
