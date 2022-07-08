using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using Moq;
using FileSystem.Interfaces;
using FileSystem.Model;
using System.Threading;
using System.Reflection;

namespace TreeSize.ViewModel.Tests
{
    [TestClass()]
    public class DataItemViewModelTests
    {
        private Mock<IContentOperations> _contentOperationsMock;

        [TestInitialize]
        public void TestInitialize()
        {
            _contentOperationsMock = new Mock<IContentOperations>();
        }

        [TestMethod()]
        public void DataItemViewModel_ConstructorTest_TypeDrive()
        {
            // arrange
            DataItem expectedDataItem = new DataItem { FullPath = @"C:\\", Name = "C", Type = DataType.Drive };
            
            // act
            DataItemViewModel dataItemViewModel = new DataItemViewModel(_contentOperationsMock.Object, expectedDataItem);
            
            DataItem actualDataItem = new DataItem() { FullPath = dataItemViewModel.FullPath, Name = dataItemViewModel.Name, Type = dataItemViewModel.Type };
            FieldInfo fieldInfoCts = typeof(DataItemViewModel).GetField("_cts", BindingFlags.Instance | BindingFlags.NonPublic);
            CancellationTokenSource actualCts = (CancellationTokenSource)fieldInfoCts.GetValue(dataItemViewModel);
            
            // assert
            Assert.AreEqual(expectedDataItem, actualDataItem);            
            Assert.IsNotNull(dataItemViewModel.Children);
            _contentOperationsMock.Verify((x) => x.GetFileSizeAsync(dataItemViewModel.FullPath, actualCts.Token), Times.Never);
            _contentOperationsMock.Verify((x) => x.GetFolderSizeAsync(dataItemViewModel.FullPath, actualCts.Token), Times.Never);
        }

        [TestMethod()]
        public void DataItemViewModel_ConstructorTest_TypeFolder()
        {
            // arrange
            DataItem expectedDataItem = new DataItem { FullPath = @"C:\\TreeSize", Name = "TreeSize", Type = DataType.Folder };
            
            // act
            DataItemViewModel dataItemViewModel = new DataItemViewModel(_contentOperationsMock.Object, expectedDataItem);

            DataItem actualDataItem = new DataItem() { FullPath = dataItemViewModel.FullPath, Name = dataItemViewModel.Name, Type = dataItemViewModel.Type };
            FieldInfo fieldInfo = typeof(DataItemViewModel).GetField("_cts", BindingFlags.Instance | BindingFlags.NonPublic);
            CancellationTokenSource actualCts = (CancellationTokenSource)fieldInfo.GetValue(dataItemViewModel);
            
            // assert
            Assert.AreEqual(expectedDataItem, actualDataItem);            
            Assert.IsNotNull(dataItemViewModel.Children);            
        }

        [TestMethod()]
        public void DataItemViewModel_ConstructorTest_TypeFile()
        {
            // arrange
            DataItem expectedDataItem = new DataItem { FullPath = @"C:\\TreeSize.cs", Name = "TreeSize.cs", Type = DataType.File };            

            // act
            DataItemViewModel dataItemViewModel = new DataItemViewModel(_contentOperationsMock.Object, expectedDataItem);

            DataItem actualDataItem = new DataItem() { FullPath = dataItemViewModel.FullPath, Name = dataItemViewModel.Name, Type = dataItemViewModel.Type };
            FieldInfo fieldInfo = typeof(DataItemViewModel).GetField("_cts", BindingFlags.Instance | BindingFlags.NonPublic);
            CancellationTokenSource actualCts = (CancellationTokenSource)fieldInfo.GetValue(dataItemViewModel);
            
            // assert
            Assert.AreEqual(expectedDataItem, actualDataItem);            
            Assert.IsNotNull(dataItemViewModel.Children);            
        }

