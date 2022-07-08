using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using Moq;
using FileSystem.Interfaces;
using FileSystem.Model;
using System.Threading;
using System.Threading.Tasks;

namespace FileSystem.Tests
{
    [TestClass()]
    public class ContentOperationsTests
    {
        public TestContext TestContext { get; set; }
        private Mock<IDriveRepository> _driveRepositoryMock;
        private Mock<IFolderRepository> _folderRepositoryMock;
        private Mock<IFileRepository> _fileRepositoryMock;
        private Mock<IInaccessibleRepository> _inaccesibleRepositoryMock;
        private ContentOperations _operations;

        [TestInitialize]
        public void TestInitialize()
        {
            _driveRepositoryMock = new Mock<IDriveRepository>();
            _folderRepositoryMock = new Mock<IFolderRepository>();
            _fileRepositoryMock = new Mock<IFileRepository>();
            _inaccesibleRepositoryMock = new Mock<IInaccessibleRepository>();
            _operations = new ContentOperations(_driveRepositoryMock.Object, 
                _folderRepositoryMock.Object, _fileRepositoryMock.Object, _inaccesibleRepositoryMock.Object);
        }

        [TestMethod()]
        public void GetDrivesTest_Dependency()
        {
            //arrange
            List<DataItem> expectedDrives = new List<DataItem>() {
                new DataItem() {FullPath = @"C:\\", Name = "C", Type = DataType.Drive},
                new DataItem() {FullPath = @"D:\\", Name = "D", Type = DataType.Drive},
                new DataItem() {FullPath = @"E:\\", Name = "E", Type = DataType.Drive}
            };
            _driveRepositoryMock.Setup(x => x.GetDrivesCollection()).Returns(() => expectedDrives);

            // act
            List<DataItem> actualDrives = _operations.GetDrives();

            // assert
            _driveRepositoryMock.Verify((x) => x.GetDrivesCollection());
            CollectionAssert.AreEqual(expectedDrives, actualDrives);
        }


        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.XML",
            "testData.xml",
            "DataItemFolders",
            DataAccessMethod.Sequential)]
        [TestMethod()]
        public void GetFoldersTest_dependency()
        {
            // arrange
            List<DataItem> expectedContent = new List<DataItem>() {
                new DataItem() {FullPath = Convert.ToString(TestContext.DataRow["FullPath"]), Name = Convert.ToString(TestContext.DataRow["Name"]), Type = DataType.Folder}
            };

            _folderRepositoryMock.Setup(x => x.GetFoldersCollection(It.IsAny<string>())).Returns(() => new string[] { Convert.ToString(TestContext.DataRow["FullPath"]) });
            _folderRepositoryMock.Setup(x => x.GetFolderName(It.IsAny<string>())).Returns(() => Convert.ToString(TestContext.DataRow["Name"]));
            _inaccesibleRepositoryMock.Setup(x=>x.CheckInaccessible(It.IsAny<string>())).Returns(false);

            // act
            List<DataItem> actualContent = _operations.GetFolders(It.IsAny<string>());

            // assert
            _folderRepositoryMock.Verify((x) => x.GetFoldersCollection(It.IsAny<string>()));
            _folderRepositoryMock.Verify((x) => x.GetFolderName(It.IsAny<string>()));
            _inaccesibleRepositoryMock.Verify((x) => x.CheckInaccessible(It.IsAny<string>()));

            CollectionAssert.AreEqual(expectedContent, actualContent);
        }

        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.XML",
            "testData.xml",
            "DataItemFiles",
            DataAccessMethod.Sequential)]
        [TestMethod()]
        public void GetFilesTest_dependency()
        {
            // arrange
            List<DataItem> expectedContent = new List<DataItem>() {
                new DataItem() {FullPath = Convert.ToString(TestContext.DataRow["FullPath"]), Name = Convert.ToString(TestContext.DataRow["Name"]), Type = DataType.File}
            };

            _fileRepositoryMock.Setup(x => x.GetFilesCollection(It.IsAny<string>())).Returns(() => new string[] { Convert.ToString(TestContext.DataRow["FullPath"]) });
            _fileRepositoryMock.Setup(x => x.GetFileName(It.IsAny<string>())).Returns(() => Convert.ToString(TestContext.DataRow["Name"]));
            _inaccesibleRepositoryMock.Setup(x => x.CheckInaccessible(It.IsAny<string>())).Returns(false);

            // act
            List<DataItem> actualContent = _operations.GetFiles(It.IsAny<string>());

            // assert
            _fileRepositoryMock.Verify((x) => x.GetFilesCollection(It.IsAny<string>()));
            _fileRepositoryMock.Verify((x) => x.GetFileName(It.IsAny<string>()));
            _inaccesibleRepositoryMock.Verify((x) => x.CheckInaccessible(It.IsAny<string>()));

            CollectionAssert.AreEqual(expectedContent, actualContent);
        }

        [TestMethod()]
        public async Task GetFolderSizeAsyncTest_Dependency()
        {
            // act
            await _operations.GetFolderSizeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>());

            // assert
            _folderRepositoryMock.Verify((x) => x.GetFolderSize(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [TestMethod()]
        public async Task GetFileSizeAsyncTest_Dependency()
        {
            // act
            await _operations.GetFileSizeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>());

            // assert
            _fileRepositoryMock.Verify((x) => x.GetFileSize(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}