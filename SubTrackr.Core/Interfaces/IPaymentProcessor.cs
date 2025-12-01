using System.Collections.Generic;
using SubTrackr.Core.Models;

namespace SubTrackr.Core.Interfaces
{
    public interface IPaymentProcessor
    {
        Payment ProcessPayment(User user, SubscriptionBase subscription, decimal amount);
        bool RetryFailedPayment(Payment payment);
        List<Payment> GetPaymentHistory(string userId);
    }
}
