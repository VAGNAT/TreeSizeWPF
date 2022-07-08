using CommonServiceLocator;
using FileSystem;
using FileSystem.Interfaces;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;

namespace TreeSize.ViewModel
{
    
    public class ViewModelLocator
    {
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
                        
            SimpleIoc.Default.Register<IDriveRepository, DriveRepository>();
            SimpleIoc.Default.Register<IFolderRepository, FolderRepository>();
            SimpleIoc.Default.Register<IFileRepository, FileRepository>();
            SimpleIoc.Default.Register<IContentOperations, ContentOperations>();
            SimpleIoc.Default.Register<IInaccessibleRepository, InaccessibleRepository>();
            SimpleIoc.Default.Register<MainWindowViewModel>();
            
        }

        public MainWindowViewModel Main
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MainWindowViewModel>();
            }
        }
        
        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}