using DatabaseHelper;
using System;
using System.Data.SqlClient;

namespace SQLDatabaseHelper
{
    public class SQLDatabase : Database<SqlConnection>
    {
        public SQLDatabase(string connectionString, bool useSingletone = true) : base (connectionString, useSingletone)
        {

        }
    }
}
