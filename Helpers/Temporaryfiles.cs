using System;
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
        private static readonly object UsedFilesListLock = new object();
            private static string _filesListPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
        /// <summary>
        /// Exposed property to allow the override of the location the temp file list file is stored.
        /// </summary>
        /// <remarks>The default will be the same location as the EXE but this might be a read only location So overide this value to specify the alternate.</remarks>        
        public static string FilesListPath
        {
            get => _filesListPath;
            set => _filesListPath = value ?? throw new ArgumentNullException(nameof(value));
        }
        private static string GetUsedFilesListFilename()
        {
            return Path.Combine(FilesListPath, $"{Assembly.GetEntryAssembly().GetName().Name}{UserFilesListFilenamePrefix}");
        }
        /// <summary>Add a manually created file name to the list</summary>
        /// <param name="filename">The file name (full path)</param>
        public static void AddToUsedFilesList(string filename)
        {
            lock (UsedFilesListLock)
            {
                using (var writer = File.AppendText(GetUsedFilesListFilename()))
                {
                    writer.WriteLine(filename);
                }
            }
        }
        /// <summary>Return a new Temporary file and record it </summary>
        /// <param name="extension">The Extension of the new file</param>
        /// <param name="subFolder">Place new temp files in this sub folder in the Temp directory</param>
        /// <returns>New string file name and extension</returns>

        public static string GetNew(string extension = ".tmp", string subFolder = "")
        {
            string fileName = GenerateUniqueFileName(extension, subFolder);
            AddToUsedFilesList(fileName);
            return fileName;
        }
        /// <summary>Return a path to a temporary file using the name supplied in the temporary folder and record it </summary>
        /// <param name="requiredFileName">The FileName required</param>
        /// <param name="extension">The Extension of the new file</param>
        /// <param name="subFolder">Place new temp files in this sub folder in the Temp directory</param>
        /// <returns>New string file name and extension</returns>
        public static string GetNewAlt(string requiredFileName, string extension = ".tmp", string subFolder = "")
        {
            string fileName = Path.Combine(Path.GetTempPath(), subFolder, Path.ChangeExtension(requiredFileName, extension));
            EnsureSubfolderExists(subFolder);

            int attempt = 0;
            while (true)
            {
                try
                {
                    using (new FileStream(fileName, FileMode.CreateNew)) { }
                    File.Delete(fileName); // Delete the file to keep it truly unique
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
                string usedFilesListFilename = GetUsedFilesListFilename();

                if (!File.Exists(usedFilesListFilename))
                    return;

                try
                {
                    using (StreamReader reader = new StreamReader(File.Open(usedFilesListFilename, FileMode.Open)))
                    {
                        string tempFileToDelete;
                        while ((tempFileToDelete = reader.ReadLine()) != null)
                        {
                            if (File.Exists(tempFileToDelete))
                                File.Delete(tempFileToDelete);
                        }
                    }
                    // Clean up
                    File.WriteAllText(usedFilesListFilename, string.Empty); // Truncate the file
                }
                catch (Exception ex)
                {
                    // If it errors then the file is probably open. lets just skip and leave to tidy up next time                    
                    Console.WriteLine($"An error occurred while deleting previously used files: {ex.Message}");
                }
            }
        }
        private static void EnsureSubfolderExists(string subFolder)
        {
            if (!string.IsNullOrEmpty(subFolder))
            {
                string folderPath = Path.Combine(Path.GetTempPath(), subFolder);
                Directory.CreateDirectory(folderPath);
            }
        }

        private static string GenerateUniqueFileName(string extension, string subFolder)
        {
            int attempt = 0;
            while (true)
            {
                string fileName = Path.Combine(Path.GetTempPath(), subFolder, Path.ChangeExtension(Path.GetRandomFileName(), extension));
                EnsureSubfolderExists(subFolder);

                try
                {
                    using (new FileStream(fileName, FileMode.CreateNew)) { }
                    File.Delete(fileName); // Delete the file to keep it truly unique
                    return fileName;
                }
                catch (IOException ex)
                {
                    if (++attempt == 20)
                        throw new IOException("No unique temporary file name is available.", ex);
                }
            }
        }
    }
}
