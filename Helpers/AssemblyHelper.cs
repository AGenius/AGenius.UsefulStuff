using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
using System.Security;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace AGenius.UsefulStuff.Helpers
{
    /// <summary>
    /// This helper class provides methods to retrieve information about assemblies.
    /// </summary>
    public static class AssemblyHelper
    {
        /// <summary>
        /// Static fields to cache assembly information
        /// </summary>
        public static string CallingAssemblyName => Assembly.GetCallingAssembly().GetName().Name;
        //public static Assembly EntryAssembly => Assembly.GetEntryAssembly();
        private static Assembly _entryAssembly; // Backing field for EntryAssembly
        /// <summary>
        /// Retrieves the entry assembly for the current application domain.
        /// </summary>
        public static Assembly EntryAssembly
        {
            get { return _entryAssembly ?? (_entryAssembly = Assembly.GetEntryAssembly()); }
            set { _entryAssembly = value; }
        }
        private static NameValueCollection _entryAssemblyAttribCollection;

        /// <summary>
        /// Properties to get commonly used assembly names
        /// </summary>
        public static string EntryAssemblyName => EntryAssembly?.GetName().Name ?? "(not available)";
        /// <summary>
        /// The Assembly Name
        /// </summary>
        public static string ExecutingAssemblyName => Assembly.GetExecutingAssembly().GetName().Name;

        /// <summary>
        /// Property to get attributes of the entry assembly
        /// </summary>
        public static NameValueCollection EntryAssemblyAttribCollection
        {
            get
            {
                if (_entryAssemblyAttribCollection == null)
                {
                    _entryAssemblyAttribCollection = AssemblyAttributes(EntryAssembly);
                }
                return _entryAssemblyAttribCollection;
            }
        }

        /// <summary>
        /// Retrieves all assembly attributes and their values for a specified assembly.
        /// </summary>
        public static NameValueCollection AssemblyAttributes(Assembly assembly)
        {
            var attributeCollection = new NameValueCollection();

            if (assembly == null)
            {
                return attributeCollection;
            }
            Regex r = new Regex(@"(\.Assembly|\.)(?<Name>[^.]*)Attribute$", RegexOptions.IgnoreCase);
            foreach (var attrib in assembly.GetCustomAttributes(false))
            {
                string name = attrib.GetType().Name;
                string value = GetAttributeValue(attrib);
                string TypeName = attrib.GetType().ToString();
                string rName = r.Match(TypeName).Groups["Name"].ToString();

                if (!string.IsNullOrEmpty(rName) && !string.IsNullOrEmpty(value))
                {
                    attributeCollection.Add(rName, value);
                }
            }

            // Additional information not available in AssemblyInfo
            attributeCollection.Add("FullName", assembly.FullName);
            attributeCollection.Add("CodeBase", GetSafeCodeBase(assembly));
            attributeCollection.Add("BuildDate", GetAssemblyBuildDate(assembly).ToString("yyyy-MM-dd hh:mm tt"));
            attributeCollection.Add("Location", GetSafeLocation(assembly));
            attributeCollection.Add("Version", assembly.GetName().Version?.ToString() ?? "(unknown)");

            return attributeCollection;
        }

        /// <summary>
        /// Retrieves the build date of an assembly.
        /// </summary>
        public static DateTime GetAssemblyBuildDate(Assembly assembly)
        {
            Version version = assembly?.GetName().Version;
            if (version == null)
            {
                return DateTime.MaxValue;
            }

            DateTime buildDate;
            try
            {
                buildDate = new DateTime(2000, 1, 1).AddDays(version.Build).AddSeconds(version.Revision * 2);
                if (TimeZone.IsDaylightSavingTime(buildDate, TimeZone.CurrentTimeZone.GetDaylightChanges(buildDate.Year)))
                {
                    buildDate = buildDate.AddHours(1);
                }
            }
            catch (Exception)
            {
                buildDate = DateTime.MaxValue;
            }

            return buildDate;
        }

        /// <summary>
        /// Retrieves the last write time of the assembly file.
        /// </summary>
        public static DateTime GetAssemblyLastWriteTime(Assembly assembly)
        {
            try
            {
                string location = assembly?.Location;
                if (string.IsNullOrEmpty(location))
                {
                    return DateTime.MaxValue;
                }

                return File.GetLastWriteTime(location);
            }
            catch (Exception)
            {
                return DateTime.MaxValue;
            }
        }

        /// <summary>
        /// Retrieves a specific attribute value safely.
        /// </summary>
        private static string GetAttributeValue(object attribute)
        {
            switch (attribute)
            {
                case CLSCompliantAttribute attr:
                    return attr.IsCompliant.ToString();
                case DebuggableAttribute attr:
                    return attr.IsJITTrackingEnabled.ToString();
                case AssemblyCompanyAttribute attr:
                    return attr.Company;
                case AssemblyConfigurationAttribute attr:
                    return attr.Configuration;
                case AssemblyCopyrightAttribute attr:
                    return attr.Copyright;
                case AssemblyDefaultAliasAttribute attr:
                    return attr.DefaultAlias;
                case AssemblyDelaySignAttribute attr:
                    return attr.DelaySign.ToString();
                case AssemblyDescriptionAttribute attr:
                    return attr.Description;
                case AssemblyInformationalVersionAttribute attr:
                    return attr.InformationalVersion;
                case AssemblyKeyFileAttribute attr:
                    return attr.KeyFile;
                case AssemblyProductAttribute attr:
                    return attr.Product;
                case AssemblyTrademarkAttribute attr:
                    return attr.Trademark;
                case AssemblyTitleAttribute attr:
                    return attr.Title;
                case NeutralResourcesLanguageAttribute attr:
                    return attr.CultureName;
                case SatelliteContractVersionAttribute attr:
                    return attr.Version;
                case ComVisibleAttribute attr:
                    return attr.Value.ToString();
                case GuidAttribute attr:
                    return attr.Value;
                case TypeLibVersionAttribute attr:
                    return $"{attr.MajorVersion}.{attr.MinorVersion}";
                case AllowPartiallyTrustedCallersAttribute:
                    return "(Present)";
                default:
                    return attribute.GetType().Name;
            }
        }

        /// <summary>
        /// Safely retrieves the CodeBase of an assembly.
        /// </summary>
        private static string GetSafeCodeBase(Assembly assembly)
        {
            try
            {
                return assembly?.CodeBase?.Replace("file:///", string.Empty) ?? "(not supported)";
            }
            catch (NotSupportedException)
            {
                return "(not supported)";
            }
        }

        /// <summary>
        /// Safely retrieves the Location of an assembly.
        /// </summary>
        private static string GetSafeLocation(Assembly assembly)
        {
            try
            {
                return assembly?.Location ?? "(not supported)";
            }
            catch (NotSupportedException)
            {
                return "(not supported)";
            }
        }

        /// <summary>
        /// Retrieves a cached value from the entry assembly attribute lookup collection.
        /// </summary>
        public static string EntryAssemblyAttrib(string attributeName)
        {
            if (EntryAssemblyAttribCollection[attributeName] == null)
            {
                return $"<Assembly: Assembly{attributeName}(\"\")>";
            }
            else
            {
                return EntryAssemblyAttribCollection[attributeName];
            }
        }

        /// <summary>
        /// Retrieves the entry assembly for the current application domain.
        /// </summary>
        /// <remarks>
        /// This is usually read-only, but in some cases, you may want to set this manually.
        /// </remarks>
        public static Assembly AppEntryAssembly
        {
            get { return EntryAssembly; }
            set { EntryAssembly = value; }
        }
    }
}
