## How to use

In [MapLess](https://www.github.com/muh00mad/MapLess/) the namespace `MapLess` contains all public classes you need to use (don't worry if you didn't get everything from description, they will be explained more as you go on):
* [`Db<>`](Ref.Db.md): this is an optinal class to connect to database and get the result you want to map.
* [`AliasAttribute`](Ref.Alias.md): this is an optional attribute you apply to property to give it multiple names.
* [`ConcreteAttribute`](Ref.Concrete.md): this attribute mark interface to be used by MapLess.
* [`TypeMap<>`](Ref.TypeMap.md): this class used to map interface to database result.
* [`MapManager`](Ref.MapManager.md): this class contains helper functions that affect the behavior of the library.


### A word before start

Before diving into examples you need to know that only [`ConcreteAttribute`](Ref.Concrete.md) and [`TypeMap<>`](Ref.TypeMap.md) are the only essential components you need to use, however if you want to go with this option you might need to consider executing [`IDbCommand.ExecuteReader`](https://msdn.microsoft.com/en-us/library/68etdec0(v=vs.110).aspx) with option [`CommandBehavior.SequentialAccess`](https://msdn.microsoft.com/en-us/library/system.data.commandbehavior(v=vs.110).aspx) for best performance.

### Basic Example

Please refer to [Basic Example](Example.Basic.md) from Example section in [Documentation](ReadMe.md) page.
