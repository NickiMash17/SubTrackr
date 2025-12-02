using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SubTrackr.Core.Models;
using SubTrackr.Core.Interfaces;
using SubTrackr.Core.Enums;

namespace SubTrackr.Core.Services
{
    public class ReportService : IReportGenerator
    {
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<SubscriptionBase> _subscriptionRepository;
        private readonly IRepository<Payment> _paymentRepository;

        public ReportService(
            IRepository<User> userRepository,
            IRepository<SubscriptionBase> subscriptionRepository,
            IRepository<Payment> paymentRepository)
        {
            _userRepository = userRepository ??
                throw new ArgumentNullException(nameof(userRepository));
            _subscriptionRepository = subscriptionRepository ??
                throw new ArgumentNullException(nameof(subscriptionRepository));
            _paymentRepository = paymentRepository ??
                throw new ArgumentNullException(nameof(paymentRepository));
        }

        public MonthlyReport GenerateMonthlyReport(string userId, DateTime month)
        {
            var user = _userRepository.GetById(userId);
            if (user == null)
                throw new InvalidOperationException($"User with ID {userId} not found");

            var subscriptions = _subscriptionRepository.GetAll()
                .Where(s => s.UserId == userId && s.IsActive())
                .ToList();

            var payments = _paymentRepository.GetAll()
                .Where(p => p.UserId == userId
                    && p.PaymentDate.Year == month.Year
                    && p.PaymentDate.Month == month.Month)
                .ToList();

            var report = new MonthlyReport
            {
                UserId = userId,
                UserName = user.Name,
                ReportMonth = month,
                ActiveSubscriptions = subscriptions,
                PaymentHistory = payments,
                TotalAmountBilled = payments
                    .Where(p => p.Status == PaymentStatus.Success)
                    .Sum(p => p.Amount),
                FailedPayments = payments.Count(p => p.Status == PaymentStatus.Failed)
            };

            return report;
        }

        public string ExportReportToString(MonthlyReport report)
        {
            var sb = new StringBuilder();

            sb.AppendLine("=== MONTHLY SUBSCRIPTION REPORT ===");
            sb.AppendLine($"User: {report.UserName}");
            sb.AppendLine($"Period: {report.ReportMonth:MMMM yyyy}");
            sb.AppendLine();

            sb.AppendLine("Active Subscriptions:");
            foreach (var sub in report.ActiveSubscriptions)
            {
                sb.AppendLine($"  - {sub.PlanName}: ${sub.Cost:F2}/{sub.RenewalFrequency}");
            }
            sb.AppendLine();

            sb.AppendLine("Payment History:");
            foreach (var payment in report.PaymentHistory)
            {
                sb.AppendLine($"  - {payment.PaymentDate:yyyy-MM-dd}: " +
                             $"${payment.Amount:F2} - {payment.Status}");
            }
            sb.AppendLine();

            sb.AppendLine($"Total Billed: ${report.TotalAmountBilled:F2}");
            sb.AppendLine($"Failed Payments: {report.FailedPayments}");

            return sb.ToString();
        }
    }
}