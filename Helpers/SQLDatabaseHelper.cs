using Dapper;
using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security.Principal;
using System.Text;

namespace AGenius.UsefulStuff.Helpers
{
    /// <summary>
    /// This class is used to help encapsulate the use of Dapper objects to access SQL Server
    /// </summary>
    public class SQLDatabaseHelper : IDisposable
    {

        /// <summary>Used to hold Table Column information</summary>
        public class TableColumn
        {
            /// <summary>Column Name</summary>
            public string COLUMN_NAME { get; set; }
            /// <summary>Columns Data type</summary>
            public string DATA_TYPE { get; set; }
            /// <summary>Columns Max length</summary>
            public int? CHARACTER_MAXIMUM_LENGTH { get; set; }
            /// <summary>Sort Order</summary>
            public int ORDINAL_POSITION { get; set; }
            public bool ALLOW_NULL { get; set; }
            public bool IS_ID { get; set; }
            public bool IDENTITY { get; set; }
        }
        public class TableRecord
        {
            /// <summary>Scheme - usually dbo</summary>
            public string TABLE_SCHEME { get; set; }
            /// <summary>Table Name</summary>
            public string TABLE_NAME { get; set; }
            /// <summary>Type - Base Table or View</summary>
            public string TABLE_TYPE { get; set; }
        }
        public class ColumnMetadata
        {
            public string ColumnName { get; set; }
            public int ObjectId { get; set; }
            public int ColumnId { get; set; }
            public byte SystemTypeId { get; set; }
            public int UserTypeId { get; set; }
            public short MaxLength { get; set; }
            public byte Precision { get; set; }
            public byte Scale { get; set; }
            public string CollationName { get; set; }
            public bool IsNullable { get; set; }
            public bool IsAnsiPadded { get; set; }
            public bool IsRowGuidCol { get; set; }
            public bool IsIdentity { get; set; }
            public bool IsComputed { get; set; }
            public bool IsFileStream { get; set; }
            public bool IsReplicated { get; set; }
            public bool IsNonSqlSubscribed { get; set; }
            public bool IsMergePublished { get; set; }
            public bool IsDtsReplicated { get; set; }
            public bool IsXmlDocument { get; set; }
            public int XmlCollectionId { get; set; }
            public int DefaultObjectId { get; set; }
            public int RuleObjectId { get; set; }
            public bool IsSparse { get; set; }
            public bool IsColumnSet { get; set; }
            // The following are SQL Server 2016+ only — comment out for 2008-2014 support
            //public byte GeneratedAlwaysType { get; set; }
            //public string GeneratedAlwaysTypeDesc { get; set; }
            //public byte EncryptionType { get; set; }
            //public string EncryptionTypeDesc { get; set; }
            //public string EncryptionAlgorithmName { get; set; }
            //public int ColumnEncryptionKeyId { get; set; }
            //public string ColumnEncryptionKeyDatabaseName { get; set; }
            //public bool IsHidden { get; set; }
            //public bool IsMasked { get; set; }
            // SQL 2022 Support
            //public bool IsDataDeletionFilterColumn { get; set; }
            //public byte LedgerViewColumnType { get; set; }
            //public string LedgerViewColumnTypeDesc { get; set; }
            //public bool IsDroppedLedgerColumn { get; set; }
            public DateTime CreateDate { get; set; }
            public DateTime ModifyDate { get; set; }
        }


