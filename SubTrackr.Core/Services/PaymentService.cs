using System;
using System.Collections.Generic;
using System.Linq;
using SubTrackr.Core.Models;
using SubTrackr.Core.Interfaces;
using SubTrackr.Core.Enums;

namespace SubTrackr.Core.Services
{
    public class PaymentService : IPaymentProcessor
    {
        private readonly IRepository<Payment> _paymentRepository;
        private readonly INotificationService _notificationService;
        private readonly Random _random;

        public PaymentService(IRepository<Payment> paymentRepository,
            INotificationService notificationService)
        {
            _paymentRepository = paymentRepository ??
                throw new ArgumentNullException(nameof(paymentRepository));
            _notificationService = notificationService ??
                throw new ArgumentNullException(nameof(notificationService));
            _random = new Random();
        }

        public Payment ProcessPayment(User user, SubscriptionBase subscription, decimal amount)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (subscription == null) throw new ArgumentNullException(nameof(subscription));
            if (amount <= 0)
                throw new ArgumentException("Payment amount must be greater than zero");

            var payment = new Payment
            {
                UserId = user.Id,
                SubscriptionId = subscription.Id,
                Amount = amount,
                TransactionReference = GenerateTransactionReference()
            };

            // Simulate payment processing (90% success rate)
            bool isSuccessful = _random.Next(100) < 90;

            if (isSuccessful)
            {
                payment.Status = PaymentStatus.Success;
            }
            else
            {
                payment.Status = PaymentStatus.Failed;
                payment.FailureReason = "Insufficient funds";
                _notificationService.SendPaymentFailedNotification(user, payment);
            }

            _paymentRepository.Add(payment);
            return payment;
        }

        public bool RetryFailedPayment(Payment payment)
        {
            if (payment == null) throw new ArgumentNullException(nameof(payment));
            if (payment.Status != PaymentStatus.Failed)
                throw new InvalidOperationException("Can only retry failed payments");

            if (payment.RetryCount >= 3)
                return false;

            payment.RetryCount++;

            // Simulate retry (70% success rate)
            bool isSuccessful = _random.Next(100) < 70;

            if (isSuccessful)
            {
                payment.Status = PaymentStatus.Success;
                payment.PaymentDate = DateTime.Now;
            }

            _paymentRepository.Update(payment);
            return isSuccessful;
        }

        public List<Payment> GetPaymentHistory(string userId)
        {
            return _paymentRepository.GetAll()
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.PaymentDate)
                .ToList();
        }

        private string GenerateTransactionReference()
        {
            return $"TXN-{DateTime.Now:yyyyMMddHHmmss}-{_random.Next(1000, 9999)}";
        }
    }
}