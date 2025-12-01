namespace SubTrackr.Core.Models
{
    // Demonstrates Inheritance and Polymorphism
    public class BasicSubscription : SubscriptionBase
    {
        public int MaxDevices { get; set; }

        public BasicSubscription() : base()
        {
            MaxDevices = 1;
        }

        public override decimal CalculateRenewalCost()
        {
            // Basic subscriptions have standard pricing
            return Cost;
        }
    }
}