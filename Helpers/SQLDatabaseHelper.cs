


using Dapper;
using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace AGenius.UsefulStuff.Helpers
{
    /// <summary>
    /// This class is used to help encapsulate the use of Dapper objects to access SQL Server
    /// </summary>
    public class SQLDatabaseHelper : IDisposable
    {
        /// <summary>Event Handler for the ErrorEventArgs event</summary>

        public string DBConnectionString
        {
            get; set;
        }
        public Boolean? LastSaveState
        {
            get;
            private set;
        }
        public SQLDatabaseHelper()
        {
            DBConnectionString = "";
        }
        public SQLDatabaseHelper(string ConnectionString)
        {
            DBConnectionString = ConnectionString;
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
                using (IDbConnection db = new SqlConnection(DBConnectionString))
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
                using (IDbConnection db = new SqlConnection(DBConnectionString))
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
                using (IDbConnection db = new SqlConnection(DBConnectionString))
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
        /// <param name="ID">a Guid representing the ID of the record</param>
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
                using (IDbConnection db = new SqlConnection(DBConnectionString))
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
        /// <param name="FieldValue">The Value of the Key field to find</param>
        /// <param name="KeyFieldName">The Key FieldName</param>
        /// <returns>entity records</returns>
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
                using (IDbConnection db = new SqlConnection(DBConnectionString))
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
        public TENTITY ReadRecord<TENTITY>(string KeyFieldName, int FieldValue) where TENTITY : class
        {
            try
            {
                string TableName = GetTableName<TENTITY>();
                if (string.IsNullOrEmpty(TableName))
                {
                    _lastError = "Invalid Table Name";
                    throw new ArgumentException(_lastError);
                }
                if (string.IsNullOrEmpty(KeyFieldName))
                {
                    _lastError = "Missing FieldName or Value for search";
                    throw new ArgumentException(_lastError);
                }
                if (string.IsNullOrEmpty(DBConnectionString))
                {
                    _lastError = "Connection String not set";
                    throw new ArgumentException(_lastError);
                }
                string sWhere = $"WHERE {KeyFieldName} = {FieldValue} ";

                string sSQL = $"SELECT * FROM {TableName} {sWhere}";
                // var Results = null;
                using (IDbConnection db = new SqlConnection(DBConnectionString))
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
                using (IDbConnection db = new SqlConnection(DBConnectionString))
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

                using (IDbConnection db = new SqlConnection(DBConnectionString))
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
                using (IDbConnection db = new SqlConnection(DBConnectionString))
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
        public IList<TENTITY> ReadRecords<TENTITY>(string Where = "")
            where TENTITY : class
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
                string sSQL = $"SELECT * FROM {TableName} {sWhere}";
                // var Results = null;
                using (IDbConnection db = new SqlConnection(DBConnectionString))
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
        /// <param name="TopCount">Only return n records</param>
        /// <returns>entity records</returns>
        public IList<TENTITY> ReadRecords<TENTITY>(string Where, int TopCount)
            where TENTITY : class
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
                using (IDbConnection db = new SqlConnection(DBConnectionString))
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
        public IList<TENTITY> ReadRecords<TENTITY>(string Where = "", string OverrideTableName = "")
            where TENTITY : class
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
                using (IDbConnection db = new SqlConnection(DBConnectionString))
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
        /// <summary> Multi Queries </summary>
        /// <typeparam name="TENTITY"></typeparam>
        /// <typeparam name="DETAIL"></typeparam>
        /// <param name="splitOnField"></param>
        /// <param name="detailPropertyName"></param>
        /// <param name="SQLQuery"></param>
        /// <returns></returns>
        public IList<TENTITY> ReadRecords<TENTITY, DETAIL>(string SQLQuery, string splitOnField, string detailPropertyName)
            where TENTITY : class
            where DETAIL : class
        {
            try
            {
                if (string.IsNullOrEmpty(DBConnectionString))
                {
                    _lastError = "Connection String not set";
                    throw new ArgumentException(_lastError);
                }
                string sSQL = SQLQuery.Replace("[tablename]", GetTableName<TENTITY>());
                using (IDbConnection db = new SqlConnection(DBConnectionString))
                {
                    var results = db.Query<TENTITY, DETAIL, TENTITY>(sSQL,
                                                                   (parent, detail) =>
                                                                   {
                                                                       parent.GetType().GetProperty(detailPropertyName).SetValue(parent, detail, null);
                                                                       return parent;
                                                                   },
                                                                   splitOn: splitOnField, commandTimeout: 360)
                        .Distinct()
                        .ToList();
                    return results;
                }
            }
            catch (DbException ex)
            {
                _lastError = ex.Message;
                throw new DatabaseAccessHelperException(ex.Message);
            }
        }

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
                if (string.IsNullOrEmpty(DBConnectionString))
                {
                    _lastError = "Connection String not set";
                    throw new ArgumentException(_lastError);
                }
                string sSQL = SQLQuery.Replace("[tablename]", GetTableName<TENTITY>());
                using (IDbConnection db = new SqlConnection(DBConnectionString))
                {
                    var results = db.Query<TENTITY, DETAIL1, DETAIL2, TENTITY>(sSQL,
                                                                   (parent, detail1, detail2) =>
                                                                   {
                                                                       // Correct the ID of the parent
                                                                       parent.GetType().GetProperty("ID").SetValue(parent, detail1.GetType().GetProperty("HeaderID").GetValue(detail1), null);
                                                                       parent.GetType().GetProperty(detailPropertyName1).SetValue(parent, detail1, null);
                                                                       parent.GetType().GetProperty(detailPropertyName2).SetValue(parent, detail2, null);
                                                                       return parent;
                                                                   },
                                                                   splitOn: splitOnField)
                        .Distinct()
                        .ToList();
                    return results;
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

                using (IDbConnection db = new SqlConnection(DBConnectionString))
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
        public bool InsertRecords<TENTITY>(List<TENTITY> Records) where TENTITY : class
        {
            try
            {
                if (string.IsNullOrEmpty(DBConnectionString))
                {
                    _lastError = "Connection String not set";
                    throw new ArgumentException(_lastError);
                }

                using (IDbConnection db = new SqlConnection(DBConnectionString))
                {
                    db.Insert(Records);
                    return true;


                    //db.Open();
                    //var trans = db.BeginTransaction();
                    //try
                    //{
                    //    long rows = db.Insert(Records, trans);
                    //    trans.Commit();

                    //    return rows;
                    //}
                    //catch (Exception ex)
                    //{
                    //    trans.Rollback();
                    //    return -1;
                    //}

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

                using (IDbConnection db = new SqlConnection(DBConnectionString))
                {
                    db.Execute(sqlCmd, null, null, 0); // Excute with no timeout
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

                using (IDbConnection db = new SqlConnection(DBConnectionString))
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
        /// <summary>Replicate the MSACCESS DLookup feature.<br />
        /// This will perform a simple lookup of a value from a field based on specific <br />
        /// selection criteria <br />WHERE statement is not required in the Criteria specified</summary>
        /// <param name="FieldName">Field Name for the return value</param>
        /// <param name="tableName">Table or View name to use</param>
        /// <param name="Criteria">the Where criteria - WHERE statement not required</param>
        public object DLookup(string FieldName, string tableName, string Criteria)
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

                string sWhere = $"WHERE {Criteria.ToLower().Replace("where ", "")} ";
                string sSQL = $"SELECT {FieldName} FROM {tableName} {sWhere}";
                using (IDbConnection db = new SqlConnection(DBConnectionString))
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
        /// <summary>Replicate the MSACCESS DCount feature.<br />
        /// This will perform a simple count of a number of rows in the table/view with the provided criteria<br />
        /// selection criteria <br />WHERE statement is not required in the Criteria specified</summary>        
        /// <param name="tableName">Table or View name to use</param>
        /// <param name="Criteria">the Where criteria - WHERE statement not required</param>
        public int DCount(string tableName, string Criteria)
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

                string sWhere = $"WHERE {Criteria.ToLower().Replace("where ", "")} ";
                string sSQL = $"SELECT count(*) FROM {tableName} {sWhere}";
                using (IDbConnection db = new SqlConnection(DBConnectionString))
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

                using (IDbConnection db = new SqlConnection(DBConnectionString))
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
        /// <summary>Update an Entity record</summary>
        /// <typeparam name="TENTITY">Entity Object type</typeparam>
        /// <param name="Record">The Record to Update</param>
        /// <param name="originalEntity">A Copy of the original Record before changes made</param>
        /// <returns>True/false for success</returns>
        public bool UpdateRecord<TENTITY>(TENTITY Record, TENTITY originalEntity) where TENTITY : class
        {
            try
            {
                if (string.IsNullOrEmpty(DBConnectionString))
                {
                    _lastError = "Connection String not set";
                    throw new ArgumentException(_lastError);
                }

                // If Changes Only true and an original record is passed then perform a compare to get a list of differences then
                // build an update query
                if (originalEntity != null)
                {
                    // Get a list of all the changes (compare original to current)
                    // This will also return the field type and an entry for the ID (false passed into the Compare)
                    List<ObjectExtensions.Variance> diffs = ((TENTITY)originalEntity).DetailedCompare(Record, false);
                    if (diffs != null && diffs.Count > 0)
                    {
                        string TableName = GetTableName<TENTITY>();
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
                        using (IDbConnection db = new SqlConnection(DBConnectionString))
                        {
                            int rows = db.Execute(update, Record);
                            return rows != 0;
                        }
                    }
                    return false;
                }
                else
                {
                    using (IDbConnection db = new SqlConnection(DBConnectionString))
                    {
                        return db.Update(Record);
                    }
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

                using (IDbConnection db = new SqlConnection(DBConnectionString))
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

        /// <summary> Return the TableName of the POCO </summary>
        /// <typeparam name="TENTITY">the PCO Entity</typeparam>
        /// <returns>string</returns>
        public string GetTableName<TENTITY>()
        {
            var attr = typeof(TENTITY).GetCustomAttribute<Dapper.Contrib.Extensions.TableAttribute>(false);
            return attr != null ? attr.Name : "";
        }
        public string LastError()
        {
            return _lastError;
        }


    }
}
