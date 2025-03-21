﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Data.Entity.Design.PluralizationServices;
using System.Drawing;
using System.ComponentModel;

//Copied from Tazmainiandevil/Useful.Extension
namespace AGenius.UsefulStuff
{
    /// <summary>
    /// Extensions for string object
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>Chop up string into smaller chunks</summary>
        /// <param name="str">The String to process</param>
        /// <param name="charCount">Number of characters per chunk</param>
        /// <returns></returns>
        static public List<string> ChopString(this string str, int charCount)
        {
            var words = new List<string>();

            for (int i = 0; i < str.Length; i += charCount)
            {
                if (str.Length - i >= charCount)
                {
                    words.Add(str.Substring(i, charCount));
                }
                else
                {
                    words.Add(str.Substring(i, str.Length - i));
                }
            }
            return words;
        }
        /// <summary>Convert a Serialized Font string to a real font</summary>
        /// <param name="str">The serialized font string , e.g. Font, Microsoft Sans Serif, 10, Regular, Point, 1, False</param>
        /// <returns>Font object</returns>
        static public Font ToFont(this string str, string sep = ",")
        {
            var parts = str.Split(new string[] { sep }, StringSplitOptions.None);
            return new Font(
                parts[1],                                                       // FontFamily.Name
                float.Parse(parts[2]),                                          // Size
                (FontStyle)parts[3].ToEnum<FontStyle>(FontStyle.Regular),       // Style
                (GraphicsUnit)parts[4].ToEnum<GraphicsUnit>(GraphicsUnit.Point),
                byte.Parse(parts[5]),                                           // GdiCharSet
                bool.Parse(parts[6])                                            // GdiVerticalFont
            );
        }
        /// <summary>
        /// Use the current thread's culture info for conversion
        /// </summary>
        public static string ToTitleCase(this string str)
        {
            var cultureInfo = System.Threading.Thread.CurrentThread.CurrentCulture;
            return cultureInfo.TextInfo.ToTitleCase(str.ToLower());
        }

        /// <summary>
        /// Overload which uses the culture info with the specified name
        /// </summary>
        public static string ToTitleCase(this string str, string cultureInfoName)
        {
            var cultureInfo = new CultureInfo(cultureInfoName);
            return cultureInfo.TextInfo.ToTitleCase(str.ToLower());
        }

        /// <summary>
        /// Overload which uses the specified culture info
        /// </summary>
        public static string ToTitleCase(this string str, CultureInfo cultureInfo)
        {
            return cultureInfo.TextInfo.ToTitleCase(str.ToLower());
        }
        /// <summary>Return a clean string usable as a filename </summary>
        /// <param name="StringValue">The String to process</param>
        /// <returns>Clean file system safe string <see cref="string"/></returns>
        /// <remarks>This will remove any special characters from a string</remarks>
        public static string CleanFileName(this string StringValue)
        {
            return Path.GetInvalidFileNameChars().Aggregate(StringValue, (current, c) => current.Replace(c.ToString(), string.Empty));
        }
        /// <summary>Return a clean alphanumeric string </summary>
        /// <param name="stringValue">The String to process</param>
        /// <remarks>This will remove any special characters from a string</remarks>
        public static string CleanString(this string stringValue)
        {
            return Regex.Replace(stringValue, "[^a-zA-Z0-9]", string.Empty);
        }
        /// <summary>Decode the Base36 Encoded string into a number</summary>
        internal static Int64 DecodeBase36(this string input)
        {
            string CharList = "0123456789abcdefghijklmnopqrstuvwxyz";
            var reversed = input.ToLower().Reverse();
            long result = 0;
            int pos = 0;

            foreach (char c in reversed)
            {
                result += CharList.IndexOf(c) * (long)Math.Pow(36, pos);
                pos++;
            }

            return result;
        }

        /// <summary>
        /// Compresses a string using GZip
        /// </summary>
        /// <param name="StringValue">The string to compress</param>
        /// <returns>The compressed string</returns>
        public static string CompressString(this string StringValue)
        {
            if (string.IsNullOrEmpty(StringValue))
            {
                return "";
            }
            try
            {
                byte[] buffer = Encoding.UTF8.GetBytes(StringValue);
                var memoryStream = new MemoryStream();
                using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Compress, true))
                {
                    gZipStream.Write(buffer, 0, buffer.Length);
                }

                memoryStream.Position = 0;

                var compressedData = new byte[memoryStream.Length];
                memoryStream.Read(compressedData, 0, compressedData.Length);

