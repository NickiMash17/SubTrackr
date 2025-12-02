using System.Collections.Generic;
using System.Linq;
using SubTrackr.Core.Models;

namespace SubTrackr.Core.Repositories
{
    public class SubscriptionRepository : GenericRepository<SubscriptionBase>
    {
        public SubscriptionRepository() : base(s => s.Id) { }

        public List<SubscriptionBase> GetByUserId(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return new List<SubscriptionBase>();

            return _data.Where(s => s.UserId == userId).ToList();
        }

        public List<SubscriptionBase> GetActiveSubscriptions()
        {
            return _data.Where(s => s.IsActive()).ToList();
        }
    }
}
