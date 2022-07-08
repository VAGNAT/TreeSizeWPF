using System.Threading;

namespace FileSystem.Interfaces
{
    public interface IFolderRepository
    {
        string[] GetFoldersCollection(string fullPath);
        string GetFolderName(string fullPath);
        long GetFolderSize(string fullPath, CancellationToken cts);
    }
}
