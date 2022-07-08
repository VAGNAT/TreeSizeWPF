using FileSystem.Model;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FileSystem.Interfaces
{
    public interface IContentOperations
    {
        List<DataItem> GetDrives();
        List<DataItem> GetFolders(string fullPath);
        List<DataItem> GetFiles(string fullPath);
        IEnumerable<SizeType> GetTypesSize();
        Task<long> GetFolderSizeAsync(string fullPath, CancellationToken cts);
        Task<long> GetFileSizeAsync(string fullPath, CancellationToken cts);
        string GetSizeRepresentation(long size, SizeType type);
        string GetSizeRepresentationDrive(long size, SizeType type);
        string GetSizeRepresentationInaccessibleItem();
    }
}
