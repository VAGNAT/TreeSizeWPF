using FileSystem.Interfaces;
using FileSystem.Model;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FileSystem
{
    public class DriveRepository : IDriveRepository
    {
        public List<DataItem> GetDrivesCollection()
        {            
            return DriveInfo.GetDrives().Select(drive => new DataItem
            {
                FullPath = drive.Name,
                Name = drive.IsReady ? drive.Name + " (" + drive.VolumeLabel + ")" : drive.Name,
                Type = DataType.Drive,
                Size = drive.IsReady ? drive.TotalSize : 0,
                Inaccessible = !drive.IsReady
            }).ToList();
        }
    }
}