                var gZipBuffer = new byte[compressedData.Length + 4];
                Buffer.BlockCopy(compressedData, 0, gZipBuffer, 4, compressedData.Length);
                Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, gZipBuffer, 0, 4);
                return Convert.ToBase64String(gZipBuffer);
            }
            catch (Exception)
            {
                return "";
            }
        }

        /// <summary>Decompresses a string using GZip</summary>
        /// <param name="CompressedStringValue">The compressed string</param>
        /// <returns>The uncompressed string</returns>
        public static string DecompressString(this string CompressedStringValue)
        {
            if (string.IsNullOrEmpty(CompressedStringValue))
            {
                return "";
            }
            try
            {
                byte[] gZipBuffer = Convert.FromBase64String(CompressedStringValue);
                using (var memoryStream = new MemoryStream())
                {
                    int dataLength = BitConverter.ToInt32(gZipBuffer, 0);
                    memoryStream.Write(gZipBuffer, 4, gZipBuffer.Length - 4);

                    var buffer = new byte[dataLength];

                    memoryStream.Position = 0;
                    using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
                    {
                        gZipStream.Read(buffer, 0, buffer.Length);
                    }

                    return Encoding.UTF8.GetString(buffer);
                }
            }
            catch (Exception)
            {
                return "";
            }

        }

        /// <summary>Encrypt a string into a web safe string.</summary>
        /// <param name="StringValue">The string being processed <see cref="string"/></param>
        /// <param name="passphrase">A passphrase to use to encrypt with. Can only be decrypted using the same passphrase <see cref="string"/></param>
        /// <returns>Encrypted String result <see cref="string"/></returns>
        public static string EncryptStringWebSafe(this string StringValue, string passphrase = "Theres no fate but what we make.")
        {
            byte[] results;
            var utf8 = new System.Text.UTF8Encoding();

            // Step 1. We hash the passphrase using MD5
            // We use the MD5 hash generator as the result is a 128 bit byte array
            // which is a valid length for the TripleDES encoder we use below
            var hashProvider = new MD5CryptoServiceProvider();
            byte[] tdesKey = hashProvider.ComputeHash(utf8.GetBytes(passphrase));

            // Step 2. Create a new TripleDESCryptoServiceProvider object
            var tdesAlgorithm = new TripleDESCryptoServiceProvider
            {
                // Step 3. Setup the encoder
                Key = tdesKey,
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };

            // Step 4. Convert the input string to a byte[]
            var dataToEncrypt = utf8.GetBytes(StringValue);

            // Step 5. Attempt to encrypt the string
            try
            {
                var encryptor = tdesAlgorithm.CreateEncryptor();
                results = encryptor.TransformFinalBlock(dataToEncrypt, 0, dataToEncrypt.Length);
            }
            finally
            {
                // Clear the TripleDes and Hashprovider services of any sensitive information
                tdesAlgorithm.Clear();
                hashProvider.Clear();
            }

            // Step 6. Return the encrypted string as a base64 encoded string
            return Convert.ToBase64String(results);
        }
        /// <summary>
        /// Decrypt a web safe encrypted string
        /// </summary>
        /// <param name="StringValue">The string being processed</param>
        /// <param name="passphrase">A passphrase to use to decrypt with. Can only be decrypted using the same passphrase as it was encrypted with</param>
        /// <returns>Decrypted String result <see cref="string"/></returns>
        public static string DecryptStringWebSafe(this string StringValue, string passphrase = "Theres no fate but what we make.")
        {
            try
            {
                if (string.IsNullOrEmpty(StringValue))
                {
                    return "";
                }
                byte[] results;
                var utf8 = new System.Text.UTF8Encoding();

                // Step 1. We hash the passphrase using MD5
                // We use the MD5 hash generator as the result is a 128 bit byte array
                // which is a valid length for the TripleDES encoder we use below
                var hashProvider = new MD5CryptoServiceProvider();
                var tdesKey = hashProvider.ComputeHash(utf8.GetBytes(passphrase));

                // Step 2. Create a new TripleDESCryptoServiceProvider object
                var tdesAlgorithm = new TripleDESCryptoServiceProvider
                {
                    // Step 3. Setup the decoder
                    Key = tdesKey,
                    Mode = CipherMode.ECB,
                    Padding = PaddingMode.PKCS7
                };

                // Convert.FromBase64String(message) does not work well with spaces in the string for some odd reason
                // Plus sign will be interpreted as a space when u call the FromBase64String method
                StringValue = StringValue.Replace(" ", "+");

                try
                {
                    // Step 4. Convert the input string to a byte[]
                    var dataToDecrypt = Convert.FromBase64String(StringValue);

                    // Step 5. Attempt to decrypt the string
                    var decryptor = tdesAlgorithm.CreateDecryptor();
                    results = decryptor.TransformFinalBlock(dataToDecrypt, 0, dataToDecrypt.Length);
                }
                finally
                {
                    // Clear the TripleDes and Hashprovider services of any sensitive information
                    tdesAlgorithm.Clear();
                    hashProvider.Clear();
                }

                // Step 6. Return the decrypted string in UTF8 format
                return utf8.GetString(results);
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
        /// <summary>
        /// Decode and convert a JSON Web Token string to a JSON object string
        /// </summary>
        /// <param name="JWTTokenString">The string to decode</param>
        /// <returns>JSON string</returns>
        public static string JWTtoJSON(this string JWTTokenString)
        {
            return Utils.JWTtoJSON(JWTTokenString);
        }

        /// <summary>Simple string encryption using a simple hex swap method</summary>
        /// <param name="StringValue">String to Encrypt</param>
        /// <remarks>This will convert each character in the string
        /// into hex then swap the hex around then turn it back to
        /// decimal and turn it into a char
        /// Passing a previously SimpleEncrpted string will decrypt it</remarks>
        public static string SimpleEncrypt(this string StringValue)
        {
            string NewString = string.Empty;
            string stmp = string.Empty;
            try
            {
                for (int i = 0; i <= StringValue.Length - 1; i++)
                {
                    // Turn Char into a hex string
                    int ichar = Convert.ToChar(StringValue.Substring(i, 1));
                    string sHex = ichar.ToString("X").PadLeft(2, '0');
                    // Swap Hex awound for example
                    // 6E becomes E6
                    string sNewHex = sHex.Substring(sHex.Length - 1) + sHex.Substring(0, 1);
                    // Convert the new hex into decimal
                    int iDec = int.Parse(sNewHex, System.Globalization.NumberStyles.HexNumber);

                    if (iDec > 0)
                    {
                        // now add the char value to the new string
                        stmp += Convert.ToChar(iDec);
                        if (stmp.Length > 50)
                        {
                            // This increase performance on large strings
                            NewString += stmp;
                            stmp = "";
                        }
                    }
                }
                NewString += stmp;
                return NewString;
            }

            catch (System.Exception)
            {
                return "";
            }
        }
        /// <summary>
        /// Convert an enumerated list to a CSV string
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string ToCsv<T>(this IEnumerable<T> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            return string.Join(",", source.Select(s => s.ToString()).ToArray());
        }

        /// <summary>Returns the FieldNames from the Fields Parameter passed in as FieldName=FieldValue</summary>
        /// <param name="SearchFieldString">FieldName=FieldValue</param>
        /// <returns>string result</returns>
        public static string[] ToFieldNames(this string[] SearchFieldString)
        {
            string[] names = new string[SearchFieldString.Length];
            for (int i = 0; i < SearchFieldString.Length; i++)
            {
                names[i] = SearchFieldString[i].Split('=')[0];
            }
            return names;
        }
        /// <summary>Returns the FieldNames as a csv string from the Fields Parameter passed in as FieldName=FieldValue</summary>
        /// <param name="SearchFieldString">FieldName=FieldValue</param>
        /// <returns>string result <see cref="string"/></returns>
        public static string ToFieldNamesCSV(this string[] SearchFieldString)
        {
            string names = String.Empty;
            for (int i = 0; i < SearchFieldString.Length; i++)
            {
                if (!string.IsNullOrEmpty(names)) names += ",";
                names += SearchFieldString[i].Split('=')[0];
            }
            return names;
        }
        /// <summary>Returns the Values from the Fields Parameter passed in as FieldName=FieldValue</summary>
        /// <param name="SearchFieldString">FieldName=FieldValue</param>
        /// <returns>string result <see cref="Array"/> </returns>
        public static string[] ToFieldValues(this string[] SearchFieldString)
        {
            string[] values = new string[SearchFieldString.Length];
            for (int i = 0; i < SearchFieldString.Length; i++)
            {
                string[] SplitUp = SearchFieldString[i].Split('=');
                for (int ii = 1; ii < SplitUp.Length; ii++)
                {
                    values[i] += SplitUp[ii];
                }
            }
            return values;
        }
        /// <summary>Returns the Values as a csv string from the Fields Parameter passed in as FieldName=FieldValue</summary>
        /// <param name="SearchFieldString">FieldName=FieldValue</param>
        /// <returns>string result <see cref="string"/></returns>
        public static string ToFieldValuesCSV(this string[] SearchFieldString)
        {
            string values = string.Empty;
            for (int i = 0; i < SearchFieldString.Length; i++)
            {
                string[] SplitUp = SearchFieldString[i].Split('=');
                for (int ii = 1; ii < SplitUp.Length; ii++)
                {
                    if (!string.IsNullOrEmpty(values)) values += ",";
                    values += SplitUp[ii];
                }
            }
            return values;
        }
        /// <summary>
        /// Extract a number of months from a string
        /// </summary>
        /// <param name="selection">The string to parse e.g.  1 Month,2 Months or 2 Years</param>
        /// <returns>An integer containing the number of months</returns>
        /// <exception cref="ArgumentException">No string supplied</exception>
        public static int ToMonthCount(this string selection)
        {
            if (string.IsNullOrEmpty(selection))
                throw new ArgumentException("Selection cannot be null or empty", nameof(selection));

            string[] parts = selection.Split(' ');
            if (parts.Length != 2)
                throw new ArgumentException("Invalid selection format", nameof(selection));

            int value = int.Parse(parts[0]);
            string unit = parts[1].ToLower();

            switch (unit)
            {
                case "month":
                case "months":
                    return value;
                case "year":
                case "years":
                    return value * 12;
                default:
                    throw new ArgumentException("Invalid time unit", nameof(selection));
            }
        }

        /// <summary>Convert unsecure string to readonly SecureString. </summary>
        /// <param name="StringValue">The unsecure string for conversion.</param>
        /// <param name="ReadOnly">Set result as readonly <see cref="bool"/></param>
        /// <returns>SecureString <see cref="SecureString"/></returns>
        public static SecureString ToSecureString(this string StringValue, bool ReadOnly = true)
        {
            if (string.IsNullOrEmpty(StringValue))
            {
                throw new ArgumentNullException("Missing string value");
            }
            SecureString securePassword = new SecureString();
            foreach (char c in StringValue)
            {
                securePassword.AppendChar(c);
            }
            if (ReadOnly) securePassword.MakeReadOnly();
            return securePassword;
        }
        /// <summary>
        /// Convert a SecureString to a standard string value
        /// </summary>
        /// <param name="SecureStringValue">The SecureString object</param>
        /// <returns>string result <see cref="string"/></returns>
        public static string ToUnSecureString(this SecureString SecureStringValue)
        {
            return new System.Net.NetworkCredential(string.Empty, SecureStringValue).Password;
        }
        /// <summary>
        /// Return the Index of a specific occurrence of a string
        /// </summary>
        /// <param name="StringValue">The string containing the content</param>
        /// <param name="SearchString">The String value to search for</param>
        /// <param name="occurrence">0 based occurrence</param>
        /// <returns>int holding the offset of the nth searchstring</returns>
        /// <exception cref="ArgumentException"></exception>
        public static int IndexOfOccurrence(this string StringValue, string SearchString, int occurrence = 0)
        {
            if (occurrence < 0)
                throw new ArgumentException("Can not find a negative index of substring in string. Must start with 0");

            int offset = StringValue.IndexOf(SearchString);
            for (int i = 0; i < occurrence; i++)
            {
                if (offset == -1) return -1;
                offset = StringValue.IndexOf(SearchString, offset + 1);
            }

            return offset;
        }
        /// <summary>Get the content of the string before the given character or string </summary>
        /// <param name="StringValue">The string containing the content</param>
        /// <param name="SearchString">The String value to search for</param>
        /// <param name="includeFindValue">Return the SearchString as part of the result</param>
        /// <returns>String result <see cref="string"/></returns>
        public static string GetBefore(this string StringValue, string SearchString, bool includeFindValue = false)
        {
            try
            {
                string returnValue = StringValue.Substring(0, StringValue.ToLower().IndexOf(SearchString.ToLower()));
                if (includeFindValue)
                {
                    return returnValue + SearchString;
                }
                else
                {
                    return returnValue;
                }
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
        /// <summary>Get the content of the string before the given occurence of a character or string</summary>
        /// <param name="StringValue">The string containing the content</param>
        /// <param name="SearchString">The String value to search for</param>
        /// <param name="occurrence">0 based occurrence</param>
        /// <returns>int holding the offset of the nth searchstring</returns>
        /// <exception cref="ArgumentException"></exception>
        public static string GetBefore(this string StringValue, string SearchString, int occurrence)
        {
            try
            {
                if (occurrence < 0)
                    throw new ArgumentException("Can not find a negative index of substring in string. Must start with 0");
                int offset = IndexOfOccurrence(StringValue, SearchString, occurrence);
                string returnValue = StringValue.Substring(0, offset);

                return returnValue;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
        /// <summary>Get the string contents after a given character or string </summary>
        /// <param name="StringValue">The string containing the content</param>
        /// <param name="SearchString">The String value to search for</param>
        /// <param name="includeFindValue">Return the SearchString as part of the result</param>
        /// <returns>String result <see cref="string"/></returns>
        public static string GetAfter(this string StringValue, string SearchString, bool includeFindValue = false)
        {
            try
            {
                string returnValue = StringValue.Substring(StringValue.ToLower().IndexOf(SearchString.ToLower()) + SearchString.Length);
                if (includeFindValue)
                {
                    return SearchString + returnValue;
                }
                else
                {
                    return returnValue;
                }
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
        /// <summary>Get the string contents after a given character or string </summary>
        /// <param name="StringValue">The string containing the content</param>
        /// <param name="SearchString">The String value to search for</param>
        /// <param name="occurrence">0 based occurrence</param>
        /// <returns>String result <see cref="string"/></returns>
        public static string GetAfter(this string StringValue, string SearchString, int occurrence)
        {
            try
            {
                if (occurrence < 0)
                    throw new ArgumentException("Can not find a negative index of substring in string. Must start with 0");
                int offset = IndexOfOccurrence(StringValue, SearchString, occurrence);
                string returnValue = StringValue.Substring(occurrence + SearchString.Length);
                return returnValue;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
        /// <summary>Get the strin contents between two string values</summary>
        /// <param name="StringValue">The string containing the content</param>
        /// <param name="StartValue">The starting string value to search</param>
        /// <param name="EndValue">The ending string value to search</param>
        /// <param name="inclusive">Include the Start and End values in the result</param>
        /// <returns>String result <see cref="string"/></returns>
        public static string GetBetween(this string StringValue, string StartValue, string EndValue, bool inclusive = false)
        {
            if (inclusive)
            {
                return $"{StartValue}{StringValue.GetAfter(StartValue).GetBefore(EndValue)}{EndValue}";
            }
            return StringValue.GetAfter(StartValue).GetBefore(EndValue);
        }
        /// <summary>Get a string between the search string</summary>
        /// <param name="StringValue">The String to process</param>
        /// <param name="SearchString">The sring search to use</param>
        /// <returns>String result <see cref="string"/></returns>
        public static string GetBetween(this string StringValue, string SearchString)
        {
            return StringValue.GetAfter(SearchString).GetBefore(SearchString);
        }
        /// <summary>Return the string portion on the right of the string</summary>
        /// <param name="StringValue">The string passed in</param>
        /// <param name="MaxLength">The number of characters from the right side of the string to pass back</param>
        /// <returns>String result <see cref="string"/></returns>
        public static string Right(this string StringValue, int MaxLength)
        {
            //Check if the value is valid
            if (string.IsNullOrEmpty(StringValue))
            {
                //Set valid empty string as string could be null
                StringValue = string.Empty;
            }
            else if (StringValue.Length > MaxLength)
            {
                //Make the string no longer than the max length
                StringValue = StringValue.Substring(StringValue.Length - MaxLength, MaxLength);
            }

            //Return the string
            return StringValue;
        }

        #region ContainsValue / HasValue

        #region Field Replacement

        /// <summary>Replace field placeholders in a string with object properties of the same name</summary>
        /// <typeparam name="T">The object type</typeparam>
        /// <param name="StringValue">The content to process</param>
        /// <param name="TheEntity">The object holding the properties</param>
        /// <param name="ReplaceIfNullWith">If the field is null then replace the field place holder with this value</param>
        /// <param name="StartField">The expected string for the start of a field place holder</param>
        /// <param name="EndField">The expected string for the end of a field place holder</param>
        /// <returns>The new string content with the new content</returns>
        /// <remarks>Will detect DATETIME and replace with the current date and time as ToLongDateString <see cref="DateTime.Now"/> </remarks>
        public static string ReplaceObjectFields<T>(this string StringValue, T TheEntity, string ReplaceIfNullWith = null, string StartField = "[[", string EndField = "]]")
        {
            return Utils.ReplaceObjectFields(StringValue, TheEntity, ReplaceIfNullWith, StartField, EndField);
        }
        /// <summary>Returns a list of tokens from a supplied string</summary>        
        /// <param name="StringValue">The content to process</param>
        /// <param name="StartField">The expected string for the start of a field place holder</param>
        /// <param name="EndField">The expected string for the end of a field place holder</param>
        /// <param name="includeFields">True to return the token with the matching Start and End values included</param>
        /// <returns>List of tokens in a list object></returns>
        public static List<string> GetTokensFromString(this string StringValue, string StartField = "[[", string EndField = "]]", bool includeFields = false)
        {
            return Utils.GetTokensFromString(StringValue, StartField, EndField, includeFields);
        }
        /// <summary>Replace the tokens in a string that are considered empty</summary>
        /// <param name="StringValue">The content to process</param>      
        /// <param name="LenthLimit">How many characters to check, anything at this or below will be removed</param>
        /// <param name="StartField">The expected string for the start of a field place holder</param>
        /// <param name="EndField">The expected string for the end of a field place holder</param>
        /// <returns></returns>
        /// <remarks>The process will check each found token by trimming and if the result is less than the supplied LenthLimit characters then it is removed</remarks>
        public static string ReplaceEmptyTokensInString(this string StringValue, int LenthLimit = 1, string StartField = "[[", string EndField = "]]")
        {
            return Utils.ReplaceEmptyTokensInString(StringValue, LenthLimit, StartField, EndField);
        }
        /// <summary>Replaces all occurences of a specific collection of Tokens</summary>
        /// <param name="StringValue">The content to process</param>
        /// <param name="StartField">The expected string for the start of a field place holder</param>
        /// <param name="EndField">The expected string for the end of a field place holder</param>
        /// <param name="ReplaceWith">String to replace with (default to empty string value)</param>
        /// <param name="includeFields">True to return the token with the matching Start and End values included</param>
        /// <returns></returns>
        public static string ReplaceTokens(this string StringValue, string StartField = "[[", string EndField = "]]", string ReplaceWith = "", bool includeFields = false)
        {
            string content = StringValue;

            try
            {
                var findList = content.GetTokensFromString(StartField, EndField, includeFields);

                foreach (string entry in findList)
                {
                    if (!string.IsNullOrEmpty(entry))
                    {
                        content = content.Replace(entry, ReplaceWith);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return content;
        }
        #endregion

        /// <summary>
        /// Find
        /// </summary>
        /// <param name="src">The string to find a src in</param>
        /// <param name="find">The string to find</param>
        /// <param name="caseCompare">(optional)The type of compare to use the default is to ignore case</param>
        /// <returns>A boolean denoting if the find src is in the string</returns>
        public static bool ContainsValue(this string src, string find, StringComparison caseCompare = StringComparison.OrdinalIgnoreCase)
        {
            if (string.IsNullOrEmpty(src) || string.IsNullOrEmpty(find))
            {
                return false;
            }

            var result = src.IndexOf(find, caseCompare) >= 0;
            return result;
        }

        /// <summary>
        /// Find
        /// </summary>
        /// <param name="src">The string to find a src in</param>
        /// <param name="find">The string to find</param>
        /// <param name="caseCompare">(optional)The type of compare to use the default is to ignore case</param>
        /// <returns>A boolean denoting if the find src is in the string</returns>
        public static bool HasValue(this string src, string find, StringComparison caseCompare = StringComparison.OrdinalIgnoreCase)
        {
            if (string.IsNullOrEmpty(src) || string.IsNullOrEmpty(find))
            {
                return false;
            }

            var result = src.IndexOf(find, caseCompare) >= 0;
            return result;
        }

        #endregion ContainsValue / HasValue

        #region Equals Ignore Case

        /// <summary>
        /// Wraps equals to remove the need to specify the case for ignoring case
        /// </summary>
        /// <param name="src">The string to perform equality on</param>
        /// <param name="compare">The string to compare</param>
        /// <returns>A boolean denoting the two strings are equal ignoring case</returns>
        public static bool EqualsIgnoreCase(this string src, string compare)
        {
            if (src == null && compare == null)
            {
                return true;
            }

            return (src ?? string.Empty).Equals(compare, StringComparison.OrdinalIgnoreCase);
        }

        #endregion Equals Ignore Case

        #region Safe StartsWith

        /// <summary>
        /// Perform a Starts With even if the src is null
        /// </summary>
        /// <param name="src">The string to perform startwith on</param>
        /// <param name="find">The value to look for</param>
        /// <returns>A boolean denoting if value starts with given value</returns>
        public static bool SafeStartsWith(this string src, string find)
        {
            return (src ?? string.Empty).StartsWith(find);
        }

        /// <summary>
        /// Perform a Starts With even if the src is null
        /// </summary>
        /// <param name="src">The string to perform startwith on</param>
        /// <param name="find">The value to look for</param>
        /// <param name="comparison">The string comparison type (defaults to Ordinal)</param>
        /// <returns>A boolean denoting if value starts with given value</returns>
        public static bool SafeStartsWith(this string src, string find, StringComparison comparison)
        {
            return (src ?? string.Empty).StartsWith(find, comparison);
        }

#if NETSTANDARD2_0

        /// <summary>
        /// Perform a Starts With even if the src is null
        /// </summary>
        /// <param name="src">The string to perform startwith on</param>
        /// <param name="find">The value to look for</param>
        /// <param name="ignoreCase"></param>
        /// <param name="culture"></param>
        /// <returns>A boolean denoting if value starts with given value</returns>
        public static bool SafeStartsWith(this string src, string find, bool ignoreCase, CultureInfo culture)
        {
            return (src ?? string.Empty).StartsWith(find, ignoreCase, culture);
        }
#endif

        #endregion Safe StartsWith

        #region Safe EndsWith

        /// <summary>
        /// Perform a Ends With even if the src is null
        /// </summary>
        /// <param name="src">The string to perform startwith on</param>
        /// <param name="find">The value to look for</param>
        /// <returns>A boolean denoting if value ends with given value</returns>
        public static bool SafeEndsWith(this string src, string find)
        {
            return (src ?? string.Empty).EndsWith(find);
        }

        /// <summary>
        /// Perform a Ends With even if the src is null
        /// </summary>
        /// <param name="src">The string to perform startwith on</param>
        /// <param name="find">The value to look for</param>
        /// <param name="comparison">The string comparison type (defaults to Ordinal)</param>
        /// <returns>A boolean denoting if value ends with given value</returns>
        public static bool SafeEndsWith(this string src, string find, StringComparison comparison)
        {
            return (src ?? string.Empty).EndsWith(find, comparison);
        }

#if NETSTANDARD2_0
        /// <summary>
        /// Perform a Ends With even if the src is null
        /// </summary>
        /// <param name="src">The string to perform startwith on</param>
        /// <param name="find">The value to look for</param>
        /// <param name="ignoreCase"></param>
        /// <param name="culture"></param>
        /// <returns>A boolean denoting if value starts with given value</returns>
        public static bool SafeEndsWith(this string src, string find, bool ignoreCase, CultureInfo culture)
        {
            return (src ?? string.Empty).EndsWith(find, ignoreCase, culture);
        }
#endif

        #endregion Safe EndsWith

        #region Substring

        /// <summary>
        /// SubstringOrEmpty allows substring to be used without throwing an exception if out of range
        /// </summary>
        /// <param name="src">The string to substring</param>
        /// <param name="start">The substring start point</param>
        /// <param name="length">(optional)The length to substring, the default is the remaining length from the start point</param>
        /// <returns>The requested substring or an empty string if the out of range or empty</returns>
        public static string SubstringOrEmpty(this string src, int start, int length = 0)
        {
            if (string.IsNullOrEmpty(src))
            {
                return string.Empty;
            }

            if (length.Equals(0))
            {
                length = src.Length;
            }

            // Get the requested substring
            return new string(src.Skip(start).Take(length).ToArray());
        }

        /// <summary>
        /// Return the substring after a given character
        /// </summary>
        /// <param name="src">The string containing the content</param>
        /// <param name="find">The character to find</param>
        /// <param name="comparison">The string comparison type (defaults to OrdinalIgnoreCase)</param>
        /// <returns>The substring value</returns>
        public static string SubstringAfterValue(this string src, char find, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        {
            return src.SubstringAfterValue(find.ToString(), comparison);
        }

        /// <summary>
        /// Return the substring after a given character or string
        /// </summary>
        /// <param name="src">The string containing the content</param>
        /// <param name="find">The string to find</param>
        /// <param name="comparison">The string comparison type (defaults to OrdinalIgnoreCase)</param>
        /// <returns>The substring value</returns>
        public static string SubstringAfterValue(this string src, string find, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        {
            if (string.IsNullOrWhiteSpace(src))
            {
                return string.Empty;
            }

            var index = src.IndexOf(find ?? string.Empty, comparison);

            return index < 0 ? src : new string(src.Skip(index + (find ?? string.Empty).Length).Take(src.Length).ToArray());
        }

        /// <summary>
        /// Return the substring after the last occurance of a given character
        /// </summary>
        /// <param name="src">The string containing the content</param>
        /// <param name="find">The string to find</param>
        /// <param name="comparison">The string comparison type (defaults to OrdinalIgnoreCase)</param>
        /// <returns>The substring value</returns>
        public static string SubstringAfterLastValue(this string src, char find, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        {
            return src.SubstringAfterLastValue(find.ToString(), comparison);
        }

        /// <summary>
        /// Return the substring after the last occurrence of a given character or string
        /// </summary>
        /// <param name="src">The string containing the content</param>
        /// <param name="find">The string to find</param>
        /// <param name="comparison">The string comparison type (defaults to OrdinalIgnoreCase)</param>
        /// <returns>The substring value</returns>
        public static string SubstringAfterLastValue(this string src, string find, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        {
            if (string.IsNullOrWhiteSpace(src))
            {
                return string.Empty;
            }

            if (string.IsNullOrEmpty(find))
            {
                return src;
            }

            var index = src.LastIndexOf(find, comparison);

            return index < 0 ? src : new string(src.Skip(index + find.Length).Take(src.Length).ToArray());
        }

        /// <summary>
        /// Return the substring before a given character
        /// </summary>
        /// <param name="src">The string containing the content</param>
        /// <param name="find">The character to find</param>
        /// <param name="comparison">The string comparison type (defaults to OrdinalIgnoreCase)</param>
        /// <returns>The substring value</returns>
        public static string SubstringBeforeValue(this string src, char find, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        {
            return src.SubstringBeforeValue(find.ToString(), comparison);
        }

        /// <summary>
        /// Return the substring before a given character or string
        /// </summary>
        /// <param name="src">The string containing the content</param>
        /// <param name="find">The string to find</param>
        /// <param name="comparison">The string comparison type (defaults to OrdinalIgnoreCase)</param>
        /// <returns>The substring value</returns>
        public static string SubstringBeforeValue(this string src, string find, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        {
            if (string.IsNullOrEmpty(src))
            {
                return string.Empty;
            }

            var index = src.IndexOf(find ?? string.Empty, comparison);

            if (string.IsNullOrEmpty(find) || index < 0)
            {
                return src;
            }

            return new string(src.Take(index).ToArray());
        }

        /// <summary>
        /// Return the substring before the last occurrence of a given character
        /// </summary>
        /// <param name="src">The string containing the content</param>
        /// <param name="find">The character to find</param>
        /// <param name="comparison">The string comparison type (defaults to OrdinalIgnoreCase)</param>
        /// <returns>The substring value</returns>
        public static string SubstringBeforeLastValue(this string src, char find, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        {
            return src.SubstringBeforeLastValue(find.ToString(), comparison);
        }

        /// <summary>
        /// Return the substring before the last occurrence of a given character or string
        /// </summary>
        /// <param name="src">The string containing the content</param>
        /// <param name="find">The string to find</param>
        /// <param name="comparison">The string comparison type (defaults to OrdinalIgnoreCase)</param>
        /// <returns>The substring value</returns>
        public static string SubstringBeforeLastValue(this string src, string find, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        {
            if (string.IsNullOrEmpty(src))
            {
                return string.Empty;
            }

            var index = src.LastIndexOf(find ?? string.Empty, comparison);

            if (string.IsNullOrEmpty(find) || index < 0)
            {
                return src;
            }

            return new string(src.Take(index).ToArray());
        }

        #endregion Substring

        #region Safe Trim

        /// <summary>
        /// Trim a string and return even if null or empty
        /// </summary>
        /// <param name="src">The string to perform a trim on</param>
        /// <returns>The trimmed string or null or empty</returns>
        public static string SafeTrim(this string src)
        {
            return string.IsNullOrEmpty(src) ? src : src.Trim();
        }

        #endregion Safe Trim

        #region Is Base64

        /// <summary>
        /// Check if a string is base 64 encoded
        /// </summary>
        /// <param name="src">The string to test</param>
        /// <returns>A boolean denoting if a string is base 64 encoded</returns>
        public static bool IsBase64(this string src)
        {
            if (string.IsNullOrWhiteSpace(src))
            {
                return false;
            }

            const string pattern = "^([A-Za-z0-9+/]{4})*([A-Za-z0-9+/]{4}|[A-Za-z0-9+/]{3}=|[A-Za-z0-9+/]{2}==)$";

            return Regex.IsMatch(src, pattern);
        }

        #endregion Is Base64

        #region IsAllNumbers

        private const string IsNumericPattern = @"^\d+$";

        /// <summary>
        /// Does the whole string contain only numeric characters
        /// </summary>
        /// <param name="src">The input string</param>
        /// <returns>A boolean denoting if all the characters are numeric</returns>
        public static bool IsAllNumber(this string src)
        {
            if (string.IsNullOrWhiteSpace(src))
            {
                return false;
            }

            return Regex.IsMatch(src, IsNumericPattern, RegexOptions.Compiled);
        }

        #endregion IsAllNumbers

        #region IsAllAlpha

        private const string IsAlphaPattern = @"^[a-zA-Z]+$";

        /// <summary>
        /// Does the whole string contain only alpha characters
        /// </summary>
        /// <param name="src">The input string</param>
        /// <returns>A boolean denoting if all the characters are alpha</returns>
        public static bool IsAllAlpha(this string src)
        {
            if (string.IsNullOrWhiteSpace(src))
            {
                return false;
            }

            return Regex.IsMatch(src, IsAlphaPattern, RegexOptions.Compiled);
        }

        #endregion IsAllAlpha

        #region IsAllAlphaOrNumbers

        private const string IsAlphaNumericPattern = @"^[a-zA-Z0-9]+$";

        /// <summary>
        /// Does the whole string contain only alpha and numeric characters, no special characters
        /// </summary>
        /// <param name="src">The input string</param>
        /// <returns>A boolean denoting if all the characters are alpha and/or numeric</returns>
        public static bool IsAllAlphaOrNumbers(this string src)
        {
            if (string.IsNullOrWhiteSpace(src))
            {
                return false;
            }

            return Regex.IsMatch(src, IsAlphaNumericPattern, RegexOptions.Compiled);
        }

        #endregion IsAllAlphaOrNumbers

        #region Plural/Singular convertion
        private static readonly PluralizationService plservice = PluralizationService.CreateService(CultureInfo.GetCultureInfo("en-US"));
        /// <summary>
        /// Convert a Plural name to singular string
        /// </summary>
        /// <param name="word">The word to convert</param>
        /// <returns>String containing the converted word</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static string ToSingular(this string word)
        {
            if (word == null)
                throw new ArgumentNullException("word");

            bool isUpperWord = (string.Compare(word, word.ToUpper(), false) == 0);
            if (isUpperWord)
            {
                string lowerWord = word.ToLower();
                return (plservice.IsSingular(lowerWord) ? lowerWord : plservice.Singularize(lowerWord)).ToUpper();
            }

            return (plservice.IsSingular(word) ? word : plservice.Singularize(word));
        }
        /// <summary>
        /// Convert a singular word to a plural version
        /// </summary>
        /// <param name="word">The word to convert</param>
        /// <returns>String containing the converted word</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static string ToPlural(this string word)
        {
            if (word == null)
                throw new ArgumentNullException("word");

            bool isUpperWord = (string.Compare(word, word.ToUpper(), false) == 0);
            if (isUpperWord)
            {
                string lowerWord = word.ToLower();
                return (plservice.IsPlural(lowerWord) ? lowerWord : plservice.Pluralize(lowerWord)).ToUpper();
            }

            return (plservice.IsPlural(word) ? word : plservice.Pluralize(word));
        }
        #endregion
        /// <summary>Convert CSV string to List of strings</summary>
        /// <param name="src">The input string</param>
        /// <returns> <see cref="List{T}"/></returns>
        public static List<string> CSVtoList(this string src)
        {
            List<string> results = new List<string>();
            var split = src.Split(',');
            foreach (var s in split)
            {
                results.Add(s);
            }
            return results;
        }
        /// <summary>Returns a List of strings where the source does not contain from compare</summary>
        /// <param name="src">The input string</param>
        /// <param name="compare">The comparison string</param>
        /// <returns> <see cref="List{T}"/></returns>
        public static List<string> StringsNotIn(this string src, string compare)
        {
            var s = CSVtoList(src);
            var t = CSVtoList(compare);
            var result = s.Where(f1 => t.All(f2 => f2 != f1));
            return result.ToList();
        }
    }
}