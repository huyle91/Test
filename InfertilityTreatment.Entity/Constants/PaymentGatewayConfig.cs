namespace InfertilityTreatment.Entity.Constants
{
    public class VNPayConfig
    {
        public string TmnCode { get; set; } = string.Empty;
        public string HashSecret { get; set; } = string.Empty;
        public string PaymentUrl { get; set; } = string.Empty;
        public string CallbackUrl { get; set; } = string.Empty;
        public string ReturnUrl { get; set; } = string.Empty;
    }

    public class PaymentGatewayConfig
    {
        public VNPayConfig VNPay { get; set; } = new();
    }
}
