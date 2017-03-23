## What is Mapless?

[MapLess](https://github.com/muh00mad/MapLess/) is a micro-ORM library that map database result<sup>[1](#note1)</sup> to interfaces.

It been said that [Data Access Layer](https://en.wikipedia.org/wiki/Data_access_layer) should only expose interfaces that map to database result, and this is what this library do.

Interfaces defined for mapping can be implemented by user or implemented dynamically and fear not the caching mechanism inside MapLess makes the execution as fast as if you implemented them by yourself.

The mapping time for 100,000 records with 41 columns of different datatypes is from 700 milliseconds to 1.3 second<sup>[2](#note2)</sup>.

The internal implementation for interfaces is optimizated for each interface and that lead to small object size at runtime - in other words each interface with its dependencies has its own concrete.

---
<sup><a name="#note1">1</a></sup>A database result is any result produced from executing a statement against database.

<sup><a name="#note2">2</a></sup>This time is purly mapping the data to the properties execluding fetch time.
