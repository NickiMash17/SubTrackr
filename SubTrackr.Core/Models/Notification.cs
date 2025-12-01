using System;

namespace SubTrackr.Core.Models
{
    public class Notification
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string Message { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsRead { get; set; }
        public string Type { get; set; }

        public Notification()
        {
            Id = Guid.NewGuid().ToString();
            CreatedDate = DateTime.Now;
            IsRead = false;
        }
    }
}