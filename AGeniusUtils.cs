using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

/// <summary>
/// Some Common usefull methods
/// </summary>
public static class AGeniusUtils
{
    #region Scramble Methods
    public static string ApplicationPath = Directory.GetParent(System.Reflection.Assembly.GetEntryAssembly().Location).FullName;


    /// <summary>
    /// Handle a list of enums
    /// </summary>
    /// <typeparam name="T">Entity Type</typeparam>
    /// <param name="fieldName">The FieldName to include in the filter string  e.g.  ContactStatus</param>
    /// <param name="objectList">List of Items (Enums)</param>
    /// <returns>Concatenated string list of results</returns>
    internal static string BuildFilterString<T>(string fieldName, List<T> objectList)
    {
        List<string> itemList = objectList.ConvertAll(f => f.ToString());
        string filter = string.Empty;

        if (itemList != null)
        {
            foreach (var itm in itemList)
            {
                if (!string.IsNullOrEmpty(filter)) filter += " || "; //  
                filter += $"{fieldName}=\"{itm}\"";
            }
            if (itemList.Count > 1)
            {
                filter = "(" + filter + ")";
            }
        }
        return filter;
    }
    /// <summary>
    /// Handle a Single Enum list - Builds a string of the enum names
    /// </summary>
    /// <typeparam name="T">Entity Type (Enum)</typeparam>
    /// <param name="fieldName">The FieldName to include in the filter string  e.g.  ContactStatus</param>
    /// <param name="enumItem">The Enum</param>
    /// <returns>Concatenated string list of results</returns>
    internal static string BuildFilterString<T>(string fieldName, T enumItem)
    {
        string filter = string.Empty;

        if (enumItem != null)
        {
            filter += $"{fieldName}=\"{enumItem}\"";
        }
        return filter;
    }

