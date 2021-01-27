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

namespace AGenius.UsefulStuff.Helpers
{
    /// <summary>
    /// This class is used to help encapsulate the use of Dapper objects to access SQL Lite
    /// </summary>
    public class SQLiteDatabaseHelper : IDisposable
    {
        /// <summary>Event Handler for the ErrorEventArgs event</summary>
        string dbFilePath = "default.db3";
        public string DBConnectionString
        {
            get; set;
        }
        public Boolean? LastSaveState
        {
            get;
            private set;
        }
        public SQLiteDatabaseHelper()
        {
            DBConnectionString = "Data Source=" + dbFilePath;
        }
        public SQLiteDatabaseHelper(string DatabaseName)
        {
            if (!DatabaseName.EndsWith(".db3"))
            {
                DatabaseName += ".db3";
            }
            DBConnectionString = String.Format("Data Source={0}", DatabaseName);
        }
        public void Dispose()
        {
            //   throw new NotImplementedException();
        }
        private string _lastError = string.Empty;

        /// <summary>Read ALL records for an entity </summary>
        /// <typeparam name="TENTITY">Entity Object type</typeparam>
        /// <returns>Entity records</returns>
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
        /// <param name="ID"></param>
        /// <returns></returns>
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
        /// <param name="ID"></param>
        /// <returns></returns>
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
        /// <summary>Return an object that matches the selection criteria </summary>
        /// <typeparam name="TENTITY">Entity Object type</typeparam>
        /// <param name="FieldValue">The Value of the Key field to find</param>
        /// <param name="KeyFieldName">The Key FieldName</param>
        /// <returns>entity records</returns>
        public TENTITY ReadRecord<TENTITY>(string KeyFieldName, string FieldValue) where TENTITY : class
        {
            try
            {
                string TableName = GetTableName<TENTITY>();
                if (string.IsNullOrEmpty(TableName))
                {
                    throw new ArgumentException("Invalid Table Name");
                }
                if (string.IsNullOrEmpty(KeyFieldName) || FieldValue == null)
                {
                    throw new ArgumentException("Missing FieldName or Value for search");
                }
                if (string.IsNullOrEmpty(DBConnectionString))
                {
                    throw new ArgumentException("Connection String not set");
                }
                string sWhere = $"WHERE {KeyFieldName} = '{FieldValue}' ";

                string sSQL = $"SELECT * FROM {TableName} {sWhere}";
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
        public TENTITY ReadRecord<TENTITY>(string KeyFieldName, int FieldValue) where TENTITY : class
        {
            try
            {
                string TableName = GetTableName<TENTITY>();
                if (string.IsNullOrEmpty(TableName))
                {
                    throw new ArgumentException("Invalid Table Name");
                }
                if (string.IsNullOrEmpty(KeyFieldName))
                {
                    throw new ArgumentException("Missing FieldName or Value for search");
                }
                if (string.IsNullOrEmpty(DBConnectionString))
                {
                    throw new ArgumentException("Connection String not set");
                }
                string sWhere = $"WHERE {KeyFieldName} = {FieldValue} ";

                string sSQL = $"SELECT * FROM {TableName} {sWhere}";
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
        /// <summary>Return a single objects that match the selection criteria </summary>
        /// <typeparam name="TENTITY">Entity Object type</typeparam>
        /// <param name="Where">criteria</param>
        /// <returns>entity records</returns>
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
        /// <returns>entity records</returns>
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
        /// <returns>entity records</returns>
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


        /// <summary>Insert a new Entity record</summary>
        /// <typeparam name="TENTITY">Entity Object type</typeparam>
        /// <param name="Record">The Record to insert</param>
        /// <returns>Long ID</returns>
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
        /// <summary>Update an Entity record</summary>
        /// <typeparam name="TENTITY">Entity Object type</typeparam>
        /// <param name="Record">The Record to Update</param>
        /// <returns>True/false for success</returns>
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
        /// <returns>True/false for success</returns>
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
        /// <returns>string</returns>
        public string GetTableName<TENTITY>()
        {
            var attr = typeof(TENTITY).GetCustomAttribute<Dapper.Contrib.Extensions.TableAttribute>(false);
            return attr != null ? attr.Name : "";
        }
        public bool CreateTable(string TableName)
        {
            try
            {
                //Create Table
                this.ExecuteSQL($"CREATE TABLE IF NOT EXISTS {TableName} (id INTEGER PRIMARY KEY AUTOINCREMENT);");
                this.CreateIndex(TableName, "idxid", "id");
                return true;
            }

            catch (Exception fail)
            {
                return false;
            }
        }
        public bool CreateIndex(string TableName, string IndexName, string IndexCol)
        {

            try
            {
                //Create Index 
                this.ExecuteSQL($"CREATE UNIQUE INDEX IF NOT EXISTS {IndexName} ON {TableName} ({IndexCol});");
                return true;
            }

            catch (Exception fail)
            {
                return false;
            }
        }
        public bool AddColumn(string TableName, string ColName, string Type, int Size, bool AllowNull)
        {
            try
            {
                //Create Columns
                string sSQL = null;
                sSQL = "ALTER TABLE {0} ADD COLUMN [{1}] {2}";

                if (Type.ToUpper() != "INTEGER" && Type.ToUpper() != "DATETIME" && Type.ToUpper() != "BLOB")
                {
                    sSQL += "({3}) ";
                }

                if (AllowNull)
                {
                    sSQL += " NULL;";
                }
                else
                {
                    sSQL += " NOT NULL;";
                }
                sSQL = String.Format(sSQL, TableName, ColName, Type.ToUpper(), Size);
                this.ExecuteSQL(String.Format(sSQL, TableName));
                return true;
            }

            catch (Exception fail)
            {
                return false;
            }
        }
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

            catch (Exception fail)
            {
                return false;
            }
        }
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

            catch (Exception fail)
            {
                return false;
            }
        }
        public string ExecuteScalar(string SQLQuery)
        {
            SQLiteConnection cnn = new SQLiteConnection(DBConnectionString);
            cnn.Open();
            SQLiteCommand mycommand = new SQLiteCommand(cnn);
            mycommand.CommandText = SQLQuery;
            object value = mycommand.ExecuteScalar();
            cnn.Close();

            if (value != null)
            {
                return value.ToString();
            }
            return "";
        }
    }
}
