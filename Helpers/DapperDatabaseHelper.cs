using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper.Contrib.Extensions;
using Dapper;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Runtime.Serialization;
using System.Data.Common;

namespace AGenius.UsefulStuff.Helpers
{
    /// <summary>
    /// This class is used to help encapsulate the use of Dapper objects to access SQL Server
    /// </summary>
    [Serializable]
    public class DataAccessHelperException : ApplicationException
    {
        public DataAccessHelperException(string Message, Exception innerException) : base(Message, innerException) { }
        public DataAccessHelperException(string Message) : base(Message) { }
        public DataAccessHelperException() { }

        #region Serializeable Code
        public DataAccessHelperException(SerializationInfo info, StreamingContext context) : base(info, context) { }
        #endregion Serializeable Code
    }


    public class DataAccessFunctions : IDisposable
    {
        /// <summary>Event Handler for the ErrorEventArgs event</summary>

        public string DBConnectionString { get; set; }
        public Boolean? LastSaveState
        {
            get;
            private set;
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
                using (IDbConnection db = new SqlConnection(DBConnectionString))
                {
                    return db.GetAll<TENTITY>().ToList();
                }
            }
            catch (DbException ex)
            {
                throw new DataAccessHelperException(ex.Message);
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
                using (IDbConnection db = new SqlConnection(DBConnectionString))
                {
                    return db.Get<TENTITY>(ID);
                }
            }
            catch (DbException ex)
            {
                throw new DataAccessHelperException(ex.Message);
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
                using (IDbConnection db = new SqlConnection(DBConnectionString))
                {
                    return db.Get<TENTITY>(ID);
                }
            }
            catch (DbException ex)
            {
                throw new DataAccessHelperException(ex.Message);
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
                using (IDbConnection db = new SqlConnection(DBConnectionString))
                {
                    return db.Query<TENTITY>(sSQL).SingleOrDefault();
                }
            }
            catch (DbException ex)
            {
                throw new DataAccessHelperException(ex.Message);
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
                using (IDbConnection db = new SqlConnection(DBConnectionString))
                {
                    return db.Query<TENTITY>(sSQL).SingleOrDefault();
                }
            }
            catch (DbException ex)
            {
                throw new DataAccessHelperException(ex.Message);
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

                string sSQL = $"SELECT * FROM {TableName} {sWhere}";
                // var Results = null;
                using (IDbConnection db = new SqlConnection(DBConnectionString))
                {
                    return db.Query<TENTITY>(sSQL).SingleOrDefault();
                }
            }
            catch (DbException ex)
            {
                throw new DataAccessHelperException(ex.Message);
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

                using (IDbConnection db = new SqlConnection(DBConnectionString))
                {
                    return db.Query<TENTITY>(SprocName, Params, commandType:
                    CommandType.StoredProcedure).ToList();
                }
            }
            catch (DbException ex)
            {
                throw new DataAccessHelperException(ex.Message);
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

                using (IDbConnection db = new SqlConnection(DBConnectionString))
                {
                    return db.Query<TENTITY>(SQLQuery).ToList();
                }

            }
            catch (DbException ex)
            {
                throw new DataAccessHelperException(ex.Message);
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
                using (IDbConnection db = new SqlConnection(DBConnectionString))
                {
                    return db.Query<TENTITY>(sSQL).ToList();
                }

            }
            catch (DbException ex)
            {
                throw new DataAccessHelperException(ex.Message);
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

                using (IDbConnection db = new SqlConnection(DBConnectionString))
                {
                    return db.Insert(Record);
                }

            }
            catch (DbException ex)
            {
                throw new DataAccessHelperException(ex.Message);
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

                using (IDbConnection db = new SqlConnection(DBConnectionString))
                {
                    db.Execute(sqlCmd);
                }

            }
            catch (DbException ex)
            {
                throw new DataAccessHelperException(ex.Message);
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

                using (IDbConnection db = new SqlConnection(DBConnectionString))
                {
                    bool result = db.Update(Record);
                    return result;
                }
            }
            catch (DbException ex)
            {
                throw new DataAccessHelperException(ex.Message);
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

                using (IDbConnection db = new SqlConnection(DBConnectionString))
                {
                    return db.Delete(Record);
                }
            }
            catch (DbException ex)
            {
                throw new DataAccessHelperException(ex.Message);
            }
        }

        /// <summary> Return the TableName of the POCO </summary>
        /// <typeparam name="TENTITY">the PCO Entity</typeparam>
        /// <returns>string</returns>
        private string GetTableName<TENTITY>()
        {
            var attr = typeof(TENTITY).GetCustomAttribute<Dapper.Contrib.Extensions.TableAttribute>(false);
            return attr != null ? attr.Name : "";
        }

    }
}
