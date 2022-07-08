using System.Threading;

namespace FileSystem.Interfaces
{
    public interface IFileRepository
    {
        string[] GetFilesCollection(string fullPath);
        string GetFileName(string fullPath);
        long GetFileSize(string fullPath, CancellationToken cts);        
    }
}
