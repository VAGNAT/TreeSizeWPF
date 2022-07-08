using FileSystem.Interfaces;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;

namespace FileSystem
{
    public class InaccessibleRepository : IInaccessibleRepository
    {
        private static BlockingCollection<string> _listInaccessible;

        public InaccessibleRepository()
        {
            _listInaccessible = new BlockingCollection<string>();
        }

        public void CreateInaccessibleFolders(string fullPath)
        {
            if (CheckInaccessible(fullPath))
            {
                return;
            }
            try
            {
                string[] dirs = Directory.GetDirectories(fullPath);
                foreach (string dir in dirs)
                {
                    if (CheckInaccessible(dir))
                    {
                        return;
                    }
                    DirectorySecurity DS = Directory.GetAccessControl(dir);
                    var rules = DS.GetAccessRules(true, true, typeof(SecurityIdentifier));
                    if (rules == null)
                    {
                        _listInaccessible.Add(dir);
                        continue;
                    }
                    foreach (FileSystemAccessRule rule in rules)
                    {
                        if ((rule.FileSystemRights & FileSystemRights.Read | FileSystemRights.Write) == 0 || rule.AccessControlType == AccessControlType.Deny)
                        {
                            //WriteAccess = true;
                            _listInaccessible.Add(dir);
                            break;
                        }
                    }
                }
            }
            catch (UnauthorizedAccessException)
            {
                _listInaccessible.Add(fullPath);
            }
        }


        public void CreateInaccessibleFiles(string fullPath)
        {
            if (CheckInaccessible(fullPath))
            {
                return;
            }
            string[] files = Directory.GetFiles(fullPath);
            foreach (string file in files)
            {
                FileAttributes fileAttributes = new FileInfo(file).Attributes;
                if (((fileAttributes & FileAttributes.NotContentIndexed) == FileAttributes.NotContentIndexed && !_listInaccessible.Contains(file)) || fileAttributes < 0)
                {
                    _listInaccessible.Add(file);
                }
            }
        }

        public bool CheckInaccessible(string fullPath)
        {
            return _listInaccessible.Contains(fullPath);
        }
    }
}
