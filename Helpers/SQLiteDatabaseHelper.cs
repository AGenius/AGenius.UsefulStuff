using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using Dapper.Contrib.Extensions;
using Dapper;
using System.Reflection;
using System.Runtime.Serialization;
using System.Data.Common;
using System.Text;
//using System.Transactions;

namespace AGenius.UsefulStuff.Helpers
{
    /// <summary>
    /// This class is used to help encapsulate the use of Dapper objects to access SQL Lite
    /// </summary>
    public class SQLiteDatabaseHelper : IDisposable
    {
        /// <summary>The SQL Lite database file name used</summary>
        public string dbFilePath = "default.db3";
        /// <summary>Holds the db File Name</summary>
        public string dbName { get; set; }
        /// <summary>Provides access to the  Connection string in use</summary>
        public string DBConnectionString
        {
            get; set;
        }
        /// <summary>Initializes a new instance of the <see cref="SQLiteDatabaseHelper"/> class </summary>
        public SQLiteDatabaseHelper()
        {
            DBConnectionString = "Data Source=" + dbFilePath;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="SQLiteDatabaseHelper"/> class using the supplied database file name  
        /// </summary>
        /// <param name="DatabaseName">The SQLite database file name(full path)</param>
        public SQLiteDatabaseHelper(string DatabaseName)
        {
            if (!DatabaseName.EndsWith(".db3"))
            {
                DatabaseName += ".db3";
            }
            dbFilePath = DatabaseName;
            DBConnectionString = String.Format("Data Source={0}", DatabaseName);
            dbName = System.IO.Path.GetFileName(DatabaseName).Replace(".db3", "");
        }
        public void Dispose()
        {
            //   throw new NotImplementedException();
        }
        private string _lastError;

        /// <summary>Read ALL records for an entity </summary>
        /// <typeparam name="TENTITY">Entity Object type</typeparam>
        /// <returns><see cref="IList{T}"/> containing the Entity records</returns>
        public IList<TENTITY> ReadALL<TENTITY>() where TENTITY : class
        {
            if (string.IsNullOrEmpty(DBConnectionString))
            {
                throw new ArgumentException("Connection String not set");
            }
            try
            {
                using (IDbConnection db = new SQLiteConnection(DBConnectionString))
                {
                    return db.GetAll<TENTITY>().ToList();
                }
            }
            catch (DbException ex)
            {
                throw new DatabaseAccessHelperException(ex.Message);
            }

        }

        /// <summary>Return a single record for the specified ID </summary>
        /// <typeparam name="TENTITY">Entity Object type</typeparam>
        /// <param name="ID"><see cref="int"/> ID of the record to read</param>
        /// <returns>The Entity record requested or null</returns>
        public TENTITY ReadRecord<TENTITY>(int? ID) where TENTITY : class
        {
            try
            {
                if (string.IsNullOrEmpty(DBConnectionString))
                {
                    throw new ArgumentException("Connection String not set");
                }
                if (!ID.HasValue || ID.Value <= 0)
                {
                    throw new ArgumentException("Invalid ID specified");
                }
                using (IDbConnection db = new SQLiteConnection(DBConnectionString))
                {
                    return db.Get<TENTITY>(ID);
                }
            }
            catch (DbException ex)
            {
                throw new DatabaseAccessHelperException(ex.Message);
            }
        }

        /// <summary>Return a single record for the specified ID </summary>
        /// <typeparam name="TENTITY">Entity Object type</typeparam>
        /// <param name="ID"><see cref="string"/> ID of the record to read</param>
        /// <returns>The Entity record requested or null</returns>
        public TENTITY ReadRecord<TENTITY>(string ID) where TENTITY : class
        {
            try
            {
                if (string.IsNullOrEmpty(DBConnectionString))
                {
                    throw new ArgumentException("Connection String not set");
                }
                if (string.IsNullOrEmpty(ID))
                {
                    throw new ArgumentException("Invalid ID specified");
                }
                using (IDbConnection db = new SQLiteConnection(DBConnectionString))
                {
                    return db.Get<TENTITY>(ID);
                }
            }
            catch (DbException ex)
            {
                throw new DatabaseAccessHelperException(ex.Message);
            }
        }

        /// <summary>Return a single record for the specified ID </summary>
        /// <typeparam name="TENTITY">Entity Object type</typeparam>
        /// <param name="ID">a Guid representing the ID of the record</param>
        /// <returns>The Entity record requested or null</returns>
        public TENTITY ReadRecord<TENTITY>(Guid ID) where TENTITY : class
        {
            try
            {
                if (string.IsNullOrEmpty(DBConnectionString))
                {
                    _lastError = "Connection String not set";
                    throw new ArgumentException(_lastError);
                }
                if (string.IsNullOrEmpty(ID.ToString()))
                {
                    _lastError = "Invalid ID specified";
                    throw new ArgumentException(_lastError);
                }
                using (IDbConnection db = new SQLiteConnection(DBConnectionString))
                {
                    return db.Get<TENTITY>(ID.ToString());
                }
            }
            catch (DbException ex)
            {
                _lastError = ex.Message;
                throw new DatabaseAccessHelperException(ex.Message);
            }
        }

        /// <summary>Return an object that matches the selection criteria </summary>
        /// <typeparam name="TENTITY">Entity Object type</typeparam>
        /// <param name="fieldValue">The Value of the Key field to find</param>
        /// <param name="keyFieldName">The Key FieldName</param>
        /// <param name="operatorType">Comparison Operator - Default is Equals</param>
        /// <returns>The Entity record requested or null</returns>
        public TENTITY ReadRecord<TENTITY>(string keyFieldName, string fieldValue, string operatorType = "=") where TENTITY : class
        {
            try
            {
                string TableName = GetTableName<TENTITY>();
                if (string.IsNullOrEmpty(TableName))
                {
                    _lastError = "Invalid Table Name";
                    throw new ArgumentException(_lastError);
                }
                if (string.IsNullOrEmpty(keyFieldName) || fieldValue == null)
                {
                    _lastError = "Missing FieldName or Value for search";
                    throw new ArgumentException(_lastError);
                }
                if (string.IsNullOrEmpty(DBConnectionString))
                {
                    _lastError = "Connection String not set";
                    throw new ArgumentException(_lastError);
                }
                string sWhere = $"WHERE {keyFieldName} {operatorType} '{fieldValue}' ";

                string sSQL = $"SELECT * FROM {TableName} {sWhere}";
                // var Results = null;
                using (IDbConnection db = new SQLiteConnection(DBConnectionString))
                {
                    return db.Query<TENTITY>(sSQL).SingleOrDefault();
                }
            }
            catch (DbException ex)
            {
                _lastError = ex.Message;
                throw new DatabaseAccessHelperException(ex.Message);
            }
        }

        /// <summary>Return an object that matches the specified key field </summary>
        /// <typeparam name="TENTITY">Entity Object type</typeparam>
        /// <param name="keyFieldName"><see cref="string"/> representing the KeyField name in the table</param>
        /// <param name="fieldValue"><see cref="int"/> representing the ID of the required record</param>
        /// <param name="operatorType">Comparison Operator - Default is Equals</param>
        /// <returns>The Entity record requested or null</returns>
        public TENTITY ReadRecord<TENTITY>(string keyFieldName, int fieldValue, string operatorType = "=") where TENTITY : class
        {
            try
            {
                string TableName = GetTableName<TENTITY>();
                if (string.IsNullOrEmpty(TableName))
                {
                    _lastError = "Invalid Table Name";
                    throw new ArgumentException(_lastError);
                }
                if (string.IsNullOrEmpty(keyFieldName))
                {
                    _lastError = "Missing FieldName or Value for search";
                    throw new ArgumentException(_lastError);
                }
                if (string.IsNullOrEmpty(DBConnectionString))
                {
                    _lastError = "Connection String not set";
                    throw new ArgumentException(_lastError);
                }
                string sWhere = $"WHERE {keyFieldName} {operatorType} '{fieldValue}' ";
                string sSQL = $"SELECT * FROM {TableName} {sWhere}";
                // var Results = null;
                using (IDbConnection db = new SQLiteConnection(DBConnectionString))
                {
                    return db.Query<TENTITY>(sSQL).SingleOrDefault();
                }
            }
            catch (DbException ex)
            {
                _lastError = ex.Message;
                throw new DatabaseAccessHelperException(ex.Message);
            }
        }

        /// <summary>Return a single objects that matches the selection criteria </summary>
        /// <typeparam name="TENTITY">Entity Object type</typeparam>
        /// <param name="Where">criteria</param>
        /// <returns>The Entity record requested or null</returns>
        public TENTITY ReadRecordWithWhere<TENTITY>(string Where = "")
            where TENTITY : class
        {
            try
            {
                string TableName = GetTableName<TENTITY>();
                if (string.IsNullOrEmpty(TableName))
                {
                    throw new ArgumentException("Invalid Table Name");
                }

                if (string.IsNullOrEmpty(DBConnectionString))
                {
                    throw new ArgumentException("Connection String not set");
                }
                string sWhere = string.IsNullOrEmpty(Where) ? "" : $"WHERE {Where}";
                // as this metod is to read 1 record we need to add a TOP 1
                string sSQL = $"SELECT * FROM {TableName} {sWhere} LIMIT 1";
                // var Results = null;
                using (IDbConnection db = new SQLiteConnection(DBConnectionString))
                {
                    return db.Query<TENTITY>(sSQL).SingleOrDefault();
                }
            }
            catch (DbException ex)
            {
                throw new DatabaseAccessHelperException(ex.Message);
            }
        }

        /// <summary>Return a list of objects from a stored procedure with parameters</summary>
        /// <typeparam name="TENTITY">Entity Object type</typeparam>
        /// <param name="SprocName">Stored Procedure Name</param>
        /// <param name="Params"> DynamicParamters collection</param>
        /// <returns><see cref="IList{T}"/> containing the Entity records</returns>
        /// <remarks>DynamicParameters param = new DynamicParameters();
        ///param.Add( "@@Name" , obj.Name );
        ///        param.Add( "@City" , obj.City );
        ///        param.Add( "@Address" , obj.Address );</remarks>
        public IList<TENTITY> ReadRecords<TENTITY>(string SprocName, DynamicParameters Params)
            where TENTITY : class
        {
            try
            {
                if (string.IsNullOrEmpty(DBConnectionString))
                {
                    throw new ArgumentException("Connection String not set");
                }

                using (IDbConnection db = new SQLiteConnection(DBConnectionString))
                {
                    return db.Query<TENTITY>(SprocName, Params, commandType:
                    CommandType.StoredProcedure).ToList();
                }
            }
            catch (DbException ex)
            {
                throw new DatabaseAccessHelperException(ex.Message);
            }
        }

        /// <summary>Return a list of objects using the SQL Query string supplied</summary>
        /// <typeparam name="TENTITY">Entity Object type</typeparam>
        /// <param name="SQLQuery">The SQL Query to be used</param>        
        /// <returns><see cref="IList{T}"/> containing the Entity records</returns>
        /// <remarks>Use the token [tablename] to replace with the correct tablename for the requested entity</remarks>
        public IList<TENTITY> ReadRecordsSQL<TENTITY>(string SQLQuery = "") where TENTITY : class
        {
            try
            {
                if (string.IsNullOrEmpty(DBConnectionString))
                {
                    throw new ArgumentException("Connection String not set");
                }
                using (IDbConnection db = new SQLiteConnection(DBConnectionString))
                {
                    return db.Query<TENTITY>(SQLQuery).ToList();
                }
            }
            catch (DbException ex)
            {
                throw new DatabaseAccessHelperException(ex.Message);
            }
        }

        /// <summary>Return a list of objects that match the selection criteria </summary>
        /// <typeparam name="TENTITY">Entity Object type</typeparam>
        /// <param name="Where">criteria</param>
        /// <returns><see cref="IList{T}"/> containing the Entity records</returns>
        public IList<TENTITY> ReadRecords<TENTITY>(string Where = "") where TENTITY : class
        {
            try
            {
                string TableName = GetTableName<TENTITY>();
                if (string.IsNullOrEmpty(TableName))
                {
                    throw new ArgumentException("Invalid Table Name");
                }

                if (string.IsNullOrEmpty(DBConnectionString))
                {
                    throw new ArgumentException("Connection String not set");
                }
                string sWhere = string.IsNullOrEmpty(Where) ? "" : $"WHERE {Where}";

                string sSQL = $"SELECT * FROM {TableName} {sWhere}";
                // var Results = null;
                using (IDbConnection db = new SQLiteConnection(DBConnectionString))
                {
                    return db.Query<TENTITY>(sSQL).ToList();
                }

            }
            catch (DbException ex)
            {
                throw new DatabaseAccessHelperException(ex.Message);
            }
        }

        /// <summary>Replicate the MSACCESS DLookup feature.
        /// This will perform a simple lookup of a value from a field based on specific 
        /// selection criteria</summary>
        /// <param name="FieldName">Field Name for the return value</param>
        /// <param name="tableName">Table or View name to use</param>
        /// <param name="Criteria">The Where criteria (optional)</param>
        /// <remarks>WHERE statement is not required in the Criteria specified</remarks>
        public object DLookup(string FieldName, string tableName, string Criteria = "")
        {
            try
            {
                if (string.IsNullOrEmpty(DBConnectionString))
                {
                    _lastError = "Connection String not set";
                    throw new ArgumentException(_lastError);
                }

                if (string.IsNullOrEmpty(tableName))
                {
                    _lastError = "Invalid Table Name";
                    throw new ArgumentException(_lastError);
                }
                if (string.IsNullOrEmpty(FieldName))
                {
                    _lastError = "Missing FieldName or Value for return";
                    throw new ArgumentException(_lastError);
                }
                string sWhere = string.IsNullOrEmpty(Criteria) ? "" : $"WHERE {Criteria}";

                string sSQL = $"SELECT {FieldName} FROM {tableName} {sWhere}";
                using (IDbConnection db = new SQLiteConnection(DBConnectionString))
                {
                    return db.ExecuteScalar(sSQL);
                }
            }
            catch (DbException ex)
            {
                _lastError = ex.Message;
                throw new DatabaseAccessHelperException(ex.Message);
            }
        }

        /// <summary>Replicate the MSACCESS DCount feature.
        /// This will perform a simple count of a number of rows in the table/view with the provided criteria
        /// selection criteria <br />WHERE statement is not required in the Criteria specified</summary>        
        /// <param name="tableName">Table or View name to use</param>
        /// <param name="Criteria">The Where criteria (optional)</param>
        /// <returns><see cref="long"/> number of records counted</returns>
        /// <remarks>WHERE statement is not required in the Criteria specified</remarks>
        public long DCount(string tableName, string Criteria = "")
        {
            try
            {
                if (string.IsNullOrEmpty(DBConnectionString))
                {
                    _lastError = "Connection String not set";
                    throw new ArgumentException(_lastError);
                }

                if (string.IsNullOrEmpty(tableName))
                {
                    _lastError = "Invalid Table Name";
                    throw new ArgumentException(_lastError);
                }
                string sWhere = string.IsNullOrEmpty(Criteria) ? "" : $"WHERE {Criteria}";

                string sSQL = $"SELECT count(*) FROM {tableName} {sWhere}";
                using (IDbConnection db = new SQLiteConnection(DBConnectionString))
                {
                    return db.Query<int>(sSQL).FirstOrDefault();
                }
            }
            catch (DbException ex)
            {
                _lastError = ex.Message;
                throw new DatabaseAccessHelperException(ex.Message);
            }
        }

        /// <summary>Insert a new Entity record</summary>
        /// <typeparam name="TENTITY">Entity Object type</typeparam>
        /// <param name="Record">The Record to insert</param>
        /// <returns><see cref="long"/> ID of the inserted record</returns>
        public long InsertRecord<TENTITY>(TENTITY Record) where TENTITY : class
        {
            try
            {
                if (string.IsNullOrEmpty(DBConnectionString))
                {
                    throw new ArgumentException("Connection String not set");
                }

                using (IDbConnection db = new SQLiteConnection(DBConnectionString))
                {
                    return db.Insert(Record);
                }

            }
            catch (DbException ex)
            {
                throw new DatabaseAccessHelperException(ex.Message);
            }
        }
        /// <summary>Insert a number of records</summary>
        /// <param name="tableName">Table Name to insert records into</param>
        /// <param name="recordsCollection">The list of records to insert</param>
        public void InsertRecords(string tableName, List<Dictionary<string, object>> recordsCollection)
        {
            StringBuilder sbInsert = new StringBuilder();

            try
            {
                if (string.IsNullOrEmpty(DBConnectionString))
                {
                    throw new ArgumentException("Connection String not set");
                }

                // Build columns list
                string colNames = "";
                foreach (var cols in recordsCollection[0])
                {
                    if (!string.IsNullOrEmpty(colNames))
                    {
                        colNames += ", ";
                    }
                    colNames += $"[{cols.Key}]";
                }

                foreach (var record in recordsCollection)
                {
                    string rowValues = "";
                    foreach (var item in record)
                    {
                        if (!string.IsNullOrEmpty(rowValues))
                        {
                            rowValues += ", ";
                        }
                        if (item.Value.GetType().Name.ToLower() == "string")
                        {

                            rowValues += $"'{item.Value.ToString().Replace("'", "''")}'";
                        }
                        else
                        {
                            rowValues += item.Value.GetType().Name == "DBNull" ? "Null" : item.Value;
                        }
                    }
                    sbInsert.AppendLine($"INSERT INTO [{tableName}] ({colNames}) VALUES ({rowValues});");
                }

                using (var db = new SQLiteConnection(DBConnectionString))
                {
                    db.Open();

                    using (var transaction = db.BeginTransaction())
                    {
                        var affectedRows = db.Execute(sbInsert.ToString());

                        transaction.Commit();
                    }
                }
            }
            catch (DbException ex)
            {
                throw new DatabaseAccessHelperException(ex.Message);
            }
        }
        /// <summary>Read ALL records for an entity </summary>
        /// <param name="tableName">Table name to query</param>
        /// <returns><see cref="DataTable"/> containing the Entity records</returns>
        public DataTable ReadDataTable(string tableName)
        {
            if (string.IsNullOrEmpty(DBConnectionString))
            {
                throw new ArgumentException("Connection String not set");
            }
            try
            {
                using (var db = new SQLiteConnection(DBConnectionString))
                {
                    db.Open();
                    SQLiteCommand dbCommand = db.CreateCommand();
                    dbCommand.CommandText = "SELECT * FROM " + tableName;

                    SQLiteDataReader executeReader = dbCommand.ExecuteReader(CommandBehavior.SingleResult);
                    DataTable dt = new DataTable();
                    dt.Load(executeReader); // <-- FormatException
                    db.Close();
                    return dt;
                }

            }
            catch (DbException ex)
            {
                throw new DatabaseAccessHelperException(ex.Message);
            }

        }
        /// <summary>Execute an SQL Statement </summary>
        /// <param name="sqlCmd">String holding the SQL Command</param>
        public void ExecuteSQL(string sqlCmd)
        {
            try
            {
                if (string.IsNullOrEmpty(DBConnectionString))
                {
                    throw new ArgumentException("Connection String not set");
                }

                using (IDbConnection db = new SQLiteConnection(DBConnectionString))
                {
                    db.Execute(sqlCmd);
                }

            }
            catch (DbException ex)
            {
                throw new DatabaseAccessHelperException(ex.Message);
            }
        }

        /// <summary>Execute an SQL Statement </summary>
        /// <param name="sqlCmd">String holding the SQL Command</param>
        public string ExecuteScalar(string sqlCmd)
        {
            SQLiteConnection cnn = new SQLiteConnection(DBConnectionString);
            cnn.Open();
            SQLiteCommand mycommand = new SQLiteCommand(cnn) { CommandText = sqlCmd };
            object value = mycommand.ExecuteScalar();
            cnn.Close();

            if (value != null)
            {
                return value.ToString();
            }
            return "";
        }

        /// <summary>Update an Entity record</summary>
        /// <typeparam name="TENTITY">Entity Object type</typeparam>
        /// <param name="Record">The Record to Update</param>
        /// <returns><see cref="bool"/> true/false for success/failure</returns>
        public bool UpdateRecord<TENTITY>(TENTITY Record) where TENTITY : class
        {
            try
            {
                if (string.IsNullOrEmpty(DBConnectionString))
                {
                    throw new ArgumentException("Connection String not set");
                }

                using (IDbConnection db = new SQLiteConnection(DBConnectionString))
                {
                    return db.Update(Record);
                }
            }
            catch (DbException ex)
            {
                throw new DatabaseAccessHelperException(ex.Message);
            }
        }

        /// <summary>Delete an Entity record</summary>
        /// <typeparam name="TENTITY">Entity Object type</typeparam>
        /// <param name="Record">The Record to Delete</param>
        /// <returns><see cref="bool"/> true/false for success/failure</returns>
        public bool DeleteRecord<TENTITY>(TENTITY Record) where TENTITY : class
        {
            try
            {
                if (string.IsNullOrEmpty(DBConnectionString))
                {
                    throw new ArgumentException("Connection String not set");
                }

                using (IDbConnection db = new SQLiteConnection(DBConnectionString))
                {
                    return db.Delete(Record);
                }
            }
            catch (DbException ex)
            {
                throw new DatabaseAccessHelperException(ex.Message);
            }
        }

        /// <summary> Return the TableName of the POCO </summary>
        /// <typeparam name="TENTITY">the PCO Entity</typeparam>
        /// <returns><see cref="string"/> holding the tablename</returns>
        public string GetTableName<TENTITY>()
        {
            var attr = typeof(TENTITY).GetCustomAttribute<Dapper.Contrib.Extensions.TableAttribute>(false);
            return attr != null ? attr.Name : "";
        }
        /// <summary>Create a table if it does not exist</summary>
        /// <param name="TableName">The Table Name to create</param>
        /// <param name="primaryKeyName">Primary Key Name : default id</param>
        /// <returns><see cref="bool"/> true/false for success/failure</returns>
        public bool CreateTable(string TableName, string primaryKeyName = "id")
        {
            try
            {
                //Create Table
                this.ExecuteSQL($"CREATE TABLE IF NOT EXISTS {TableName} ({primaryKeyName} INTEGER PRIMARY KEY AUTOINCREMENT);");
                this.CreateIndex(TableName, $"idx_{primaryKeyName}", primaryKeyName);
                return true;
            }

            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>Create an index for a table if it does not exist</summary>
        /// <param name="TableName">The table Name</param>
        /// <param name="IndexName">The index Name</param>
        /// <param name="IndexCol">The indexed column name</param>
        /// <returns><see cref="bool"/> true/false for success/failure</returns>
        public bool CreateIndex(string TableName, string IndexName, string IndexCol)
        {
            try
            {
                //Create Index 
                this.ExecuteSQL($"CREATE UNIQUE INDEX IF NOT EXISTS {IndexName} ON {TableName} ({IndexCol});");
                return true;
            }

            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>Add a new column to a table if it does not exist</summary>
        /// <param name="tableName">The Table Name to create</param>
        /// <param name="columnName">The column name to create</param>
        /// <param name="columnType">The column type</param>
        /// <param name="columnSize">The column size</param>
        /// <param name="allowNull">Does the column allow null values</param>
        /// <returns><see cref="bool"/> true/false for success/failure</returns>
        public bool AddColumn(string tableName, string columnName, string columnType, int columnSize, bool allowNull)
        {
            try
            {
                if (!columnExists(tableName, columnName))
                {
                    //Create Columns
                    string sSQL = null;
                    sSQL = "ALTER TABLE {0} ADD COLUMN [{1}] {2}";

                    if (columnType.ToUpper() != "INTEGER" && columnType.ToUpper() != "DATETIME" && columnType.ToUpper() != "BLOB")
                    {
                        sSQL += "({3}) ";
                    }

                    if (allowNull)
                    {
                        sSQL += " NULL;";
                    }
                    else
                    {
                        sSQL += " NOT NULL;";
                    }
                    sSQL = String.Format(sSQL, tableName, columnName, columnType.ToUpper(), columnSize);
                    this.ExecuteSQL(String.Format(sSQL, tableName));
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>Check if a table exists in the database</summary>
        /// <param name="tableName">The Table Name to check</param>
        /// <returns><see cref="bool"/> true/false for success/failure</returns>
        public bool TableExists(String tableName)
        {
            bool HasRows;

            try
            {
                SQLiteConnection cnn = new SQLiteConnection(DBConnectionString);
                cnn.Open();
                SQLiteCommand mycommand = new SQLiteCommand(cnn) { CommandText = String.Format("select name from sqlite_master where name ='{0}';", tableName) };
                SQLiteDataReader reader = mycommand.ExecuteReader();

                if (reader.HasRows)
                    HasRows = true;
                else
                    HasRows = false;
                reader.Close();
                cnn.Close();
                return HasRows;
            }

            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>Check if a column exists in a given table</summary>
        /// <param name="tableName">The Table Name to check</param>
        /// <param name="columnName">The column name to check</param>
        /// <returns><see cref="bool"/> true/false for success/failure</returns>
        public bool columnExists(String tableName, string columnName)
        {
            try
            {
                SQLiteConnection cnn = new SQLiteConnection(DBConnectionString);
                cnn.Open();

                DataTable ColsTable = cnn.GetSchema("Columns");

                cnn.Close();

                var data = ColsTable.Select(string.Format("COLUMN_NAME='{1}' AND TABLE_NAME='{0}'", tableName, columnName));

                return data.Length == 1;


            }

            catch (Exception)
            {
                return false;
            }
        }
    }
}