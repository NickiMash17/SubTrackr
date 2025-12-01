using System;
using SubTrackr.Core.Enums;

namespace SubTrackr.Core.Models
{
    // Abstract Base Class - Demonstrates Abstraction
    public abstract class SubscriptionBase
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string PlanName { get; set; }
        public decimal Cost { get; set; }
        public RenewalFrequency RenewalFrequency { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public SubscriptionStatus Status { get; set; }

        protected SubscriptionBase()
        {
            Id = Guid.NewGuid().ToString();
            StartDate = DateTime.Now;
            Status = SubscriptionStatus.Active;
        }

        // Abstract method - must be implemented by derived classes
        public abstract decimal CalculateRenewalCost();

        // Virtual method - can be overridden by derived classes
        public virtual bool IsActive()
        {
            return Status == SubscriptionStatus.Active
                && (EndDate == null || EndDate > DateTime.Now);
        }

        public virtual DateTime GetNextRenewalDate()
        {
            DateTime baseDate = EndDate ?? StartDate;

            return RenewalFrequency switch
            {
                RenewalFrequency.Monthly => baseDate.AddMonths(1),
                RenewalFrequency.Quarterly => baseDate.AddMonths(3),
                RenewalFrequency.Yearly => baseDate.AddYears(1),
                _ => baseDate.AddMonths(1)
            };
        }
    }
}