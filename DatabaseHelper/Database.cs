using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;



namespace DatabaseHelper
{
    public abstract class Database<TConnection> : IDisposable
    where TConnection : DbConnection , new()
    {
        protected readonly bool _useSingleTone;
        protected TConnection _connection;
        protected IDbTransaction _transaction;


        public string ConnectionString { get; private set; }

        public Database(string connectionString, bool useSingleTone = true)
        {
            ConnectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
            _useSingleTone = useSingleTone;
        }

        public TConnection GetConnection()
        {
            if (_connection == null || !_useSingleTone)
            {
                _connection = new TConnection() ;
                _connection.ConnectionString = ConnectionString;
            }
            return _connection;
        }

        public void BeginTransaction()
        {
            if (!_useSingleTone) throw new Exception("Transaction is supported only in singletone mode!");
            if (_transaction != null) throw new Exception("Transaction is already started!");

            if (!GetConnection().State.HasFlag(ConnectionState.Open))
            {
                GetConnection().Open();
            }
            _transaction = GetConnection().BeginTransaction();
        }

        public void CommitTransaction()
        {
            if (_transaction == null) throw new Exception("Transaction is not started!");

            _transaction.Commit();
            _transaction = null;
        }

        public void RollBackTransaction()
        {
            if (_transaction == null) throw new Exception("Transaction is not started!");

            _transaction.Rollback();
            _transaction = null;
        }

        public IDbCommand GetCommand(string commandText, CommandType commandType, params IDataParameter[] parameters)
        {
            IDbCommand command = GetConnection().CreateCommand();
            command.CommandText = commandText;
            command.CommandType = commandType;
            foreach (var p in parameters)
                command.Parameters.Add(p);
            if (_transaction != null)
                command.Transaction = _transaction;

            return command;
        }

        public  IDbCommand GetCommand(string commandText, params IDataParameter[] parameters)
        {
            return GetCommand(commandText, CommandType.Text, parameters);
        }

        public int ExecuteNonQuery(string commandText, CommandType commandType, params IDataParameter[] parameters )
        {
            var command = GetCommand(commandText, commandType, parameters);
            try
            {
                if (!command.Connection.State.HasFlag(ConnectionState.Open))
                {
                    command.Connection.Open();
                }
                return command.ExecuteNonQuery();
            }
            finally
            {
               if (!_useSingleTone) command.Connection.Close();
            }
        }

        public int ExecuteNonQuery(string commandText,  params IDataParameter[] parameters)
        {
           return ExecuteNonQuery(commandText, CommandType.Text, parameters);
        }

        public object ExecuteScalar(string commandText, CommandType commandType, params IDataParameter[] parameters)
        {
            var command = GetCommand(commandText, commandType, parameters);
            try
            {
                if (!command.Connection.State.HasFlag(ConnectionState.Open))
                {
                    command.Connection.Open();
                }
                return command.ExecuteScalar();
            }
            finally 
            {
                if (!_useSingleTone) command.Connection.Close();
            }
        }

        public object ExecuteScalar(string commandText,  params IDataParameter[] parameters)
        {
            return ExecuteScalar(commandText, CommandType.Text, parameters);
        }

        public DataTable GetDataTble(string commandText, CommandType commandType, params IDataParameter[] parameters)
        {
            var command = GetCommand(commandText, commandType, parameters);
            try
            {
                if (!command.Connection.State.HasFlag(ConnectionState.Open))
                {
                    command.Connection.Open();
                }
                IDataReader reader = command.ExecuteReader();
                DataTable table = new DataTable();
                table.Load(reader);
                return table;
            }
            finally
            {
                if (!_useSingleTone) command.Connection.Close();
            }
        }

        public DataTable GetDataTable(string commandText, params IDataParameter[] parameters)
        {
            return GetDataTble(commandText, CommandType.Text, parameters);
        }

        public IDataReader ExecuteReader(string commandText, CommandType commandType, params IDataParameter[] parameters)
        {
            var command = GetCommand( commandText,  commandType,  parameters);

            if (!command.Connection.State.HasFlag(ConnectionState.Open))
            {
                command.Connection.Open();
            }
            return command.ExecuteReader(!_useSingleTone ? CommandBehavior.CloseConnection : CommandBehavior.Default);
        }

        public IDataReader ExecuteReader(string commandText, params IDataParameter[] parameters)
        {
            return ExecuteReader(commandText, CommandType.Text, parameters);
        }

        public void Dispose()
        {
            if (_connection != null)
            {
                _connection.Close();
            }
            GC.SuppressFinalize(this);
        }
    }
}
