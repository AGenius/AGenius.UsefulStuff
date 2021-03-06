﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using static AGenius.UsefulStuff.ObjectExtensions;

namespace AGenius.UsefulStuff
{
    /// <summary>
    /// Some Common usefull methods
    /// </summary>
    public static class Utils
    {
        /// <summary>Determine if an executable is running</summary>
        /// <param name="FullPath">The full path to the EXE file</param>
        /// <returns>true/false <see cref="bool"/></returns>
        public static bool isExeRunning(string FullPath)
        {
            try
            {
                string FilePath = Path.GetDirectoryName(FullPath);
                string FileName = Path.GetFileNameWithoutExtension(FullPath).ToLower();
                bool isRunning = false;

                Process[] pList = Process.GetProcessesByName(FileName);
                foreach (Process p in pList)
                {
                    if (p.MainModule.FileName.StartsWith(FilePath, StringComparison.InvariantCultureIgnoreCase))
                    {
                        isRunning = true;
                        break;
                    }
                }

                return isRunning;
            }
            catch
            {


            }
            return true; // Ensures if it errors it will not cause it to be re-lanuched as it was possibly terminated
        }

        /// <summary>Copy contents of a directory to a new locaiton</summary>
        /// <param name="sourceDirName">The Source Directory Path</param>
        /// <param name="destDirName">The Target Directory Path</param>
        /// <param name="copySubDirs">Set true if you want all sub directories also copied.</param>
        public static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    $"Source directory does not exist or could not be found: {sourceDirName}");
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }

        #region Scramble Methods       

        /// <summary>The path to the assembly</summary>
        public static string ApplicationPath
        {
            get
            {
                if (System.Reflection.Assembly.GetEntryAssembly() != null)
                {
                    return Directory.GetParent(System.Reflection.Assembly.GetEntryAssembly().Location).FullName;
                }
                return null;
            }
        }

        /// <summary>
        /// Return a DateTime derived from a Unix Epoch time (seconds from 01/01/1970
        /// </summary>
        /// <param name="unixTime">The long value representing the Unix Time</param>
        /// <returns>Date Time value <see cref="DateTime"/></returns>
        public static DateTime DateTimeFromUnixTime(long unixTime)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(unixTime);
        }

        /// <summary>
        /// Handle a list of enums
        /// </summary>
        /// <typeparam name="T">Entity Type</typeparam>
        /// <param name="fieldName">The FieldName to include in the filter string  e.g.  ContactStatus</param>
        /// <param name="objectList">List of Items (Enums)</param>
        /// <returns>Concatenated string list of results</returns>
        public static string BuildFilterString<T>(string fieldName, List<T> objectList)
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
        public static string BuildFilterString<T>(string fieldName, T enumItem)
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
        /// <param name="contents">The string content to write</param>
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
                string sPath = Path.Combine(LogPath, SubFolder ?? "", $"{LogFileName}.log");

                if (System.IO.File.Exists(sPath).Equals(false))
                {
                    Directory.CreateDirectory(Path.Combine(LogPath, SubFolder ?? ""));

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
                    if (SubFolder != null)
                    {
                        Directory.CreateDirectory(Path.Combine(LogPath, SubFolder, "Completed"));
                        // If a subfolder is provided then keep tidy and move to a completed sub folder
                        File.Move(sPath, $@"{ApplicationPath}\{SubFolder}\Completed\{LogFileName}_{DateTime.Now:dd-MM-yyyy HHmmss}.log");
                    }
                    else
                    {
                        // If no subfolder is provided then rename file to include datetime stamp
                        File.Move(sPath, $@"{ApplicationPath}\{LogFileName}_{DateTime.Now:dd-MM-yyyy HHmmss}.log");
                    }

                }
            }
            catch (System.Exception)
            {
                throw; // rethrow the exception to client
            }
        }
        /// <summary>Generate a random string password</summary>
        /// <param name="Length">The length to generate</param>
        /// <param name="NonAlphaNumericChars">The number of non alpha numeric characters to include (optional, Default = 0)</param>
        /// <returns>String result containing a random password <see cref="string"/></returns>
        public static string GeneratePassword(int Length, int NonAlphaNumericChars = 0)
        {
            string allowedChars = "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ0123456789";
            string allowedNonAlphaNum = "!@#$%^&*()_-+;:|./?";
            Random rd = new Random();

            if (NonAlphaNumericChars > Length || Length <= 0 || NonAlphaNumericChars < 0)
            {
                throw new ArgumentOutOfRangeException();
            }

            char[] pass = new char[Length];
            int[] pos = new int[Length];

            //Random the position values of the pos array for the string Pass
            int i = 0;
            while (i < Length - 1)
            {
                bool flag = false;
                int temp = rd.Next(0, Length);
                for (int j = 0; j < Length; j++)
                {
                    if (temp == pos[j])
                    {
                        flag = true;
                        j = Length;
                    }
                }

                if (!flag)
                {
                    pos[i] = temp;
                    i++;
                }
            }

            //Random the AlphaNumericChars
            for (int ii = 0; ii < Length - NonAlphaNumericChars; ii++)
            {
                pass[ii] = allowedChars[rd.Next(0, allowedChars.Length)];
            }

            //Random the NonAlphaNumericChars
            for (int ii = Length - NonAlphaNumericChars; ii < Length; ii++)
            {
                pass[ii] = allowedNonAlphaNum[rd.Next(0, allowedNonAlphaNum.Length)];
            }

            //Set the sorted array values by the pos array for the rigth posistion
            char[] sorted = new char[Length];
            for (i = 0; i < Length; i++)
            {
                sorted[i] = pass[pos[i]];
            }

            string Pass = new String(sorted);

            return Pass;
        }

        /// <summary>return the next filename by adding _(1) or _(2) etc or _1 or _2</summary>
        /// <param name="FileName">Base Filename to start</param>
        /// <param name="addBrackets">Add () around the new file number if applicable</param>
        /// <returns>FileName</returns>
        public static string GetNewfileName(string FileName, bool addBrackets)
        {
            string fileExt = Path.GetExtension(FileName).Replace(".", "");
            string FolderPath = Path.GetDirectoryName(FileName);
            string basefilename = Path.GetFileNameWithoutExtension(FileName);
            int counter = 1;
            //string fullFileName = string.Format(@"{0}\{1} ({2}).{3}", FolderPath, basefilename, counter, fileExt);            
            string fullFileName = $"{FolderPath}\\{basefilename}.{fileExt}";
            while (File.Exists(fullFileName))
            {
                counter++;
                if (addBrackets)
                {
                    fullFileName = $"{FolderPath}\\{basefilename}_({counter}).{fileExt}";
                }
                else
                {
                    fullFileName = $"{FolderPath}\\{basefilename}_{counter}.{fileExt}";
                }
            }
            return fullFileName;
        }
        /// <summary>return the next filename by adding _(1) or _(2) etc or _1 or _2</summary>
        /// <param name="FileName">Base Filename to start</param>
        /// <param name="addBrackets">Add () around the new file number if applicable</param>
        /// <param name="filesList">Compare files using the provided list</param>
        /// <returns>FileName</returns>
        public static string GetNewfileName(string FileName, bool addBrackets, List<string> filesList)
        {
            string fileExt = Path.GetExtension(FileName).Replace(".", "");
            string FolderPath = Path.GetDirectoryName(FileName);
            string basefilename = Path.GetFileNameWithoutExtension(FileName);
            int counter = 1;
            //string fullFileName = string.Format(@"{0}\{1} ({2}).{3}", FolderPath, basefilename, counter, fileExt);            
            string fullFileName = $"{FolderPath}\\{basefilename}.{fileExt}";
            string foundFile = filesList.Find(x => x.ToLower() == fullFileName.ToLower());
            while (!string.IsNullOrEmpty(foundFile))
            {
                counter++;
                if (addBrackets)
                {
                    fullFileName = $"{FolderPath}\\{basefilename}_({counter}).{fileExt}";
                }
                else
                {
                    fullFileName = $"{FolderPath}\\{basefilename}_{counter}.{fileExt}";
                }
                foundFile = filesList.Find(x => x.ToLower() == fullFileName.ToLower());
            }
            return fullFileName;
        }
        /// <summary>Return a list of files from a given location</summary>
        /// <param name="rootPath">Directory location to use</param>
        /// <param name="fileExt">Only return files of this extension</param>
        /// <param name="searchOption">The required search option <see cref="SearchOption"/></param>
        /// <returns></returns>
        public static List<string> GetFilesList(string rootPath, string fileExt, SearchOption searchOption)
        {
            if (!Directory.Exists(rootPath.Replace(@"\\", @"\").ToLower()))
            {
                return null;
            }
            string[] filePaths = Directory.GetFiles($"{rootPath.Replace(@"\\", @"\").ToLower()}", fileExt, searchOption);
            List<string> filenames = new List<string>();

            if (filePaths.Length > 0)
            {
                // add to list of documents to convert to single PDF 
                foreach (string docFile in filePaths)
                {
                    filenames.Add(docFile);
                }
            }
            return filenames;
        }
        /// <summary>Return a random number </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
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
        /// <param name="Seed">The numeric seed to use when scrambling</param>
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
                    strTemp += Convert.ToChar((lTemp) + 32);
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

        /// <summary>Scramble a string - will produce dfiffent results each time - User Provided Seed</summary>
        /// <param name="StringToScramble">String to scramble</param>
        /// <param name="Seed">The numeric seed to use when scrambling</param>
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
                        strUnscrambled += Convert.ToChar((Seed - lTemp) + 32);
                    }
                    strUnscrambled += bytKey.ToString("X");

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

        #endregion
        #region JSON Serialization methods
        /// <summary>
        /// Convert an Object to a JSON String
        /// </summary>
        /// <typeparam name="TENTITY">Entity type</typeparam>
        /// <param name="objectRecord">The object record</param>
        /// <returns>JSON String</returns>
        public static string SerializeObject<TENTITY>(TENTITY objectRecord)
        {
            string serialVersion = JsonConvert.SerializeObject(objectRecord, Formatting.Indented, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Error
            });
            return serialVersion;
        }
        /// <summary>
        /// Deserialize a JSON string an object 
        /// </summary>
        /// <typeparam name="TENTITY">Entity type</typeparam>
        /// <param name="serializedString">JSON String</param>
        /// <returns>Object of type TENTITY</returns>
        public static TENTITY DeSerializeObject<TENTITY>(string serializedString)
        {
            return JsonConvert.DeserializeObject<TENTITY>(serializedString);
        }
        /// <summary>
        /// Decode and convert a JSON Web Token string to a JSON object string
        /// </summary>
        /// <param name="JWTTokenString">The JWT token to be decoded</param>
        /// <returns>string containing the JSON object</returns>
        public static string JWTtoJSON(string JWTTokenString)
        {
            var jwtHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();

            //Check if readable token (string is in a JWT format)            
            if (jwtHandler.CanReadToken(JWTTokenString))
            {
                var token = jwtHandler.ReadJwtToken(JWTTokenString);

                //Extract the payload of the JWT                
                string payload = "{";
                foreach (var item in token.Payload)
                {
                    if (item.Value.GetType().Name == "JArray")
                    {
                        payload += '"' + item.Key + "\":" + item.Value + ",";
                    }
                    else
                    {
                        payload += '"' + item.Key + "\":\"" + item.Value + "\",";
                    }
                }
                payload += "}";
                return Newtonsoft.Json.Linq.JToken.Parse(payload).ToString(Newtonsoft.Json.Formatting.Indented);
            }
            return null;
        }
        #endregion

        #region Form Fader
        /// <summary>
        /// Fade into view a WinForms form
        /// </summary>
        /// <param name="form">The Form to fade</param>
        /// <param name="interval">Interval to pause between each increase</param>
        public async static void FadeIn(Form form, int interval = 10)
        {
            FormBorderStyle currentBorder = form.FormBorderStyle;

            try
            {
                //Object is not fully invisible. Fade it in
                form.Opacity = 0;
                form.FormBorderStyle = FormBorderStyle.None;
                while (form.Opacity < 1.0)
                {
                    await Task.Delay(interval);
                    form.Opacity += 0.05;
                }
                form.Opacity = 1; //make fully visible       
                form.FormBorderStyle = currentBorder;
            }
            catch (Exception)
            {
                // Set to full visible as some error has occured (possible form now closed)
                form.Opacity = 1;
                form.FormBorderStyle = currentBorder;
            }
        }
        /// <summary>
        /// Fade out of view a WinForms form
        /// </summary>
        /// <param name="form">The Form to fade</param>
        /// <param name="interval">Interval to pause between each decrease</param>
        public async static void FadeOut(Form form, int interval = 10)
        {
            try
            {
                //Object is fully visible. Fade it out
                form.Opacity = 1;
                while (form.Opacity > 0.0)
                {
                    await Task.Delay(interval);
                    form.Opacity -= 0.05;
                }
                form.Opacity = 0; //make fully invisible       
            }
            catch (Exception)
            {
                // Set to full invisible as some error has occured (possible form now closed)
                form.Opacity = 0; //make fully invisible       
            }
        }
        #endregion
        /// <summary>Find the Value of an Objects Propertie for a given field name using reflection</summary>
        /// <typeparam name="TEntity">Entity Object Type</typeparam>
        /// <param name="EntityObject">The Object Reference</param>
        /// <param name="PropertyName">Name of the Property to find</param>
        /// <returns></returns>
        public static object GetValueForPropertyBystringName<TEntity>(TEntity EntityObject, string PropertyName)
        {
            // Build the Properties list so it can be accessed via the name string
            foreach (PropertyInfo p in EntityObject.GetType().GetProperties())
            {
                if (p.Name.ToLower() == PropertyName.ToLower())
                {
                    return p.GetValue(EntityObject);
                }
            }

            return null;
        }


        #region Send Email via SMTP
        /// <summary>
        /// Holds any Error messages from the Email Process
        /// </summary>
        public static string EmailFailedReason = null;
        /// <summary>
        /// Send an Email using System.Net.Mail
        /// </summary>
        /// <param name="isHTML">Indicates the Email body should be set to HTML</param>
        /// <param name="MailFrom">Email Address From</param>
        /// <param name="EmailTo">Email Address To</param>
        /// <param name="MessageBody">The Email message body</param>
        /// <param name="Subject">The Email Subject</param>
        /// <param name="SMTPHost">SMTP Host address</param>
        /// <param name="SMTPUser">SMTP Username</param>
        /// <param name="SMTPPass">SMTP Password</param>
        /// <param name="SMTPPort">SMTP Port to use (optional, default=25) <see cref="int"/> </param>
        /// <param name="SMTPSSL">SMTP SSL required (optional, default=false) <see cref="bool"/> </param>
        /// <param name="SMTPAuth">SMTP Authentication required (optional, default=false) <see cref="bool"/></param>
        /// <param name="BCCList">BCC List to send email too, this is a ; seperated list (optional)</param>
        /// <param name="CCList">CC List to send email too, this is a ; seperated list (optional)</param>
        /// <param name="imagespath">Path to where any images are located to embed in the Email body. will convert any src= references to the embeded image</param>
        /// <param name="AttachmentPaths">A list of attachments to include in the email message (optional) <see cref="List{T}"/></param>
        /// <param name="LogErrors">Log any errors to "SMTPErrors.Log" file in the application folder</param>
        /// <returns>True if successful or False if an error occurs. <seealso cref="EmailFailedReason"/></returns>
        public static bool SendEmailMessage(bool isHTML,
            string MailFrom,
            string EmailTo,
            string MessageBody,
            string Subject,
            string SMTPHost,
            string SMTPUser,
            string SMTPPass,
            int SMTPPort = 25,
            bool SMTPSSL = false,
            bool SMTPAuth = false,
            string BCCList = null,
            string CCList = null,
            string imagespath = null,
            List<string> AttachmentPaths = null,
            bool LogErrors = false)
        {
            List<string> images = new List<string>(); // This is to store the images found
            AlternateView avHtml = null;

            try
            {
                // Now deal with images so they can be embedded
                // --------------------------
                string tempcontent = MessageBody + "src=\""; // Ensures we get the last field
                string strTemp = "";

                if (!String.IsNullOrEmpty(imagespath))
                {
                    // Build up the list of images parts in the Content
                    do
                    {
                        tempcontent = tempcontent.GetAfter("src=\"");
                        strTemp = tempcontent.GetBefore("\"");

                        if (!String.IsNullOrEmpty(strTemp))
                        {
                            images.Add(strTemp.Replace("%20", " "));
                        }
                    }
                    while (!String.IsNullOrEmpty(tempcontent));

                    // Found some images so carry on
                    if (images.Count > 0)
                    {
                        int counter = 1;

                        foreach (string item in images)
                        {
                            // replace images with CID entries
                            MessageBody = MessageBody.Replace(String.Format("src=\"{0}\"", item), String.Format("src=\"cid:image{0}\"", counter++));
                        }
                        avHtml = AlternateView.CreateAlternateViewFromString(MessageBody, null, MediaTypeNames.Text.Html);
                        //
                        //AlternateView avText = AlternateView.CreateAlternateViewFromString(sMessage, null, MediaTypeNames.Text.Plain);
                        counter = 1;

                        if (!imagespath.EndsWith(@"\"))
                        {
                            imagespath += @"\";
                        }
                        string strItem = "";

                        try
                        {
                            foreach (string item in images)
                            {
                                // Create a LinkedResource object for each embedded image and
                                // add to the Alternative view object
                                string ext = System.IO.Path.GetExtension(item);
                                strItem = item;
                                LinkedResource imageitem;

                                switch (ext.ToLower())
                                {
                                    case ".jpg":
                                    case ".jpeg":
                                        imageitem = new LinkedResource(imagespath + strItem, MediaTypeNames.Image.Jpeg);
                                        break;
                                    case ".gif":
                                        imageitem = new LinkedResource(imagespath + strItem, MediaTypeNames.Image.Gif);
                                        break;
                                    case ".png":
                                        imageitem = new LinkedResource(imagespath + strItem, "image/png");
                                        break;
                                    default:
                                        imageitem = null;
                                        break;
                                }
                                imageitem.ContentId = "image" + counter++;
                                avHtml.LinkedResources.Add(imageitem);
                                //
                            }
                        }

                        catch (Exception ex)
                        {
                            if (LogErrors)
                            {
                                WriteLogFile("Possible invalid Image - " + strItem, "SMTPErrors", null, null, true, true);
                                WriteLogFile($"Trace- {ex.Message} {Environment.NewLine}{ex.StackTrace}", "SMTPErrors", null, null, true, true);
                            }

                            EmailFailedReason = ex.Message;
                            return false;
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                if (LogErrors)
                {
                    WriteLogFile($"Trace- {ex.Message} {Environment.NewLine}{ex.StackTrace}", "SMTPErrors", null, null, true, true);
                }

                EmailFailedReason = ex.Message;
            }

            try
            {
                Int32 iSMTPPort = SMTPPort;
                MailMessage mailMsg = new MailMessage();

                if (!EmailTo.EndsWith(";"))
                {
                    EmailTo += ";";
                }
                string[] sToAddresses = EmailTo.Split(';');

                foreach (string address in sToAddresses)
                {
                    if (!String.IsNullOrEmpty(address))
                    {
                        mailMsg.To.Add(address);
                    }
                }

                // Bcc ?
                if (!String.IsNullOrEmpty(BCCList))
                {
                    if (!BCCList.EndsWith(";"))
                    {
                        BCCList += ";";
                    }
                    string[] sBccAddresses = BCCList.Split(';');

                    foreach (string address in sBccAddresses)
                    {
                        if (!String.IsNullOrEmpty(address))
                        {
                            mailMsg.Bcc.Add(address);
                        }
                    }
                }
                // CC ?
                if (!String.IsNullOrEmpty(CCList))
                {
                    if (!CCList.EndsWith(";"))
                    {
                        CCList += ";";
                    }
                    string[] sCcAddresses = CCList.Split(';');

                    foreach (string address in sCcAddresses)
                    {
                        if (!String.IsNullOrEmpty(address))
                        {
                            mailMsg.CC.Add(address);
                        }
                    }
                }
                // From
                if (MailFrom == "")
                {
                    MailFrom = "postmaster@localhost";
                }
                MailAddress mailAddress = new MailAddress(MailFrom);
                mailMsg.From = mailAddress;
                // Subject and Body
                mailMsg.Subject = Subject;
                mailMsg.Body = MessageBody;
                mailMsg.IsBodyHtml = isHTML;
                if (isHTML)
                {
                    if (images.Count > 0)
                    {
                        mailMsg.AlternateViews.Add(avHtml);
                    }
                }

                // Add attachments if any
                if (AttachmentPaths != null)
                {
                    foreach (string AttPath in AttachmentPaths)
                    {
                        mailMsg.Attachments.Add(new System.Net.Mail.Attachment(AttPath));
                    }
                }
                // Init SmtpClient and send
                SmtpClient smtpClient = new SmtpClient(SMTPHost, Convert.ToInt32(iSMTPPort));

                if (SMTPAuth)
                {
                    System.Net.NetworkCredential credentials = new System.Net.NetworkCredential(SMTPUser, SMTPPass);
                    smtpClient.Credentials = credentials;
                }
                if (SMTPSSL)
                {
                    smtpClient.EnableSsl = true;
                }
                smtpClient.Send(mailMsg);
                mailMsg.Dispose();
                return true;
            }

            catch (Exception ex)
            {
                EmailFailedReason = ex.Message;
                if (LogErrors)
                {
                    WriteLogFile("Error Sending Email", "SMTPErrors", null, null, true, true);
                    WriteLogFile($"Trace- {ex.Message} {Environment.NewLine}{ex.StackTrace}", "SMTPErrors", null, null, true, true);
                }

                return false;
            }
        }
        /// <summary>Replace field placeholders in a string with object properties of the same name</summary>
        /// <typeparam name="T">The object type</typeparam>
        /// <param name="ContentString">The content to process</param>
        /// <param name="TheEntity">The object holding the properties</param>
        /// <param name="ReplaceIfNullWith">If the field is null then replace the field place holder with this value</param>
        /// <param name="StartField">The expected string for the start of a field place holder</param>
        /// <param name="EndField">The expected string for the end of a field place holder</param>
        /// <returns>The new string content with the new content</returns>
        /// <remarks>Will detect DATETIME and replace with the current date and time as ToLongDateString <see cref="DateTime.Now"/> </remarks>
        public static string ReplaceObjectFields<T>(string ContentString, T TheEntity, string ReplaceIfNullWith = null, string StartField = "[[", string EndField = "]]")
        {
            string NewContentString = ContentString;
            List<string> FieldsList = GetTokensFromString(ContentString, StartField, EndField);

            // Process the found fields            
            foreach (string fieldNameEntry in FieldsList)
            {
                string fieldName = fieldNameEntry;
                string objectName = string.Empty;
                if (fieldName.Contains("."))
                {
                    // Extract object name
                    objectName = fieldNameEntry.Split('.')[0];
                    fieldName = fieldNameEntry.Split('.')[1];

                    switch (objectName.ToUpper())
                    {
                        case "DATETIME":
                            if (fieldName.ToUpper() == "NOW")
                            {
                                NewContentString = NewContentString.Replace($"{StartField}{objectName}.{fieldName}{EndField}", DateTime.Now.ToLongDateString());
                            }

                            break;
                        default:

                            if (TheEntity.GetPropertyValue(fieldName) != null)
                            {
                                NewContentString = FormatFieldValue(NewContentString, TheEntity.GetPropertyValue(fieldName), fieldName, objectName, StartField, EndField);
                            }
                            else
                            {
                                if (ReplaceIfNullWith != null)
                                {
                                    NewContentString = FormatFieldValue(NewContentString, ReplaceIfNullWith, fieldName, objectName, StartField, EndField);
                                }
                            }

                            break;
                    }
                }
                else
                {
                    if (fieldName.ToUpper() == "NOW")
                    {
                        NewContentString = NewContentString.Replace($"{StartField}{fieldName}{EndField}", DateTime.Now.ToLongDateString());
                    }
                    else
                    {
                        if (TheEntity.GetPropertyValue(fieldName) != null)
                        {
                            NewContentString = FormatFieldValue(NewContentString, TheEntity.GetPropertyValue(fieldName), fieldName, "", StartField, EndField);
                        }
                        else
                        {
                            if (ReplaceIfNullWith != null)
                            {
                                NewContentString = FormatFieldValue(NewContentString, ReplaceIfNullWith, fieldName, objectName, StartField, EndField);
                            }
                        }
                    }
                }
            }

            return NewContentString;
        }
        private static string FormatFieldValue(string newTemplateText, object fieldValue, string fieldName, string objectName = "", string StartField = "[[", string EndField = "]]")
        {
            string searchString = $"{StartField}{fieldName}{EndField}";

            if (!string.IsNullOrEmpty(objectName))
            {
                searchString = $"{StartField}{objectName}.{fieldName}{EndField}";
            }

            if (fieldValue == null)
            {
                return newTemplateText.Replace(searchString, string.Empty);
            }
            if (fieldValue.GetType().Name == "DateTime")
            {
                return newTemplateText.Replace(searchString, ((DateTime)fieldValue).ToShortDateString());
            }
            else if (fieldValue.GetType().Name == "Double")
            {
                return newTemplateText.Replace(searchString, ((double)fieldValue).ToString("c2"));
            }
            else
            {
                return newTemplateText.Replace(searchString, fieldValue.ToString());
            }
        }
        /// <summary>Returns a list of tokens from a supplied string</summary>        
        /// <param name="ContentString">The content to process</param>
        /// <param name="StartField">The expected string for the start of a field place holder</param>
        /// <param name="EndField">The expected string for the end of a field place holder</param>
        /// <returns>List of tokens in a list object></returns>     
        public static List<string> GetTokensFromString(string ContentString, string StartField = "[[", string EndField = "]]")
        {
            string NewContentString = ContentString;
            List<string> FieldsList = new List<string>();
            bool bDone = false;

            // Find all the fields in the content string
            do
            {
                NewContentString = NewContentString.GetAfter(StartField);
                if (!string.IsNullOrEmpty(NewContentString) && NewContentString.Contains(EndField))
                {
                    FieldsList.Add(NewContentString.GetBefore(EndField));
                    NewContentString = NewContentString.GetAfter(EndField);
                }
                else
                {
                    bDone = true;
                }
            }
            while (!bDone);

            return FieldsList;
        }
        /// <summary>Replace the tokens in a string that are considered empty</summary>
        /// <param name="ContentString">The content to process</param>      
        /// <param name="LenthLimit">How many characters to check, anything at this or below will be removed</param>
        /// <param name="StartField">The expected string for the start of a field place holder</param>
        /// <param name="EndField">The expected string for the end of a field place holder</param>
        /// <returns></returns>
        /// <remarks>The process will check each found token by trimming and if the result is less than the supplied LenthLimit characters then it is removed</remarks>
        public static string ReplaceEmptyTokensInString(string ContentString, int LenthLimit = 1, string StartField = "[[", string EndField = "]]")
        {
            List<string> tokens = ContentString.GetTokensFromString(StartField, EndField);
            if (tokens != null && tokens.Count > 0)
            {
                foreach (string token in tokens)
                {
                    if (token.Trim().Length <= LenthLimit)
                    {
                        ContentString = ContentString.Replace(StartField + token + EndField, "");
                    }
                    else
                    {
                        ContentString = ContentString.Replace(StartField + token + EndField, token);
                    }
                }
            }
            return ContentString;
        }
        /// <summary>
        /// Send an Email using System.Net.Mail
        /// </summary>
        /// <param name="isHTML">Indicates the Email body should be set to HTML</param>
        /// <param name="MailFrom">Email Address From</param>
        /// <param name="EmailTo">Email Address To</param>
        /// <param name="MessageBody">The Email message body</param>
        /// <param name="Subject">The Email Subject</param>
        /// <param name="SMTPHost">SMTP Host address</param>
        /// <param name="SMTPUser">SMTP Username</param>
        /// <param name="SMTPPass">SMTP Password</param>
        /// <param name="SMTPPort">SMTP Port to use (optional, default=25) <see cref="int"/> </param>
        /// <param name="LogErrors">Log any errors to "SMTPErrors.Log" file in the application folder</param>
        /// <returns></returns>
        public static bool SendEmailMessage(bool isHTML,
            string MailFrom,
            string EmailTo,
            string MessageBody,
            string Subject,
            string SMTPHost,
            string SMTPUser,
            string SMTPPass,
            int SMTPPort = 25,
            bool LogErrors = false)
        {
            return SendEmailMessage(isHTML, MailFrom, EmailTo, MessageBody, Subject, SMTPHost, SMTPUser, SMTPPass, SMTPPort, LogErrors);
        }
        #endregion
    }
}