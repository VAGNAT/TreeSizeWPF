using FileSystem.Interfaces;
using FileSystem.Model;
using GalaSoft.MvvmLight;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TreeSize.ViewModel
{
    public class DataItemViewModel : ViewModelBase
    {
        private static SizeType _sizeType;
        private readonly IContentOperations _contentOperations;
        private readonly CancellationTokenSource _cts;
        private readonly bool _inaccessible;
        private string _size;
        private long _sizeNumber;

        public DataType Type { get; set; }
        public string FullPath { get; set; }
        public string Name { get; set; }
        public bool Inaccessible => _inaccessible;
        public long SizeNumber { get => _sizeNumber; set => _sizeNumber = value; }
        public ObservableCollection<DataItemViewModel> Children { get; set; }
        public string Size
        {
            get => _size;
            set
            {
                _size = value;
                RaisePropertyChanged(nameof(Size));
            }
        }

        public bool IsExpanded
        {
            get => Children?.Count(f => f != null) > 0;
            set
            {
                if (value)
                {
                    Expand();
                }
                else
                {
                    CancelSetupSize();
                    SetupChildren();
                }
                RaisePropertyChanged(nameof(Children));
            }
        }

        public SizeType SizeType
        {
            get
            {
                if (_sizeType == 0)
                    _sizeType = SizeType.Kilobytes;
                return _sizeType;
            }

            set
            {
                _sizeType = value;

                if (_inaccessible)
                {
                    Size = _contentOperations.GetSizeRepresentationInaccessibleItem();
                }
                else if (Type == DataType.Drive)
                {
                    Size = _contentOperations.GetSizeRepresentationDrive(_sizeNumber, _sizeType);
                }
                else
                {
                    Size = _contentOperations.GetSizeRepresentation(_sizeNumber, _sizeType);
                }
            }
        }

        public DataItemViewModel(IContentOperations contentOperations, DataItem dataItem)
        {
            _contentOperations = contentOperations;
            _inaccessible = dataItem.Inaccessible;
            _cts = new CancellationTokenSource();
            FullPath = dataItem.FullPath;
            Name = dataItem.Name;
            Type = dataItem.Type;
            SetupChildren();
        }

        private void Expand()
        {
            List<DataItem> children = _contentOperations.GetFolders(FullPath);
            children.AddRange(_contentOperations.GetFiles(FullPath));

            Children = new ObservableCollection<DataItemViewModel>(children.Select(content => new DataItemViewModel(_contentOperations, content)));
            
            foreach (var child in Children)
            {
                if (child.Inaccessible)
                {
                    child.Size = _contentOperations.GetSizeRepresentationInaccessibleItem();
                    continue;
                }
                SetupSize(child);
            }
        }

        private void SetupChildren()
        {
            Children = new ObservableCollection<DataItemViewModel>();

            //Если убрать, тогда не виден флажок для раскрытия папки или диска
            if (Type != DataType.File && !_inaccessible)
            {
                Children.Add(null);
            }
        }

        private async Task SetupSize(DataItemViewModel child)
        {
            if (child.Type == DataType.File)
            {
                child.SizeNumber = await _contentOperations.GetFileSizeAsync(child.FullPath, child._cts.Token);
            }
            else if (child.Type == DataType.Folder)
            {
                child.SizeNumber = await _contentOperations.GetFolderSizeAsync(child.FullPath, child._cts.Token);
            }
            
            child.Size = _contentOperations.GetSizeRepresentation(child.SizeNumber, SizeType);
        }        

        private void CancelSetupSize()
        {
            foreach (var child in Children)
            {
                child._cts.Cancel();
                child._cts.Dispose();
            }
        }

        public override bool Equals(object obj)
        {
            return obj is DataItemViewModel model &&
                   Type == model.Type &&
                   FullPath == model.FullPath &&
                   Name == model.Name &&
                   SizeType == model.SizeType;
        }

        public override int GetHashCode()
        {
            int hashCode = 762698412;
            hashCode = hashCode * -1521134295 + Type.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(FullPath);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            hashCode = hashCode * -1521134295 + SizeType.GetHashCode();
            return hashCode;
        }
    }
}
