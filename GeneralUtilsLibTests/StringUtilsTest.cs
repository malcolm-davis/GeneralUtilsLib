using GeneralUtils;

namespace GeneralUtilsLibTests
{
    [TestClass]
    public class StringUtilsTest
    {

        [TestMethod]
        public void TestParseBool()
        {
            Assert.IsTrue(StringUtils.ParseBool("true"));
            Assert.IsTrue(StringUtils.ParseBool("TRUE"));
            Assert.IsTrue(StringUtils.ParseBool("1"));

            Assert.IsFalse(StringUtils.ParseBool("false"));
            Assert.IsFalse(StringUtils.ParseBool("FALSE"));
            Assert.IsFalse(StringUtils.ParseBool("0"));
            Assert.IsFalse(StringUtils.ParseBool(null));
            Assert.IsFalse(StringUtils.ParseBool(""));
        }


        [TestMethod]
        public void TestIsBool()
        {
            Assert.IsTrue(StringUtils.IsBool("true"));
            Assert.IsTrue(StringUtils.IsBool("TRUE"));
            Assert.IsTrue(StringUtils.IsBool("1"));

            Assert.IsTrue(StringUtils.IsBool("false"));
            Assert.IsTrue(StringUtils.IsBool("FALSE"));
            Assert.IsTrue(StringUtils.IsBool("0"));

            Assert.IsFalse(StringUtils.IsBool(null));
            Assert.IsFalse(StringUtils.IsBool(""));
        }

        [TestMethod]
        public void TestParseInt()
        {
            Assert.AreEqual(StringUtils.ParseInt("true", 1), 1);
            Assert.AreEqual(StringUtils.ParseInt(null, 1), 1);
            Assert.AreEqual(StringUtils.ParseInt("", 1), 1);
            Assert.AreEqual(StringUtils.ParseInt(" ", 1), 1);
            Assert.AreEqual(StringUtils.ParseInt("2", 1), 2);
            Assert.AreEqual(StringUtils.ParseInt("2.2", 1), 1);
        }


    }
}
