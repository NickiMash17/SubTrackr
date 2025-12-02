using System;
using System.Collections.Generic;
using SubTrackr.Core.Models;
using SubTrackr.Core.Enums;

namespace SubTrackr.Core.Factories
{
    public class SubscriptionFactory
    {
        public static SubscriptionBase CreateSubscription(
            string type,
            string userId,
            string planName,
            decimal cost,
            RenewalFrequency frequency)
        {
            if (string.IsNullOrWhiteSpace(type))
                throw new ArgumentException("Subscription type cannot be empty");

            if (cost <= 0)
                throw new ArgumentException("Cost must be greater than zero");

            switch (type.ToLower())
            {
                case "basic":
                    return new BasicSubscription
                    {
                        UserId = userId,
                        PlanName = planName,
                        Cost = cost,
                        RenewalFrequency = frequency,
                        MaxDevices = 1
                    };

                case "premium":
                    return new PremiumSubscription
                    {
                        UserId = userId,
                        PlanName = planName,
                        Cost = cost,
                        RenewalFrequency = frequency,
                        DiscountPercentage = 10,
                        BonusFeatures = new List<string>
                        {
                            "Priority Support",
                            "Advanced Analytics"
                        }
                    };

                default:
                    throw new ArgumentException($"Unknown subscription type: {type}");
            }
        }
    }
}