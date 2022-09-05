namespace GeneralUtilsLibTests
{
    [TestClass]
    public class SmtpUtilTest
    {

        [TestMethod]
        public void TestIsValidEmail()
        {
            Assert.IsFalse(GeneralUtils.SmtpUtil.IsValidEmail("mikey.nuke"));
            Assert.IsFalse(GeneralUtils.SmtpUtil.IsValidEmail("mikey.nuke@"));
            Assert.IsTrue(GeneralUtils.SmtpUtil.IsValidEmail("mikey.nuke@gmail.com"));
        }
 
        [TestMethod]
        public void TestValidateAddress()
        {
            Assert.IsFalse(GeneralUtils.SmtpUtil.ValidateAddress("mikey.nuke"));
            Assert.IsFalse(GeneralUtils.SmtpUtil.ValidateAddress("mikey.nuke@, mikey.nuke@gmail.com"));
            Assert.IsTrue(GeneralUtils.SmtpUtil.ValidateAddress("mikey.nuke@gmail.com"));
            Assert.IsTrue(GeneralUtils.SmtpUtil.ValidateAddress("mikey.nuke@gmail.com,nukemikey@gmail.com"));
            Assert.IsTrue(GeneralUtils.SmtpUtil.ValidateAddress("mikey.nuke@gmail.com,nukemikey@gmail.com,gadetadmin@gmail.com"));
        }
    }
}
