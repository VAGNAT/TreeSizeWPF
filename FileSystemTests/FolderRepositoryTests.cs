using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Moq;
using FileSystem.Interfaces;
using System.Threading;

namespace FileSystem.Tests
{
    [TestClass()]
    public class FolderRepositoryTests
    {
        public TestContext TestContext { get; set; }
        private FolderRepository _folderRepository;
        private Mock<IInaccessibleRepository> _inaccesibleRepositoryMock;
        [TestInitialize]
        public void TestInitialize()
        {
            _inaccesibleRepositoryMock = new Mock<IInaccessibleRepository>();
            _folderRepository = new FolderRepository(_inaccesibleRepositoryMock.Object);
        }
        [TestMethod()]
        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.XML",
            "testData.xml",
            "DataItemFolders",
            DataAccessMethod.Sequential)]
        public void GetFolderNameTest()
        {
            // arrange
            string FullPath = Convert.ToString(TestContext.DataRow["FullPath"]);
            string expectedName = Convert.ToString(TestContext.DataRow["Name"]);

            // act
            string actualName = _folderRepository.GetFolderName(FullPath);

            // assert
            Assert.AreEqual(expectedName, actualName);
        }

        [TestMethod()]
        public void GetFolderSize_Dependency()
        {
            // arrange
            _inaccesibleRepositoryMock.Setup(x => x.CheckInaccessible(It.IsAny<string>())).Returns(true);

            // act
            _folderRepository.GetFolderSize(It.IsAny<string>(), It.IsAny<CancellationToken>());

            // assert
            _inaccesibleRepositoryMock.Verify((x) => x.CheckInaccessible(It.IsAny<string>()));
            _inaccesibleRepositoryMock.Verify((x) => x.CreateInaccessibleFolders(It.IsAny<string>()));
        }        
    }
}