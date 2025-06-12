// ===== Models/Billing.cs =====
using KosBuIpungApp.Enums;
using System;

namespace KosBuIpungApp.Models
{
    public class Billing
    {
        public int BillingId { get; set; }
        public int TenantId { get; set; }
        public decimal Amount { get; set; }
        public DateTime DueDate { get; set; }
        public BillingStatus Status { get; set; }
        public string ProofOfPaymentPath { get; set; }
    }
}