        readonly Helpers.AGLogger.Logger _errorLogger = new Helpers.AGLogger.Logger(LogPath: Path.Combine(Utils.ApplicationPath, "Logs", "SQLDatabaseHelper_Errors.log"), AddTimeStamp: true, RolloverSubFolder: "Complete");
        /// <summary>Provides access to the  Connection string in use</summary>
        public string DBConnectionString
        {
            get; set;
        }
        /// <summary>Command Timeout - default :120 seconds</summary>
        public int DefaultTimeOut { get; set; } = 120;
        /// <summary>Initializes a new instance of the <see cref="SQLDatabaseHelper"/> class </summary>
        public SQLDatabaseHelper()
        {
            DBConnectionString = "";
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="SQLDatabaseHelper"/> class using the supplied connection string  
        /// </summary>
        /// <param name="connectionString">The connection string used to open the SQL connection</param>
        public SQLDatabaseHelper(string connectionString)
        {
            DBConnectionString = connectionString;
        }
        public void Dispose()
        {
            //   throw new NotImplementedException();
        }

        private string _lastError = "";
        private string _lastQuery = "";

        /// <summary>Read ALL records for an entity </summary>
        /// <typeparam name="TENTITY">Entity Object type</typeparam>
        /// <returns><see cref="IList{T}"/> containing the Entity records</returns>
        public IList<TENTITY> ReadALL<TENTITY>() where TENTITY : class
        {
            _lastError = "";
            _lastQuery = "";
            if (string.IsNullOrEmpty(DBConnectionString))
            {
                _lastError = "Connection String not set";
                throw new ArgumentException(_lastError);
            }
            try
            {
                using IDbConnection db = new SqlConnection(DBConnectionString);
                return db.GetAll<TENTITY>(commandTimeout: DefaultTimeOut).ToList();
            }
            catch (DbException ex)
            {
                if (ex.Message.Contains("deadlocked"))
                {
                    // Retry
                    _errorLogger.LogError(ex.Message);
                    return ReadALL<TENTITY>();
                }
                _lastError = ex.Message;
                throw new DatabaseAccessHelperException(ex.Message);
            }
        }
        /// <summary>Return a single record for the specified ID </summary>
        /// <typeparam name="TENTITY">Entity Object type</typeparam>
        /// <param name="ID"><see cref="int"/> ID of the record to read</param>
        /// <returns>The Entity record requested or null</returns>
        public TENTITY ReadRecord<TENTITY>(int? ID) where TENTITY : class => DoReadRecord<TENTITY>(ID);
        /// <summary>Return a single record for the specified ID </summary>
        /// <typeparam name="TENTITY">Entity Object type</typeparam>
        /// <param name="ID"><see cref="long"/> ID of the record to read</param>
        /// <returns>The Entity record requested or null</returns>
        public TENTITY ReadRecord<TENTITY>(long? ID) where TENTITY : class => DoReadRecord<TENTITY>(ID);

        /// <summary>Return a single record for the specified ID </summary>
        /// <typeparam name="TENTITY">Entity Object type</typeparam>
        /// <param name="ID"><see cref="string"/> ID of the record to read</param>
        /// <returns>The Entity record requested or null</returns>
        public TENTITY ReadRecord<TENTITY>(string ID) where TENTITY : class => DoReadRecord<TENTITY>(ID);
        /// <summary>Return a single record for the specified ID </summary>
        /// <typeparam name="TENTITY">Entity Object type</typeparam>
        /// <param name="ID"><see cref="Guid"/> ID of the record to read</param>
        /// <returns>The Entity record requested or null</returns>
        public TENTITY ReadRecord<TENTITY>(Guid ID) where TENTITY : class => DoReadRecord<TENTITY>(ID);

        /// <summary>Return a single record for the specified ID </summary>
        /// <typeparam name="TENTITY">Entity Object type</typeparam>
        /// <param name="ID">Can be any of : <see cref="int"/><see cref="long"/><see cref="string"/> or <see cref="Guid"/> </param>
        /// <returns>The Entity record requested or null</returns>
        private TENTITY DoReadRecord<TENTITY>(object ID) where TENTITY : class
        {
            try
            {
                _lastError = "";
                _lastQuery = "";
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
                using IDbConnection db = new SqlConnection(DBConnectionString);
                if (ID is int iID)
                {
                    return db.Get<TENTITY>(iID, commandTimeout: DefaultTimeOut);
                }
                else if (ID is long lID)
                {
                    return db.Get<TENTITY>(lID, commandTimeout: DefaultTimeOut);
                }
                else if (ID is string sID)
                {
                    return db.Get<TENTITY>(sID.ToString(), commandTimeout: DefaultTimeOut);
                }
                else if (ID is Guid gID)
                {
                    return db.Get<TENTITY>(gID.ToString(), commandTimeout: DefaultTimeOut);
                }
                return null;
            }
            catch (DbException ex)
            {
                if (ex.Message.Contains("deadlocked"))
                {
                    // Retry
                    _errorLogger.LogError(ex.Message);
                    return DoReadRecord<TENTITY>(ID);
                }
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
                _lastError = "";
                _lastQuery = "";
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
                _lastQuery = sSQL;
                // var Results = null;
                using IDbConnection db = new SqlConnection(DBConnectionString);
                return db.Query<TENTITY>(sSQL, commandTimeout: DefaultTimeOut).SingleOrDefault();
            }
            catch (DbException ex)
            {
                if (ex.Message.Contains("deadlocked"))
                {
                    // Retry
                    _errorLogger.LogError(ex.Message);
                    return ReadRecord<TENTITY>(keyFieldName, fieldValue, operatorType);
                }
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
                _lastError = "";
                _lastQuery = "";
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
                _lastQuery = sSQL;
                // var Results = null;
                using IDbConnection db = new SqlConnection(DBConnectionString);
                return db.Query<TENTITY>(sSQL, commandTimeout: DefaultTimeOut).SingleOrDefault();
            }
            catch (DbException ex)
            {
                if (ex.Message.Contains("deadlocked"))
                {
                    // Retry
                    _errorLogger.LogError(ex.Message);
                    return ReadRecord<TENTITY>(keyFieldName, fieldValue);
                }
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
        public TENTITY ReadRecord<TENTITY>(string keyFieldName, Guid fieldValue, string operatorType = "=") where TENTITY : class
        {
            try
            {
                _lastError = "";
                _lastQuery = "";
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
                _lastQuery = sSQL;
                // var Results = null;
                using IDbConnection db = new SqlConnection(DBConnectionString);
                return db.Query<TENTITY>(sSQL, commandTimeout: DefaultTimeOut).SingleOrDefault();
            }
            catch (DbException ex)
            {
                if (ex.Message.Contains("deadlocked"))
                {
                    // Retry
                    _errorLogger.LogError(ex.Message);
                    return ReadRecord<TENTITY>(keyFieldName, fieldValue);
                }
                _lastError = ex.Message;
                throw new DatabaseAccessHelperException(ex.Message);
            }
        }
        /// <summary>Return a single objects that matches the selection criteria </summary>
        /// <typeparam name="TENTITY">Entity Object type</typeparam>
        /// <param name="Where">criteria</param>
        /// <param name="OverrideTableName">Override the Entity Table Name</param>
        /// <returns>The Entity record requested or null</returns>
        public TENTITY ReadRecordWithWhere<TENTITY>(string Where = "", string OverrideTableName = "") where TENTITY : class
        {
            try
            {
                _lastError = "";
                _lastQuery = "";
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

                string sSQL = $"SELECT TOP 1 * FROM {TableName} {sWhere}";
                _lastQuery = sSQL;
                // var Results = null;
                using IDbConnection db = new SqlConnection(DBConnectionString);
                return db.Query<TENTITY>(sSQL, commandTimeout: DefaultTimeOut).SingleOrDefault();
            }
            catch (DbException ex)
            {
                if (ex.Message.Contains("deadlocked"))
                {
                    // Retry
                    _errorLogger.LogError(ex.Message);
                    return ReadRecordWithWhere<TENTITY>(Where);
                }
                _lastError = ex.Message;
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
        public IList<TENTITY> ReadRecordsSproc<TENTITY>(string SprocName, DynamicParameters Params)
            where TENTITY : class
        {
            try
            {
                _lastError = "";
                _lastQuery = "";
                if (string.IsNullOrEmpty(DBConnectionString))
                {
                    _lastError = "Connection String not set";
                    throw new ArgumentException(_lastError);
                }

                using IDbConnection db = new SqlConnection(DBConnectionString);
                return db.Query<TENTITY>(SprocName, Params, commandType: CommandType.StoredProcedure, commandTimeout: DefaultTimeOut).ToList();
            }
            catch (DbException ex)
            {
                if (ex.Message.Contains("deadlocked"))
                {
                    // Retry
                    _errorLogger.LogError(ex.Message);
                    return ReadRecordsSproc<TENTITY>(SprocName, Params);
                }
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
                _lastError = "";
                _lastQuery = "";
                if (string.IsNullOrEmpty(DBConnectionString))
                {
                    _lastError = "Connection String not set";
                    throw new ArgumentException(_lastError);
                }
                string sSQL = SQLQuery.Replace("[tablename]", GetTableName<TENTITY>()).Replace("[TABLENAME]", GetTableName<TENTITY>());
                _lastQuery = sSQL;
                using IDbConnection db = new SqlConnection(DBConnectionString);
                return db.Query<TENTITY>(sSQL, commandTimeout: DefaultTimeOut).ToList();
            }
            catch (DbException ex)
            {
                if (ex.Message.Contains("deadlocked"))
                {
                    // Retry
                    _errorLogger.LogError(ex.Message);
                    return ReadRecordsSQL<TENTITY>(SQLQuery);
                }
                _lastError = ex.Message;
                throw new DatabaseAccessHelperException(ex.Message);
            }
        }
        /// <summary>Return a list of objects that match the selection criteria </summary>
        /// <typeparam name="TENTITY">Entity Object type</typeparam>
        /// <param name="Where">criteria</param>
        /// <param name="noTimeout">Set no Timeout and wait indefinitely</param>
        /// <returns><see cref="IList{T}"/> containing the Entity records</returns>
        public IList<TENTITY> ReadRecordsPropertiesOnly<TENTITY>(string Where = "", bool noTimeout = false) where TENTITY : class
        {
            try
            {
                _lastError = "";
                _lastQuery = "";
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
                // Get comma-separated property names

                var fieldList = string.Join(", ", typeof(TENTITY).GetProperties().Where(p => !Attribute.IsDefined(p, typeof(ExcludeFromListReads)))
                    .Select(p => $"[{p.Name}]"));
                string sSQL = $"SELECT {fieldList} FROM {TableName} {sWhere}";


                _lastQuery = sSQL;
                // var Results = null;
                using IDbConnection db = new SqlConnection(DBConnectionString);
                return db.Query<TENTITY>(sSQL, commandTimeout: noTimeout ? 0 : DefaultTimeOut).ToList();
            }
            catch (DbException ex)
            {
                if (ex.Message.Contains("deadlocked"))
                {
                    // Retry
                    _errorLogger.LogError(ex.Message);
                    return ReadRecordsPropertiesOnly<TENTITY>(Where, noTimeout);
                }
                _lastError = ex.Message;
                //       throw new DatabaseAccessHelperException(ex.Message);
                return null;
            }
        }
        /// <summary>Return a list of objects that match the selection criteria </summary>
        /// <typeparam name="TENTITY">Entity Object type</typeparam>
        /// <param name="Where">criteria</param>
        /// <param name="noTimeout">Set no Timeout and wait indefinitely</param>
        /// <returns><see cref="IList{T}"/> containing the Entity records</returns>
        public IList<TENTITY> ReadRecords<TENTITY>(string Where = "") where TENTITY : class
        {
            try
            {
                _lastError = "";
                _lastQuery = "";
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
                string sSQL = $"SELECT * FROM {TableName} {sWhere}";
                _lastQuery = sSQL;
                // var Results = null;
                using IDbConnection db = new SqlConnection(DBConnectionString);
                return db.Query<TENTITY>(sSQL, commandTimeout: DefaultTimeOut).ToList();
            }
            catch (DbException ex)
            {
                if (ex.Message.Contains("deadlocked"))
                {
                    // Retry
                    _errorLogger.LogError(ex.Message);
                    return ReadRecords<TENTITY>(Where);
                }
                _lastError = ex.Message;
                //       throw new DatabaseAccessHelperException(ex.Message);
                return null;
            }
        }
        /// <summary>Return a list of objects that match the selection criteria </summary>
        /// <typeparam name="TENTITY">Entity Object type</typeparam>
        /// <param name="Where">criteria</param>
        /// <param name="TopCount">Only return n records</param>
        /// <returns><see cref="IList{T}"/> containing the Entity records</returns>
        public IList<TENTITY> ReadRecords<TENTITY>(string Where, int TopCount)
            where TENTITY : class
        {
            try
            {
                _lastError = "";
                _lastQuery = "";
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
                _lastQuery = sSQL;
                // var Results = null;
                using IDbConnection db = new SqlConnection(DBConnectionString);
                return db.Query<TENTITY>(sSQL, commandTimeout: DefaultTimeOut).ToList();
            }
            catch (DbException ex)
            {
                if (ex.Message.Contains("deadlocked"))
                {
                    // Retry
                    _errorLogger.LogError(ex.Message);
                    return ReadRecords<TENTITY>(Where, TopCount);
                }
                _lastError = ex.Message;
                throw new DatabaseAccessHelperException(ex.Message);
            }
        }
        /// <summary>Return a list of objects that match the selection criteria </summary>
        /// <typeparam name="TENTITY">Entity Object type</typeparam>
        /// <param name="Where">criteria</param>
        /// <param name="OverrideTableName">Supply the tablename to override the default tablename of the entity</param>
        /// <returns><see cref="IList{T}"/> containing the Entity records</returns>
        public IList<TENTITY> ReadRecords<TENTITY>(string Where = "", string OverrideTableName = "")
            where TENTITY : class
        {
            try
            {
                _lastError = "";
                _lastQuery = "";
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
                _lastQuery = sSQL;
                // var Results = null;
                using IDbConnection db = new SqlConnection(DBConnectionString);
                return db.Query<TENTITY>(sSQL, commandTimeout: DefaultTimeOut).ToList();
            }
            catch (DbException ex)
            {
                if (ex.Message.Contains("deadlocked"))
                {
                    // Retry
                    _errorLogger.LogError(ex.Message);
                    return ReadRecords<TENTITY>(Where, OverrideTableName);
                }
                _lastError = ex.Message;
                throw new DatabaseAccessHelperException(ex.Message);
            }
        }
        /// <summary> Multi Queries </summary>
        /// <typeparam name="TENTITY">Entity Object type</typeparam>
        /// <typeparam name="DETAIL">The expected Detail entity object type</typeparam>
        /// <param name="SQLQuery">SQL Query to use</param>
        /// <param name="splitOnField">The field name to split on</param>
        /// <param name="detailPropertyName">The detail name property name</param>
        /// <returns><see cref="IList{T}"/> containing the Entity records</returns>
        public IList<TENTITY> ReadRecords<TENTITY, DETAIL>(string SQLQuery, string splitOnField, string detailPropertyName)
            where TENTITY : class
            where DETAIL : class
        {
            try
            {
                _lastError = "";
                _lastQuery = "";
                if (string.IsNullOrEmpty(DBConnectionString))
                {
                    _lastError = "Connection String not set";
                    throw new ArgumentException(_lastError);
                }
                string sSQL = SQLQuery.Replace("[tablename]", GetTableName<TENTITY>()).Replace("[TABLENAME]", GetTableName<TENTITY>());
                _lastQuery = sSQL;
                using IDbConnection db = new SqlConnection(DBConnectionString);
                var results = db.Query<TENTITY, DETAIL, TENTITY>(sSQL,
                                                               (parent, detail) =>
                                                               {
                                                                   parent.GetType().GetProperty(detailPropertyName).SetValue(parent, detail, null);
                                                                   return parent;
                                                               },
                                                               splitOn: splitOnField, commandTimeout: DefaultTimeOut)
                    .Distinct()
                    .ToList();
                return results;
            }
            catch (DbException ex)
            {
                if (ex.Message.Contains("deadlocked"))
                {
                    // Retry
                    _errorLogger.LogError(ex.Message);
                }
                _lastError = ex.Message;
                throw new DatabaseAccessHelperException(ex.Message);
            }
        }
        /// <summary> Multi Queries </summary>
        /// <typeparam name="TENTITY">Entity Object type</typeparam>
        /// <typeparam name="DETAIL1">The expected Detail entity object type</typeparam>
        /// <typeparam name="DETAIL2">The expected Detail entity object type</typeparam>
        /// <param name="SQLQuery">SQL Query to use</param>
        /// <param name="splitOnField">The field name to split on</param>
        /// <param name="detailPropertyName1">The detail name property name</param>
        /// <param name="detailPropertyName2">The detail name property name</param>
        /// <returns><see cref="IList{T}"/> containing the Entity records</returns>
        public IList<TENTITY> ReadRecords<TENTITY, DETAIL1, DETAIL2>(string SQLQuery,
                                                                    string splitOnField,
                                                                    string detailPropertyName1,
                                                                    string detailPropertyName2)
         where TENTITY : class
         where DETAIL1 : class
         where DETAIL2 : class
        {
            try
            {
                _lastError = "";
                _lastQuery = "";
                if (string.IsNullOrEmpty(DBConnectionString))
                {
                    _lastError = "Connection String not set";
                    throw new ArgumentException(_lastError);
                }
                string sSQL = SQLQuery.Replace("[tablename]", GetTableName<TENTITY>()).Replace("[TABLENAME]", GetTableName<TENTITY>());
                _lastQuery = sSQL;
                using IDbConnection db = new SqlConnection(DBConnectionString);
                var results = db.Query<TENTITY, DETAIL1, DETAIL2, TENTITY>(sSQL,
                                                               (parent, detail1, detail2) =>
                                                               {
                                                                   // Correct the ID of the parent
                                                                   parent.GetType().GetProperty("ID").SetValue(parent, detail1.GetType().GetProperty("HeaderID").GetValue(detail1), null);
                                                                   parent.GetType().GetProperty(detailPropertyName1).SetValue(parent, detail1, null);
                                                                   parent.GetType().GetProperty(detailPropertyName2).SetValue(parent, detail2, null);
                                                                   return parent;
                                                               },
                                                               splitOn: splitOnField, commandTimeout: DefaultTimeOut)
                    .Distinct()
                    .ToList();
                return results;
            }
            catch (DbException ex)
            {
                if (ex.Message.Contains("deadlocked"))
                {
                    // Retry
                    _errorLogger.LogError(ex.Message);
                }
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
                _lastError = "";
                _lastQuery = "";
                if (string.IsNullOrEmpty(DBConnectionString))
                {
                    _lastError = "Connection String not set";
                    throw new ArgumentException(_lastError);
                }

                using IDbConnection db = new SqlConnection(DBConnectionString);
                return db.Insert(Record, commandTimeout: DefaultTimeOut);
            }
            catch (DbException ex)
            {
                _lastError = ex.Message;
                throw new DatabaseAccessHelperException(ex.Message);
            }
        }

        /// <summary>Insert mew entity records from a supplied list</summary>
        /// <typeparam name="TENTITY">Entity Object type</typeparam>
        /// <param name="Records">a <see cref="List{T}"/> of records to insert</param>
        /// <returns><see cref="bool"/> true/false for success/failure</returns>
        public bool InsertRecords<TENTITY>(List<TENTITY> Records) where TENTITY : class
        {
            try
            {
                _lastError = "";
                _lastQuery = "";
                if (string.IsNullOrEmpty(DBConnectionString))
                {
                    _lastError = "Connection String not set";
                    throw new ArgumentException(_lastError);
                }

                using IDbConnection db = new SqlConnection(DBConnectionString);
                db.Insert(Records, commandTimeout: DefaultTimeOut);
                return true;
            }
            catch (DbException ex)
            {
                _lastError = ex.Message;
                _errorLogger.LogError(ex.Message);
                throw new DatabaseAccessHelperException(ex.Message);
            }
        }

        /// <summary>Execute an SQL Statement </summary>
        /// <param name="sqlCmd">String holding the SQL Command</param>
        /// <param name="Params"><see cref="DynamicParameters"/>Collection of parameters</param>
        public object ExecuteSQL(string sqlCmd, DynamicParameters Params = null)
        {
            try
            {
                _lastError = "";
                _lastQuery = "";
                if (string.IsNullOrEmpty(DBConnectionString))
                {
                    _lastError = "Connection String not set";
                    throw new ArgumentException(_lastError);
                }

                using IDbConnection db = new SqlConnection(DBConnectionString);
                return db.Execute(sqlCmd, Params, commandTimeout: DefaultTimeOut);
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
                _lastError = "";
                _lastQuery = "";
                if (string.IsNullOrEmpty(DBConnectionString))
                {
                    _lastError = "Connection String not set";
                    throw new ArgumentException(_lastError);
                }

                using IDbConnection db = new SqlConnection(DBConnectionString);
                return db.ExecuteScalar(sqlCmd, commandTimeout: DefaultTimeOut);

            }
            catch (DbException ex)
            {
                _lastError = ex.Message;
                _errorLogger.LogError(ex.Message);
                throw new DatabaseAccessHelperException(ex.Message);
            }
        }
        /// <summary>Execute an SQL Statement </summary>
        /// <param name="sqlCmd">String holding the SQL Command</param>
        public dynamic ExecuteQuery(string sqlCmd)
        {
            try
            {
                _lastError = "";
                _lastQuery = "";
                if (string.IsNullOrEmpty(DBConnectionString))
                {
                    _lastError = "Connection String not set";
                    throw new ArgumentException(_lastError);
                }

                using IDbConnection db = new SqlConnection(DBConnectionString);
                return db.Query(sqlCmd, commandTimeout: DefaultTimeOut);

            }
            catch (DbException ex)
            {
                _lastError = ex.Message;
                _errorLogger.LogError(ex.Message);
                throw new DatabaseAccessHelperException(ex.Message);
            }
        }
        /// <summary>Execute an SQL Statement </summary>
        /// <param name="SprocName">String holding the SQL Stored Proceedure Name</param>
        /// <param name="dParams">String holding the SQL params</param>
        /// <remarks>Modified to detect output params and return either a single value or a list of values</remarks>
        public object ExecuteSproc(string SprocName, DynamicParameters dParams, List<string> outputParamNames = null)
        {
            try
            {
                _lastError = "";
                _lastQuery = "";
                if (string.IsNullOrEmpty(DBConnectionString))
                {
                    _lastError = "Connection String not set";
                    throw new ArgumentException(_lastError);
                }

                using IDbConnection db = new SqlConnection(DBConnectionString);

                if (outputParamNames != null && outputParamNames.Count > 0)
                {
                    db.Execute(SprocName, dParams, commandType: CommandType.StoredProcedure, commandTimeout: DefaultTimeOut);

                    if (outputParamNames.Count == 1)
                    {
                        // Return single value
                        return dParams.Get<object>(outputParamNames[0]);
                    }
                    else
                    {
                        // Return multiple values
                        List<object> list = new List<object>();
                        foreach (var outputParamName in outputParamNames)
                        {
                            list.Add(dParams.Get<object>(outputParamName));
                        }
                        return list;
                    }
                }
                else
                {
                    return db.Execute(SprocName, dParams, commandType: CommandType.StoredProcedure, commandTimeout: DefaultTimeOut);
                }
            }
            catch (DbException ex)
            {
                _lastError = ex.Message;
                _errorLogger.LogError(ex.Message);
                throw new DatabaseAccessHelperException(ex.Message);
            }
        }
        /// <summary>Execute an SQL Statement </summary>
        /// <param name="SprocName">String holding the SQL Stored Proceedure Name</param>
        /// <param name="dParams">String holding the SQL params</param>
        /// <returns>Dynamic list</returns>
        public IList<dynamic> ExecuteSprocList(string SprocName, DynamicParameters dParams)
        {
            try
            {
                _lastError = "";
                _lastQuery = "";
                if (string.IsNullOrEmpty(DBConnectionString))
                {
                    _lastError = "Connection String not set";
                    throw new ArgumentException(_lastError);
                }

                using IDbConnection db = new SqlConnection(DBConnectionString);
                return db.Query(SprocName, dParams, commandType: CommandType.StoredProcedure, commandTimeout: DefaultTimeOut).ToList();

            }
            catch (DbException ex)
            {
                _lastError = ex.Message;
                _errorLogger.LogError(ex.Message);
                throw new DatabaseAccessHelperException(ex.Message);
            }
        }
        /// <summary>Execute a SQL Query and return the Effected row count</summary>
        /// <param name="sqlCmd">String holding the SQL Command</param>
        /// <returns>The number of effected rows <see cref="long"/></returns>
        public long ExecuteSQLWithEffectedCount(string sqlCmd)
        {
            _lastError = "";
            _lastQuery = "";
            try
            {
                if (string.IsNullOrEmpty(DBConnectionString))
                {
                    _lastError = "Connection String not set";
                    throw new ArgumentException(_lastError);
                }
                using IDbConnection db = new SqlConnection(DBConnectionString);
                return db.Execute(sqlCmd, commandTimeout: DefaultTimeOut);
            }
            catch (DbException ex)
            {
                _lastError = ex.Message;
                _errorLogger.LogError(ex.Message);
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
                _lastError = "";
                _lastQuery = "";
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
                _lastQuery = sSQL;
                using IDbConnection db = new SqlConnection(DBConnectionString);
                return db.ExecuteScalar(sSQL, commandTimeout: DefaultTimeOut);
            }
            catch (DbException ex)
            {
                _lastError = ex.Message;
                _errorLogger.LogError(ex.Message);
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
                _lastError = "";
                _lastQuery = "";
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
                _lastQuery = sSQL;
                using IDbConnection db = new SqlConnection(DBConnectionString);
                return db.Query<int>(sSQL, commandTimeout: DefaultTimeOut).FirstOrDefault();
            }
            catch (DbException ex)
            {
                _lastError = ex.Message;
                _errorLogger.LogError(ex.Message);
                throw new DatabaseAccessHelperException(ex.Message);
            }
        }

        /// <summary>Update an Entity record</summary>
        /// <typeparam name="TENTITY">Entity Object type</typeparam>
        /// <param name="Record">The Record to Update</param>
        /// <returns><see cref="bool"/> true/false for success/failure</returns>
        public bool UpdateRecord<TENTITY>(TENTITY Record) where TENTITY : class
        {
            try
            {
                _lastError = "";
                _lastQuery = "";
                if (string.IsNullOrEmpty(DBConnectionString))
                {
                    _lastError = "Connection String not set";
                    throw new ArgumentException(_lastError);
                }

                using IDbConnection db = new SqlConnection(DBConnectionString);
                return db.Update(Record, commandTimeout: DefaultTimeOut);
            }
            catch (DbException ex)
            {
                _lastError = ex.Message;
                _errorLogger.LogError(ex.Message);
                throw new DatabaseAccessHelperException(ex.Message);
            }
        }

        /// <summary>Update an Entity record</summary>
        /// <typeparam name="TENTITY">Entity Object type</typeparam>
        /// <param name="Record">The Record to Update</param>
        /// <param name="originalEntity">A Copy of the original Record before changes made</param>
        /// <param name="OverrideTableName">Override the tablename of the entity</param>
        /// <returns><see cref="bool"/> true/false for success/failure</returns>
        public bool UpdateRecord<TENTITY>(TENTITY Record, TENTITY originalEntity, string OverrideTableName = "") where TENTITY : class
        {
            try
            {
                _lastError = "";
                _lastQuery = "";
                if (string.IsNullOrEmpty(DBConnectionString))
                {
                    _lastError = "Connection String not set";
                    throw new ArgumentException(_lastError);
                }

                // If originalEntity record is passed then perform a compare to get a list of differences then
                // build an update query to only commit those differences
                if (originalEntity != null)
                {
                    // Get a list of all the changes (compare original to current)
                    // This will also return the field type and an entry for the ID (false passed into the Compare)
                    List<ObjectExtensions.Variance> diffs = ((TENTITY)originalEntity).DetailedCompare(Record, false);
                    if (diffs != null && diffs.Count > 0)
                    {
                        string TableName = GetTableName<TENTITY>();
                        if (!string.IsNullOrEmpty(OverrideTableName))
                        {
                            TableName = OverrideTableName;
                        }
                        string update = $"UPDATE {TableName}\r";
                        string fieldsList = string.Empty;
                        string IDField = string.Empty;
                        foreach (var field in diffs)
                        {
                            if (field.isKeyField)
                            {
                                IDField = field.PropertyName;
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(fieldsList))
                                {
                                    fieldsList += ",\r";
                                }
                                fieldsList += $"{field.PropertyName} = @{field.PropertyName}";
                            }
                        }
                        if (!string.IsNullOrEmpty(fieldsList))
                        {
                            fieldsList += "\r";
                        }
                        update += $"SET {fieldsList}\r WHERE {IDField} = @{IDField}";
                        _lastQuery = update;
                        using IDbConnection db = new SqlConnection(DBConnectionString);
                        int rows = db.Execute(update, Record, commandTimeout: DefaultTimeOut);
                        return rows != 0;
                    }
                    return false;
                }
                else
                {
                    using IDbConnection db = new SqlConnection(DBConnectionString);
                    return db.Update(Record, commandTimeout: DefaultTimeOut);
                }
            }
            catch (DbException ex)
            {
                _lastError = ex.Message;
                _errorLogger.LogError(ex.Message);
                throw new DatabaseAccessHelperException(ex.Message);
            }
        }
        /// <summary>
        /// Updates table T with the values in param.
        /// The table must have a key named "Id" and the value of id must be included in the "param" anon object. 
        /// The Id value is used as the "where" clause in the generated SQL
        /// </summary>
        /// <typeparam name="TENTITY">Type to update. Translates to table name</typeparam>     
        /// <param name="Record">Object holding the entity record</param>
        /// <param name="param">list of fields</param>
        /// <param name="OverrideTableName">Override the table name of the object</param>
        /// <returns>The Id of the updated row. If no row was updated or id was not part of fields, returns null</returns>
        public int? UpdateFields<TENTITY>(TENTITY Record, List<string> param, string OverrideTableName = "")
        {
            _lastError = "";
            _lastQuery = "";
            List<string> names = new List<string>();
            List<object> values = new List<object>();
            List<DbType> types = new List<DbType>();

            int? id = (int)Record.GetType().GetProperty("ID").GetValue(Record);
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

            foreach (string field in param)
            {
                if (field.ToLower() != "id")
                {
                    names.Add(field);
                    values.Add(Record.GetType().GetProperty(field).GetValue(Record));
                    if (Record.GetType().GetProperty(field).PropertyType.FullName.Contains("eDocStatus"))
                    {
                        types.Add(SQLDataTypeHelper.GetDbType(typeof(int)));
                    }
                    else
                    {
                        types.Add(SQLDataTypeHelper.GetDbType(Record.GetType().GetProperty(field).PropertyType));
                    }
                }
            }

            if (id != null && values.Count > 0)
            {
                string sql = $"UPDATE {TableName} SET {string.Join(",", names.Select(t => { t = $"{t} = @{t}"; return t; }))} WHERE ID=@id";
                using IDbConnection db = new SqlConnection(DBConnectionString);
                using IDbCommand cmd = db.CreateCommand();
                cmd.CommandText = sql;
                cmd.CommandType = CommandType.Text;
                cmd.CommandTimeout = DefaultTimeOut;
                for (int i = 0; i < names.Count; i++)
                {
                    IDbDataParameter p = cmd.CreateParameter();
                    p.ParameterName = $"@{names[i]}";
                    if (values[i] == null)
                    {
                        p.Value = DBNull.Value;
                    }
                    else
                    {
                        p.Value = values[i];
                    }

                    p.DbType = types[i];
                    cmd.Parameters.Add(p);
                }
                // Add the id parameter
                IDbDataParameter pID = cmd.CreateParameter();
                pID.ParameterName = $"@id";
                pID.Value = id;
                pID.DbType = DbType.Int32;
                cmd.Parameters.Add(pID);
                //return db.Execute(sql,cmd) > 0 ? id : null;
                db.Open();
                return cmd.ExecuteNonQuery();
            }
            return null;
        }

        /// <summary>
        /// Updates table T with the values in param.
        /// The table must have a key named "Id" and the value of id must be included in the "param" anon object. 
        /// The Id value is used as the "where" clause in the generated SQL
        /// </summary>
        /// <typeparam name="TENTITY">Type to update. Translates to table name</typeparam>     
        /// <param name="Record">Object holding the entity record</param>
        /// <param name="fieldName">The single field name</param>      
        /// <param name="fieldValue">The new field value</param>        
        /// <returns>The Id of the updated row. If no row was updated or id was not part of fields, returns null</returns>
        public int? UpdateField<TENTITY>(TENTITY Record, string fieldName, object fieldValue)
        {
            _lastError = "";
            _lastQuery = "";
            List<string> names = new List<string>();
            List<object> values = new List<object>();
            List<DbType> types = new List<DbType>();

            int? id = (int)Record.GetType().GetProperty("ID").GetValue(Record);
            string TableName = GetTableName<TENTITY>();
            if (string.IsNullOrEmpty(TableName))
            {
                _lastError = "Invalid Table Name";
                throw new ArgumentException(_lastError);
            }

            if (fieldName.ToLower() != "id")
            {
                names.Add(fieldName);
                values.Add(fieldValue);
                types.Add(SQLDataTypeHelper.GetDbType(Record.GetType().GetProperty(fieldName).PropertyType));
            }

            if (id != null && values.Count > 0)
            {
                string sql = $"UPDATE {TableName} SET {string.Join(",", names.Select(t => { t = $"{t} = @{t}"; return t; }))} WHERE ID=@id";
                using IDbConnection db = new SqlConnection(DBConnectionString);
                using IDbCommand cmd = db.CreateCommand();
                cmd.CommandText = sql;
                cmd.CommandType = CommandType.Text;
                cmd.CommandTimeout = DefaultTimeOut;
                for (int i = 0; i < names.Count; i++)
                {
                    IDbDataParameter p = cmd.CreateParameter();
                    p.ParameterName = $"@{names[i]}";
                    if (values[i] == null)
                    {
                        p.Value = DBNull.Value;
                    }
                    else
                    {
                        p.Value = values[i];
                    }

                    p.DbType = types[i];
                    cmd.Parameters.Add(p);
                }
                // Add the id parameter
                IDbDataParameter pID = cmd.CreateParameter();
                pID.ParameterName = $"@id";
                pID.Value = id;
                pID.DbType = DbType.Int32;
                cmd.Parameters.Add(pID);
                //return db.Execute(sql,cmd) > 0 ? id : null;
                db.Open();
                return cmd.ExecuteNonQuery();
            }
            return null;
        }
        /// <summary>
        /// Updates table with the values in param.
        /// The table must have a key field (default is "ID")
        /// The Id value is used as the "where" clause in the generated SQL
        /// </summary>
        /// <param name="id">The Record ID to update</param>
        /// <param name="TableName">Table to update</param>
        /// <param name="fieldName">The single field name</param>      
        /// <param name="fieldValue">The new field value</param>    
        /// <param name="IDFieldName">The Field Name of the ID (default is "ID")</param>
        /// <returns>The Id of the updated row. If no row was updated or id was not part of fields, returns null</returns>
        public int? UpdateField(string TableName, int id, string fieldName, object fieldValue, string IDFieldName = "ID")
        {
            _lastError = "";
            _lastQuery = "";
            List<string> names = new List<string>();
            List<object> values = new List<object>();
            List<DbType> types = new List<DbType>();


            if (string.IsNullOrEmpty(TableName))
            {
                _lastError = "Invalid Table Name";
                throw new ArgumentException(_lastError);
            }

            if (fieldName.ToLower() != "id")
            {
                names.Add(fieldName);
                values.Add(fieldValue);
                types.Add(SQLDataTypeHelper.GetDbType(fieldValue.GetType()));
            }

            if (values.Count > 0)
            {
                string sql = $"UPDATE {TableName} SET {string.Join(",", names.Select(t => { t = $"{t} = @{t}"; return t; }))} WHERE {IDFieldName}=@id";
                using IDbConnection db = new SqlConnection(DBConnectionString);
                using IDbCommand cmd = db.CreateCommand();
                cmd.CommandText = sql;
                cmd.CommandType = CommandType.Text;
                cmd.CommandTimeout = DefaultTimeOut;
                for (int i = 0; i < names.Count; i++)
                {
                    IDbDataParameter p = cmd.CreateParameter();
                    p.ParameterName = $"@{names[i]}";
                    if (values[i] == null)
                    {
                        p.Value = DBNull.Value;
                    }
                    else
                    {
                        p.Value = values[i];
                    }

                    p.DbType = types[i];
                    cmd.Parameters.Add(p);
                }
                // Add the id parameter
                IDbDataParameter pID = cmd.CreateParameter();
                pID.ParameterName = $"@id";
                pID.Value = id;
                pID.DbType = DbType.Int32;
                cmd.Parameters.Add(pID);
                //return db.Execute(sql,cmd) > 0 ? id : null;
                db.Open();
                return cmd.ExecuteNonQuery();
            }
            return null;
        }
        /// <summary>Delete an Entity record</summary>
        /// <typeparam name="TENTITY">Entity Object type</typeparam>
        /// <param name="Record">The Record to Delete</param>
        /// <returns><see cref="bool"/> true/false for success/failure</returns>
        public bool DeleteRecord<TENTITY>(TENTITY Record) where TENTITY : class
        {
            try
            {
                _lastError = "";
                _lastQuery = "";
                if (string.IsNullOrEmpty(DBConnectionString))
                {
                    _lastError = "Connection String not set";
                    throw new ArgumentException(_lastError);
                }

                using IDbConnection db = new SqlConnection(DBConnectionString);
                return db.Delete(Record, commandTimeout: DefaultTimeOut);
            }
            catch (DbException ex)
            {
                _lastError = ex.Message;
                _errorLogger.LogError(ex.Message);
                throw new DatabaseAccessHelperException(ex.Message);
            }
        }

        /// <summary>Return the TableName of the POCO </summary>
        /// <typeparam name="TENTITY">the PCO Entity</typeparam>
        /// <returns><see cref="string"/> holding the tablename</returns>
        public string GetTableName<TENTITY>()
        {
            var attr = typeof(TENTITY).GetCustomAttribute<Dapper.Contrib.Extensions.TableAttribute>(false);
            return attr != null ? attr.Name : "";
        }
        /// <summary>Returns the last error message if any of the specified action</summary>
        /// <returns><see cref="string"/> value containing any error messages.</returns>
        public string LastError { get { return _lastError; } }
        /// <summary>Returns the last query string if any of the specified action</summary>
        /// <returns><see cref="string"/> value containing the query string.</returns>
        public string LastQuery { get { return _lastQuery; } }
        /// <summary>Retrieve Table columns list</summary>
        /// <param name="tableName">Table to query</param>
        /// <param name="schemaName">Schema Name : default :dbo </param>
        /// <returns>IList of TableColumns</returns>
        public IList<TableColumn> GetTableColumns(string tableName, string schemaName = "dbo")
        {
            if (TableExists(tableName))
            {
                IList<TableColumn> columns = ReadRecordsSQL<TableColumn>($"SELECT COLUMN_NAME,DATA_TYPE,CHARACTER_MAXIMUM_LENGTH ,ORDINAL_POSITION FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{tableName}' AND TABLE_SCHEMA='{schemaName}'");

                return columns;
            }
            return null;
        }
        /// <summary>Retrieve Tables list or Views List</summary> 
        /// <param name="schemaName">Schema Name : default :dbo </param>
        /// <param name="TableType">Table Type (BASE TABLE or VIEW)  : default :BASE TABLE </param>
        /// <returns>IList of RableRecords</returns>
        public IList<TableRecord> GetTables(string schemaName = "dbo", string TableType = "BASE TABLE")
        {
            IList<TableRecord> tables = ReadRecordsSQL<TableRecord>($"SELECT TABLE_SCHEMA, TABLE_NAME, TABLE_TYPE FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='{schemaName}' AND TABLE_TYPE = '{TableType}' ORDER BY TABLE_NAME");
            return tables;
        }
        /// <summary>Create a Column in the Database</summary>
        /// <param name="tableName">Table to create the column for</param>
        /// <param name="columnName">Unique Column Name</param>
        /// <param name="dataType">Column Data Type : default varchar</param>
        /// <param name="colSize">Column Size : defaul :50</param>
        /// <param name="allowNull">Column Null allowed : default :true</param>
        /// <param name="schemaName">Schema Name : default :dbo </param>
        public void CreateColumn(string tableName, string columnName, string dataType = "varchar", int colSize = 50, bool allowNull = true, string schemaName = "dbo")
        {
            string colDataTypeSQL = dataType;
            string allowNullSQL = allowNull ? "NULL" : "NOT NULL";

            if (dataType.ToLower() == "varchar")
            {
                colDataTypeSQL = $"{dataType}({colSize})";
            }
            if (dataType.ToLower() == "bit")
            {
                allowNullSQL = "NOT NULL";
            }

            string createSQL = @"BEGIN TRANSACTION
                    SET QUOTED_IDENTIFIER ON
                    SET ARITHABORT ON
                    SET NUMERIC_ROUNDABORT OFF
                    SET CONCAT_NULL_YIELDS_NULL ON
                    SET ANSI_NULLS ON
                    SET ANSI_PADDING ON
                    SET ANSI_WARNINGS ON
                    COMMIT
                    BEGIN TRANSACTION                    
                    ";
            createSQL += $@"ALTER TABLE {schemaName}.{tableName} ADD {columnName} {colDataTypeSQL} {allowNullSQL}";

            if (dataType.ToLower() == "bit")
            {
                createSQL += $@" CONSTRAINT DF_{tableName}_{columnName} DEFAULT((0))
                ";
            }
            createSQL += $@"
                    ALTER TABLE {schemaName}.{tableName} SET(LOCK_ESCALATION = TABLE)                
                    COMMIT";

            ExecuteScalar(createSQL);

        }
        /// <summary>Check if table exists in the database</summary>
        /// <param name="tableName">The name of the table to check</param>
        /// <returns><see cref="bool"/> : true if exists</returns>
        public bool TableExists(string tableName)
        {
            var rslt = ExecuteScalar($"SELECT CASE WHEN OBJECT_ID('dbo.{tableName}', 'U') IS NOT NULL THEN 1 ELSE 0 END");
            bool exists = (int)rslt == 1;
            return exists;
        }
        /// <summary>Check if table exists in the database</summary>
        /// <param name="tableName">The name of the table to check</param>
        /// <param name="ColumnName">The name of the column to check</param>
        /// <returns><see cref="bool"/> : true if exists</returns>        
        public bool ColumnExists(string tableName, string ColumnName)
        {
            var rslt = ExecuteScalar($"SELECT CASE WHEN EXISTS(SELECT 1 FROM sys.columns WHERE Name = N'{ColumnName}' AND Object_ID = Object_ID(N'dbo.{tableName}')) THEN 1 ELSE 0 END");
            bool exists = (int)rslt == 1;
            return exists;
        }
        public IList<ColumnMetadata> GetColumnsMetaData(string tableName, string SchemaName = "dbo")
        {
            var sql = @$"
SELECT
       c.name                                AS ColumnName,
       c.object_id                           AS ObjectId,
       c.system_type_id                      AS SystemTypeId,
       c.user_type_id                        AS UserTypeId,   
       c.max_length                          AS MaxLength,
       c.precision                           AS Precision,
       c.scale                               AS Scale,
       c.collation_name                      AS CollationName,
       c.is_nullable                         AS IsNullable,
       c.is_ansi_padded                      AS IsAnsiPadded,
       c.is_rowguidcol                       AS IsRowGuidCol,
       c.is_identity                         AS IsIdentity,
       c.is_computed                         AS IsComputed,
       c.is_filestream                       AS IsFileStream,
       c.is_replicated                       AS IsReplicated,
       c.is_non_sql_subscribed               AS IsNonSqlSubscribed,
       c.is_merge_published                  AS IsMergePublished,
       c.is_dts_replicated                   AS IsDtsReplicated,
       c.is_xml_document                     AS IsXmlDocument,
       c.xml_collection_id                   AS XmlCollectionId,
       c.default_object_id                   AS DefaultObjectId,
       c.rule_object_id                      AS RuleObjectId,
       c.is_sparse                           AS IsSparse,
       c.is_column_set                       AS IsColumnSet,
       -- The following are SQL Server 2016+ only — comment out for 2008-2014 support
       --c.generated_always_type               AS GeneratedAlwaysType,
       --c.generated_always_type_desc          AS GeneratedAlwaysTypeDesc,
       --c.encryption_type                     AS EncryptionType,
       --c.encryption_type_desc                AS EncryptionTypeDesc,
       --c.encryption_algorithm_name           AS EncryptionAlgorithmName,
       --c.column_encryption_key_id            AS ColumnEncryptionKeyId,
       --c.column_encryption_key_database_name AS ColumnEncryptionKeyDatabaseName,
       --c.is_hidden                           AS IsHidden,
       --c.is_masked                           AS IsMasked,
       -- SQL 2022 Support
       --c.is_data_deletion_filter_column      AS IsDataDeletionFilterColumn,
       --c.ledger_view_column_type             AS LedgerViewColumnType,
       --c.ledger_view_column_type_desc        AS LedgerViewColumnTypeDesc,
       --c.is_dropped_ledger_column            AS IsDroppedLedgerColumn,
        t.create_date                         AS CreateDate,
        t.modify_date                         AS ModifyDate
       
    FROM
       sys.columns           AS c
       INNER JOIN sys.tables AS t ON c.object_id = t.object_id
    WHERE
       (t.name                      = '{tableName}')
      AND (SCHEMA_NAME(t.schema_id) = '{SchemaName}')
    ORDER BY
       c.column_id;";
            IList<ColumnMetadata> columns = ReadRecordsSQL<ColumnMetadata>(sql);
            return columns;
        }
        /// <summary>Get a Fields MaxLength value</summary>
        /// <param name="tableName">The name of the table to check</param>
        /// <param name="ColumnName">The name of the column to check</param>
        /// <returns><see cref="int"/> -1 if not found or no limit (max)</returns>
        public int ColumnMaxLength(string tableName, string ColumnName)
        {
            var rslt = ExecuteScalar($"SELECT max_length FROM sys.columns WHERE object_id =Object_ID(N'dbo.{tableName}') AND name = '{ColumnName}'");
            if (rslt == null)
            {
                return -1;
            }
            int length = int.Parse(rslt.ToString());
            return length;
        }

        /// <summary>Create a table in the Database</summary>
        /// <param name="tableName">The name of the table to create</param>
        /// <param name="idColName">The Column name for the ID column : default : ID</param>
        /// <param name="idColType">The Column type for the ID column : default : int</param>
        /// <param name="schemaName">Schema Name : default :dbo </param>
        public void CreateTable(string tableName,
            string idColName = "ID",
            string idColType = "int",
            string schemaName = "dbo"
            )
        {
            string createSQL = @"BEGIN TRANSACTION
                SET QUOTED_IDENTIFIER ON
                SET ARITHABORT ON
                SET NUMERIC_ROUNDABORT OFF
                SET CONCAT_NULL_YIELDS_NULL ON
                SET ANSI_NULLS ON
                SET ANSI_PADDING ON
                SET ANSI_WARNINGS ON
                COMMIT
                BEGIN TRANSACTION
                ";

            createSQL += $@"CREATE TABLE [{schemaName}].[{tableName}]
	                (
	                {idColName} {idColType} NOT NULL IDENTITY (1, 1)
	                )  ON [PRIMARY]                
                ALTER TABLE [{schemaName}].[{tableName}] ADD CONSTRAINT
	                PK_{tableName} PRIMARY KEY CLUSTERED 
	                (
	                {idColName}
	                ) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]                
                ALTER TABLE [{schemaName}].[{tableName}] SET (LOCK_ESCALATION = TABLE)                
                COMMIT
                ";

            ExecuteScalar(createSQL);
        }
        /// <summary>
        /// Create a table in SQL using a list of columns
        /// </summary>
        /// <param name="tableName">The tables name</param>
        /// <param name="columns">list collection holding the column data</param>
        /// <param name="schemaName">the schema name ,default is dbo</param>
        /// <param name="collation">The collation to use, set blank to use database collation</param>
        /// <param name="prettyFormat">Build SQL Create string with formatting </param>
        /// <returns></returns>
        public void CreateTable(string tableName,
            List<TableColumn> columns,
            string schemaName = "dbo",
            string collation = "SQL_Latin1_General_CP1_CI_AS",
            bool prettyFormat = true)
        {
            StringBuilder createSQL = new StringBuilder();

            createSQL.AppendLine("BEGIN TRANSACTION");
            createSQL.AppendLine("SET QUOTED_IDENTIFIER ON");
            createSQL.AppendLine("SET ARITHABORT ON");
            createSQL.AppendLine("SET NUMERIC_ROUNDABORT OFF");
            createSQL.AppendLine("SET CONCAT_NULL_YIELDS_NULL ON");
            createSQL.AppendLine("SET ANSI_NULLS ON");
            createSQL.AppendLine("SET ANSI_PADDING ON");
            createSQL.AppendLine("SET ANSI_WARNINGS ON");
            createSQL.AppendLine("COMMIT");
            createSQL.AppendLine("BEGIN TRANSACTION");

            // Find index col if any
            string idColName = "";
            string idColType = "";
            bool isIdentity = false;
            string colDetail = "";
            // Determine the max field name length
            int maxNameWidth = 0;
            int maxTypeWidth = 0;
            // If pretty is set, find out some sizes so some padding can be applied
            if (prettyFormat)
            {
                foreach (var col in columns)
                {
                    if (col.COLUMN_NAME.Length > maxNameWidth)
                    {
                        maxNameWidth = col.COLUMN_NAME.Length;
                    }

                    int lenCheck = col.DATA_TYPE.Length;
                    switch (col.DATA_TYPE.ToUpper())
                    {
                        case "VARCHAR":
                        case "NVARCHAR":
                        case "CHAR":
                        case "TEXT":
                        case "NCHAR":
                        case "NTEXT":
                            if (col.CHARACTER_MAXIMUM_LENGTH == 0)
                            {
                                lenCheck += 5; // (max)
                            }
                            else
                            {
                                lenCheck += 2 + col.CHARACTER_MAXIMUM_LENGTH.ToString().Length;
                            }
                            break;
                    }
                    if (lenCheck > maxTypeWidth)
                    {
                        maxTypeWidth = lenCheck;
                    }
                }
            }
            foreach (var col in columns)
            {
                if (col.IS_ID)
                {
                    idColName = col.COLUMN_NAME;
                    idColType = col.DATA_TYPE;
                    isIdentity = true;
                }
                else
                {
                    colDetail += GetColString(col, 8, collation, maxNameWidth, maxTypeWidth) + Environment.NewLine;
                }
            }
            createSQL.AppendLine($"CREATE TABLE [{schemaName}].[{tableName}] ");
            createSQL.AppendLine($"    ( ");
            if (!string.IsNullOrEmpty(idColName))
            {
                createSQL.Append($"        [{idColName}]");

                if (maxNameWidth > 0)
                {
                    // pad spaces
                    int diff = maxNameWidth - idColName.Length + 2;
                    createSQL.Append(' ', diff);
                }
                else
                {
                    createSQL.Append(' ');
                }
                createSQL.Append($"[{idColType.ToUpper()}] ");
                if (maxTypeWidth > 0)
                {
                    // pad spaces
                    int diff = maxTypeWidth - idColType.Length;
                    createSQL.Append(' ', diff);
                }
                else
                {
                    createSQL.Append(' ');
                }
                createSQL.Append($" {(isIdentity ? "NOT NULL IDENTITY" : "NULL")} (1, 1), ");
            }
            createSQL.AppendLine();
            createSQL.AppendLine($"{colDetail} ");
            createSQL.AppendLine($"    )  ON [PRIMARY]");
            if (!string.IsNullOrEmpty(idColName))
            {
                createSQL.AppendLine($"ALTER TABLE [{schemaName}].[{tableName}]");
                createSQL.AppendLine($"ADD  ");
                createSQL.AppendLine($"    CONSTRAINT PK_{tableName}");
                createSQL.AppendLine($"    PRIMARY KEY CLUSTERED ([{idColName}])");
                createSQL.AppendLine($"    WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];");
                createSQL.AppendLine($"ALTER TABLE [{schemaName}].[{tableName}] SET (LOCK_ESCALATION = TABLE); ");
            }
            createSQL.AppendLine($"COMMIT; ");
            try
            {
                var rslt = ExecuteScalar(createSQL.ToString());
            }
            catch (Exception ex)
            {
                _lastError = ex.Message;
            }

            _lastQuery = createSQL.ToString();
        }
        /// <summary>
        /// Return a formatted string for the required column
        /// </summary>
        /// <param name="colItem">The TableColumn entry</param>
        /// <param name="indent">default indent</param>
        /// <param name="collation">collation (will override the default set for the database)</param>
        /// <param name="maxNameWidth">Max width of the column names for pretty formatting</param>
        /// <param name="maxTypeWidth">The max with of the column type for pretty formatting</param>
        /// <returns></returns>
        string GetColString(TableColumn colItem, int indent = 10, string collation = "", int maxNameWidth = 0, int maxTypeWidth = 0)
        {
            string collationSQL = "";
            bool canHaveCollation = false;
            switch (colItem.DATA_TYPE.ToUpper())
            {
                case "VARCHAR":
                case "NVARCHAR":
                case "CHAR":
                case "TEXT":
                case "NCHAR":
                case "NTEXT":
                    canHaveCollation = true;
                    if (!string.IsNullOrEmpty(collation))
                        collationSQL = $"COLLATE {collation} ";
                    break;
                default:
                    break;
            }
            StringBuilder colData = new StringBuilder();
            colData.Append(' ', indent);

            colData.Append($"[{colItem.COLUMN_NAME}]");
            if (maxNameWidth > 0)
            {
                // pad spaces
                int diff = maxNameWidth - colItem.COLUMN_NAME.Length;
                colData.Append(' ', diff + 1);
            }
            else
            {
                colData.Append(' ');
            }
            StringBuilder sbType = new StringBuilder();
            sbType.Append($"[{colItem.DATA_TYPE.ToLower()}]");

            if (canHaveCollation)
            {
                if (colItem.CHARACTER_MAXIMUM_LENGTH == 0)
                {
                    sbType.Append($"(max)");
                }
                else
                {
                    sbType.Append($"({colItem.CHARACTER_MAXIMUM_LENGTH})");
                }

                if (maxTypeWidth > 0)
                {
                    // pad spaces
                    int diff = (maxTypeWidth + 2) - sbType.ToString().Length;
                    sbType.Append(' ', diff + 1);
                }
                sbType.Append(collationSQL);
            }
            else
            {
                if (maxTypeWidth > 0)
                {
                    // pad spaces
                    int diff = (maxTypeWidth + 2) - sbType.Length;
                    sbType.Append(' ', diff + 1);
                }
                else
                {
                    sbType.Append(' ');
                }
            }

            colData.Append(sbType.ToString());

            // Null Check
            colData.Append($"{(colItem.ALLOW_NULL ? "NULL" : "NOT NULL")}, ");

            return colData.ToString();
        }
        /// <summary>
        /// Create a table in SQL from a class object
        /// </summary>
        /// <param name="type">The class type</param>
        /// <param name="unincode">use unincode string (nvarchar) over (varchar)</param>        
        /// <param name="collation">The collation to use, set blank to use database collation</param>
        /// <remarks>e.g. SQL_Latin1_General_CP1_CI_AS </remarks>
        /// <returns>true/false</returns>
        public bool CreateTable(Type type, bool unincode = true, string collation = "")
        {
            _lastError = "";

            var properties = type.GetPropertiesWithAnnotations();
            string tableName = type.Name;
            List<TableColumn> columns = new List<TableColumn>();

            foreach (var prop in properties)
            {
                // Get TableName
                if (prop.PropertyCategory == "CLASS" && prop.Attributes.Count > 0)
                {
                    foreach (var att in prop.Attributes)
                    {
                        if (att.GetType().Name == "TableAttribute")
                        {
                            tableName = ((Dapper.Contrib.Extensions.TableAttribute)att).Name;
                        }
                    }
                }
                else
                {
                    bool NotMappedAttribute = false;
                    bool isID = false;
                    bool isRequired = false;
                    ColumnAttribute columnAttribute = null;
                    DatabaseGeneratedAttribute databaseGeneratedAttribute = null;
                    foreach (var att in prop.Attributes)
                    {
                        switch (att.GetType().Name)
                        {
                            case "NotMappedAttribute":
                            case "ComputedAttribute":
                                NotMappedAttribute = true;
                                break;
                            case "ColumnAttribute":
                                columnAttribute = att as ColumnAttribute;
                                break;
                            case "DatabaseGeneratedAttribute":
                                databaseGeneratedAttribute = att as DatabaseGeneratedAttribute;
                                break;
                            case "KeyAttribute":
                                isID = true;
                                break;
                            case "RequiredAttribute":
                                isRequired = true;
                                break;
                            default:
                                break;
                        }
                    }
                    if (!NotMappedAttribute)
                    {
                        TableColumn col = new TableColumn()
                        {
                            COLUMN_NAME = prop.PropertyName,
                            DATA_TYPE = SQLDataTypeHelper.GetSqlDbType(prop.PropertyType, unincode).ToString(),
                            ALLOW_NULL = (!isID && !isRequired),
                            IDENTITY = (databaseGeneratedAttribute != null && databaseGeneratedAttribute.DatabaseGeneratedOption == DatabaseGeneratedOption.Identity),
                            CHARACTER_MAXIMUM_LENGTH = 0,
                            ORDINAL_POSITION = 0,
                            IS_ID = isID
                        };
                        //prop.PropertyType.Name.Contains("Nullable")
                        if (columnAttribute != null)
                        {
                            if (columnAttribute.TypeName != null)
                            {
                                col.DATA_TYPE = columnAttribute.TypeName;
                            }
                            if (columnAttribute.TypeName != null && columnAttribute.TypeName.Contains("(") & columnAttribute.TypeName.Contains(")"))
                            {
                                string lenData = columnAttribute.TypeName.GetBetween("(", ")");
                                col.CHARACTER_MAXIMUM_LENGTH = lenData.IsAllNumber() ? int.Parse(columnAttribute.TypeName.GetBetween("(", ")")) : 0;
                                col.DATA_TYPE = col.DATA_TYPE.Replace($"({lenData})", "");
                            }
                        }
                        // hack to change varchar to nvarchar and datetime2 to datetime
                        if (col.DATA_TYPE == "VarChar")
                        {
                            col.DATA_TYPE = "NVarChar";
                        }
                        if (col.DATA_TYPE == "DateTime2")
                        {
                            col.DATA_TYPE = "DateTime";
                        }
                        columns.Add(col);
                    }
                }
            }
            try
            {
                CreateTable(tableName, columns, collation: collation);
                string lastQuery = _lastQuery; // Preserve Query to be returned after exist test
                string lastError = _lastError; // Preserve error to be returned after exist test
                var exists = TableExists(tableName);
                _lastQuery = lastQuery;
                _lastError = lastError;
                if (exists)
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                _lastError = ex.Message;
                _errorLogger.LogError(ex.Message);
            }
            return false;
        }
    }
}