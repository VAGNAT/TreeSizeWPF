namespace FileSystem.Interfaces
{
    public interface IInaccessibleRepository
    {        
        void CreateInaccessibleFolders(string fullPath);
        void CreateInaccessibleFiles(string fullPath);
        bool CheckInaccessible(string fullPath);
    }
}
