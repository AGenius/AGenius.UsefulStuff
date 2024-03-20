using System;
using System.Collections.Generic;
using System.Data;

namespace AGenius.UsefulStuff.Helpers
{
    /// <summary>Helper for retrieving the SQL field data types</summary>
    public static class SQLDataTypeHelper
    {
        private static readonly Dictionary<Type, SqlDbType> sqlTypeMap;
        private static readonly Dictionary<Type, DbType> dbTypeMap;
        /// Create and populate the dictionary in the static constructor
        static SQLDataTypeHelper()
        {
            sqlTypeMap = new Dictionary<Type, SqlDbType>
            {
                [typeof(string)] = SqlDbType.VarChar,
                [typeof(char[])] = SqlDbType.NVarChar,
                [typeof(byte)] = SqlDbType.TinyInt,
                [typeof(short)] = SqlDbType.SmallInt,
                [typeof(int)] = SqlDbType.Int,
                [typeof(short)] = SqlDbType.SmallInt,
                [typeof(long)] = SqlDbType.BigInt,
                [typeof(long)] = SqlDbType.BigInt,
                [typeof(byte[])] = SqlDbType.VarBinary,
                [typeof(bool)] = SqlDbType.Bit,
                [typeof(DateTime)] = SqlDbType.DateTime2,
                [typeof(DateTimeOffset)] = SqlDbType.DateTimeOffset,
                [typeof(decimal)] = SqlDbType.Money,
                [typeof(float)] = SqlDbType.Real,
                [typeof(double)] = SqlDbType.Float,
                [typeof(TimeSpan)] = SqlDbType.Time
            };

            dbTypeMap = new Dictionary<Type, DbType>
            {
                [typeof(byte)] = DbType.Byte,
                [typeof(sbyte)] = DbType.SByte,
                [typeof(short)] = DbType.Int16,
                [typeof(ushort)] = DbType.UInt16,
                [typeof(int)] = DbType.Int32,
                [typeof(uint)] = DbType.UInt32,
                [typeof(long)] = DbType.Int64,
                [typeof(ulong)] = DbType.UInt64,
                [typeof(float)] = DbType.Single,
                [typeof(double)] = DbType.Double,
                [typeof(decimal)] = DbType.Decimal,
                [typeof(bool)] = DbType.Boolean,
                [typeof(string)] = DbType.String,
                [typeof(char)] = DbType.StringFixedLength,
                [typeof(Guid)] = DbType.Guid,
                [typeof(DateTime)] = DbType.DateTime,
                [typeof(DateTimeOffset)] = DbType.DateTimeOffset,
                [typeof(byte[])] = DbType.Binary,
                [typeof(byte?)] = DbType.Byte,
                [typeof(sbyte?)] = DbType.SByte,
                [typeof(short?)] = DbType.Int16,
                [typeof(ushort?)] = DbType.UInt16,
                [typeof(int?)] = DbType.Int32,
                [typeof(uint?)] = DbType.UInt32,
                [typeof(long?)] = DbType.Int64,
                [typeof(ulong?)] = DbType.UInt64,
                [typeof(float?)] = DbType.Single,
                [typeof(double?)] = DbType.Double,
                [typeof(decimal?)] = DbType.Decimal,
                [typeof(bool?)] = DbType.Boolean,
                [typeof(char?)] = DbType.StringFixedLength,
                [typeof(Guid?)] = DbType.Guid,
                [typeof(DateTime?)] = DbType.DateTime,
                [typeof(DateTimeOffset?)] = DbType.DateTimeOffset,
                [typeof(System.Data.Linq.Binary)] = DbType.Binary,
                [typeof(Enum)] = DbType.Int32
            };
        }
        /// <summary>Non-generic argument-based method</summary>
        /// <param name="giveType"></param>
        /// <returns></returns>
        public static SqlDbType GetSqlDbType(Type giveType)
        {
            // Allow nullable types to be handled        
            giveType = Nullable.GetUnderlyingType(giveType) ?? giveType;
            if (sqlTypeMap.ContainsKey(giveType))
            {
                return sqlTypeMap[giveType];
            }
            throw new ArgumentException($"{giveType.FullName} is not a supported .NET class");
        }
        /// <summary>Get the SqlDbType from an type</summary>
        /// <typeparam name="T">Type</typeparam>
        /// <returns><see cref="SqlDbType"/></returns>
        public static SqlDbType GetSqlDbType<T>()
        {
            return GetSqlDbType(typeof(T));
        }
        /// <summary>Non-generic argument-based method</summary>
        /// <param name="giveType">The Type to convert</param>
        /// <returns><see cref="DbType"/></returns>
        public static DbType GetDbType(Type giveType)
        {
            // Allow nullable types to be handled        
            giveType = Nullable.GetUnderlyingType(giveType) ?? giveType;
            if (dbTypeMap.ContainsKey(giveType))
            {
                return dbTypeMap[giveType];
            }
            throw new ArgumentException($"{giveType.FullName} is not a supported .NET class");
        }
        /// <summary>Generic version</summary>
        /// <returns><see cref="string"/></returns>
        public static DbType GetDbType<T>()
        {
            return GetDbType(typeof(T));
        }
    }
}