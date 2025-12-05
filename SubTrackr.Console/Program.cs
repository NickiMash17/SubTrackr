/*
 * SubTrackr - Subscription Management Platform
 * 
 * Created by: Nicolette Mashaba
 * GitHub: https://github.com/NickiMash17/SubTrackr
 * 
 * Copyright (c) 2025 Nicolette Mashaba
 */

using System;
using System.Linq;
using SubTrackr.Core.Models;
using SubTrackr.Core.Services;
using SubTrackr.Core.Repositories;
using SubTrackr.Core.Enums;

namespace SubTrackr.ConsoleApp
{
    class Program
    {
        private static UserRepository _userRepo = null!;
        private static SubscriptionRepository _subscriptionRepo = null!;
        private static PaymentRepository _paymentRepo = null!;
        private static NotificationRepository _notificationRepo = null!;

        private static UserService _userService = null!;
        private static NotificationService _notificationService = null!;
        private static PaymentService _paymentService = null!;
        private static SubscriptionService _subscriptionService = null!;
        private static ReportService _reportService = null!;

        private const string UsersFile = "users.json";
        private const string SubscriptionsFile = "subscriptions.json";
        private const string PaymentsFile = "payments.json";
        private const string NotificationsFile = "notifications.json";

        static void Main(string[] args)
        {
            try
            {
                InitializeSystem();
                LoadData();

                Console.WriteLine("╔════════════════════════════════════════╗");
                Console.WriteLine("║   Welcome to SubTrackr System          ║");
                Console.WriteLine("║   Subscription Management Platform     ║");
                Console.WriteLine("╚════════════════════════════════════════╝");
                Console.WriteLine();

                bool running = true;
                while (running)
                {
                    running = ShowMainMenu();
                }

                SaveData();
                Console.WriteLine("\nThank you for using SubTrackr!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n[ERROR] Application error: {ex.Message}");
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
            }
        }

        static void InitializeSystem()
        {
            _userRepo = new UserRepository();
            _subscriptionRepo = new SubscriptionRepository();
            _paymentRepo = new PaymentRepository();
            _notificationRepo = new NotificationRepository();

            _notificationService = new NotificationService(_notificationRepo);
            _paymentService = new PaymentService(_paymentRepo, _notificationService);
            _userService = new UserService(_userRepo);
            _subscriptionService = new SubscriptionService(_subscriptionRepo, _notificationService);
            _reportService = new ReportService(_userRepo, _subscriptionRepo, _paymentRepo);
        }

        static void LoadData()
        {
            try
            {
                // Get the directory where the executable is located
                string exeDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string usersPath = Path.Combine(exeDirectory, UsersFile);
                string subscriptionsPath = Path.Combine(exeDirectory, SubscriptionsFile);
                string paymentsPath = Path.Combine(exeDirectory, PaymentsFile);
                string notificationsPath = Path.Combine(exeDirectory, NotificationsFile);

                _userRepo.LoadFromFile(usersPath);
                _subscriptionRepo.LoadFromFile(subscriptionsPath);
                _paymentRepo.LoadFromFile(paymentsPath);
                _notificationRepo.LoadFromFile(notificationsPath);

                Console.WriteLine("[INFO] Data loaded successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[WARNING] Could not load previous data: {ex.Message}");
                Console.WriteLine("Starting with empty database...");
            }
        }

        static void SaveData()
        {
            try
            {
                // Get the directory where the executable is located
                string exeDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string usersPath = Path.Combine(exeDirectory, UsersFile);
                string subscriptionsPath = Path.Combine(exeDirectory, SubscriptionsFile);
                string paymentsPath = Path.Combine(exeDirectory, PaymentsFile);
                string notificationsPath = Path.Combine(exeDirectory, NotificationsFile);

                _userRepo.SaveToFile(usersPath);
                _subscriptionRepo.SaveToFile(subscriptionsPath);
                _paymentRepo.SaveToFile(paymentsPath);
                _notificationRepo.SaveToFile(notificationsPath);

                Console.WriteLine("\n[INFO] Data saved successfully");
                Console.WriteLine($"[INFO] Files saved to: {exeDirectory}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n[ERROR] Failed to save data: {ex.Message}");
                Console.WriteLine($"[ERROR] Stack trace: {ex.StackTrace}");
            }
        }

