using Dapper;
using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Odbc;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace AGenius.UsefulStuff.Helpers
{
    /// <summary>
    /// This class is used to help encapsulate the use of Dapper objects to access data via ODBC
    /// </summary>
    public class ODBCDatabaseHelper : IDisposable
    {
        /// <summary>Provides access to the  Connection string in use</summary>
        public string DBConnectionString
        {
            get; set;
        }
        
        /// <summary>Initializes a new instance of the <see cref="ODBCDatabaseHelper"/> class </summary>
        public ODBCDatabaseHelper()
        {
            DBConnectionString = "";
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="ODBCDatabaseHelper"/> class using the supplied connection string  
        /// </summary>
        /// <param name="connectionString">The connection string used to open the SQL connection</param>
        public ODBCDatabaseHelper(string connectionString)
        {
            DBConnectionString = connectionString;
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
                _lastError = "Connection String not set";
                throw new ArgumentException(_lastError);
            }
            try
            {
                using (IDbConnection db = new OdbcConnection(DBConnectionString))
                {
                    return db.GetAll<TENTITY>().ToList();
                }
            }
            catch (DbException ex)
            {
                _lastError = ex.Message;
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
                    _lastError = "Connection String not set";
                    throw new ArgumentException(_lastError);
                }
                if (!ID.HasValue || ID.Value <= 0)
                {
                    _lastError = "Invalid ID specified";
                    throw new ArgumentException(_lastError);
                }
                using (IDbConnection db = new OdbcConnection(DBConnectionString))
                {
                    return db.Get<TENTITY>(ID);
                }
            }
            catch (DbException ex)
            {
                _lastError = ex.Message;
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
                    _lastError = "Connection String not set";
                    throw new ArgumentException(_lastError);
                }
                if (string.IsNullOrEmpty(ID))
                {
                    _lastError = "Invalid ID specified";
                    throw new ArgumentException(_lastError);
                }
                using (IDbConnection db = new OdbcConnection(DBConnectionString))
                {
                    return db.Get<TENTITY>(ID);
                }
            }
            catch (DbException ex)
            {
                _lastError = ex.Message;
                throw new DatabaseAccessHelperException(ex.Message);
            }
        }
        /// <summary>Return a single record for the specified ID </summary>
        /// <typeparam name="TENTITY">Entity Object type</typeparam>
        /// <param name="ID"></param>
        /// <returns></returns>
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
                using (IDbConnection db = new OdbcConnection(DBConnectionString))
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
                using (IDbConnection db = new OdbcConnection(DBConnectionString))
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
        /// <summary>Return an object that matches the selection criteria </summary>
        /// <typeparam name="TENTITY">Entity Object type</typeparam>
        /// <param name="fieldValue">The Value of the Key field to find</param>
        /// <param name="keyFieldName">The Key FieldName</param>
        /// <param name="operatorType">Comparison Operator - Default is Equals</param>
        /// <returns>The Entity record requested or null</returns>
        public TENTITY ReadRecord<TENTITY>(string keyFieldName, int? fieldValue, string operatorType = "=") where TENTITY : class
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
                string sWhere = $"WHERE {keyFieldName} {operatorType} {fieldValue} ";

                string sSQL = $"SELECT * FROM {TableName} {sWhere}";
                // var Results = null;
                using (IDbConnection db = new OdbcConnection(DBConnectionString))
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
        /// <summary>Return a single objects that match the selection criteria </summary>
        /// <typeparam name="TENTITY">Entity Object type</typeparam>
        /// <param name="Where">criteria</param>
        /// <returns>entity records</returns>
        public TENTITY ReadRecordWithWhere<TENTITY>(string Where = "") where TENTITY : class
        {
            try
            {
                string TableName = GetTableName<TENTITY>();
                if (string.IsNullOrEmpty(TableName))
                {
                    _lastError = "Invalid Table Name";
                    throw new ArgumentException(_lastError);
                }

                if (string.IsNullOrEmpty(DBConnectionString))
                {
                    _lastError = "Connection String not set";
                    throw new ArgumentException(_lastError);
                }
                string sWhere = string.IsNullOrEmpty(Where) ? "" : $"WHERE {Where}";

                string sSQL = $"SELECT TOP 1 * FROM {TableName} {sWhere}";
                // var Results = null;
                using (IDbConnection db = new OdbcConnection(DBConnectionString))
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
        /// <summary>Return a list of objects from a stored procedure with parameters</summary>
        /// <typeparam name="TENTITY">Entity Object type</typeparam>
        /// <param name="SprocName">Stored Procedure Name</param>
        /// <param name="Params"> DynamicParamters collection</param>
        /// <returns>entity records</returns>
        /// <remarks>DynamicParameters param = new DynamicParameters();
        ///param.Add( "@@Name" , obj.Name );
        ///        param.Add( "@City" , obj.City );
        ///        param.Add( "@Address" , obj.Address );</remarks>
        public IList<TENTITY> ReadRecordsSproc<TENTITY>(string SprocName, DynamicParameters Params)
            where TENTITY : class
        {
            try
            {
                if (string.IsNullOrEmpty(DBConnectionString))
                {
                    _lastError = "Connection String not set";
                    throw new ArgumentException(_lastError);
                }

                using (IDbConnection db = new OdbcConnection(DBConnectionString))
                {
                    return db.Query<TENTITY>(SprocName, Params, commandType:
                    CommandType.StoredProcedure).ToList();
                }
            }
            catch (DbException ex)
            {
                _lastError = ex.Message;
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
                    _lastError = "Connection String not set";
                    throw new ArgumentException(_lastError);
                }
                string sSQL = SQLQuery.Replace("[tablename]", GetTableName<TENTITY>());
                using (IDbConnection db = new OdbcConnection(DBConnectionString))
                {
                    return db.Query<TENTITY>(sSQL).ToList();
                }

            }
            catch (DbException ex)
            {
                _lastError = ex.Message;
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
                    _lastError = "Invalid Table Name";
                    throw new ArgumentException(_lastError);
                }

                if (string.IsNullOrEmpty(DBConnectionString))
                {
                    _lastError = "Connection String not set";
                    throw new ArgumentException(_lastError);
                }
                string sWhere = string.IsNullOrEmpty(Where) ? "" : $"WHERE {Where}";

                string sSQL = $"SELECT * FROM {TableName}  {sWhere}";
                // var Results = null;
                using (IDbConnection db = new OdbcConnection(DBConnectionString))
                {
                    return db.Query<TENTITY>(sSQL).ToList();
                }

            }
            catch (DbException ex)
            {
                _lastError = ex.Message;
                //throw new DatabaseAccessHelperException(ex.Message);
            }
            return null;
        }
        /// <summary>Return a list of objects that match the selection criteria </summary>
        /// <typeparam name="TENTITY">Entity Object type</typeparam>
        /// <param name="Where">criteria</param>
        /// <param name="TopCount">Only return n records</param>
        /// <returns>entity records</returns>
        public IList<TENTITY> ReadRecords<TENTITY>(string Where, int TopCount) where TENTITY : class
        {
            try
            {
                string TableName = GetTableName<TENTITY>();
                if (string.IsNullOrEmpty(TableName))
                {
                    _lastError = "Invalid Table Name";
                    throw new ArgumentException(_lastError);
                }

                if (string.IsNullOrEmpty(DBConnectionString))
                {
                    _lastError = "Connection String not set";
                    throw new ArgumentException(_lastError);
                }
                string sWhere = string.IsNullOrEmpty(Where) ? "" : $"WHERE {Where}";

                string sSQL = $"SELECT TOP {TopCount} * FROM {TableName} {sWhere}";
                // var Results = null;
                using (IDbConnection db = new OdbcConnection(DBConnectionString))
                {
                    return db.Query<TENTITY>(sSQL).ToList();
                }

            }
            catch (DbException ex)
            {
                _lastError = ex.Message;
                throw new DatabaseAccessHelperException(ex.Message);
            }
        }
        /// <summary>Return a list of objects that match the selection criteria </summary>
        /// <typeparam name="TENTITY">Entity Object type</typeparam>
        /// <param name="Where">criteria</param>
        /// <param name="OverrideTableName">Supply the tablename to override the default tablename of the entity</param>
        /// <returns><see cref="IList{T}"/> containing the Entity records</returns>
        public IList<TENTITY> ReadRecords<TENTITY>(string Where = "", string OverrideTableName = "") where TENTITY : class
        {
            try
            {
                string TableName = GetTableName<TENTITY>();
                if (!string.IsNullOrEmpty(OverrideTableName))
                {
                    TableName = OverrideTableName;
                }
                if (string.IsNullOrEmpty(TableName))
                {
                    _lastError = "Invalid Table Name";
                    throw new ArgumentException(_lastError);
                }

                if (string.IsNullOrEmpty(DBConnectionString))
                {
                    _lastError = "Connection String not set";
                    throw new ArgumentException(_lastError);
                }
                string sWhere = string.IsNullOrEmpty(Where) ? "" : $"WHERE {Where}";

                string sSQL = $"SELECT * FROM {TableName} {sWhere}";
                // var Results = null;
                using (IDbConnection db = new OdbcConnection(DBConnectionString))
                {
                    return db.Query<TENTITY>(sSQL).ToList();
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
        /// <returns>Long ID</returns>
        public long InsertRecord<TENTITY>(TENTITY Record) where TENTITY : class
        {
            try
            {
                if (string.IsNullOrEmpty(DBConnectionString))
                {
                    _lastError = "Connection String not set";
                    throw new ArgumentException(_lastError);
                }

                using (IDbConnection db = new OdbcConnection(DBConnectionString))
                {
                    return db.Insert(Record);
                }

            }
            catch (DbException ex)
            {
                _lastError = ex.Message;
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
                    _lastError = "Connection String not set";
                    throw new ArgumentException(_lastError);
                }

                using (IDbConnection db = new OdbcConnection(DBConnectionString))
                {
                    db.Execute(sqlCmd);
                }

            }
            catch (DbException ex)
            {
                _lastError = ex.Message;
                throw new DatabaseAccessHelperException(ex.Message);
            }
        }
        /// <summary>Execute an SQL Statement </summary>
        /// <param name="sqlCmd">String holding the SQL Command</param>
        public object ExecuteScalar(string sqlCmd)
        {
            try
            {
                if (string.IsNullOrEmpty(DBConnectionString))
                {
                    _lastError = "Connection String not set";
                    throw new ArgumentException(_lastError);
                }

                using (IDbConnection db = new OdbcConnection(DBConnectionString))
                {
                    return db.ExecuteScalar(sqlCmd);
                }

            }
            catch (DbException ex)
            {
                _lastError = ex.Message;
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
                    _lastError = "Connection String not set";
                    throw new ArgumentException(_lastError);
                }

                using (IDbConnection db = new OdbcConnection(DBConnectionString))
                {
                    return db.Update(Record);
                }
            }
            catch (DbException ex)
            {
                _lastError = ex.Message;
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
                    _lastError = "Connection String not set";
                    throw new ArgumentException(_lastError);
                }

                using (IDbConnection db = new OdbcConnection(DBConnectionString))
                {
                    return db.Delete(Record);
                }
            }
            catch (DbException ex)
            {
                _lastError = ex.Message;
                throw new DatabaseAccessHelperException(ex.Message);
            }
        }
        /// <summary>Return the TableName of the POCO </summary>
        /// <typeparam name="TENTITY">the PCO Entity</typeparam>
        /// <returns>string</returns>
        public string GetTableName<TENTITY>()
        {
            var attr = typeof(TENTITY).GetCustomAttribute<Dapper.Contrib.Extensions.TableAttribute>(false);
            return attr != null ? attr.Name : "";
        }
        /// <summary>Returns the last error message if any of the specified action</summary>
        /// <returns><see cref="string"/> value containing any error messages.</returns>
        public string LastError
        {
            get
            {
                return _lastError;
            }
        }
    }
}