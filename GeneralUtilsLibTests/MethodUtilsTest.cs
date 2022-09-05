namespace GeneralUtilsLibTests
{
    [TestClass]
    public class MethodUtilsTest
    {

        [TestMethod]
        public void TestNotNull()
        {
            string param = "param";
            GeneralUtils.MethodUtils.NotNull(param, "param");
            Assert.IsTrue(true);

            string msg = "invalid param value, param cannot be null";
            try
            {
                param = null;
                GeneralUtils.MethodUtils.NotNull(param, "param", msg);
                Assert.IsTrue(false);
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex is ArgumentNullException);
                Assert.AreEqual(ex.Message, msg+" (Parameter 'param')");
            }
        }

        [TestMethod]
        public void TestNotNullOrEmpty()
        {
            string param = "param";
            GeneralUtils.MethodUtils.NotNullOrEmpty(param, "param");
            Assert.IsTrue(true);

            string msg = "invalid param value, param cannot be null";
            try
            {
                param = " ";
                GeneralUtils.MethodUtils.NotNullOrEmpty(param, "param", msg);
                Assert.IsTrue(false);
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex is ArgumentNullException);
                Assert.AreEqual(ex.Message, msg + " (Parameter 'param')");
            }
        }


    }
}
