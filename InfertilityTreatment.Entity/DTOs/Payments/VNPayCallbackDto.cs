using System.Collections.Generic;

namespace InfertilityTreatment.Entity.DTOs.Payments
{
    public class VNPayCallbackDto
    {
        public string? vnp_Amount { get; set; }
        public string? vnp_BankCode { get; set; }
        public string? vnp_BankTranNo { get; set; }
        public string? vnp_CardType { get; set; }
        public string? vnp_OrderInfo { get; set; }
        public string? vnp_PayDate { get; set; }
        public string? vnp_ResponseCode { get; set; }
        public string? vnp_TmnCode { get; set; }
        public string? vnp_TransactionNo { get; set; }
        public string? vnp_TransactionStatus { get; set; }
        public string? vnp_TxnRef { get; set; }
        public string? vnp_SecureHashType { get; set; }
        public string? vnp_SecureHash { get; set; }
        
        // Convert to dictionary for validation
        public Dictionary<string, string> ToValidationDict()
        {
            var dict = new Dictionary<string, string>();
            
            if (!string.IsNullOrEmpty(vnp_Amount)) dict.Add("vnp_Amount", vnp_Amount);
            if (!string.IsNullOrEmpty(vnp_BankCode)) dict.Add("vnp_BankCode", vnp_BankCode);
            if (!string.IsNullOrEmpty(vnp_BankTranNo)) dict.Add("vnp_BankTranNo", vnp_BankTranNo);
            if (!string.IsNullOrEmpty(vnp_CardType)) dict.Add("vnp_CardType", vnp_CardType);
            if (!string.IsNullOrEmpty(vnp_OrderInfo)) dict.Add("vnp_OrderInfo", vnp_OrderInfo);
            if (!string.IsNullOrEmpty(vnp_PayDate)) dict.Add("vnp_PayDate", vnp_PayDate);
            if (!string.IsNullOrEmpty(vnp_ResponseCode)) dict.Add("vnp_ResponseCode", vnp_ResponseCode);
            if (!string.IsNullOrEmpty(vnp_TmnCode)) dict.Add("vnp_TmnCode", vnp_TmnCode);
            if (!string.IsNullOrEmpty(vnp_TransactionNo)) dict.Add("vnp_TransactionNo", vnp_TransactionNo);
            if (!string.IsNullOrEmpty(vnp_TransactionStatus)) dict.Add("vnp_TransactionStatus", vnp_TransactionStatus);
            if (!string.IsNullOrEmpty(vnp_TxnRef)) dict.Add("vnp_TxnRef", vnp_TxnRef);
            
            return dict;
        }
    }
}
