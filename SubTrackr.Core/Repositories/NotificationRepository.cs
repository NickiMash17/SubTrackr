using System.Collections.Generic;
using System.Linq;
using SubTrackr.Core.Models;

namespace SubTrackr.Core.Repositories
{
    public class NotificationRepository : GenericRepository<Notification>
    {
        public NotificationRepository() : base(n => n.Id) { }

        public List<Notification> GetByUserId(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return new List<Notification>();

            return _data.Where(n => n.UserId == userId).ToList();
        }

        public List<Notification> GetUnreadNotifications(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return new List<Notification>();

            return _data.Where(n => n.UserId == userId && !n.IsRead).ToList();
        }
    }
}