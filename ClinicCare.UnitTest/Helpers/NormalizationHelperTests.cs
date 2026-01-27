using ClinicCare.Business.Helpers;

namespace ClinicCare.UnitTest.Helpers
{
    [TestClass]
    public class NormalizationHelperTests
    {
        [TestMethod]
        public void NormalizeKey_TrimsSpacesAndConvertsToLower()
        {
            string input = "  HeLLo@Example.COM  ";
            string expected = "hello@example.com";

            var result = NormalizationHelper.NormalizeKey(input);

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void NormalizeKey_AlreadyNormalized_ReturnsSame()
        {
            string input = "test@example.com";
            string expected = "test@example.com";

            var result = NormalizationHelper.NormalizeKey(input);

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void NormalizeKey_OnlySpaces_ReturnsEmptyString()
        {
            string input = "    ";
            string expected = "";

            var result = NormalizationHelper.NormalizeKey(input);

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void NormalizeKey_EmptyString_ReturnsEmptyString()
        {
            string input = "";
            string expected = "";

            var result = NormalizationHelper.NormalizeKey(input);

            Assert.AreEqual(expected, result);
        }
    }
}
