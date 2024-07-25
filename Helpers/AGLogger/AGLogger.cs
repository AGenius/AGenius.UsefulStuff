using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AGenius.UsefulStuff.Helpers.AGLogger;

/// <summary>
/// Provides a more usefull logger ability
/// </summary>
public class AGLogger
{
    internal string _logPath { get; set; }
    internal string _logFileName { get; set; }
    #region Properties
    /// <summary>
    /// The File Path to the log file location (optional - will use assembly path if excluded or null)
    /// </summary>
    /// /// <remarks>Excepts a <see langword="string"/> value - Default:empty</remarks>
    public string CurrentLogFilePath { get; set; }
    /// <summary>
    /// Add a time stamp to the begining of the message
    /// </summary>
    /// /// <remarks>Excepts a <see langword="bool"/> value - Default:true</remarks>
    public bool AddTimeStamp { get; set; } = true;
    /// <summary>
    /// >Maximum size of the log file
    /// </summary>
    /// <remarks>Excepts a <see langword="int"/> value - Default:1024000</remarks>
    public long MaxLogFileSize { get; set; } = 1024000;
    /// <summary>
    /// The length of the log level tag in the output e.g. 3 = [INF], 4 = [INFO]
    /// </summary>
    /// /// <remarks>Excepts a <see langword="int"/> value - Default:3</remarks>
    public int LogLevelChars { get; set; } = 3;
    /// <summary>
    /// The string format for the date stamp in the output
    /// </summary>
    /// <remarks>Excepts a <see langword="string"/> value - Default:dd/MM/yyyy HH:mm:ss zzz</remarks>
    public string TimeStampFormat { get; set; } = "dd/MM/yyyy HH:mm:ss zzz";
    /// <summary>
    /// The logging level - if not set to Verbose, only none Verbose events are logged.
    /// </summary>
    public LogEventLevel? LogLevel { get; set; } = LogEventLevel.Information;
    /// <summary>
    /// Include a header on the new log file creation
    /// </summary>
    /// <remarks>Excepts a <see langword="bool"/> value - Default:true</remarks>
    public bool AddNewLogHeader { get; set; } = true;
    /// <summary>
    /// If the log filesize is exceeded, roll over the log file based on this interval
    /// </summary>
    public RollingInterval? LogRolloverInterval { get; set; } = RollingInterval.Infinite;
    /// <summary>
    /// If a log file is rolled over, move it to the specified sub folder
    /// </summary>
    private string RolloverSubfolderName { get; set; }

