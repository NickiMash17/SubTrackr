using System;
using System.Linq;
using SubTrackr.Core.Models;

namespace SubTrackr.Core.Repositories
{
    public class UserRepository : GenericRepository<User>
    {
        public UserRepository() : base(u => u.Id) { }

        public User GetByEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return null;

            return _data.FirstOrDefault(u =>
                u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
        }
    }
}