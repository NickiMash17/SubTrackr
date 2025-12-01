using System;
using SubTrackr.Core.Enums;

namespace SubTrackr.Core.Models
{
    public class Payment
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string SubscriptionId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public PaymentStatus Status { get; set; }
        public string TransactionReference { get; set; }
        public int RetryCount { get; set; }
        public string FailureReason { get; set; }

        public Payment()
        {
            Id = Guid.NewGuid().ToString();
            PaymentDate = DateTime.Now;
            RetryCount = 0;
        }
    }
}