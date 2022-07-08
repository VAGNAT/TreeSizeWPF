using System.Collections.Generic;

namespace FileSystem.Model
{
    public class DataItem
    {
        public DataType Type { get; set; }
        public string FullPath { get; set; }
        public string Name { get; set; }
        public long Size { get; set; }
        public bool Inaccessible { get; set; }

        public override bool Equals(object obj)
        {
            return obj is DataItem item &&
                   Type == item.Type &&
                   FullPath == item.FullPath &&
                   Name == item.Name;
        }

        public override int GetHashCode()
        {
            int hashCode = 253345922;
            hashCode = hashCode * -1521134295 + Type.GetHashCode();            
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(FullPath);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            return hashCode;
        }
    }
}
