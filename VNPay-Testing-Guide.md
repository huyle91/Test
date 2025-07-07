# VNPay Testing Guide

## 🚀 **Step-by-Step Testing Instructions**

### **Step 1: Start the Application**
```bash
cd "d:\Class\PRN231\project3\BE"
dotnet run --project InfertilityTreatment.API
```

### **Step 2: Test Authentication First**
1. **Login to get JWT token**:
   ```json
   POST https://localhost:7077/api/auth/login
   {
     "email": "your-customer-email@example.com",
     "password": "your-password"
   }
   ```
   
2. **Copy the JWT token** from the response and use it in the Authorization header for all subsequent requests.

### **Step 3: Create VNPay Payment**
Use the following test data for creating a payment:

#### **Small Payment (10,000 VND - ~$0.40)**
```json
POST https://localhost:7077/api/payments/create
Authorization: Bearer {your-jwt-token}

{
  "customerId": 8,
  "amount": 10000,
  "description": "Payment for fertility treatment consultation",
  "treatmentCycleId": 1,
  "appointmentId": null,
  "returnUrl": "https://localhost:3000/payment/success",
  "cancelUrl": "https://localhost:3000/payment/cancel"
}
```

#### **Medium Payment (500,000 VND - ~$20)**
```json
{
  "customerId": 8,
  "amount": 500000,
  "description": "Payment for doctor consultation appointment",
  "treatmentCycleId": null,
  "appointmentId": 1,
  "returnUrl": "https://localhost:3000/payment/success",
  "cancelUrl": "https://localhost:3000/payment/cancel"
}
```

#### **Large Payment (15,000,000 VND - ~$600)**
```json
{
  "customerId": 8,
  "amount": 15000000,
  "description": "Payment for IVF Treatment Package - Premium",
  "treatmentCycleId": 2,
  "appointmentId": null,
  "returnUrl": "https://localhost:3000/payment/success",
  "cancelUrl": "https://localhost:3000/payment/cancel"
}
```

### **Step 4: Expected Response**
After creating a payment, you should receive:
```json
{
  "success": true,
  "data": {
    "paymentId": "generated-uuid",
    "paymentUrl": "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html?vnp_Amount=1000000&...",
    "status": "Pending",
    "message": "Payment URL generated successfully",
    "amount": 10000,
    "createdAt": "2025-01-07T...",
    "transactionId": "generated-uuid"
  }
}
```

### **Step 5: Test VNPay Payment Flow**
1. **Copy the `paymentUrl`** from the response
2. **Open the URL in a browser** to access VNPay's sandbox payment page
3. **Use VNPay test credentials** (you'll need to get these from VNPay documentation)
4. **Complete the payment** on VNPay's page
5. **VNPay will call back** to your `/api/payments/vnpay/callback` endpoint

### **Step 6: Verify Payment Status**
Check if the payment was processed correctly:
```http
GET https://localhost:7077/api/payments/status/{paymentId}
Authorization: Bearer {your-jwt-token}
```

### **Step 7: Check Payment History**
View all payments for your customer:
```http
GET https://localhost:7077/api/payments/history/8?page=1&pageSize=10
Authorization: Bearer {your-jwt-token}
```

## 🧪 **VNPay Test Card Information**

When testing on VNPay sandbox, you can use these test cards:

### **Successful Payment Test Cards:**
- **Visa**: `4111111111111111`
- **MasterCard**: `5555555555554444`
- **Expiry**: Any future date (e.g., `12/25`)
- **CVV**: Any 3 digits (e.g., `123`)

### **Failed Payment Test Cards:**
- **Declined Card**: `4000000000000002`

## 📋 **Payment Amount Guidelines**

### **VNPay Amount Format:**
- VNPay requires amounts in VND cents (multiply by 100)
- Your input: `10000` VND → VNPay receives: `1000000` (VND cents)
- Minimum: `5000` VND (5,000 VND = ~$0.20)
- Maximum: `500000000` VND (500,000,000 VND = ~$20,000)

### **Common Payment Scenarios:**
- **Consultation**: 10,000 - 50,000 VND
- **Basic Treatment**: 100,000 - 500,000 VND  
- **Advanced Treatment**: 1,000,000 - 15,000,000 VND
- **Premium Package**: 15,000,000+ VND

## 🔍 **Troubleshooting**

### **Common Issues:**
1. **401 Unauthorized**: Check JWT token is valid and not expired
2. **400 Bad Request**: Verify all required fields are provided
3. **404 Not Found**: Ensure customer ID exists in database
4. **500 Internal Server Error**: Check application logs for details

### **Callback Testing:**
If VNPay callback doesn't work during testing, you can manually simulate it:
```http
POST https://localhost:7077/api/payments/vnpay/callback
Content-Type: application/x-www-form-urlencoded

vnp_Amount=1000000&vnp_BankCode=NCB&vnp_ResponseCode=00&vnp_TmnCode=H9HN7C0Y&vnp_TransactionStatus=00&vnp_TxnRef={your-payment-id}&vnp_SecureHash={generated-hash}
```

## ✅ **Success Indicators**
- Payment created with status "Pending"
- VNPay URL generated successfully
- Callback processed and payment status updated
- Payment history shows correct transaction details
- All security validations pass

**🎉 Happy Testing!**
