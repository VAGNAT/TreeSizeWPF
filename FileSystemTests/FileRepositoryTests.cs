using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace FileSystem.Tests
{
    [TestClass()]
    public class FileRepositoryTests
    {
        public TestContext TestContext { get; set; }
        private FileRepository _fileRepository;
        [TestInitialize]
        public void TestInitialize()
        {
            _fileRepository = new FileRepository();
        }

        [TestMethod()]
        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.XML",
            "testData.xml",
            "DataItemFiles",
            DataAccessMethod.Sequential)]
        public void GetFileNameTest()
        {
            // arrange
            string FullPath = Convert.ToString(TestContext.DataRow["FullPath"]);
            string expectedName = Convert.ToString(TestContext.DataRow["Name"]);
            
            // act
            string actualName = _fileRepository.GetFileName(FullPath);

            // assert
            Assert.AreEqual(expectedName, actualName);
        }
    }
}