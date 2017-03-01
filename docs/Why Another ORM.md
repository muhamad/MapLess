## Why Another ORM?

[MapLess](../) philosophy based on the idea that [Data Access Layer](https://en.wikipedia.org/wiki/Data_access_layer) should only expose interfaces with fields that maps to database columns which result from executing an action.

The interface itself have two options, either you mark it with special attribute and a concrete will be created for it with all dependencies or you provide a concrete to be used for it. both options have their usage.

Also [MapLess](../) support mapping from a Single/Multiple [Result Set(s)](https://en.wikipedia.org/wiki/Result_set) with simple call. It is very ultra fast, for 100,000 records and runtime concrete generation the mapping takes averege 1.3<sup>[1](#note1)</sup> seconds and even that time can be optimizing.


---
<a name="note1">1</a>: the concrete generation is cached before calling the mapping and the time for fetching data from database excluded.
