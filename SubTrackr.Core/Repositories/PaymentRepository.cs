using System.Collections.Generic;
using System.Linq;
using SubTrackr.Core.Models;
using SubTrackr.Core.Enums;

namespace SubTrackr.Core.Repositories
{
    public class PaymentRepository : GenericRepository<Payment>
    {
        public PaymentRepository() : base(p => p.Id) { }

        public List<Payment> GetByUserId(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return new List<Payment>();

            return _data.Where(p => p.UserId == userId).ToList();
        }

        public List<Payment> GetFailedPayments()
        {
            return _data.Where(p => p.Status == PaymentStatus.Failed).ToList();
        }
    }
}