    #endregion
    /// <summary>
    /// Create an instance of AGLogger
    /// </summary>
    /// <param name="LogPath">Log path (including Log FileName)</param>
    /// <param name="AddTimeStamp">Include a time stamp for entries</param>
    /// <param name="MaxLogFileSize">Set the max file size for a log file</param>
    /// <param name="LogLevelChars">The length of the level tag in the log file (3 = [INF], 4 = [INFO] etc) : default:3</param>
    /// <param name="TimeStampFormat">Format of the timestamp - default:dd/MM/yyyy HH:mm:ss zzz/></param>
    /// <param name="AddNewLogHeader">Include a header on new log file created - default:true</param>   
    /// <param name="Interval">Sepcify the interval the file should rollover <see cref="RollingInterval"/></param>
    /// <param name="RolloverSubFolder">Move rolled over log files to the sub folder specified</param>
    public AGLogger(string LogPath = null,
        bool AddTimeStamp = true,
        int MaxLogFileSize = 1024000,
        int LogLevelChars = 3,
        string TimeStampFormat = "dd/MM/yyyy HH:mm:ss zzz",
        bool AddNewLogHeader = true,
        RollingInterval Interval = RollingInterval.Infinite,
        string RolloverSubFolder = "")
    {
        this.CurrentLogFilePath = LogPath;
        this.AddTimeStamp = AddTimeStamp;
        this.MaxLogFileSize = MaxLogFileSize;
        this.LogLevelChars = LogLevelChars;
        this.TimeStampFormat = TimeStampFormat;
        this.AddNewLogHeader = AddNewLogHeader;
        this.LogRolloverInterval = Interval;
        this.RolloverSubfolderName = RolloverSubfolderName;
        _nextCheckpoint = GetNextCheckpoint(DateTime.Now);
    }
    /// <summary>
    /// Create an instance of the AGLogger
    /// </summary>
    /// <remarks>Apply settings from app.config</remarks>
    public AGLogger()
    {
        // Attempt load from App.config
        string logFileWithPath = ConfigurationManager.AppSettings["AGLog:file.path"];
        if (string.IsNullOrEmpty(logFileWithPath))
        {
            // No log file and path supplied so set defaults
            logFileWithPath = Path.Combine(Utils.ApplicationPath, "Application_Log.log");
        }
        //string logFileName = ConfigurationManager.AppSettings["AGLog:file.name"];// Change this to get name from path
        _logFileName = Path.GetFileName(logFileWithPath);
        _logPath = Path.GetDirectoryName(logFileWithPath);
        if (Environment.ExpandEnvironmentVariables(Path.GetDirectoryName(logFileWithPath)).StartsWith(@"\"))
        {
            logFileWithPath = Path.Combine(Utils.ApplicationPath, logFileWithPath.TrimStart(Path.DirectorySeparatorChar));
        }

        this.CurrentLogFilePath += logFileWithPath;

        string sizeLimit = ConfigurationManager.AppSettings["AGLog:file.sizelimitbytes"];
        if (!string.IsNullOrEmpty(sizeLimit))
        {
            this.MaxLogFileSize = int.Parse(sizeLimit);
        }

        string logLevel = ConfigurationManager.AppSettings["AGLog:minimum-level"];
        if (!string.IsNullOrEmpty(logLevel))
        {
            this.LogLevel = logLevel.ToEnum<LogEventLevel>();
        }

        string logLevelLen = ConfigurationManager.AppSettings["AGLog:level.length"];
        if (!string.IsNullOrEmpty(logLevelLen))
        {
            this.LogLevelChars = int.Parse(logLevelLen);
        }

        string addTimeStamp = ConfigurationManager.AppSettings["AGLog:timestamp.visible"];
        if (!string.IsNullOrEmpty(addTimeStamp) && addTimeStamp.ToLower() == "false")
        {
            this.AddTimeStamp = false;
        }

        string timeStampFormat = ConfigurationManager.AppSettings["AGLog:timestamp.format"];
        if (!string.IsNullOrEmpty(timeStampFormat))
        {
            this.TimeStampFormat = timeStampFormat;
        }
        string Rolloverinterval = ConfigurationManager.AppSettings["AGLog:rollover.interval"];
        if (!string.IsNullOrEmpty(Rolloverinterval))
        {
            this.LogRolloverInterval = Rolloverinterval.ToEnum(RollingInterval.Infinite, true);
        }
        string Rolloversubfolder = ConfigurationManager.AppSettings["AGLog:rollover.subfolder"];
        if (!string.IsNullOrEmpty(Rolloversubfolder))
        {
            this.RolloverSubfolderName = Rolloversubfolder;
        }
        _nextCheckpoint = GetNextCheckpoint(DateTime.Now);
    }

    /// <summary>Write to a text log file.</summary>
    /// <param name="EventText">The string message to write</param>
    /// <param name="logLevel">The event level of the entry</param>        
    /// <param name="appendNewLine">Automatically add a new line after the message</param>        
    public void WriteLog(string EventText, LogEventLevel? logLevel = null, bool appendNewLine = true)
    {
        try
        {
            if (logLevel == null)
            {
                logLevel = this.LogLevel;
            }
            if (CurrentLogFilePath == null)
            {
                CurrentLogFilePath = Path.Combine(Utils.ApplicationPath, "Logfile.log"); // Default path and name
            }
            if (!CurrentLogFilePath.ToLower().EndsWith(".log"))
            {
                CurrentLogFilePath += ".log";
            }
            string logFileName = Path.GetFileName(CurrentLogFilePath); // Get the file name portion
            string logPathName = Path.GetDirectoryName(CurrentLogFilePath); // Get Folder Path
            string logLevelTag = GetLevelMoniker(_upperCaseLevelMap, logLevel.Value, this.LogLevelChars);

            //  string sPath = Path.Combine(LogPath, SubFolder ?? "", $"{logFileName}.log");

            if (System.IO.File.Exists(CurrentLogFilePath).Equals(false))
            {
                Directory.CreateDirectory(Path.Combine(logPathName));
                if (AddNewLogHeader)
                {
                    File.AppendAllText(CurrentLogFilePath, @"-----------------------------" + Environment.NewLine);
                    File.AppendAllText(CurrentLogFilePath, $"{logFileName} Log file{Environment.NewLine}");
                    File.AppendAllText(CurrentLogFilePath, @"-----------------------------" + Environment.NewLine);
                    File.AppendAllText(CurrentLogFilePath, $"Created {DateTime.Now.ToString(TimeStampFormat)}{Environment.NewLine}");
                    File.AppendAllText(CurrentLogFilePath, @"--------------------------------------------------------------------");
                }
            }

            if (AddTimeStamp)
            {
                EventText = $"{DateTime.Now.ToString(TimeStampFormat)} [{logLevelTag}] {EventText}";
            }
            else
            {
                EventText = $"[{logLevelTag}] {EventText}";
            }
            if (logLevel != LogEventLevel.Verbose || this.LogLevel == LogEventLevel.Verbose)
            {
                if (appendNewLine)
                {
                    File.AppendAllText(CurrentLogFilePath, Environment.NewLine + EventText);
                }
                else
                {
                    File.AppendAllText(CurrentLogFilePath, EventText);
                }
            }

            RolloverLogFile();
            //FileInfo fiLog = new FileInfo(CurrentLogFilePath);

            //if (fiLog.Length > MaxLogFileSize)
            //{
            //    Directory.CreateDirectory(Path.Combine(logPathName, "Completed"));
            //    // If no subfolder is provided then rename file to include datetime stamp
            //    File.Move(CurrentLogFilePath, $@"{logPathName}\{logFileName.Replace(".log", "")}_{DateTime.Now:dd-MM-yyyy HHmmss}.log");
            //}
        }
        catch (System.Exception)
        {
            throw; // rethrow the exception to client
        }
    }
    /// <summary>Write an event entry with a specified level.</summary>
    /// <param name="EventText">The string message to write</param>
    /// <param name="logLevel">The event level of the entry</param>        
    /// <param name="objects">Array of objects to be serialized and embeded in the message template e.g. WriteLogEntry("Record 1:{@Record1}, Record 2:{@Record2}", new object[] { Record1, Record2 }</param>
    public void WriteLog(string EventText, object[] objects, LogEventLevel? logLevel = null)
    {
        try
        {
            string logMessage = SerializeObjects((EventText, objects));
            if (logLevel != null)
            {
                WriteLog(logMessage, logLevel);
            }
            else
            {
                WriteLog(logMessage, this.LogLevel);
            }

        }
        catch (System.Exception)
        {
            throw; // rethrow the exception to client
        }
    }
    /// <summary>
    /// Write a log event with the <see cref="LogEventLevel.Error"/> level and associated exception.
    /// </summary>
    /// <param name="EventText">The string message to describe the log entry</param>

    public void LogError(string EventText)
    {
        WriteLog(EventText, LogEventLevel.Error);
    }
    /// <summary>
    /// Write a log event with the <see cref="LogEventLevel.Error"/> level and associated exception.
    /// </summary>
    /// <param name="EventText">The string message to describe the log entry</param>        
    /// <param name="objects">Array of objects to be serialized and embeded in the message template e.g. WriteLogEntry("Record 1:{@Record1}, Record 2:{@Record2}", new object[] { Record1, Record2 }</param>
    public void LogError(string EventText, object[] objects)
    {
        string logMessage = SerializeObjects((EventText, objects));
        LogError(logMessage);
    }

    /// <summary>
    /// Write a log event with the <see cref="LogEventLevel.Verbose"/> level and associated exception.
    /// </summary>
    /// <param name="EventText">The string message to describe the log entry</param>
    public void LogVerbose(string EventText)
    {
        WriteLog(EventText, LogEventLevel.Verbose);
    }
    /// <summary>
    /// Write a log event with the <see cref="LogEventLevel.Verbose"/> level and associated exception.
    /// </summary>
    /// <param name="EventText">The string message to describe the log entry</param>       
    /// <param name="objects">Array of objects to be serialized and embeded in the message template 
    /// e.g. WriteLogEntry("Record 1:{@Record1}, Record 2:{@Record2}", new object[] { Record1, Record2 }</param>
    public void LogVerbose(string EventText, object[] objects)
    {
        string logMessage = SerializeObjects((EventText, objects));
        LogVerbose(logMessage);
    }
    /// <summary>
    /// Write a log event with the <see cref="LogEventLevel.Verbose"/> level and associated exception.
    /// </summary>
    /// <param name="EventText">The string message to describe the log entry</param>       
    /// <param name="param">object to be serialized and embeded in the message template 
    /// e.g. WriteLogEntry("Record 1:{@Record1}", PersonRecord)</param>
    public void LogVerbose<T>(string EventText, T param)
    {
        var objects = new object[] { param };
        string logMessage = SerializeObjects((EventText, objects));
        LogVerbose(logMessage);
    }
    /// <summary>
    /// Write a log event with the <see cref="LogEventLevel.Verbose"/> level and associated exception.
    /// </summary>
    /// <param name="EventText">The string message to describe the log entry</param>       
    /// <param name="param0">object to be serialized and embeded in the message template 
    /// <param name="param1">object to be serialized and embeded in the message template 
    /// e.g. WriteLogEntry("Record 1:{@Record1}, Record 2:{@Record2}", PersonRecord1, PersonRecord2 )</param>
    public void LogVerbose<T0, T1>(string EventText, T0 param0, T1 param1)
    {
        var objects = new object[] { param0, param1 };
        string logMessage = SerializeObjects((EventText, objects));
        LogVerbose(logMessage);
    }
    /// <summary>
    /// Write a log event with the <see cref="LogEventLevel.Verbose"/> level and associated exception.
    /// </summary>
    /// <param name="EventText">The string message to describe the log entry</param>       
    /// <param name="param0">object to be serialized and embeded in the message template 
    /// <param name="param1">object to be serialized and embeded in the message template 
    /// <param name="param2">object to be serialized and embeded in the message template 
    /// e.g. WriteLogEntry("Record 1:{@Record1}, Record 2:{@Record2}, Record 3:{@Record3}", PersonRecord1, AddressRecord1, PersonRecord2 )</param>
    public void LogVerbose<T0, T1, T2>(string EventText, T0 param0, T1 param1, T2 param2)
    {
        var objects = new object[] { param0, param1, param2 };
        string logMessage = SerializeObjects((EventText, objects));
        LogVerbose(logMessage);
    }
    /// <summary>
    /// Write a log event with the <see cref="LogEventLevel.Verbose"/> level and associated exception.
    /// </summary>
    /// <param name="EventText">The string message to describe the log entry</param>       
    /// <param name="param0">object to be serialized and embeded in the message template 
    /// <param name="param1">object to be serialized and embeded in the message template 
    /// <param name="param2">object to be serialized and embeded in the message template 
    /// <param name="param3">object to be serialized and embeded in the message template 
    /// e.g. WriteLogEntry("Record 1:{@Record1}, Record 2:{@Record2}, Record 3:{@Record3}", PersonRecord1, AddressRecord1, PersonRecord2, AddressRecord2 )</param>
    public void LogVerbose<T0, T1, T2, T3>(string EventText, T0 param0, T1 param1, T2 param2, T3 param3)
    {
        var objects = new object[] { param0, param1, param2, param3 };
        string logMessage = SerializeObjects((EventText, objects));
        LogVerbose(logMessage);
    }

    /// <summary>
    /// Write a log event with the <see cref="LogEventLevel.Information"/> level and associated exception.
    /// </summary>
    /// <param name="EventText">The string message to describe the log entry</param>
    public void LogInfo(string EventText)
    {
        WriteLog(EventText, LogEventLevel.Information);
    }
    /// <summary>
    /// Write a log event with the <see cref="LogEventLevel.Information"/> level and associated exception.
    /// </summary>
    /// <param name="EventText">The string message to describe the log entry</param>     
    /// <param name="objects">Array of objects to be serialized and embeded in the message template 
    /// e.g. WriteLogEntry("Record 1:{@Record1}, Record 2:{@Record2}", new object[] { Record1, Record2 }</param>
    public void LogInfo(string EventText, object[] objects)
    {
        string logMessage = SerializeObjects((EventText, objects));
        LogInfo(logMessage);
    }

    /// <summary>
    /// Write a log event with the <see cref="LogEventLevel.Debug"/> level and associated exception.
    /// </summary>
    /// <param name="EventText">The string message to describe the log entry</param>
    public void LogDebug(string EventText)
    {
        WriteLog(EventText, LogEventLevel.Debug);
    }
    /// <summary>
    /// Write a log event with the <see cref="LogEventLevel.Debug"/> level and associated exception.
    /// </summary>
    /// <param name="EventText">The string message to describe the log entry</param>       
    /// <param name="objects">Array of objects to be serialized and embeded in the message template 
    /// e.g. WriteLogEntry("Record 1:{@Record1}, Record 2:{@Record2}", new object[] { Record1, Record2 }</param>
    public void LogDebug(string EventText, object[] objects)
    {
        string logMessage = SerializeObjects((EventText, objects));
        LogDebug(logMessage);
    }

    /// <summary>
    /// Write a log event with the <see cref="LogEventLevel.Warning"/> level and associated exception.
    /// </summary>
    /// <param name="EventText">The string message to describe the log entry</param>
    public void LogWarning(string EventText)
    {
        WriteLog(EventText, LogEventLevel.Warning);
    }
    /// <summary>
    /// Write a log event with the <see cref="LogEventLevel.Warning"/> level and associated exception.
    /// </summary>
    /// <param name="EventText">The string message to describe the log entry</param>      
    /// <param name="objects">Array of objects to be serialized and embeded in the message template 
    /// e.g. WriteLogEntry("Record 1:{@Record1}, Record 2:{@Record2}", new object[] { Record1, Record2 }</param>
    public void LogWarning(string EventText, object[] objects)
    {
        string logMessage = SerializeObjects((EventText, objects));
        LogWarning(logMessage);
    }

    /// <summary>
    /// Write a log event with the <see cref="LogEventLevel.Fatal"/> level and associated exception.
    /// </summary>
    /// <param name="EventText">The string message to describe the log entry</param>
    public void LogFatal(string EventText)
    {
        WriteLog(EventText, LogEventLevel.Warning);
    }
    /// <summary>
    /// Write a log event with the <see cref="LogEventLevel.Fatal"/> level and associated exception.
    /// </summary>
    /// <param name="EventText">The string message to describe the log entry</param>    
    /// <param name="objects">Array of objects to be serialized and embeded in the message template 
    /// e.g. WriteLogEntry("Record 1:{@Record1}, Record 2:{@Record2}", new object[] { Record1, Record2 }</param>
    public void LogFatal(string EventText, object[] objects)
    {
        string logMessage = SerializeObjects((EventText, objects));
        LogFatal(logMessage);
    }

    private string SerializeObjects((string, object[]) records)
    {
        try
        {
            var (message, objects) = records;
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Formatting = Formatting.Indented
            };
            var tokens = Utils.GetTokensFromString(message, "{", "}", true);
            for (int i = 0; i < objects.Length; i++)
            {
                string serializedRecord = JsonConvert.SerializeObject(objects[i], settings);
                message = message.Replace(tokens[i], serializedRecord);
            }
            return message;
        }
        catch (Exception)
        {
            return "";
        }
    }
    static readonly string[][] _titleCaseLevelMap =
       [
        ["V", "Vb", "Vrb", "Verb", "Verbo", "Verbos", "Verbose"],
        ["D", "De", "Dbg", "Dbug", "Debug"],
        ["I", "In", "Inf", "Info", "Infor", "Inform", "Informa", "Informat", "Informati", "Informatio", "Information"],
        ["W", "Wn", "Wrn", "Warn", "Warni", "Warnin", "Warning"],
        ["E", "Er", "Err", "Eror", "Error"],
        ["F", "Fa", "Ftl", "Fatl", "Fatal"]
       ];

    static readonly string[][] _lowerCaseLevelMap =
    [
        ["v", "vb", "vrb", "verb", "verbo", "verbos", "verbose"],
        ["d", "de", "dbg", "dbug", "debug"],
        ["i", "in", "inf", "info", "infor", "inform", "informa", "informat", "informati", "informatio", "information"],
        ["w", "wn", "wrn", "warn", "warni", "warnin", "warning"],
        ["e", "er", "err", "eror", "error"],
        ["f", "fa", "ftl", "fatl", "fatal"]
    ];

    static readonly string[][] _upperCaseLevelMap =
    [
        ["V", "VB", "VRB", "VERB", "VERBO", "VERBOS", "VERBOSE"],
        ["D", "DE", "DBG", "DBUG", "DEBUG"],
        ["I", "IN", "INF", "INFO", "INFOR", "INFORM", "INFORMA", "INFORMAT", "INFORMATI", "INFORMATIO", "INFORMATION"],
        ["W", "WN", "WRN", "WARN", "WARNI", "WARNIN", "WARNING"],
        ["E", "ER", "ERR", "EROR", "ERROR"],
        ["F", "FA", "FTL", "FATL", "FATAL"]
    ];
    internal static string GetLevelMoniker(LogEventLevel value, string? format = null)
    {
        // handle unknown LogEventLevel
        if (value is < 0 or > LogEventLevel.Fatal)
            return Casing.Format(value.ToString(), format);

        if (format == null || format.Length != 2 && format.Length != 3)
            return Casing.Format(GetLevelMoniker(_titleCaseLevelMap, value), format);

        // Using int.Parse() here requires allocating a string to exclude the first character prefix.
        // Junk like "wxy" will be accepted but produce benign results.
        var width = format[1] - '0';
        if (format.Length == 3)
        {
            width *= 10;
            width += format[2] - '0';
        }

        if (width < 1)
            return string.Empty;

        return format[0] switch
        {
            'w' => GetLevelMoniker(_lowerCaseLevelMap, value, width),
            'u' => GetLevelMoniker(_upperCaseLevelMap, value, width),
            't' => GetLevelMoniker(_titleCaseLevelMap, value, width),
            _ => Casing.Format(GetLevelMoniker(_titleCaseLevelMap, value), format)
        };
    }
    internal static string GetLevelMoniker(string[][] caseLevelMap, LogEventLevel level, int width)
    {
        var caseLevel = caseLevelMap[(int)level];
        return caseLevel[Math.Min(width, caseLevel.Length) - 1];
    }
    internal static string GetLevelMoniker(string[][] caseLevelMap, LogEventLevel level)
    {
        var caseLevel = caseLevelMap[(int)level];
        return caseLevel[caseLevel.Length - 1];
    }

    #region LogFile rolling

    private DateTime _nextCheckpoint;
    private DateTime GetNextCheckpoint(DateTime instant)
    {
        return LogRolloverInterval.HasValue ? LogRolloverInterval.Value.GetNextCheckpoint(instant) ?? DateTime.MaxValue : DateTime.MaxValue;
    }
    private void RolloverLogFile()
    {
        FileInfo logFile = new FileInfo(CurrentLogFilePath);

        if (logFile.Exists && (LogRolloverInterval != RollingInterval.Infinite && logFile.Length >= MaxLogFileSize) || DateTime.Now >= _nextCheckpoint)
        {
            string directory = logFile.DirectoryName;
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(CurrentLogFilePath);
            string extension = logFile.Extension;
            string timestamp = DateTime.Now.ToString(LogRolloverInterval.HasValue ? LogRolloverInterval.Value.GetFormat() : RollingInterval.Minute.ToString());
            string newFileName = $"{fileNameWithoutExtension}_{timestamp}{extension}";
            string newFilePath = Path.Combine(directory, RolloverSubfolderName, newFileName);

            try
            {
                Directory.CreateDirectory(Path.Combine(directory, RolloverSubfolderName));
                logFile.MoveTo(newFilePath);
                //using (File.Create(CurrentLogFilePath)) { }
                _nextCheckpoint = GetNextCheckpoint(DateTime.Now);
                Console.WriteLine($"Log file rolled over to: {newFilePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during log file rollover: {ex.Message}");
            }
        }
    }

    #endregion
}

