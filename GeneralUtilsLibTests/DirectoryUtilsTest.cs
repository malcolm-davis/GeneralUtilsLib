using GeneralUtils;


namespace GeneralUtilsLibTests
{
    [TestClass]
    public class DirectoryUtilsTest
    {
        [ClassInitialize]
        public static void TestClassInit(TestContext context)
        {
            // C:\dev\projects\c#GeneralUtilsLib\source\GeneralUtilsLib\GeneralUtilsLibTests\bin\Debug\net6.0\GeneralUtilsLibTests.dll
            string configPath = Directory.GetCurrentDirectory();
            int binIndex = configPath.LastIndexOf("\\bin");
            if (binIndex == -1)
            {
                string msg = "unable to locate bin folder in configPath=" + configPath;
                System.Diagnostics.Trace.WriteLine(msg);
                Assert.Fail(msg);
            }


            string workingFolder = configPath.Substring(0, binIndex) + "/TestFolder/";
            sourceFolder = workingFolder + "sourceFolder";
            copyToFolder = workingFolder + "copyToFolder";
            moveToFolder = workingFolder + "moveToFolder";
            noFileFolder = workingFolder + "noFileFolder";

            Directory.CreateDirectory(sourceFolder);
            Directory.CreateDirectory(copyToFolder);
            Directory.CreateDirectory(moveToFolder);
            Directory.CreateDirectory(noFileFolder);


            string[] lines = { "First line", "Second line", "Third line" };
            File.WriteAllLines((sourceFolder + "/File1.txt"), lines);
            File.WriteAllLines((sourceFolder + "/File2.txt"), lines);
            File.WriteAllLines((sourceFolder + "/File3.txt"), lines);
            File.WriteAllLines((sourceFolder + "/File1.ini"), lines);
            File.WriteAllLines((sourceFolder + "/File2.ini"), lines);
        }

        [TestMethod]
        public void TestVerifyFolderRead()
        {
            Assert.IsTrue(DirectoryUtils.VerifyFolder(sourceFolder));
        }

        [TestMethod]
        public void TestVerifyFolderDoesNotExist()
        {
            Assert.IsFalse(DirectoryUtils.VerifyFolder(missingFolder));
        }

        [TestMethod]
        public void TestVerifyFolderNull()
        {
            try
            {
                DirectoryUtils.VerifyFolder(null);
                Assert.Fail("VerifyFolder did not fail when passed a null");
            }
            catch(Exception)
            {
                Assert.IsTrue(true, "VerifyFolder failed as expected");
            }
        }


        [TestMethod]
        public void TestCopyFolder()
        {
            try
            {
                DirectoryUtils.CopyDirectory(sourceFolder, copyToFolder);
                Assert.IsTrue(DirectoryUtils.CompareDirectories(sourceFolder, copyToFolder), "source and target folders should match");
            }
            catch (Exception ex)
            {
                Assert.IsTrue(false, "CopyDirectory failed", ex);
            }
        }


        [TestMethod]
        public void TestCopyFolderInvalidSource()
        {
            try
            {
                DirectoryUtils.CopyDirectory(missingFolder, copyToFolder);
                Assert.Fail("VerifyFolder did not fail when passed an invalid folder");
            }
            catch (Exception)
            {
                Assert.IsTrue(true, "VerifyFolder failed as expected");
            }
        }

        [TestMethod]
        public void TestCopyFolderNullSource()
        {
            try
            {
                DirectoryUtils.CopyDirectory(null, copyToFolder);
                Assert.Fail("VerifyFolder did not fail when passed an invalid folder");
            }
            catch (Exception)
            {
                Assert.IsTrue(true, "VerifyFolder failed as expected");
            }
        }

        [TestMethod]
        public void TestCopyFolderNullTarget()
        { 
            try
            {
                DirectoryUtils.CopyDirectory(sourceFolder, null);
                Assert.Fail("VerifyFolder did not fail when passed an invalid folder");
            }
            catch (Exception)
            {
                Assert.IsTrue(true, "VerifyFolder failed as expected");
            }
        }

        [TestMethod]
        public void TestCompareNonEqualDirectores()
        {
            try
            {
                Assert.IsFalse(DirectoryUtils.CompareDirectories(sourceFolder, noFileFolder), "source and target folders should not match");
            }
            catch (Exception ex)
            {
                Assert.IsTrue(false, "CompareDirectories failed", ex);
            }
        }

        [TestMethod]
        public void TestCopyIniFiles()
        {
            DirectoryUtils.Clean(copyToFolder);
            Directory.Delete(copyToFolder);

            int copyCount = DirectoryUtils.CopyDirectory(sourceFolder, copyToFolder, "*.ini");
            Assert.AreEqual(2, copyCount);

            DirectoryInfo targetDir = new DirectoryInfo(copyToFolder);
            Assert.IsTrue(targetDir.Exists);

            Assert.IsTrue(DirectoryUtils.CompareDirectories(sourceFolder, copyToFolder, "*.ini"));
        }

        [TestMethod]
        public void TestMoveIniFiles()
        {
            TestCopyIniFiles();

            // move files from copyToFolder to moveToFolder
            int moveCount = DirectoryUtils.MoveDirectory(copyToFolder, moveToFolder, "*.ini");
            Assert.AreEqual(2, moveCount);

            DirectoryInfo sourceDir= new DirectoryInfo(copyToFolder);
            Assert.IsTrue(sourceDir.Exists);

            // all the files in the copyToFolder directory should be gone
            FileInfo[]sourceFileList = sourceDir.GetFiles("*.*", System.IO.SearchOption.AllDirectories);
            Assert.AreEqual(sourceFileList.Length, 0);


            DirectoryInfo targetDir = new DirectoryInfo(moveToFolder);
            Assert.IsTrue(targetDir.Exists);

            // all the files in the copyToFolder directory should be gone
            FileInfo[] targetFileList = targetDir.GetFiles("*.*", System.IO.SearchOption.AllDirectories);
            Assert.AreEqual(targetFileList.Length, 2);

            // the ini files in the orginal sourceFolder should be in moveToFolder
            Assert.IsTrue(DirectoryUtils.CompareDirectories(sourceFolder, moveToFolder, "*.ini"));
        }

        [ClassCleanup]
        public static void TestClassCleanup()
        {
            DirectoryUtils.DeleteQuietly(sourceFolder);
            DirectoryUtils.DeleteQuietly(copyToFolder);
            DirectoryUtils.DeleteQuietly(moveToFolder);
            DirectoryUtils.DeleteQuietly(noFileFolder);
        }

        private static string sourceFolder;
        private static string copyToFolder;
        private static string moveToFolder;
        private static string noFileFolder;
        private static string missingFolder = "C:/invalid/folder/does.not.exist/";
    }
}
