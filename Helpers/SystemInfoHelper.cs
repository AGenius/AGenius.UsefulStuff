using Newtonsoft.Json.Linq;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Management;
using System.Text;

namespace AGenius.UsefulStuff.Helpers
{
    public class SystemInfoHelper
    {
        public static string CaptureSystemInfo(string filePath = "")
        {
            StringBuilder systemInfo = new StringBuilder();

            // Gather system information
            systemInfo.AppendLine("System Information");
            systemInfo.AppendLine("==================");
            systemInfo.AppendLine();

            // Operating System
            systemInfo.AppendLine("Operating System:");
            systemInfo.AppendLine(GetWmiProperty("Win32_OperatingSystem", "*"));
            //systemInfo.Append($" - Version:{GetWmiProperty("Win32_OperatingSystem", "Version")} ({GetWmiProperty("Win32_OperatingSystem", "OSArchitecture")})");
            //systemInfo.AppendLine(GetWmiProperty("Win32_OperatingSystem", "Manufacturer"));            
            systemInfo.AppendLine();

            // Processor
            systemInfo.AppendLine("Processor:");
            systemInfo.AppendLine(GetWmiProperty("Win32_Processor", "*"));
            //systemInfo.AppendLine($"{GetWmiProperty("Win32_Processor", "Name")} ({GetWmiProperty("Win32_Processor", "Manufacturer")})");            
            //systemInfo.Append($" - Clock Speed :{GetWmiProperty("Win32_Processor", "MaxClockSpeed")}");
            systemInfo.AppendLine();

            // Memory
            systemInfo.AppendLine("Memory:");
            systemInfo.AppendLine(GetWmiProperty("Win32_ComputerSystem", "*"));
            systemInfo.AppendLine();

            // Disk Drives
            systemInfo.AppendLine("Disk Drives:");
            systemInfo.AppendLine(GetWmiProperty("Win32_DiskDrive", "*"));
            //systemInfo.AppendLine(GetWmiProperty("Win32_DiskDrive", "Size"));
            systemInfo.AppendLine();

            // Network Adapters
            systemInfo.AppendLine("Network Adapters:");
            systemInfo.AppendLine(GetWmiProperty("Win32_NetworkAdapter", "*"));
            //systemInfo.AppendLine(GetWmiProperty("Win32_NetworkAdapter", "MACAddress"));
            systemInfo.AppendLine();

            // BIOS
            systemInfo.AppendLine("BIOS:");
            systemInfo.AppendLine(GetWmiProperty("Win32_BIOS", "*"));
            //systemInfo.AppendLine($"{GetWmiProperty("Win32_BIOS", "Manufacturer")} - {GetWmiProperty("Win32_BIOS", "Name")} - {GetWmiProperty("Win32_BIOS", "Version")}");            
            systemInfo.AppendLine();

            if (!string.IsNullOrEmpty(filePath))
            {
                // Write the information to a text file
                File.WriteAllText(filePath, systemInfo.ToString());
                return filePath;
            }
            return systemInfo.ToString();
        }
        private static bool IsDateTimeString(string value)
        {
            return !string.IsNullOrEmpty(value) && value.Length >= 19 && value[14] == '.' && value.EndsWith("+000");
        }
        private static string ConvertToReadableDateTime(string dateTimeString)
        {
            // Define the format of the input string
            string format = "yyyyMMddHHmmss";

            // Parse the string into a DateTime object
            if (DateTime.TryParseExact(dateTimeString.GetBefore("."), format, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateTime))
            {
                return dateTime.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
            }
            else
            {
                // Handle the parsing error (e.g., log the error, return a default value, etc.)
                return "Invalid DateTime Format";
            }


            // Format the DateTime object into a more readable string
            return dateTime.ToString("yyyy-MM-dd HH:mm:ss.fff zzz", CultureInfo.InvariantCulture);
        }

        private static string GetWmiProperty(string wmiClass, string wmiProperty)
        {
            string[] convertToGBFields = { "FreePhysicalMemory", "FreeSpaceInPagingFiles", "MaxProcessMemorySize", "SizeStoredInPagingFiles", "TotalVirtualMemorySize", "TotalVisibleMemorySize", "L2CacheSize", "L3CacheSize", "TotalPhysicalMemory", "Size" };


            StringBuilder result = new StringBuilder();
            ManagementObjectSearcher searcher = new ManagementObjectSearcher($"SELECT {wmiProperty} FROM {wmiClass}");
            foreach (ManagementObject obj in searcher.Get())
            {
                if (wmiProperty == "*")
                {
                    foreach (var item in obj.Properties)
                    {
                        var value = obj[item.Name]?.ToString();
                        if (IsDateTimeString(value))
                        {
                            value = ConvertToReadableDateTime(value);
                            result.AppendLine($"{item.Name}:{value}");
                        }
                        else
                        {
                            if (convertToGBFields != null && convertToGBFields.Length > 0)
                            {
                                if (convertToGBFields.Contains(item.Name))
                                {
                                    if (long.TryParse(value, out long bytes))
                                    {
                                        if (bytes >= (1024.0 * 1024.0 * 1024.0))
                                        {
                                            double gigabytes = bytes / (1024.0 * 1024.0 * 1024.0);
                                            result.AppendLine($"{item.Name}:{gigabytes:F2} GB");
                                        }
                                        else
                                        {
                                            double megabytes = bytes / (1024.0 * 1024.0);
                                            result.AppendLine($"{item.Name}:{megabytes:F2} MB");
                                        }
                                    }
                                    else
                                    {
                                        result.AppendLine($"{item.Name}:{value}");
                                    }
                                }
                                else
                                {
                                    result.AppendLine($"{item.Name}:{value}");
                                }
                            }
                            else
                            {
                                result.AppendLine($"{item.Name}:{value}");
                            }
                            if (wmiClass == "Win32_DiskDrive" && item.Name == "TracksPerCylinder")
                            {
                                result.AppendLine("------------------------------------------");
                            }
                        }
                    }
                }
                else
                {
                    var value = obj[wmiProperty]?.ToString();
                    if (convertToGBFields != null && convertToGBFields.Length > 0)
                    {
                        if (convertToGBFields.Contains(wmiProperty))
                        {
                            if (long.TryParse(value, out long bytes))
                            {
                                double gigabytes = bytes / (1024.0 * 1024.0 * 1024.0);
                                result.AppendLine($"{gigabytes:F2} GB");
                            }
                            else
                            {
                                result.AppendLine(value);
                            }
                        }
                        else
                        {
                            result.AppendLine(value);
                        }
                    }
                    else
                    {
                        result.AppendLine(value);
                    }
                }
            }
            return result.ToString();
        }
    }
}