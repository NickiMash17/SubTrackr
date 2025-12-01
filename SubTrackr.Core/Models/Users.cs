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
		public DateTime CreatedDate { get; set; }

		public User()
		{
			Id = Guid.NewGuid().ToString();
			CreatedDate = DateTime.Now;
		}
	}
