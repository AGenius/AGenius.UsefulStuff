using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace AGenius.UsefulStuff.Helpers
{
    /// <summary>Provides a way to get the Mime type from a file extension</summary>
    public static class MimeTypesList
    {
        /// <summary>
        /// URL where the mimetypes can be downloaded from
        /// </summary>
        /// <remarks>Currently this is set to decode from the default URL - default : "https://cdn.jsdelivr.net/gh/jshttp/mime-db@master/db.json" </remarks>
        public static string MimeTypesUrl { get; set; } = "https://cdn.jsdelivr.net/gh/jshttp/mime-db@master/db.json";
        private const string mimesJsonFile = "mime.types.json";

        private static readonly Lazy<IDictionary<string, string>> _mappings = new Lazy<IDictionary<string, string>>(BuildList);

        private static IDictionary<string, string> BuildList()
        {
            // See if the external "mimetypes.json" exists otherwise download and build it
            var jsonFile = Path.Combine(Utils.ApplicationPath, mimesJsonFile);
            if (!File.Exists(jsonFile))
            {
                var client = new WebClient();
                var jsonString = client.DownloadString(MimeTypesUrl);
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                Dictionary<string, Dictionary<string, object>> mimeTypeData = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, object>>>(jsonString, options);
                Dictionary<string, string> extensionToMimeType = new Dictionary<string, string>();

                foreach (var mimeType in mimeTypeData)
                {
                    if (mimeType.Value.TryGetValue("extensions", out var extensionsObject) && extensionsObject is JsonElement extensionsElement && extensionsElement.ValueKind == JsonValueKind.Array)
                    {
                        foreach (JsonElement extension in extensionsElement.EnumerateArray())
                        {
                            extensionToMimeType[extension.GetString()] = mimeType.Key;
                        }
                    }
                }
                // Serialize dictionary to JSON
                string json = SerializeToJson(extensionToMimeType);

                // Save JSON to file
                SaveJsonToFileAsync(json, jsonFile);
            }
            var mappings = LoadMimeTypesFromFile(jsonFile);

            return mappings;

        }
        /// <summary>Return the Mime type for a supplied FileExtension</summary>
        /// <param name="FileExt">The file extension (can include .)</param>
        /// <param name="throwErrorIfNotFound">Cause a throw to happen if the File Extension is not found</param>
        /// <returns>A string containing the MimeType</returns>
        public static string GetMimeType(string FileExt, bool throwErrorIfNotFound = true)
        {
            if (FileExt == null)
            {
                throw new ArgumentNullException("No file extension provided");
            }

            if (FileExt.StartsWith(".") && FileExt.Length > 1)
            {
                FileExt = FileExt.TrimStart('.');
            }

            if (_mappings.Value.TryGetValue(FileExt, out string mimeType))
            {
                return mimeType;
            }

            if (throwErrorIfNotFound)
            {
                throw new ArgumentException("Requested File Extension is not registered: " + FileExt);
            }

            return string.Empty;
        }
        /// <summary>Return the file extension from a given mime type string</summary>
        /// <param name="mimeType">The Mime Type to find</param>
        /// <param name="throwErrorIfNotFound">Cause a throw to happen if the Mime type is not found</param>
        /// <returns>String containing the extension</returns>
        /// <remarks>This will not be perfect , if any extension has the same MimeType the returned extension may be wrong.</remarks>
        public static string GetExtension(string mimeType, bool throwErrorIfNotFound = true)
        {
            if (mimeType == null)
            {
                throw new ArgumentNullException(nameof(mimeType));
            }

            if (mimeType.StartsWith("."))
            {
                throw new ArgumentException("Requested mime type is not valid: " + mimeType);
            }

            if (_mappings.Value.TryGetValue(mimeType, out string extension))
            {
                return extension;
            }

            if (throwErrorIfNotFound)
            {
                throw new ArgumentException("Requested mime type is not registered: " + mimeType);
            }

            return string.Empty;
        }


        private static string SerializeToJson(Dictionary<string, string> dictionary)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(dictionary, Newtonsoft.Json.Formatting.Indented);
        }
        private static void SaveJsonToFileAsync(string json, string filePath)
        {
            // Ensure the directory exists before writing the file
            string directory = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.Write(json);
            }
        }
        private static Dictionary<string, string> LoadMimeTypesFromFile(string filePath)
        {
            string jsonString = Utils.ReadTextFile(filePath);
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var mimeTypeData = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonString, options);

            return mimeTypeData;

        }
    }
}
