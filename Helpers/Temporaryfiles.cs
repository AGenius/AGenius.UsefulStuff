using System.IO;
using System.Reflection;

namespace AGenius.UsefulStuff.Helpers
{
    /// <summary>
    /// This is a very useful class that will keep your temporary files under control
    /// </summary> 
    public static partial class TemporaryFiles
    {
        private const string UserFilesListFilenamePrefix = ".used-temporary-files.txt";
        static private readonly object UsedFilesListLock = new object();
        /// <summary>
        /// Exposed property to allow the override of the location the temp file list file is stored.
        /// </summary>
        /// <remarks>The default will be the same location as the EXE but this might be a read only location So overide this value to specify the alternate.</remarks>
        public static string filesListPath { get; set; } = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
        private static string GetUsedFilesListFilename()
        {
            return $"{filesListPath}\\{Assembly.GetEntryAssembly().GetName().Name}{UserFilesListFilenamePrefix}";        
        }
        /// <summary>Add a manually created file name to the list</summary>
        /// <param name="filename">The file name (full path)</param>
        public static void AddToUsedFilesList(string filename)
        {
            lock (UsedFilesListLock)
            {
                using (var writer = File.AppendText(GetUsedFilesListFilename()))
                    writer.WriteLine(filename);
            }
        }
        /// <summary>Return a new Temporary file and record it </summary>
        /// <param name="extension">The Extension of the new file</param>
        /// <param name="subFolder">Place new temp files in this sub folder in the Temp directory</param>
        /// <returns>New string file name and extension</returns>
        public static string GetNew(string extension = ".tmp", string subFolder = "")
        {
            string fileName;
            int attempt = 0;
            while (true)
            {
                fileName = Path.GetRandomFileName();
                fileName = Path.ChangeExtension(fileName, extension);
                fileName = Path.Combine(Path.GetTempPath(), subFolder, fileName);
                if (!string.IsNullOrEmpty(subFolder))
                {
                    Directory.CreateDirectory($"{Path.GetTempPath()}\\{subFolder}");
                }
                try
                {
                    using (new FileStream(fileName, FileMode.CreateNew))
                    {
                    }

                    break;
                }
                catch (IOException ex)
                {
                    if (++attempt == 20)
                        throw new IOException("No unique temporary file name is available.", ex);
                }
            }

            AddToUsedFilesList(fileName);
            return fileName;
        }

        /// <summary>Delete all previously recorded temporary files</summary>
        public static void DeleteAllPreviouslyUsed()
        {
            lock (UsedFilesListLock)
            {
                var usedFilesListFilename = GetUsedFilesListFilename();

                if (!File.Exists(usedFilesListFilename))
                    return;
                try
                {
                    using (var listFile = File.Open(usedFilesListFilename, FileMode.Open))
                    {
                        using (var reader = new StreamReader(listFile))
                        {
                            string tempFileToDelete;
                            while ((tempFileToDelete = reader.ReadLine()) != null)
                            {
                                if (File.Exists(tempFileToDelete))
                                    File.Delete(tempFileToDelete);
                            }
                        }
                    }

                    // Clean up
                    using (File.Open(usedFilesListFilename, FileMode.Truncate)) { }
                }
                catch (System.Exception)
                {
                    // If it errors then the file is probably open. lets just skip and leave to tidy up next time                    
                }

            }
        }
    }
}
