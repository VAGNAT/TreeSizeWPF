using FileSystem.Interfaces;
using FileSystem.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FileSystem
{
    public class ContentOperations : IContentOperations
    {
        private readonly IDriveRepository _driveRepository;
        private readonly IFolderRepository _folderRepository;
        private readonly IFileRepository _fileRepository;
        private readonly IInaccessibleRepository _inaccessibleRepository;
        private readonly Dictionary<SizeType, string> _valueSize = new Dictionary<SizeType, string>
        {
            {SizeType.Kilobytes, "Kb" },
            {SizeType.Megabytes, "Mb" },
            {SizeType.Gigabytes, "Gb" }
        };
        public ContentOperations(IDriveRepository driveRepository, IFolderRepository folderRepository, IFileRepository fileRepository,
            IInaccessibleRepository inaccessibleRepository)
        {
            _driveRepository = driveRepository;
            _folderRepository = folderRepository;
            _fileRepository = fileRepository;
            _inaccessibleRepository = inaccessibleRepository;
        }

        public List<DataItem> GetDrives()
        {
            return _driveRepository.GetDrivesCollection();
        }

        public List<DataItem> GetFolders(string fullPath)
        {
            _inaccessibleRepository.CreateInaccessibleFolders(fullPath);

            List<DataItem> content = new List<DataItem>();
            string[] dirs = _folderRepository.GetFoldersCollection(fullPath);

            if (dirs.Length > 0)
            {
                content.AddRange(dirs.Select(dir => new DataItem
                {
                    FullPath = dir,
                    Name = _folderRepository.GetFolderName(dir),
                    Type = DataType.Folder,
                    Inaccessible = _inaccessibleRepository.CheckInaccessible(dir)
                }));
            }
            return content;
        }
        public List<DataItem> GetFiles(string fullPath)
        {
            _inaccessibleRepository.CreateInaccessibleFiles(fullPath);

            List<DataItem> content = new List<DataItem>();
            string[] files = _fileRepository.GetFilesCollection(fullPath);
            if (files.Length > 0)
            {
                content.AddRange(files.Select(file => new DataItem
                {
                    FullPath = file,
                    Name = _fileRepository.GetFileName(file),
                    Type = DataType.File,
                    Inaccessible = _inaccessibleRepository.CheckInaccessible(file)
                }));
            }
            return content;
        }

        public async Task<long> GetFolderSizeAsync(string fullPath, CancellationToken cts)
        {
            try
            {
                return await Task.Run(() => _folderRepository.GetFolderSize(fullPath, cts), cts);
            }
            catch (OperationCanceledException)
            {
                return 0;
            }
        }

        public async Task<long> GetFileSizeAsync(string fullPath, CancellationToken cts)
        {
            try
            {
                return await Task.Run(() => _fileRepository.GetFileSize(fullPath, cts), cts);
            }
            catch (OperationCanceledException)
            {
                return 0;
            }
        }

        public IEnumerable<SizeType> GetTypesSize()
        {
            return Enum.GetValues(typeof(SizeType)).Cast<SizeType>();
        }

        public string GetSizeRepresentation(long size, SizeType type) =>
            " Size - " + (Convert.ToDecimal(size) / (int)type).ToString("N") + " " + _valueSize[type];

        public string GetSizeRepresentationDrive(long size, SizeType type) =>
            " Total size - " + (Convert.ToDecimal(size) / (int)type).ToString("N") + " " + _valueSize[type];

        public string GetSizeRepresentationInaccessibleItem() => " DENIED ACCESS";
    }
}
