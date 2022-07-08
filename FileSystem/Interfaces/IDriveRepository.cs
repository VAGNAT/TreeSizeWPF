using FileSystem.Model;
using System.Collections.Generic;

namespace FileSystem.Interfaces
{
    public interface IDriveRepository
    {
        List<DataItem> GetDrivesCollection();        
    }
}
