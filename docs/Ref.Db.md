## Db class

This class help connect to database and and execute command.

### Declaration
```csharp
public class Db<T> where T : IDbConnection, new()
{
    public Db(string connectionString);
    public Db(string connectionString, int commandTimeOut);

    public int CommandTimeOut { get; }
    public T Connection { get; }

    public int ExecuteNonQuery(string storedProcedure, params IDbDataParameter[] parameters);
    public int ExecuteNonQuery(CommandType commandType, string commandText, params IDbDataParameter[] parameters);

    public object ExecuteScalar(string storedProcedure, params IDbDataParameter[] parameters);
    public object ExecuteScalar(CommandType commandType, string commandText, params IDbDataParameter[] parameters);

    public IDataReader ExecuteReader(string storedProcedure, params IDbDataParameter[] parameters);
    public IDataReader ExecuteReader(CommandType commandType, string commandText, params IDbDataParameter[] parameters);
}
```

### Generic parameters
```csharp
T : IDbConnection, new()
```
*Description*: `T` is the database connection class and it must implement interface [`IDbConnection`](https://msdn.microsoft.com/en-us/library/system.data.idbconnection(v=vs.110).aspx) and have a public default constructor

### Methdos
```csharp
public Db(string connectionString);
public Db(string connectionString, int commandTimeOut);
```
*Description*: initialize new instance with connection string and optional command timeout.

`connectionString`: connection string to pass to underling `Connection` class.

`commandTimeOut`: number of seconds used to wait for command to finish executing.

```csharp
public int ExecuteNonQuery(string storedProcedure, params IDbDataParameter[] parameters);
public int ExecuteNonQuery(CommandType commandType, string commandText, params IDbDataParameter[] parameters);
```
*Description*: execute command against database and return number of affected rows.

`storedProcedure`: stored procedure name to execute.

`commandType`: type of command to execute.

`commandText`: command text.

`parameters`: command parameters.

```csharp
public object ExecuteScalar(string storedProcedure, params IDbDataParameter[] parameters);
public object ExecuteScalar(CommandType commandType, string commandText, params IDbDataParameter[] parameters);
```
*Description*: execute command against database and return the first column of the first row in the returned resultset.

`storedProcedure`: stored procedure name to execute.

`commandType`: type of command to execute.

`commandText`: command text.

`parameters`: command parameters.

```csharp
public IDataReader ExecuteReader(string storedProcedure, params IDbDataParameter[] parameters);
public IDataReader ExecuteReader(CommandType commandType, string commandText, params IDbDataParameter[] parameters);
```
*Description*: execute command against database and return its [`IDataReader`](https://msdn.microsoft.com/en-us/library/system.data.idatareader(v=vs.110).aspx).

`storedProcedure`: stored procedure name to execute.

`commandType`: type of command to execute.

`commandText`: command text.

`parameters`: command parameters.

The functions `ExecuteNonQuery`, `ExecuteScalar` and `ExecuteReader` are shortcut to function with same name defined in [`IDbCommand`](https://msdn.microsoft.com/en-us/library/system.data.idbcommand(v=vs.110).aspx).

### Properties

```csharp
public int CommandTimeOut { get; }
```
*Description*: get number of seconds to wait for command to finish executing.

```csharp
public T Connection { get; }
```
*Description*: get the connection object.

## Example

Please refer to [Basic Example](Example.Basic.md) in [Documentation](ReadMe.md#examples) page.
