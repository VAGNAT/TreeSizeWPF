using FileSystem.Interfaces;
using System.IO;
using System.Threading;

namespace FileSystem
{
    public class FolderRepository : IFolderRepository
    {
        private readonly IInaccessibleRepository _inaccessibleRepository;

        public FolderRepository(IInaccessibleRepository inaccessibleRepository)
        {
            _inaccessibleRepository = inaccessibleRepository;
        }
        public string GetFolderName(string fullPath)
        {
            if (string.IsNullOrEmpty(fullPath))
            {
                return string.Empty;
            }

            string normalizedPath = fullPath.Replace('/', '\\');
            int lastIndex = normalizedPath.LastIndexOf('\\');

            if (lastIndex <= 0)
            {
                return fullPath;
            }

            return fullPath.Substring(lastIndex + 1);
        }

        public string[] GetFoldersCollection(string fullPath)
        {
            string[] dir = Directory.GetDirectories(fullPath);

            return dir;
        }

        public long GetFolderSize(string fullPath, CancellationToken cts)
        {
            if (cts.IsCancellationRequested)
                cts.ThrowIfCancellationRequested();

            _inaccessibleRepository.CreateInaccessibleFolders(fullPath);

            if (_inaccessibleRepository.CheckInaccessible(fullPath))
            {
                return 0;
            }

            long Size = 0;
            string[] files;
            files = Directory.GetFiles(fullPath);
            _inaccessibleRepository.CreateInaccessibleFiles(fullPath);
            foreach (string file in files)
            {
                if (_inaccessibleRepository.CheckInaccessible(file))
                {
                    continue;
                }
                FileInfo fileInf = new FileInfo(file);
                Size += fileInf.Length;
            }

            string[] subDirectoryes = GetFoldersCollection(fullPath);
            foreach (string subDir in subDirectoryes)
            {
                Size += GetFolderSize(subDir, cts);
            }
            return Size;
        }

    }
}
