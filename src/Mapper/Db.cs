using System;
using System.Data;

namespace MapLess
{
    /// <summary>
    /// database back-end
    /// </summary>
    /// <typeparam name="T">database connection</typeparam>
    public class Db<T> where T : IDbConnection, new()
    {
        #region fields

        private readonly T connection;
        private readonly int commandTimeOut = -1;

        #endregion

        #region constructor

        /// <summary>
        /// initialize new instance
        /// </summary>
        /// <param name="connectionString">connection string</param>
        public Db(string connectionString)
        {
            connection = new T();
            connection.ConnectionString = connectionString;
        }

        /// <summary>
        /// initialize new instance
        /// </summary>
        /// <param name="connectionString">connection string</param>
        /// <param name="commandTimeOut">command timeout</param>
        public Db(string connectionString, int commandTimeOut)
            : this(connectionString)
        {
            this.commandTimeOut = commandTimeOut;
        }

        #endregion

        #region public properties

        /// <summary>
        /// get command timeout
        /// </summary>
        public int CommandTimeOut
        {
            get { return commandTimeOut; }
        }

        /// <summary>
        /// get connection object
        /// </summary>
        public T Connection
        {
            get { return connection; }
        }
        
        #endregion

        #region private methods

        /// <summary>
        /// create command with input criteria
        /// </summary>
        /// <param name="type">command type</param>
        /// <param name="name">command target name</param>
        /// <param name="parameters">command parameters</param>
        /// <returns>return command object</returns>
        private IDbCommand CreateCommand(CommandType type, string name, params IDbDataParameter[] parameters)
        {
            IDbCommand command = connection.CreateCommand();
            command.CommandType = type;
            command.CommandText = name;
            command.CommandTimeout = commandTimeOut;

            foreach (var param in parameters)
            {
                if (param.Direction == ParameterDirection.Input && param.Value == null)
                    param.Value = DBNull.Value;

                command.Parameters.Add(param);
            }

            return command;
        }

        #endregion

        #region Instacne methods

        /// <summary>
        /// execute stored procedure and return number of affected rows
        /// </summary>
        /// <param name="storedProcedure">stored procedure name</param>
        /// <param name="parameters">procedure parameters</param>
        /// <returns>return number of affected rows</returns>
        public int ExecuteNonQuery(string storedProcedure, params IDbDataParameter[] parameters)
        {
            return ExecuteNonQuery(CommandType.StoredProcedure, storedProcedure, parameters);
        }

        /// <summary>
        /// execute command and return number of affected rows
        /// </summary>
        /// <param name="commandType">type of command</param>
        /// <param name="commandText">command name</param>
        /// <param name="parameters">command parameters</param>
        /// <returns>return number of affected rows</returns>
        public int ExecuteNonQuery(CommandType commandType, string commandText, params IDbDataParameter[] parameters)
        {
            if (string.IsNullOrEmpty(commandText)) throw new ArgumentNullException("commandText", "Parameter commandText cannot be null");

            using (IDbCommand command = CreateCommand(commandType, commandText, parameters))
            {
                try
                {
                    command.Connection.Open();
                    return command.ExecuteNonQuery();
                }
                catch
                {
                    throw;
                }
                finally
                {
                    command.Connection.Close();
                }
            }
        }

        /// <summary>
        /// execute stored procedure and return single value
        /// </summary>
        /// <param name="storedProcedure">stored procedure name</param>
        /// <param name="parameters">procedure parameters</param>
        /// <returns>return result value</returns>
        public object ExecuteScalar(string storedProcedure, params IDbDataParameter[] parameters)
        {
            return ExecuteScalar(CommandType.StoredProcedure, storedProcedure, parameters);
        }

        /// <summary>
        /// execute command and return single value
        /// </summary>
        /// <param name="commandType">type of command</param>
        /// <param name="commandText">command name</param>
        /// <param name="parameters">command parameters</param>
        /// <returns>return result value</returns>
        public object ExecuteScalar(CommandType commandType, string commandText, params IDbDataParameter[] parameters)
        {
            if (string.IsNullOrEmpty(commandText)) throw new ArgumentNullException("commandText", "Parameter commandText cannot be null");

            using (IDbCommand command = CreateCommand(commandType, commandText, parameters))
            {
                try
                {
                    command.Connection.Open();
                    return command.ExecuteScalar();
                }
                catch
                {
                    throw;
                }
                finally
                {
                    command.Connection.Close();
                }
            }
        }

        /// <summary>
        /// execute stored procedure and return data reader
        /// </summary>
        /// <param name="storedProcedure">stored procedure name</param>
        /// <param name="parameters">procedure parameters</param>
        /// <returns>return data reader object</returns>
        /// <remarks>DataReader opens using SequentialAccess and CloseConnection behavior</remarks>
        public IDataReader ExecuteReader(string storedProcedure, params IDbDataParameter[] parameters)
        {
            return ExecuteReader(CommandType.StoredProcedure, storedProcedure, parameters);
        }

        /// <summary>
        /// execute command and return data reader
        /// </summary>
        /// <param name="commandType">type of command</param>
        /// <param name="commandText">command name</param>
        /// <param name="parameters">command parameters</param>
        /// <returns>return data reader object</returns>
        /// <remarks>DataReader opens using SequentialAccess and CloseConnection behavior</remarks>
        public IDataReader ExecuteReader(CommandType commandType, string commandText, params IDbDataParameter[] parameters)
        {
            if (string.IsNullOrEmpty(commandText)) throw new ArgumentNullException("commandText", "Parameter commandText cannot be null");

            using (IDbCommand command = CreateCommand(commandType, commandText, parameters))
            {
                try
                {
                    command.Connection.Open();
                    return command.ExecuteReader(CommandBehavior.SequentialAccess | CommandBehavior.CloseConnection);
                }
                catch
                {
                    command.Connection.Close();
                    throw;
                }
            }
        }

        #endregion
    }
}
