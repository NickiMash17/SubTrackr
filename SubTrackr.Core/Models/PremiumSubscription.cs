using System.Collections.Generic;

namespace SubTrackr.Core.Models
{
    // Demonstrates Inheritance and Polymorphism
    public class PremiumSubscription : SubscriptionBase
    {
        public List<string> BonusFeatures { get; set; }
        public decimal DiscountPercentage { get; set; }

        public PremiumSubscription() : base()
        {
            BonusFeatures = new List<string>();
            DiscountPercentage = 0;
        }

        public override decimal CalculateRenewalCost()
        {
            // Premium subscriptions can have discounts
            decimal discountAmount = Cost * (DiscountPercentage / 100);
            return Cost - discountAmount;
        }

        public override bool IsActive()
        {
            // Premium subscriptions have extended grace period
            bool baseActive = base.IsActive();
            if (!baseActive && EndDate.HasValue)
            {
                // 7-day grace period for premium
                return System.DateTime.Now <= EndDate.Value.AddDays(7);
            }
            return baseActive;
        }
    }
}