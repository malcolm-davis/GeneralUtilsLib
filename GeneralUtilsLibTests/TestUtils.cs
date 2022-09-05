namespace GeneralUtilsLibTests
{
    public class TestUtils
    {
        public static string GetTestFolder()
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

            return configPath.Substring(0, binIndex) + "/TestFolder/";
        }
    }
}
