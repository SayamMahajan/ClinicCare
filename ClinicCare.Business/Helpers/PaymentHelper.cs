namespace ClinicCare.Business.Helpers
{
    internal static class PaymentHelper
    {
        public static string GenerateTransactionId()
        {
            return $"TXN-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..8].ToUpper()}";
        }

    }
}
