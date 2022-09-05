using ConfigurationUtils;
using GeneralUtils;
using static System.Net.Mime.MediaTypeNames;

namespace GeneralUtilsLibTests
{
    [TestClass]
    public class ApplicationConfigTest
    {
        [ClassInitialize]
        public static void TestClassInit(TestContext context)
        {
            workingFolder = TestUtils.GetTestFolder();
            System.Diagnostics.Trace.WriteLine("workingFolder=" + workingFolder);

            textFile = (workingFolder + "TestFile.txt");
            string[] lines = { "First line", "Second line", "Third line" };
            File.WriteAllLines(textFile, lines);

            System.Diagnostics.Trace.WriteLine("Setup complete");
        }

        const string ServiceName = "Fake Service Test";

        enum FakeService
        {
            FakeText, FakeFile, FakeFolder, FakeEmail, FakeInt, FakeBoolean
        }

        public ApplicationConfiguration BuildConfiguration(string filename)
        {
            ConfigurationRulesBuilder builder = ConfigurationRulesBuilder.NewBuilder(ServiceName);
            return builder.SetConfigurationFolder(workingFolder)
                .SetConfigurationName(filename)
                .AddField(FakeService.FakeText, Condition.Required, ParamType.Text)
                .AddField(FakeService.FakeFolder, Condition.Required, ParamType.Folder)
                .AddField(FakeService.FakeEmail, Condition.Required, ParamType.Email)
                .AddField(FakeService.FakeInt, Condition.Required, ParamType.Number)
                .AddField(FakeService.FakeBoolean, Condition.Required, ParamType.Boolean)
                .AddField(FakeService.FakeFile, Condition.Optional, ParamType.File)
                .Build();
        }


        [TestMethod]
        public void TestGoodAppConfig()
        {
            // string configFilename, string text, string file, string folder, string email, string intValue, string boolValue
            BuildTestConfigFile(configName, "Test text", textFile, workingFolder, "mikey.mikey@gmail2.com", "1", "false");
            ApplicationConfiguration config = BuildConfiguration(configName);
            Assert.AreEqual("Test text", config[FakeService.FakeText]);
            Assert.AreEqual(textFile, config[FakeService.FakeFile]);
            Assert.AreEqual(workingFolder, config[FakeService.FakeFolder]);
            Assert.AreEqual("mikey.mikey@gmail2.com", config[FakeService.FakeEmail]);
            Assert.AreEqual(1, config.Int(FakeService.FakeInt));
            Assert.AreEqual(1, config.Int("FakeService.FakeInt"));
            Assert.AreEqual("1", config[FakeService.FakeInt]);
            Assert.AreEqual(false, config.Bool(FakeService.FakeBoolean));
            Assert.AreEqual(false, config.Bool("FakeService.FakeBoolean"));
            Assert.AreEqual("false", config[FakeService.FakeBoolean]);
        }

        [TestMethod]
        public void TestBadAppConfig()
        {
            BuildTestConfigFile(configName, "Test text", textFile, workingFolder, "mikey.mikey@gmail2.com", "a", "");
            try
            {
                ApplicationConfiguration config = BuildConfiguration(configName);
                Assert.Fail("BuildConfiguration should fail due to bad ini file");
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex.Message.Contains("Expected a number"));
                Assert.IsTrue(ex.Message.Contains("Missing configuration value for key=FakeService.FakeBoolean"));
                Assert.IsTrue(true, "BuildConfiguration failed as expected");
            }
        }

        [TestMethod]
        public void TestMissingFile()
        {
            BuildTestConfigFile(configName, "Test text", "c:/do.nothing.temp/missingfile.txt", workingFolder, "mikey.mikey@gmail2.com", "1", "0");
            try
            {
                ApplicationConfiguration config = BuildConfiguration(configName);
                Assert.Fail("BuildConfiguration should fail due to missing file");
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex.Message.Contains("File does not exist"));
                Assert.IsTrue(ex.Message.Contains("Invalid configuration value for key=FakeService.FakeFile"));
                Assert.IsTrue(true, "BuildConfiguration failed as expected");
            }
        }

        [TestMethod]
        public void TestInvalidIEmail()
        {
            BuildTestConfigFile(configName, "Test text", textFile, workingFolder, "improper.email.address", "1", "0");
            try
            {
                ApplicationConfiguration config = BuildConfiguration(configName);
                Assert.Fail("BuildConfiguration should fail due to incorrect email format");
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex.Message.Contains("Email format is invalid"));
                Assert.IsTrue(ex.Message.Contains("key=FakeService.FakeEmail"));
                Assert.IsTrue(true, "BuildConfiguration failed as expected");
            }
        }

        [TestMethod]
        public void TestMissingEmail()
        {
            BuildTestConfigFile(configName, "Test text", textFile, workingFolder, "", "1", "0");
            try
            {
                ApplicationConfiguration config = BuildConfiguration(configName);
                Assert.Fail("BuildConfiguration should fail due to missing email format");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
                Assert.IsTrue(ex.Message.Contains("Missing configuration value for key=FakeService.FakeEmail"));
                Assert.IsTrue(ex.Message.Contains("Fake Service Test configuration contains invalid configuration value(s)"));
                Assert.IsTrue(true, "BuildConfiguration failed as expected");
            }
        }

        [TestMethod]
        public void TestOptionalAppConfig()
        {
            string[] lines =
            {
                "FakeService.FakeText=Test text",
                "FakeService.FakeFolder="+workingFolder,
                "FakeService.FakeEmail=mikey.mikey@gmail2.com",
                "FakeService.FakeInt=1",
                "FakeService.FakeBoolean=0"
            };

            String filename = ApplicationConfiguration.ConfigName(configName);
            File.WriteAllLines((workingFolder + filename), lines);
            try
            {
                ApplicationConfiguration config = BuildConfiguration(configName);
                Assert.IsTrue(true, "missing FakeService.FakeFolder works");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.StackTrace);
                Assert.IsTrue(false, "The optinal missing FakeService.FakeFolder should not fail");
            }
        }


        [ClassCleanup]
        public static void TestClassCleanup()
        {
            if (File.Exists(configFilenameFullpath))
                File.Delete(configFilenameFullpath);

            if (File.Exists(textFile))
                File.Delete(textFile);
        }

        public static void BuildTestConfigFile(
            string configFilename, string text, string file, string folder, string email, string intValue, string boolValue)
        {
            string[] lines =
            {
                "; a test note",
                "FakeService.FakeText="+text,
                "FakeService.FakeFile="+file,
                "FakeService.FakeFolder="+folder,
                "FakeService.FakeEmail="+email,
                "FakeService.FakeInt="+intValue,
                "FakeService.FakeBoolean="+boolValue
            };

            string filename = ApplicationConfiguration.ConfigName(configFilename);
            configFilenameFullpath = (workingFolder + filename);
            File.WriteAllLines(configFilenameFullpath, lines);
        }

        private const string configName = "FakeConfig.ini";

        private static string configFilenameFullpath;

        private static string workingFolder;

        private static string textFile;
    }
}
