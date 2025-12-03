# 🎯 SubTrackr

<div align="center">

**A Modern Subscription Management Platform**

*Track, manage, and monitor your subscriptions with ease*

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![C#](https://img.shields.io/badge/C%23-12.0-239120?logo=c-sharp)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)
[![Tests](https://img.shields.io/badge/tests-51%20passing-brightgreen)](https://github.com/NickiMash17/SubTrackr)

</div>

---

## 📋 Table of Contents

- [Overview](#-overview)
- [Features](#-features)
- [Architecture](#-architecture)
- [Getting Started](#-getting-started)
- [Usage](#-usage)
- [Project Structure](#-project-structure)
- [Testing](#-testing)
- [Technologies](#-technologies)
- [Contributing](#-contributing)

---

## 🎯 Overview

**SubTrackr** is a comprehensive subscription management system built with .NET 8.0. It provides a complete solution for managing users, subscriptions, payments, and generating detailed reports. The application demonstrates modern software engineering principles including clean architecture, design patterns, and comprehensive testing.

### Key Highlights

✨ **Fully Functional Console Application** - Interactive menu-driven interface  
🧪 **Comprehensive Test Suite** - 51 passing tests covering unit, integration, and non-functional testing  
🏗️ **Clean Architecture** - Well-organized layers with separation of concerns  
💾 **Data Persistence** - JSON-based file storage for all entities  
📊 **Advanced Reporting** - Monthly reports with detailed analytics  

---

## ✨ Features

### 👥 User Management
- Create, update, and remove users
- Role-based access control (Customer, Admin, System Process)
- User profile management

### 📦 Subscription Management
- **Basic & Premium** subscription types
- Create, update, cancel, and renew subscriptions
- Support for Monthly, Quarterly, and Yearly renewal frequencies
- Automatic renewal date calculation
- Subscription status tracking

### 💳 Payment Processing
- Process payments with transaction tracking
- Automatic retry mechanism for failed payments
- Payment history tracking
- Transaction reference generation

### 🔔 Notification System
- Renewal reminders
- Payment failure notifications
- Cancellation confirmations
- Read/unread status tracking

### 📊 Reporting & Analytics
- Monthly subscription reports
- Payment history analysis
- Failed payment tracking
- Export reports to text files

---

## 🏗️ Architecture

SubTrackr follows a **layered architecture** pattern with clear separation of concerns:

```
┌─────────────────────────────────────┐
│      SubTrackr.Console              │  ← Presentation Layer
│      (User Interface)                │
└──────────────┬──────────────────────┘
               │
┌──────────────▼──────────────────────┐
│      SubTrackr.Core                 │  ← Business Logic Layer
│  ┌──────────────────────────────┐  │
│  │ Services                      │  │
│  │ Repositories                  │  │
│  │ Models                        │  │
│  │ Interfaces                    │  │
│  │ Factories                     │  │
│  └──────────────────────────────┘  │
└─────────────────────────────────────┘
               │
┌──────────────▼──────────────────────┐
│      Data Persistence              │  ← Data Layer
│      (JSON File Storage)          │
└────────────────────────────────────┘
```

### Design Patterns Implemented

- **Repository Pattern** - Abstract data access layer
- **Factory Pattern** - Subscription creation
- **Service Layer Pattern** - Business logic encapsulation
- **Dependency Injection** - Loose coupling between components

---

## 🚀 Getting Started

### Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or later
- Visual Studio 2022 / VS Code / Rider (optional)

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/NickiMash17/SubTrackr.git
   cd SubTrackr
   ```

2. **Restore dependencies**
   ```bash
   dotnet restore
   ```

3. **Build the solution**
   ```bash
   dotnet build
   ```

4. **Run the console application**
   ```bash
   dotnet run --project SubTrackr.Console/SubTrackr.Console.csproj
   ```

---

## 💻 Usage

### Running the Console App

```bash
# From the solution root
dotnet run --project SubTrackr.Console/SubTrackr.Console.csproj
```

The application will display a welcome screen and interactive menu:

```
╔════════════════════════════════════════╗
║   Welcome to SubTrackr System          ║
║   Subscription Management Platform     ║
╚════════════════════════════════════════╝

╔════════════════════════════════════════╗
║           MAIN MENU                    ║
╚════════════════════════════════════════╝
1. User Management
2. Subscription Management
3. Payment Processing
4. View Notifications
5. Generate Reports
6. Save & Exit
```

### Example Workflow

1. **Create a User**
   - Select option `1` → `1` (Add User)
   - Enter name, email, and select role

2. **Create a Subscription**
   - Select option `2` → `1` (Create Subscription)
   - Choose subscription type (Basic/Premium)
   - Enter plan details and renewal frequency

3. **Process Payment**
   - Select option `3` → `1` (Process Payment)
   - Enter user ID, subscription ID, and amount

4. **Generate Report**
   - Select option `5` (Generate Reports)
   - Enter user ID, month, and year
   - Optionally save report to file

---

## 📁 Project Structure

```
SubTrackr/
├── SubTrackr.Core/                    # Core business logic
│   ├── Models/                        # Domain models
│   │   ├── User.cs
│   │   ├── SubscriptionBase.cs        # Abstract base class
│   │   ├── BasicSubscription.cs
│   │   ├── PremiumSubscription.cs
│   │   ├── Payment.cs
│   │   ├── Notification.cs
│   │   └── MonthlyReport.cs
│   ├── Services/                      # Business logic services
│   │   ├── UserService.cs
│   │   ├── SubscriptionService.cs
│   │   ├── PaymentService.cs
│   │   ├── NotificationService.cs
│   │   └── ReportService.cs
│   ├── Repositories/                  # Data access layer
│   │   ├── GenericRepository.cs
│   │   ├── UserRepository.cs
│   │   ├── SubscriptionRepository.cs
│   │   ├── PaymentRepository.cs
│   │   └── NotificationRepository.cs
│   ├── Interfaces/                    # Contracts
│   ├── Factories/                     # Factory pattern
│   └── Enums/                         # Enumerations
│
├── SubTrackr.Console/                 # Console application
│   ├── Program.cs                     # Entry point
│   └── MenuSystem.cs                  # Menu system
│
└── SubTrackr.Tests/                   # Test suite
    ├── UnitTests/                     # Unit tests
    │   ├── UserServiceTests.cs
    │   ├── SubscriptionServiceTests.cs
    │   ├── PaymentServiceTests.cs
    │   └── ReportServiceTests.cs
    ├── IntegrationTests/              # Integration tests
    │   ├── SubscriptionPaymentIntegrationTests.cs
    │   └── EndToEndTests.cs
    ├── NonFunctionalTests/            # Performance & reliability
    │   ├── PerformanceTests.cs
    │   └── ReliabilityTests.cs
    └── TestHelpers/                   # Test utilities
        └── MockFactory.cs
```

---

## 🧪 Testing

The project includes a comprehensive test suite with **51 passing tests** covering:

### Unit Tests
- Service layer testing with mocks
- Model validation and business logic
- Factory pattern testing

### Integration Tests
- End-to-end workflows
- Data persistence verification
- Service integration testing

### Non-Functional Tests
- Performance benchmarks
- Reliability and error handling
- Concurrent operation testing

### Running Tests

```bash
# Run all tests
dotnet test SubTrackr.Tests/SubTrackr.Tests.csproj

# Run with detailed output
dotnet test SubTrackr.Tests/SubTrackr.Tests.csproj --verbosity normal

# Run specific test category
dotnet test --filter "FullyQualifiedName~UnitTests"
dotnet test --filter "FullyQualifiedName~IntegrationTests"
```

### Test Results

```
✅ Total: 52 tests
✅ Passed: 51
⏭️  Skipped: 1 (expected - polymorphic JSON limitation)
❌ Failed: 0
```

---

## 🛠️ Technologies

- **.NET 8.0** - Modern C# runtime
- **C# 12** - Latest language features
- **NUnit 3.14** - Testing framework
- **Moq 4.20** - Mocking framework
- **System.Text.Json** - JSON serialization

---

## 📊 Data Storage

SubTrackr uses JSON file-based storage for simplicity:

- `users.json` - User data
- `subscriptions.json` - Subscription records
- `payments.json` - Payment history
- `notifications.json` - Notification records

Data is automatically loaded on startup and saved on exit.

---

## 🎨 Key Features in Detail

### Subscription Types

**Basic Subscription**
- Standard pricing
- Single device support
- No discounts

**Premium Subscription**
- Discounted renewal pricing
- Multiple bonus features
- Extended grace period (7 days)

### Payment Processing

- **90% success rate** simulation
- Automatic retry mechanism (max 3 attempts)
- Transaction reference tracking
- Failure reason logging

### Reporting

Monthly reports include:
- Active subscriptions summary
- Payment history
- Total amount billed
- Failed payment count
- Export to text file

---

## 🤝 Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

---

## 📝 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

## 👤 Author

**Nicolette Mashaba**

Created by **Nicolette Mashaba**

- GitHub: [@NickiMash17](https://github.com/NickiMash17)
- Repository: [SubTrackr](https://github.com/NickiMash17/SubTrackr)

---

## 🙏 Acknowledgments

- Built with modern .NET best practices
- Comprehensive test coverage
- Clean architecture principles

---

<div align="center">

**Made with ❤️ using .NET 8.0**

⭐ Star this repo if you find it helpful!

---

**Created by Nicolette Mashaba** 🎨

*SubTrackr - Your subscription management solution*

</div>
