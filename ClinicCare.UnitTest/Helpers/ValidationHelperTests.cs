using ClinicCare.Business.Exceptions;
using ClinicCare.Business.Helpers;

namespace ClinicCare.UnitTest.Helpers
{
    [TestClass]
    public class ValidationHelperTests
    {
        [TestMethod]
        [ExpectedException(typeof(BadRequestException))]
        public void NotNull_Null_ThrowsBadRequest()
        {
            ValidationHelper.NotNull(null, "Value is required");
        }

        [TestMethod]
        public void NotNull_NotNull_DoesNotThrow()
        {
            ValidationHelper.NotNull("something", "Value is required");
        }

        [TestMethod]
        [ExpectedException(typeof(BadRequestException))]
        public void NotEmpty_Empty_ThrowsBadRequest()
        {
            ValidationHelper.NotEmpty("", "Cannot be empty");
        }

        [TestMethod]
        public void NotEmpty_NonEmpty_DoesNotThrow()
        {
            ValidationHelper.NotEmpty("abc", "Cannot be empty");
        }

        [TestMethod]
        [ExpectedException(typeof(ConflictException))]
        public void MustBeUnique_Exists_ThrowsConflict()
        {
            ValidationHelper.MustBeUnique(true, "Already exists");
        }

        [TestMethod]
        public void MustBeUnique_NotExists_DoesNotThrow()
        {
            ValidationHelper.MustBeUnique(false, "Already exists");
        }

        [TestMethod]
        [ExpectedException(typeof(BadRequestException))]
        public void GuidNotEmpty_EmptyGuid_ThrowsBadRequest()
        {
            ValidationHelper.GuidNotEmpty(Guid.Empty, "Id");
        }

        [TestMethod]
        public void GuidNotEmpty_ValidGuid_DoesNotThrow()
        {
            ValidationHelper.GuidNotEmpty(Guid.NewGuid(), "Id");
        }

        [TestMethod]
        [ExpectedException(typeof(BadRequestException))]
        public void ValidateAge_FutureDob_Throws()
        {
            ValidationHelper.ValidateAge(DateTime.UtcNow.AddDays(1));
        }

        [TestMethod]
        [ExpectedException(typeof(BadRequestException))]
        public void ValidateAge_TooOld_Throws()
        {
            ValidationHelper.ValidateAge(DateTime.UtcNow.AddYears(-130));
        }

        [TestMethod]
        public void ValidateAge_ValidAge_DoesNotThrow()
        {
            ValidationHelper.ValidateAge(DateTime.UtcNow.AddYears(-25));
        }

        [TestMethod]
        [ExpectedException(typeof(BadRequestException))]
        public void DateNotInFuture_FutureDate_Throws()
        {
            ValidationHelper.DateNotInFuture(DateTime.UtcNow.AddDays(1), "TestDate");
        }

        [TestMethod]
        public void DateNotInFuture_PastDate_DoesNotThrow()
        {
            ValidationHelper.DateNotInFuture(DateTime.UtcNow.AddDays(-1), "TestDate");
        }

        [TestMethod]
        [ExpectedException(typeof(BadRequestException))]
        public void DateNotInPast_PastDate_Throws()
        {
            ValidationHelper.DateNotInPast(DateTime.UtcNow.AddDays(-1), "TestDate");
        }

        [TestMethod]
        public void DateNotInPast_FutureDate_DoesNotThrow()
        {
            ValidationHelper.DateNotInPast(DateTime.UtcNow.AddDays(1), "TestDate");
        }
    }
}
