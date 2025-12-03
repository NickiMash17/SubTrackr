/*
 * SubTrackr - Subscription Management Platform
 * Created by: Nicolette Mashaba
 * Copyright (c) 2025 Nicolette Mashaba
 */

using System;
using SubTrackr.Core.Enums;

namespace SubTrackr.Core.Models
{
    public class User
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public UserRole Role { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsActive { get; set; }

        public User()
        {
            Id = Guid.NewGuid().ToString();
            CreatedDate = DateTime.Now;
            IsActive = true;
        }
    }
}