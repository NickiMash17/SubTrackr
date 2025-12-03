/*
 * SubTrackr - Subscription Management Platform
 * Created by: Nicolette Mashaba
 * Copyright (c) 2025 Nicolette Mashaba
 */

using System;
using System.Collections.Generic;
using SubTrackr.Core.Models;
using SubTrackr.Core.Interfaces;
using SubTrackr.Core.Enums;

namespace SubTrackr.Core.Services
{
    public class UserService
    {
        private readonly IRepository<User> _userRepository;

        public UserService(IRepository<User> userRepository)
        {
            _userRepository = userRepository ??
                throw new ArgumentNullException(nameof(userRepository));
        }

        public void AddUser(string name, string email, UserRole role)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be empty");

            if (string.IsNullOrWhiteSpace(email) || !email.Contains("@"))
                throw new ArgumentException("Invalid email address");

            var user = new User
            {
                Name = name,
                Email = email,
                Role = role
            };

            _userRepository.Add(user);
        }

        public void UpdateUser(string userId, string name, string email, UserRole role)
        {
            var user = _userRepository.GetById(userId);
            if (user == null)
                throw new InvalidOperationException($"User with ID {userId} not found");

            user.Name = name;
            user.Email = email;
            user.Role = role;

            _userRepository.Update(user);
        }

        public void RemoveUser(string userId)
        {
            _userRepository.Delete(userId);
        }

        public User GetUser(string userId)
        {
            return _userRepository.GetById(userId);
        }

        public List<User> GetAllUsers()
        {
            return _userRepository.GetAll();
        }
    }
}