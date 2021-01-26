using System.IO;
using System.Reflection;

namespace AGenius.UsefulStuff
{
    /// <summary>
    /// This is a very useful class that will keep your temporary files under control
    /// </summary> 
    public static partial class TemporaryFiles
    {
        private const string UserFilesListFilenamePrefix = ".used-temporary-files.txt";
        static private readonly object UsedFilesListLock = new object();
        private static string GetUsedFilesListFilename()
        {
            return Assembly.GetEntryAssembly().Location + UserFilesListFilenamePrefix;
        }
        public static void AddToUsedFilesList(string filename)
        {
            lock (UsedFilesListLock)
            {
                using (var writer = File.AppendText(GetUsedFilesListFilename()))
                    writer.WriteLine(filename);
            }
        }
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
