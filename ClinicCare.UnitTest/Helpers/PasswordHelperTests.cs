using ClinicCare.Business.Exceptions;
using ClinicCare.Business.Helpers;

namespace ClinicCare.UnitTest.Helpers
{
    [TestClass]
    public class PasswordHelperTests
    {
        [TestMethod]
        public void Hash_ValidPassword_ReturnsNonEmptyHash()
        {
            var password = "StrongP@ss1";

            var hash = PasswordHelper.Hash(password);

            Assert.IsFalse(string.IsNullOrWhiteSpace(hash));
        }

        [TestMethod]
        public void Verify_CorrectPassword_ReturnsTrue()
        {
            var password = "StrongP@ss1";
            var hash = PasswordHelper.Hash(password);

            var result = PasswordHelper.Verify(password, hash);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Verify_IncorrectPassword_ReturnsFalse()
        {
            var password = "StrongP@ss1";
            var hash = PasswordHelper.Hash(password);

            var result = PasswordHelper.Verify("WrongP@ss1", hash);

            Assert.IsFalse(result);
        }

        [TestMethod]
        [ExpectedException(typeof(BadRequestException))]
        public void Validate_NullPassword_ThrowsBadRequest()
        {
            PasswordHelper.Validate(null!);
        }

        [TestMethod]
        [ExpectedException(typeof(BadRequestException))]
        public void Validate_EmptyPassword_ThrowsBadRequest()
        {
            PasswordHelper.Validate("");
        }

        [TestMethod]
        [ExpectedException(typeof(BadRequestException))]
        public void Validate_WhitespacePassword_ThrowsBadRequest()
        {
            PasswordHelper.Validate("   ");
        }

        [TestMethod]
        [ExpectedException(typeof(BadRequestException))]
        public void Validate_WeakPassword_NoUppercase_ThrowsBadRequest()
        {
            PasswordHelper.Validate("weakp@ss1");
        }

        [TestMethod]
        [ExpectedException(typeof(BadRequestException))]
        public void Validate_WeakPassword_NoLowercase_ThrowsBadRequest()
        {
            PasswordHelper.Validate("WEAKP@SS1");
        }

        [TestMethod]
        [ExpectedException(typeof(BadRequestException))]
        public void Validate_WeakPassword_NoNumber_ThrowsBadRequest()
        {
            PasswordHelper.Validate("WeakPass@");
        }

        [TestMethod]
        [ExpectedException(typeof(BadRequestException))]
        public void Validate_WeakPassword_NoSpecialChar_ThrowsBadRequest()
        {
            PasswordHelper.Validate("WeakPass1");
        }

        [TestMethod]
        public void Validate_StrongPassword_DoesNotThrow()
        {
            var strong = "StrongP@ss1";
            PasswordHelper.Validate(strong);
        }
    }
}