    /// <summary>Read the contents of a text file into a string </summary>
    /// <param name="filepath">File to read</param>
    /// <returns>files contents</returns>
    public static string ReadTextFile(string filepath)
    {
        try
        {
            string test = Path.GetPathRoot(filepath);

            if (String.IsNullOrEmpty(test) || (test.StartsWith(@"\") && !test.StartsWith(@"\\")))
            {

                // No Full path supplied so start from Application root
                if (test.StartsWith(@"\"))
                {
                    filepath = ApplicationPath + filepath;
                }
                else
                {
                    filepath = $"{ApplicationPath}\\{filepath}";
                }
            }

            if (File.Exists(filepath).Equals(true))
            {
                using (StreamReader reader = new StreamReader(filepath))
                {
                    string contents = reader.ReadToEnd();
                    return contents;
                }
            }
        }
        catch (Exception)
        {
        }

        return null;
    }
    /// <summary>Write the contents of a string to a file </summary>
    /// <param name="filepath">File to write to</param>        
    public static void WriteTextFile(string filepath, string contents)
    {
        try
        {
            string test = Path.GetPathRoot(filepath);

            if (String.IsNullOrEmpty(test) || (test.StartsWith(@"\") && test.Substring(1, 1) != @"\"))
            {

                // No Full path supplied so start from Application root
                if (test.StartsWith(@"\"))
                {
                    filepath = ApplicationPath + filepath;
                }
                else
                {
                    filepath = $"{ApplicationPath}\\{filepath}";
                }
            }
            using (StreamWriter sw = new StreamWriter(filepath))
            {
                sw.WriteLine(contents);
            }
        }
        catch (Exception)
        {
        }
    }
    /// <summary>
    /// Write to a text log file. Automatically maintain a 1mb file size and archive to a "Completed" folder if exceeded
    /// </summary>
    /// <param name="MessageText">The string message to write</param>
    /// <param name="LogFileName">The filename for the log file</param>
    /// <param name="LogPath">The File Path to the log file location (optional - will use assembly path if excluded or null)</param>
    /// <param name="SubFolder">Override the sub folder (logs) name</param>
    /// <param name="AddTimeStamp">Add a time stamp to the begining of the message</param>
    /// <param name="appendNewLine">Automatically add a new line after the message</param>
    public static void WriteLogFile(string MessageText, string LogFileName, string LogPath = null, string SubFolder = "Logs", bool AddTimeStamp = false, bool appendNewLine = true)
    {
        try
        {
            if (LogPath == null)
            {
                LogPath = ApplicationPath;
            }
            string sPath = Path.Combine(LogPath, SubFolder, $"{LogFileName}.log");

            if (System.IO.File.Exists(sPath).Equals(false))
            {
                Directory.CreateDirectory(Path.Combine(LogPath, SubFolder));

                File.AppendAllText(sPath, @"-----------------------------" + Environment.NewLine);
                File.AppendAllText(sPath, $"{LogFileName} Log file{Environment.NewLine}");
                File.AppendAllText(sPath, @"-----------------------------" + Environment.NewLine);
                File.AppendAllText(sPath, $"Created {DateTime.Now}{Environment.NewLine}{Environment.NewLine}");
            }

            if (AddTimeStamp)
            {
                MessageText = $"({DateTime.Now}) {MessageText}";
            }
            if (appendNewLine)
            {
                File.AppendAllText(sPath, Environment.NewLine + MessageText);
            }
            else
            {
                File.AppendAllText(sPath, MessageText);
            }

            FileInfo fiLog = new FileInfo(sPath);

            if (fiLog.Length > 1000000)
            {
                File.Move(sPath, $@"{ApplicationPath}\{SubFolder}\Completed\{LogFileName}_{DateTime.Now.ToString("dd-mm-yyyy HHmmss")}.log");
            }
        }
        catch (System.Exception)
        {
            throw; // rethrow the exception to client
        }
    }

    /// <summary>
    /// Compresses a string using GZip
    /// </summary>
    /// <param name="text">The string to compress</param>
    /// <returns>The compressed string</returns>
    public static string CompressString(string text)
    {
        byte[] buffer = Encoding.UTF8.GetBytes(text);
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

    /// <summary>
    /// Decompresses a string using GZip
    /// </summary>
    /// <param name="compressedText">The compressed string</param>
    /// <returns>The uncompressed string</returns>
    public static string DecompressString(string compressedText)
    {
        byte[] gZipBuffer = Convert.FromBase64String(compressedText);
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

    public static int RandomNumber(int min, int max)
    {
        Random random = new Random();
        return random.Next(min, max);
    }

    /// <summary>Unscramble a string that was previously scrambled - Default Seed </summary>
    /// <param name="ScrambledString">String to UnScramble</param>
    public static string UnScrambleString(string ScrambledString)
    {
        // Default Seed = 96
        return UnScrambleString(ScrambledString, 96);
    }

    /// <summary>Unscramble a string that was previously scrambled - User Provided Seed </summary>
    /// <param name="ScrambledString">String to scramble</param>
    public static string UnScrambleString(string ScrambledString, int Seed)
    {
        try
        {
            string strTemp = "";
            int i = 0;
            int lTemp = 0;
            int bytKey = 0;
            bytKey = int.Parse(ScrambledString.Substring(ScrambledString.Length - 1), System.Globalization.NumberStyles.HexNumber);

            for (i = 0; i <= ScrambledString.Length - 2; i++)
            {
                lTemp = Convert.ToChar(ScrambledString.Substring(i, 1)) - 32;
                lTemp = Seed - lTemp;
                lTemp = (lTemp - bytKey) % Seed;
                strTemp = strTemp + Convert.ToChar((lTemp) + 32);
            }
            return strTemp;
        }

        catch (System.Exception)
        {
            return "";
        }
    }

    /// <summary>Scramble a string - will produce dfiffent results each time - Default Seed </summary>
    /// <param name="StringToScramble">String to scramble</param>
    public static string ScrambleString(string StringToScramble)
    {
        // Default Seed = 96
        return ScrambleString(StringToScramble, 96);
    }

    /// <summary>Scramble a string - will produce dfiffent results each time - User Provided Seed </summary>
    /// <param name="StringToScramble">String to scramble</param>
    public static string ScrambleString(string StringToScramble, int Seed)
    {
        try
        {
            string strTemp = "";
            string strUnscrambled = "";
            int i = 0;
            int lTemp = 0;
            int bytKey = 0;
            bool bOK = false;

            do
            {
                bytKey = RandomNumber(0, 15); //* 15;
                strTemp = StringToScramble;
                strUnscrambled = "";

                for (i = 0; i <= strTemp.Length - 1; i++)
                {
                    lTemp = Convert.ToChar(strTemp.Substring(i, 1)) - 32;
                    lTemp = (lTemp + bytKey) % Seed;
                    strUnscrambled = strUnscrambled + Convert.ToChar((Seed - lTemp) + 32);
                }
                strUnscrambled = strUnscrambled + bytKey.ToString("X");

                if (UnScrambleString(strUnscrambled, Seed) == StringToScramble)
                {
                    // Not right scramble
                    // Try Again
                    bOK = true;
                }
            }
            while (!(bOK == true));
            return strUnscrambled;
        }
        catch (System.Exception)
        {
            return "";
        }
    }

    /// <summary>
    /// Simple string encryption
    /// </summary>
    /// <param name="sString">String to Encrypt</param>
    public static string Encrypt1(string sString)
    {
        try
        {
            string s = "";
            int i = 0;
            string sHex = "";
            string sNewHex = null;
            int iDec = 0;
            string stmp = "";

            // This method will convert each character in the string
            // into hex then swap the hex around then turn it back to
            // decimal and turn it into a char
            for (i = 0; i <= sString.Length - 1; i++)
            {
                // Turn Char into a hex string
                int ichar = Convert.ToChar(sString.Substring(i, 1));
                sHex = ichar.ToString("X");
                sHex = sHex.PadLeft(2, '0');
                // Swap Hex awound for example
                // 6E becomes E6
                sNewHex = sHex.Substring(sHex.Length - 1) + sHex.Substring(0, 1);
                // Convert the new hex into decimal
                iDec = int.Parse(sNewHex, System.Globalization.NumberStyles.HexNumber);

                if (iDec > 0)
                {
                    // now add the char value to the new string
                    stmp = stmp + Convert.ToChar(iDec);

                    if (stmp.Length > 50)
                    {
                        // This increase performance on large strings
                        s = s + stmp;
                        stmp = "";
                    }
                }
            }
            s = s + stmp;
            return s;
        }

        catch (System.Exception)
        {
            return "";
        }
    }

    #endregion
    #region JSON Serialization methods
    public static string SerializeObject<TENTITY>(TENTITY objectRecord)
    {
        string serialVersion = JsonConvert.SerializeObject(objectRecord, Formatting.Indented, new JsonSerializerSettings()
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Error
        });
        return serialVersion;
    }
    public static TENTITY DeSerializeObject<TENTITY>(string serializedString)
    {
        return JsonConvert.DeserializeObject<TENTITY>(serializedString);
    }
    #endregion
}

