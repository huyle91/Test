using System.Collections.Generic;

namespace InfertilityTreatment.Entity.DTOs.Payments
{
    public class VNPayCallbackDto
    {
        public string vnp_Amount { get; set; }
        public string vnp_BankCode { get; set; }
        public string vnp_BankTranNo { get; set; }
        public string vnp_CardType { get; set; }
        public string vnp_OrderInfo { get; set; }
        public string vnp_PayDate { get; set; }
        public string vnp_ResponseCode { get; set; }
        public string vnp_TmnCode { get; set; }
        public string vnp_TransactionNo { get; set; }
        public string vnp_TransactionStatus { get; set; }
        public string vnp_TxnRef { get; set; }
        public string vnp_SecureHashType { get; set; }
        public string vnp_SecureHash { get; set; }
        
        // Convert to dictionary for validation
        public Dictionary<string, string> ToValidationDict()
        {
            return new Dictionary<string, string>
            {
                {"vnp_Amount", vnp_Amount},
                {"vnp_BankCode", vnp_BankCode},
                {"vnp_BankTranNo", vnp_BankTranNo},
                {"vnp_CardType", vnp_CardType},
                {"vnp_OrderInfo", vnp_OrderInfo},
                {"vnp_PayDate", vnp_PayDate},
                {"vnp_ResponseCode", vnp_ResponseCode},
                {"vnp_TmnCode", vnp_TmnCode},
                {"vnp_TransactionNo", vnp_TransactionNo},
                {"vnp_TransactionStatus", vnp_TransactionStatus},
                {"vnp_TxnRef", vnp_TxnRef}
            };
        }
    }
}
