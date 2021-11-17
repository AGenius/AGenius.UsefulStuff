using System;
using System.Collections.Specialized;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace AGenius.UsefulStuff.Helpers
{
    public static class AssemblyHelper
    {
        public static string CallingAssemblyName;
        public static Assembly EntryAssembly;
        private static NameValueCollection _EntryAssemblyAttribCollection;
        public static string EntryAssemblyName;
        public static string ExecutingAssemblyName;

        public static NameValueCollection EntryAssemblyAttribCollection
        {
            get
            {
                if (_EntryAssemblyAttribCollection == null)
                {
                    _EntryAssemblyAttribCollection = AssemblyAttributes(EntryAssembly);
                }
                return _EntryAssemblyAttribCollection;
            }
        }
        /// <summary>returns string name / string value pair of all attribs for specified assembly</summary>
        public static NameValueCollection AssemblyAttributes(Assembly a)
        {
            string TypeName;
            string Name;
            string Value;
            NameValueCollection nvc = new NameValueCollection();
            Regex r = new Regex(@"(\.Assembly|\.)(?<Name>[^.]*)Attribute$", RegexOptions.IgnoreCase);
            foreach (object attrib in a.GetCustomAttributes(false))
            {
                TypeName = attrib.GetType().ToString();
                Name = r.Match(TypeName).Groups["Name"].ToString();
                Value = string.Empty;
                switch (TypeName)
                {
                    case "System.CLSCompliantAttribute":
                        Value = ((CLSCompliantAttribute)attrib).IsCompliant.ToString();
                        break;
                    case "System.Diagnostics.DebuggableAttribute":
                        Value = ((System.Diagnostics.DebuggableAttribute)attrib).IsJITTrackingEnabled.ToString();
                        break;
                    case "System.Reflection.AssemblyCompanyAttribute":
                        Value = ((AssemblyCompanyAttribute)attrib).Company.ToString();
                        break;
                    case "System.Reflection.AssemblyConfigurationAttribute":
                        Value = ((AssemblyConfigurationAttribute)attrib).Configuration.ToString();
                        break;
                    case "System.Reflection.AssemblyCopyrightAttribute":
                        Value = ((AssemblyCopyrightAttribute)attrib).Copyright.ToString();
                        break;
                    case "System.Reflection.AssemblyDefaultAliasAttribute":
                        Value = ((AssemblyDefaultAliasAttribute)attrib).DefaultAlias.ToString();
                        break;
                    case "System.Reflection.AssemblyDelaySignAttribute":
                        Value = ((AssemblyDelaySignAttribute)attrib).DelaySign.ToString();
                        break;
                    case "System.Reflection.AssemblyDescriptionAttribute":
                        Value = ((AssemblyDescriptionAttribute)attrib).Description.ToString();
                        break;
                    case "System.Reflection.AssemblyInformationalVersionAttribute":
                        Value = ((AssemblyInformationalVersionAttribute)attrib).InformationalVersion.ToString();
                        break;
                    case "System.Reflection.AssemblyKeyFileAttribute":
                        Value = ((AssemblyKeyFileAttribute)attrib).KeyFile.ToString();
                        break;
                    case "System.Reflection.AssemblyProductAttribute":
                        Value = ((AssemblyProductAttribute)attrib).Product.ToString();
                        break;
                    case "System.Reflection.AssemblyTrademarkAttribute":
                        Value = ((AssemblyTrademarkAttribute)attrib).Trademark.ToString();
                        break;
                    case "System.Reflection.AssemblyTitleAttribute":
                        Value = ((AssemblyTitleAttribute)attrib).Title.ToString();
                        break;
                    case "System.Resources.NeutralResourcesLanguageAttribute":
                        Value = ((System.Resources.NeutralResourcesLanguageAttribute)attrib).CultureName.ToString();
                        break;
                    case "System.Resources.SatelliteContractVersionAttribute":
                        Value = ((System.Resources.SatelliteContractVersionAttribute)attrib).Version.ToString();
                        break;
                    case "System.Runtime.InteropServices.ComCompatibleVersionAttribute":
                        {
                            System.Runtime.InteropServices.ComCompatibleVersionAttribute x;
                            x = ((System.Runtime.InteropServices.ComCompatibleVersionAttribute)attrib);
                            Value = x.MajorVersion + "." + x.MinorVersion + "." + x.RevisionNumber + "." + x.BuildNumber;
                            break;
                        }
                    case "System.Runtime.InteropServices.ComVisibleAttribute":
                        Value = ((System.Runtime.InteropServices.ComVisibleAttribute)attrib).Value.ToString();
                        break;
                    case "System.Runtime.InteropServices.GuidAttribute":
                        Value = ((System.Runtime.InteropServices.GuidAttribute)attrib).Value.ToString();
                        break;
                    case "System.Runtime.InteropServices.TypeLibVersionAttribute":
                        {
                            System.Runtime.InteropServices.TypeLibVersionAttribute x;
                            x = ((System.Runtime.InteropServices.TypeLibVersionAttribute)attrib);
                            Value = x.MajorVersion + "." + x.MinorVersion;
                            break;
                        }
                    case "System.Security.AllowPartiallyTrustedCallersAttribute":
                        Value = "(Present)";
                        break;
                    default:
                        // debug.writeline("** unknown assembly attribute '" + TypeName + "'")
                        Value = TypeName;
                        break;
                }
                if (nvc[Name] == null)
                {
                    nvc.Add(Name, Value);
                }
            }
            // add some extra values that are not in the AssemblyInfo, but nice to have
            // codebase
            try
            {
                nvc.Add("CodeBase", a.CodeBase.Replace("file:///", string.Empty));
            }
            catch (NotSupportedException)
            {
                nvc.Add("CodeBase", "(not supported)");
            }
            // build date
            DateTime dt = AssemblyBuildDate(a, true);
            if (dt == DateTime.MaxValue)
            {
                nvc.Add("BuildDate", "(unknown)");
            }
            else
            {
                nvc.Add("BuildDate", dt.ToString("yyyy-MM-dd hh:mm tt"));
            }
            // location
            try
            {
                nvc.Add("Location", a.Location);
            }
            catch (NotSupportedException)
            {
                nvc.Add("Location", "(not supported)");
            }
            // version
            try
            {
                if (a.GetName().Version.Major == 0 && a.GetName().Version.Minor == 0)
                {
                    nvc.Add("Version", "(unknown)");
                }
                else
                {
                    nvc.Add("Version", a.GetName().Version.ToString());
                }
            }
            catch (Exception)
            {
                nvc.Add("Version", "(unknown)");
            }
            nvc.Add("FullName", a.FullName);
            return nvc;
        }
        /// <summary> returns DateTime this Assembly was last built. Will attempt to calculate from build number, if possible. 
        /// If not, the actual LastWriteTime on the assembly file will be returned.</summary>
        /// <param name="a">Assembly to get build date for</param>
        /// <param name="ForceFileDate">Don't attempt to use the build number to calculate the date</param>
        /// <returns>DateTime this assembly was last built</returns>
        public static DateTime AssemblyBuildDate(Assembly a, bool ForceFileDate)
        {
            Version AssemblyVersion = a.GetName().Version;
            DateTime dt;
            if (ForceFileDate)
            {
                dt = AssemblyLastWriteTime(a);
            }
            else
            {
                dt = DateTime.Parse("01/01/2000").AddDays(AssemblyVersion.Build).AddSeconds(AssemblyVersion.Revision * 2);
                if (TimeZone.IsDaylightSavingTime(dt, TimeZone.CurrentTimeZone.GetDaylightChanges(dt.Year)))
                {
                    dt = dt.AddHours(1);
                }
                if (dt > DateTime.Now || AssemblyVersion.Build < 730 || AssemblyVersion.Revision == 0)
                {
                    dt = AssemblyLastWriteTime(a);
                }
            }
            return dt;
        }

        /// <summary> exception-safe retrieval of LastWriteTime for this assembly.</summary>
        /// <returns>File.GetLastWriteTime, or DateTime.MaxValue if exception was encountered.</returns>
        public static DateTime AssemblyLastWriteTime(Assembly a)
        {
            try
            {
                if (a.Location == null || a.Location == string.Empty)
                {
                    return DateTime.MaxValue;
                }

                try
                {
                    return File.GetLastWriteTime(a.Location);
                }
                catch (Exception)
                {
                    return DateTime.MaxValue;
                }
            }
            catch (Exception)
            {
                return DateTime.MaxValue;
            }
        }
        /// <summary>
        /// retrieves a cached value from the entry assembly attribute lookup collection
        /// </summary>
        public static string EntryAssemblyAttrib(string strName)
        {
            if (EntryAssemblyAttribCollection[strName] == null)
            {
                return "<Assembly: Assembly" + strName + "(\"\")>";
            }
            else
            {
                return EntryAssemblyAttribCollection[strName].ToString();
            }
        }

        /// <summary>returns the entry assembly for the current application domain</summary>
        /// <remarks>This is usually read-only, but in some weird cases (Smart Client apps) 
        /// you won't have an entry assembly, so you may want to set this manually. </remarks>
        public static Assembly AppEntryAssembly
        {
            get { return AssemblyHelper.EntryAssembly; }
            set { AssemblyHelper.EntryAssembly = value; }
        }

    }
}
