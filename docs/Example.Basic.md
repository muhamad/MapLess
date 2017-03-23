## Basic Example

All examples will use [AdventureWorks Lite](https://github.com/muh00mad/AdventureWorksLite) database, so make sure to deploy it in your local server.

First create the following procedure
```sql
CREATE PROCEDURE GetAllCustomers
AS
BEGIN
    SELECT FirstName, LastName, CompanyName, EmailAddress
    FROM SalesLT.Customer
END
GO
```

Now to the example
```csharp
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using MapLess;

namespace Example1
{
   [Concrete]
   public interface ICustomer
   {
      string FirstName { get; set; }
      string LastName { get; set; }
      string CompanyName { get; set; }
      [Alias("EmailAddress")]
      string Email { get; set; }
   }
   
   public class Example
   {
      const string connectionString = @"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=AdventureWorksLT;Data Source=.;";
      public static void Main()
      {
            var db = new Db<SqlConnection>(connectionString, 100);

            IList<ICustomer> customers = null;

            using (var reader = db.ExecuteReader("GetAllEmployees"))
                customers = TypeMap<ICustomer>.ReadSingleResultset(reader);

           foreach(var customer in customers)
               Console.WriteLine("FirstName: {0}, LastName:{1}, CompanyName:{2}, Email:{3}\n", customer.FirstName, customer.LastName, customer.CompanyName, customer.Email);
      }
   }
}
```

The interface `ICustomer` represent a single row in the return resultset from `GetAllEmployees` procedure, the needed columns from resultset get mapped to corresponding property, However the `EmailAddress` column get mapped to `Email` property by using [Alias Attribute](Ref.Alias.md) which accept one or more alias for property.

The class [Db<>](Ref.Db.md) is optional and you can use any method you like to get the data.

The [TypeMap](Ref.TypeMap.md) is the heart of the mapper which do all the mapping between resultset and you interface.