        static bool ShowMainMenu()
        {
            Console.WriteLine("\n╔════════════════════════════════════════╗");
            Console.WriteLine("║           MAIN MENU                    ║");
            Console.WriteLine("╚════════════════════════════════════════╝");
            Console.WriteLine("1. User Management");
            Console.WriteLine("2. Subscription Management");
            Console.WriteLine("3. Payment Processing");
            Console.WriteLine("4. View Notifications");
            Console.WriteLine("5. Generate Reports");
            Console.WriteLine("6. Save & Exit");
            Console.Write("\nSelect option: ");

            string choice = Console.ReadLine() ?? "";

            try
            {
                switch (choice)
                {
                    case "1":
                        UserManagementMenu();
                        break;
                    case "2":
                        SubscriptionManagementMenu();
                        break;
                    case "3":
                        PaymentProcessingMenu();
                        break;
                    case "4":
                        ViewNotificationsMenu();
                        break;
                    case "5":
                        GenerateReportsMenu();
                        break;
                    case "6":
                        return false;
                    default:
                        Console.WriteLine("\n[ERROR] Invalid option. Please try again.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n[ERROR] {ex.Message}");
            }

            return true;
        }
        static void UserManagementMenu()
        {
            Console.WriteLine("\n--- USER MANAGEMENT ---");
            Console.WriteLine("1. Add User");
            Console.WriteLine("2. View All Users");
            Console.WriteLine("3. Update User");
            Console.WriteLine("4. Remove User");
            Console.Write("Select option: ");

            string choice = Console.ReadLine() ?? "";

            switch (choice)
            {
                case "1":
                    AddUser();
                    break;
                case "2":
                    ViewAllUsers();
                    break;
                case "3":
                    UpdateUser();
                    break;
                case "4":
                    RemoveUser();
                    break;
            }
        }

        static void AddUser()
        {
            Console.Write("Enter name: ");
            string name = Console.ReadLine() ?? "";

            Console.Write("Enter email: ");
            string email = Console.ReadLine() ?? "";

            Console.WriteLine("Select role:");
            Console.WriteLine("1. Customer");
            Console.WriteLine("2. Admin");
            Console.WriteLine("3. System Process");
            Console.Write("Choice: ");

            string roleChoice = Console.ReadLine() ?? "";
            UserRole role = roleChoice switch
            {
                "1" => UserRole.Customer,
                "2" => UserRole.Admin,
                "3" => UserRole.SystemProcess,
                _ => UserRole.Customer
            };

            _userService.AddUser(name, email, role);
            Console.WriteLine("\n[SUCCESS] User added successfully!");
        }

        static void ViewAllUsers()
        {
            var users = _userService.GetAllUsers();

            if (users.Count == 0)
            {
                Console.WriteLine("\nNo users found.");
                return;
            }

            Console.WriteLine("\n=== ALL USERS ===");
            Console.WriteLine($"{"ID",-38} {"Name",-20} {"Email",-30} {"Role",-15}");
            Console.WriteLine(new string('-', 105));

            foreach (var user in users)
            {
                Console.WriteLine($"{user.Id,-38} {user.Name,-20} {user.Email,-30} {user.Role,-15}");
            }
        }

        static void UpdateUser()
        {
            Console.Write("Enter user ID to update: ");
            string userId = Console.ReadLine() ?? "";

            var user = _userService.GetUser(userId);
            if (user == null)
            {
                Console.WriteLine("\n[ERROR] User not found.");
                return;
            }

            Console.Write($"Enter new name (current: {user.Name}): ");
            string name = Console.ReadLine() ?? "";
            if (string.IsNullOrWhiteSpace(name)) name = user.Name;

            Console.Write($"Enter new email (current: {user.Email}): ");
            string email = Console.ReadLine() ?? "";
            if (string.IsNullOrWhiteSpace(email)) email = user.Email;

            _userService.UpdateUser(userId, name, email, user.Role);
            Console.WriteLine("\n[SUCCESS] User updated successfully!");
        }

        static void RemoveUser()
        {
            Console.Write("Enter user ID to remove: ");
            string userId = Console.ReadLine() ?? "";

            _userService.RemoveUser(userId);
            Console.WriteLine("\n[SUCCESS] User removed successfully!");
        }
        static void SubscriptionManagementMenu()
        {
            Console.WriteLine("\n--- SUBSCRIPTION MANAGEMENT ---");
            Console.WriteLine("1. Create Subscription");
            Console.WriteLine("2. View All Subscriptions");
            Console.WriteLine("3. Update Subscription");
            Console.WriteLine("4. Cancel Subscription");
            Console.WriteLine("5. Renew Subscription");
            Console.Write("Select option: ");

            string choice = Console.ReadLine() ?? "";

            switch (choice)
            {
                case "1":
                    CreateSubscription();
                    break;
                case "2":
                    ViewAllSubscriptions();
                    break;
                case "3":
                    UpdateSubscription();
                    break;
                case "4":
                    CancelSubscription();
                    break;
                case "5":
                    RenewSubscription();
                    break;
            }
        }

        static void CreateSubscription()
        {
            Console.Write("Enter user ID: ");
            string userId = Console.ReadLine() ?? "";

            Console.WriteLine("Select subscription type:");
            Console.WriteLine("1. Basic");
            Console.WriteLine("2. Premium");
            Console.Write("Choice: ");
            string typeChoice = Console.ReadLine() ?? "";
            string type = typeChoice == "2" ? "premium" : "basic";

            Console.Write("Enter plan name: ");
            string planName = Console.ReadLine() ?? "";

            Console.Write("Enter cost: $");
            string? costInput = Console.ReadLine()?.Trim();
            if (string.IsNullOrWhiteSpace(costInput) || !decimal.TryParse(costInput, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out decimal cost))
            {
                Console.WriteLine("\n[ERROR] Invalid cost.");
                return;
            }

            Console.WriteLine("Select renewal frequency:");
            Console.WriteLine("1. Monthly");
            Console.WriteLine("2. Quarterly");
            Console.WriteLine("3. Yearly");
            Console.Write("Choice: ");
            string freqChoice = Console.ReadLine() ?? "";
            RenewalFrequency frequency = freqChoice switch
            {
                "2" => RenewalFrequency.Quarterly,
                "3" => RenewalFrequency.Yearly,
                _ => RenewalFrequency.Monthly
            };

            _subscriptionService.CreateSubscription(type, userId, planName, cost, frequency);
            Console.WriteLine("\n[SUCCESS] Subscription created successfully!");
        }

        static void ViewAllSubscriptions()
        {
            var subscriptions = _subscriptionService.GetAllSubscriptions();

            if (subscriptions.Count == 0)
            {
                Console.WriteLine("\nNo subscriptions found.");
                return;
            }

            Console.WriteLine("\n=== ALL SUBSCRIPTIONS ===");
            foreach (var sub in subscriptions)
            {
                string type = sub is PremiumSubscription ? "Premium" : "Basic";
                Console.WriteLine($"\n{type} - {sub.PlanName}");
                Console.WriteLine($"  ID: {sub.Id}");
                Console.WriteLine($"  User ID: {sub.UserId}");
                Console.WriteLine($"  Cost: ${sub.Cost:F2} / {sub.RenewalFrequency}");
                Console.WriteLine($"  Status: {sub.Status}");
                Console.WriteLine($"  Start: {sub.StartDate:yyyy-MM-dd}");
                if (sub.EndDate.HasValue)
                    Console.WriteLine($"  End: {sub.EndDate:yyyy-MM-dd}");
            }
        }

        static void UpdateSubscription()
        {
            Console.Write("Enter subscription ID: ");
            string subId = Console.ReadLine() ?? "";

            Console.Write("Enter new plan name: ");
            string planName = Console.ReadLine() ?? "";

            Console.Write("Enter new cost: $");
            string? costInput = Console.ReadLine()?.Trim();
            if (string.IsNullOrWhiteSpace(costInput) || !decimal.TryParse(costInput, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out decimal cost))
            {
                Console.WriteLine("\n[ERROR] Invalid cost.");
                return;
            }

            _subscriptionService.UpdateSubscription(subId, planName, cost);
            Console.WriteLine("\n[SUCCESS] Subscription updated successfully!");
        }

        static void CancelSubscription()
        {
            Console.Write("Enter subscription ID: ");
            string subId = Console.ReadLine() ?? "";

            Console.Write("Enter user ID: ");
            string userId = Console.ReadLine() ?? "";

            var user = _userService.GetUser(userId);
            if (user == null)
            {
                Console.WriteLine("\n[ERROR] User not found.");
                return;
            }

            _subscriptionService.CancelSubscription(subId, user);
            Console.WriteLine("\n[SUCCESS] Subscription cancelled successfully!");
        }

        static void RenewSubscription()
        {
            Console.Write("Enter subscription ID: ");
            string subId = Console.ReadLine() ?? "";

            _subscriptionService.RenewSubscription(subId);
            Console.WriteLine("\n[SUCCESS] Subscription renewed successfully!");
        }
        static void PaymentProcessingMenu()
        {
            Console.WriteLine("\n--- PAYMENT PROCESSING ---");
            Console.WriteLine("1. Process Payment");
            Console.WriteLine("2. Retry Failed Payment");
            Console.WriteLine("3. View Payment History");
            Console.Write("Select option: ");

            string choice = Console.ReadLine() ?? "";

            switch (choice)
            {
                case "1":
                    ProcessPayment();
                    break;
                case "2":
                    RetryFailedPayment();
                    break;
                case "3":
                    ViewPaymentHistory();
                    break;
            }
        }

        static void ProcessPayment()
        {
            Console.Write("Enter user ID: ");
            string userId = Console.ReadLine() ?? "";

            var user = _userService.GetUser(userId);
            if (user == null)
            {
                Console.WriteLine("\n[ERROR] User not found.");
                return;
            }

            Console.Write("Enter subscription ID: ");
            string subId = Console.ReadLine() ?? "";

            var subscription = _subscriptionService.GetAllSubscriptions()
                .FirstOrDefault(s => s.Id == subId);

            if (subscription == null)
            {
                Console.WriteLine("\n[ERROR] Subscription not found.");
                return;
            }

            Console.Write("Enter payment amount: $");
            string? amountInput = Console.ReadLine()?.Trim();
            if (string.IsNullOrWhiteSpace(amountInput) || !decimal.TryParse(amountInput, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out decimal amount))
            {
                Console.WriteLine("\n[ERROR] Invalid amount.");
                return;
            }

            var payment = _paymentService.ProcessPayment(user, subscription, amount);

            if (payment.Status == PaymentStatus.Success)
            {
                Console.WriteLine($"\n[SUCCESS] Payment processed! Transaction: {payment.TransactionReference}");
            }
            else
            {
                Console.WriteLine($"\n[FAILED] Payment failed: {payment.FailureReason}");
            }
        }

        static void RetryFailedPayment()
        {
            Console.Write("Enter payment ID: ");
            string paymentId = Console.ReadLine() ?? "";

            var payment = _paymentRepo.GetById(paymentId);
            if (payment == null)
            {
                Console.WriteLine("\n[ERROR] Payment not found.");
                return;
            }

            bool success = _paymentService.RetryFailedPayment(payment);

            if (success)
            {
                Console.WriteLine("\n[SUCCESS] Payment retry successful!");
            }
            else
            {
                Console.WriteLine("\n[FAILED] Payment retry failed.");
            }
        }

        static void ViewPaymentHistory()
        {
            Console.Write("Enter user ID: ");
            string userId = Console.ReadLine() ?? "";

            var payments = _paymentService.GetPaymentHistory(userId);

            if (payments.Count == 0)
            {
                Console.WriteLine("\nNo payment history found.");
                return;
            }

            Console.WriteLine("\n=== PAYMENT HISTORY ===");
            foreach (var payment in payments)
            {
                Console.WriteLine($"\n{payment.PaymentDate:yyyy-MM-dd HH:mm}");
                Console.WriteLine($"  Amount: ${payment.Amount:F2}");
                Console.WriteLine($"  Status: {payment.Status}");
                Console.WriteLine($"  Transaction: {payment.TransactionReference}");
                if (payment.Status == PaymentStatus.Failed)
                {
                    Console.WriteLine($"  Reason: {payment.FailureReason}");
                    Console.WriteLine($"  Retries: {payment.RetryCount}");
                }
            }
        }

        static void ViewNotificationsMenu()
        {
            Console.Write("Enter user ID: ");
            string userId = Console.ReadLine() ?? "";

            var notifications = _notificationService.GetUserNotifications(userId);

            if (notifications.Count == 0)
            {
                Console.WriteLine("\nNo notifications found.");
                return;
            }

            Console.WriteLine("\n=== NOTIFICATIONS ===");
            foreach (var notification in notifications)
            {
                string status = notification.IsRead ? "[READ]" : "[UNREAD]";
                Console.WriteLine($"\n{status} {notification.Type} - {notification.CreatedDate:yyyy-MM-dd}");
                Console.WriteLine($"  {notification.Message}");
            }
        }

        static void GenerateReportsMenu()
        {
            Console.Write("Enter user ID: ");
            string userId = Console.ReadLine() ?? "";

            Console.Write("Enter month (1-12): ");
            if (!int.TryParse(Console.ReadLine() ?? "", out int month) || month < 1 || month > 12)
            {
                Console.WriteLine("\n[ERROR] Invalid month.");
                return;
            }

            Console.Write("Enter year: ");
            if (!int.TryParse(Console.ReadLine() ?? "", out int year))
            {
                Console.WriteLine("\n[ERROR] Invalid year.");
                return;
            }

            var reportDate = new DateTime(year, month, 1);
            var report = _reportService.GenerateMonthlyReport(userId, reportDate);

            string reportText = _reportService.ExportReportToString(report);
            Console.WriteLine($"\n{reportText}");

            Console.Write("\nSave report to file? (y/n): ");
            if (Console.ReadLine()?.ToLower() == "y")
            {
                string filename = $"report_{userId}_{year}{month:D2}.txt";
                System.IO.File.WriteAllText(filename, reportText);
                Console.WriteLine($"[SUCCESS] Report saved to {filename}");
            }
        }
    }
}