        [TestMethod()]
        public void DataItemViewModel_ExpandTest_Set_true()
        {
            // arrange
            List<DataItem> expectedFolders = new List<DataItem>() {
                new DataItem() {FullPath = @"C:\\dev", Name = "dev", Type = DataType.Folder},
                new DataItem() {FullPath = @"C:\\TreeSize", Name = "TreeSize", Type = DataType.Folder},
                new DataItem() {FullPath = @"C:\\Foxminded", Name = "Foxminded", Type = DataType.Folder}
            };
            List<DataItem> expectedFiles = new List<DataItem>() {
                new DataItem() {FullPath = @"C:\\1.cs", Name = "1.cs", Type = DataType.File},
                new DataItem() {FullPath = @"C:\\2.xml", Name = "2.xml", Type = DataType.File},
                new DataItem() {FullPath = @"C:\\3.txt", Name = "3.txt", Type = DataType.File}
            };
            List<DataItem> expectedChildren = new List<DataItem>() {
                new DataItem() {FullPath = @"C:\\dev", Name = "dev", Type = DataType.Folder},
                new DataItem() {FullPath = @"C:\\TreeSize", Name = "TreeSize", Type = DataType.Folder},
                new DataItem() {FullPath = @"C:\\Foxminded", Name = "Foxminded", Type = DataType.Folder},
                new DataItem() {FullPath = @"C:\\1.cs", Name = "1.cs", Type = DataType.File},
                new DataItem() {FullPath = @"C:\\2.xml", Name = "2.xml", Type = DataType.File},
                new DataItem() {FullPath = @"C:\\3.txt", Name = "3.txt", Type = DataType.File}
            };
            
            DataItem expectedDataItem = new DataItem { FullPath = @"C:\\", Name = "C", Type = DataType.Drive };

            DataItemViewModel dataItemViewModel = new DataItemViewModel(_contentOperationsMock.Object, expectedDataItem);
            _contentOperationsMock.Setup(x => x.GetFolders(It.IsAny<string>())).Returns(() => expectedFolders);
            _contentOperationsMock.Setup(x => x.GetFiles(It.IsAny<string>())).Returns(() => expectedFiles);

            // act
            dataItemViewModel.IsExpanded = true;

            List<DataItemViewModel> children = dataItemViewModel.Children.ToList();
            List<DataItem> actualChildren = children.Select(x => new DataItem() { FullPath = x.FullPath, Name = x.Name, Type = x.Type }).ToList();

            // assert
            _contentOperationsMock.Verify((x) => x.GetFolders(It.IsAny<string>()), Times.Once);
            _contentOperationsMock.Verify((x) => x.GetFiles(It.IsAny<string>()), Times.Once);
            CollectionAssert.AreEqual(expectedChildren, actualChildren);
        }

        [TestMethod()]
        public void DataItemViewModel_ExpandTest_Set_false()
        {
            // arrange
            List<DataItem> expectedFolders = new List<DataItem>() {
                new DataItem() {FullPath = @"C:\\dev", Name = "dev", Type = DataType.Folder},
                new DataItem() {FullPath = @"C:\\TreeSize", Name = "TreeSize", Type = DataType.Folder},
                new DataItem() {FullPath = @"C:\\Foxmided", Name = "Foxmided", Type = DataType.Folder}
            };
            List<DataItem> expectedFiles = new List<DataItem>() {
                new DataItem() {FullPath = @"C:\\1.cs", Name = "1.cs", Type = DataType.File},
                new DataItem() {FullPath = @"C:\\2.xml", Name = "2.xml", Type = DataType.File},
                new DataItem() {FullPath = @"C:\\3.txt", Name = "3.txt", Type = DataType.File}
            };

            DataItem expectedDataItem = new DataItem { FullPath = @"C:\\", Name = "C", Type = DataType.Drive };
            
            _contentOperationsMock.Setup(x => x.GetFolders(It.IsAny<string>())).Returns(() => expectedFolders);
            _contentOperationsMock.Setup(x => x.GetFiles(It.IsAny<string>())).Returns(() => expectedFiles);
            DataItemViewModel dataItemViewModel = new DataItemViewModel(_contentOperationsMock.Object, expectedDataItem);


            // act
            dataItemViewModel.IsExpanded = true;
            dataItemViewModel.IsExpanded = false;            

            // assert            
            Assert.AreEqual(dataItemViewModel.Children.Count, 1);
        }
    }
}