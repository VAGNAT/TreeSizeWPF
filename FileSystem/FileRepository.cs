using FileSystem.Interfaces;
using System.IO;
using System.Threading;

namespace FileSystem
{
    public class FileRepository : IFileRepository
    {
        public string GetFileName(string fullPath)
        {
            if (string.IsNullOrEmpty(fullPath))
            {
                return string.Empty;
            }

            var normalizedPath = fullPath.Replace('/', '\\');
            var lastIndex = normalizedPath.LastIndexOf('\\');

            if (lastIndex <= 0)
            {
                return fullPath;
            }

            return fullPath.Substring(lastIndex + 1);
        }

        public string[] GetFilesCollection(string fullPath)
        {
            string[] files = Directory.GetFiles(fullPath);            

            return files;
        }               

        public long GetFileSize(string fullPath, CancellationToken cts)
        {
            if (cts.IsCancellationRequested)
                cts.ThrowIfCancellationRequested();

            FileInfo fileInf = new FileInfo(fullPath);
            long fileSize;
                        
            fileSize = fileInf.Length;                        
            
            return fileSize;            
        }
    }
}
