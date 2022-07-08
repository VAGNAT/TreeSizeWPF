using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using Moq;
using FileSystem.Interfaces;
using FileSystem.Model;

namespace TreeSize.ViewModel.Tests
{
    [TestClass()]
    public class MainWindowViewModelTests
    {
        private Mock<IContentOperations> _contentOperationsMock;

        [TestInitialize]
        public void TestInitialize()
        {
            _contentOperationsMock = new Mock<IContentOperations>();
        }
        [TestMethod()]
        public void MainWindowViewModel_ConstructorTest()
        {
            // arrange
            List<DataItem> expectedDrives = new List<DataItem>() {
                new DataItem() {FullPath = @"C:\\", Name = "C", Type = DataType.Drive},
                new DataItem() {FullPath = @"D:\\", Name = "D", Type = DataType.Drive},
                new DataItem() {FullPath = @"E:\\", Name = "E", Type = DataType.Drive}
            };

            _contentOperationsMock.Setup(x => x.GetDrives()).Returns(() => expectedDrives);

            // act
            MainWindowViewModel mainWindowViewModel = new MainWindowViewModel(_contentOperationsMock.Object);
            List<DataItemViewModel> drives = mainWindowViewModel.Items.ToList();
            List<DataItem> actualDrives = drives.Select(x => new DataItem() { FullPath = x.FullPath, Name = x.Name, Type = x.Type }).ToList();

            // assert
            _contentOperationsMock.Verify(x => x.GetDrives(), Times.AtLeastOnce);
            CollectionAssert.AreEqual(expectedDrives, actualDrives);
        }

        [TestMethod()]
        public void MainWindowViewModel_ChangeTypeSize()
        {
            // arrange
            SizeType newSyzeType = SizeType.Gigabytes;

            DataItem Drive = new DataItem() { FullPath = @"C:\\", Name = "C", Type = DataType.Drive };
            DataItem Folder = new DataItem() { FullPath = @"C:\\TreeSize", Name = "TreeSize", Type = DataType.Folder };
            DataItem File = new DataItem() { FullPath = @"C:\\TreeSize\3.txt", Name = "3.txt", Type = DataType.File };

            DataItemViewModel expectedDataItemViewModel = new DataItemViewModel(_contentOperationsMock.Object, Drive);
            DataItemViewModel expectedDataItemViewModelFolder = new DataItemViewModel(_contentOperationsMock.Object, Folder);
            DataItemViewModel expectedDataItemViewModelFile = new DataItemViewModel(_contentOperationsMock.Object, File);

            expectedDataItemViewModelFolder.Children.Add(expectedDataItemViewModelFile);
            expectedDataItemViewModel.Children.Add(expectedDataItemViewModelFolder);

            DataItemViewModel actualDataItemViewModel = new DataItemViewModel(_contentOperationsMock.Object, Drive);
            DataItemViewModel actualDataItemViewModelFolder = new DataItemViewModel(_contentOperationsMock.Object, Folder);
            DataItemViewModel actualDataItemViewModelFile = new DataItemViewModel(_contentOperationsMock.Object, File);

            actualDataItemViewModel.SizeType = newSyzeType;
            actualDataItemViewModelFolder.SizeType = newSyzeType;
            actualDataItemViewModelFile.SizeType = newSyzeType;

            actualDataItemViewModelFolder.Children.Add(actualDataItemViewModelFile);
            actualDataItemViewModel.Children.Add(actualDataItemViewModelFolder);

            //act
            expectedDataItemViewModel.SizeType = newSyzeType;

            // assert
            Assert.AreEqual(expectedDataItemViewModel, actualDataItemViewModel);
            Assert.AreEqual(expectedDataItemViewModelFolder, actualDataItemViewModelFolder);
            Assert.AreEqual(expectedDataItemViewModelFile, actualDataItemViewModelFile);
        }
    }
}