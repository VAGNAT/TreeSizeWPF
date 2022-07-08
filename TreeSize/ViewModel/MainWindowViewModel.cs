using FileSystem.Interfaces;
using FileSystem.Model;
using GalaSoft.MvvmLight;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace TreeSize.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly IContentOperations _contentOperations;
        private ObservableCollection<DataItemViewModel> _items;
        private ObservableCollection<SizeType> _sizeContent;
        private SizeType _sizeContentSelected;
        public ObservableCollection<DataItemViewModel> Items => _items;
        public ObservableCollection<SizeType> SizeContent => _sizeContent;
        public SizeType SizeContentSelected
        {
            get => _sizeContentSelected;

            set
            {
                _sizeContentSelected = value;
                ChangeSize(_items);
            }
        }

        private void ChangeSize(ObservableCollection<DataItemViewModel> children)
        {
            if (children is null)
            {
                return;
            }                
            foreach (var item in children)
            {
                if (item is null)
                {
                    break;
                }
                item.SizeType = _sizeContentSelected;
                ChangeSize(item.Children);
            }
        }

        public MainWindowViewModel(IContentOperations contentOperations)
        {
            _contentOperations = contentOperations ?? throw new ArgumentNullException(nameof(contentOperations));
            _items = new ObservableCollection<DataItemViewModel>(
                _contentOperations.GetDrives().Select(di => 
                new DataItemViewModel(_contentOperations, di) { SizeNumber = di.Size }));
            _sizeContent = new ObservableCollection<SizeType>(_contentOperations.GetTypesSize());
        }
    }
}
