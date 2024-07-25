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
public class Logger
{
    internal string _logPath { get; set; }
    internal string _logFileName { get; set; }
    internal Dictionary<string, string> _appliedSettings = new Dictionary<string, string>();
    /// <summary>
    /// Exposes the logger settings provides from the appSettings section (if applicable)
    /// </summary>
    public Dictionary<string, string> AppliedSettings { get { return _appliedSettings; } }
    bool _loggerCreated;
    string _settingPrefix;
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
    /// <param name="RolloverInterval">Sepcify the interval the file should rollover <see cref="RollingInterval"/></param>
    /// <param name="RolloverSubFolder">Move rolled over log files to the sub folder specified</param>
    public Logger(string LogPath = null,
        bool AddTimeStamp = true,
        int MaxLogFileSize = 1024000,
        LogEventLevel logEventLevel = LogEventLevel.Information,
        int LogLevelChars = 3,
        string TimeStampFormat = "dd/MM/yyyy HH:mm:ss zzz",
        bool AddNewLogHeader = true,
        RollingInterval RolloverInterval = RollingInterval.Infinite,
        string RolloverSubFolder = "")
    {
        this.CurrentLogFilePath = LogPath;
        this.AddTimeStamp = AddTimeStamp;
        this.MaxLogFileSize = MaxLogFileSize;
        this.LogLevel = logEventLevel;
        this.LogLevelChars = LogLevelChars;
        this.TimeStampFormat = TimeStampFormat;
        this.AddNewLogHeader = AddNewLogHeader;
        this.LogRolloverInterval = RolloverInterval;
        this.RolloverSubfolderName = RolloverSubFolder;
        _nextCheckpoint = GetNextCheckpoint(DateTime.Now);
    }
    /// <summary>
    /// Create an instance of the AGLogger
    /// </summary>
    /// <remarks>Apply settings from app.config</remarks>
    public Logger()
    {
        _nextCheckpoint = GetNextCheckpoint(DateTime.Now);
    }
    /// <summary>
    /// Create an instance of the AGLogger
    /// </summary>
    /// <param name="settingPrefix">override the prefix for the settings - default:"aglog:" </param>
    /// <remarks>Apply settings from app.config</remarks>
    public Logger CreateLogger(string settingPrefix = "aglog")
    {
        if (_loggerCreated) throw new InvalidOperationException("CreateAGLogger() was previously called and can only be called once.");
        _loggerCreated = true;
        _settingPrefix = settingPrefix is null ? "aglog:" : $"{settingPrefix}:".ToLower();

        // Enumerate all settings found 
        IEnumerable<KeyValuePair<string, string>> settings = ConfigurationManager.AppSettings.AllKeys.Select(k => new KeyValuePair<string, string>(k, ConfigurationManager.AppSettings[k]!));
        var pairs = settings
               .Where(k => k.Key.ToLower().StartsWith(_settingPrefix.ToLower()))
               .Select(k => new KeyValuePair<string, string>(
                   k.Key.ToLower().Substring(_settingPrefix.Length),
                   Environment.ExpandEnvironmentVariables(k.Value)));

        _appliedSettings = new Dictionary<string, string>();

        string _logFileWithPath = null;
        int _maxLogFileSize = 1024000;
        LogEventLevel _logLevel = LogEventLevel.Information;
        int _logLevelChars = 3;
        bool _addTimeStamp = true;
        string _timeStampFormat = null;
        RollingInterval _logRolloverInterval = RollingInterval.Infinite;
        string _rolloverSubfolderName = null;
        bool _addheader = true;

        foreach (var kvp in pairs)
        {
            _appliedSettings[kvp.Key] = kvp.Value;
            switch (kvp.Key)
            {
                case "file.path":
                    if (string.IsNullOrEmpty(kvp.Value))
                    {
                        string path = Path.Combine(Utils.ApplicationPath, "Application_Log.log");

                        if (Environment.ExpandEnvironmentVariables(Path.GetDirectoryName(path)).StartsWith(@"\"))
                        {
                            path = Path.Combine(Utils.ApplicationPath, path.TrimStart(Path.DirectorySeparatorChar));
                        }

                        _logFileWithPath += path;
                    }
                    break;

                case "file.sizelimitbytes":
                    if (!string.IsNullOrEmpty(kvp.Value))
                    {
                        _maxLogFileSize = int.Parse(kvp.Value);
                    }
                    break;

                case "file.addheader":
                    if (!string.IsNullOrEmpty(kvp.Value))
                    {
                        _addheader = bool.Parse(kvp.Value);
                    }
                    break;

                case "minimum-level":
                    if (!string.IsNullOrEmpty(kvp.Value))
                    {
                        _logLevel = kvp.Value.ToEnum<LogEventLevel>();
                    }
                    break;

                case "level.length":
                    if (!string.IsNullOrEmpty(kvp.Value))
                    {
                        _logLevelChars = int.Parse(kvp.Value);
                    }
                    break;

                case "timestamp.visible":
                    if (!string.IsNullOrEmpty(kvp.Value))
                    {
                        _addTimeStamp = bool.Parse(kvp.Value);
                    }
                    else
                    {
                        _addTimeStamp = true;
                    }
                    break;

                case "timestamp.format":
                    if (!string.IsNullOrEmpty(kvp.Value))
                    {
                        _timeStampFormat = kvp.Value;
                    }
                    break;

                case "rollover.interval":
                    if (!string.IsNullOrEmpty(kvp.Value))
                    {
                        _logRolloverInterval = kvp.Value.ToEnum(RollingInterval.Infinite, true);
                    }
                    break;

                case "rollover.subfolder":
                    if (!string.IsNullOrEmpty(kvp.Value))
                    {
                        _rolloverSubfolderName = kvp.Value;
                    }
                    break;

            }
        }
        var newLogger = new Logger(LogPath: _logFileWithPath,
            AddTimeStamp: _addTimeStamp,
            MaxLogFileSize: _maxLogFileSize,
            logEventLevel: _logLevel,
            LogLevelChars: _logLevelChars,
            TimeStampFormat: _timeStampFormat,
            AddNewLogHeader: _addheader,
            RolloverInterval: _logRolloverInterval,
            RolloverSubFolder: _rolloverSubfolderName

            );
        newLogger._appliedSettings = _appliedSettings;
        return newLogger;
    }
    /// <summary>Write to a text log file.</summary>
    /// <param name="EventText">The string message to write</param>
    /// <param name="logLevel">The event level of the entry</param>        
    /// <param name="appendNewLine">Automatically add a new line after the message</param>  
    /// <param name="appendTimeStamp">Prefix event text with timestamp</param>
    public void WriteLog(string EventText, LogEventLevel? logLevel = null, bool appendNewLine = true, bool? appendTimeStamp = null)
    {
        try
        {
            logLevel ??= this.LogLevel;
            CurrentLogFilePath ??= Path.Combine(Utils.ApplicationPath, "Logfile.log"); // Default path and name
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
            //Allow single event override timestamp entry (usfull for appending text to an already logged event
            bool _timeStamp = AddTimeStamp;
            if (appendTimeStamp.HasValue)
            {
                _timeStamp = appendTimeStamp.Value;
            }
            if (_timeStamp)
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
    /// Allow Override of appending new line and time stamp
    /// </summary>
    /// <param name="EventText">The string message to describe the log entry</param>
    /// <param name="appendNewLine">Automatically add a new line after the message</param>  
    /// <param name="appendTimeStamp">Prefix event text with timestamp</param>
    public void LogInfo(string EventText, bool appendNewLine = true, bool appendTimeStamp = true)
    {
        WriteLog(EventText, LogEventLevel.Information, appendNewLine, appendTimeStamp);
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
    internal static string GetLevelMoniker(LogEventLevel value, string format = null)
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

