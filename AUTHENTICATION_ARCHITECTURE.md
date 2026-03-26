# PMTool Authentication System - Clean Architecture Implementation

## Overview
A complete authentication backend following **clean architecture** principles with email/password login, 2FA, session timeout, password reset, and account lockout.

## Architecture Layers

### 1. **Domain Layer** (PMTool.Domain)
Contains business entities and enums - completely independent of external frameworks.

**Files:**
- `Entities/User.cs` - Core user entity with auth-related fields
  - Email & password management
  - Account lockout tracking (failed attempts, lockout end)
  - 2FA fields (code, expiry)
  - Password reset token management
  - Email confirmation token
  - Session management (last login, session expiry)
  
- `Enums/AuthResult.cs` - Authentication result status codes

### 2. **Application Layer** (PMTool.Application)
Contains business logic, DTOs, and validators - orchestrates use cases.

**Files:**
- `DTOs/Auth/`:
  - `RegisterRequest.cs` - User registration input
  - `LoginRequest.cs` - Login credentials
  - `LoginResponse.cs` - Login result with optional 2FA token
  - `TwoFactorRequest.cs` - 2FA code verification

- `Validators/Auth/`:
  - `RegisterRequestValidator.cs` - Fluent validation for registration (password strength, email format, etc.)
  - `LoginRequestValidator.cs` - Login input validation

- `Services/Auth/`:
  - `IAuthenticationService.cs` - Service interface
  - `AuthenticationService.cs` - Core authentication business logic:
    - **Login**: Email/password validation, failed attempt tracking, account lockout (5 attempts in 15 min), 2FA trigger
    - **2FA**: Code generation and verification (5-minute validity)
    - **Register**: User creation with email confirmation token
    - **Email Confirmation**: Token-based email verification
    - **Password Reset**: Self-service password reset with time-limited tokens (1 hour)
    - **Enable/Disable 2FA**: User preference management

### 3. **Infrastructure Layer** (PMTool.Infrastructure)
Implements data access, external services, and technical concerns.

**Files:**
- `Data/AppDbContext.cs` - EF Core database context with User DbSet and model configuration
  - Email unique index
  - Automatic CreatedAt/UpdatedAt timestamps

- `Repositories/`:
  - `Interfaces/IUserRepository.cs` - Repository contract
  - `UserRepository.cs` - User data access implementation (CRUD operations)

- `Services/`:
  - `Interfaces/IEmailService.cs` - Email service contract
  - `Interfaces/ITokenService.cs` - Token and password hashing contract
  - `EmailService.cs` - Email sending (placeholder for SMTP/SendGrid integration)
    - Password reset emails
    - Email confirmation emails
    - 2FA code delivery
    - Account locked notifications
  - `TokenService.cs` - Token generation and password hashing using BCrypt
    - Secure random token generation (base64)
    - Password hashing with BCrypt (workFactor: 12)
    - Password verification

### 4. **Web Layer** (PMTool.Web)
Razor Pages presentation layer - uses application services.

**Configuration in Program.cs:**
```csharp
// DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Infrastructure Services
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Application Services
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
```

## Key Features Implemented

### 1. Email & Password Login
- User lookup by email
- BCrypt password verification
- Invalid credentials handling

### 2. Account Lockout
- Tracks failed login attempts
- Locks account after 5 failed attempts in 15 minutes
- Lockout notification via email
- Auto-unlock after timeout

### 3. Two-Factor Authentication (TFA)
- 6-digit code generation
- 5-minute validity
- Email delivery
- Enable/disable by user

### 4. Session Timeout
- 30-minute default session duration
- Updated on successful login or 2FA verification
- SessionExpiresAt tracked in database

### 5. Password Reset
- Self-service password reset via email link
- Time-limited tokens (1 hour validity)
- Email link generation with secure tokens
- Account lockout cleared on password reset

### 6. Email Confirmation
- Verification tokens sent on registration
- 24-hour token validity
- Required before login is allowed

## Database Schema
**Users Table:**
```
Id (GUID, PK)
Email (string, unique)
PasswordHash (string)
FirstName (string)
LastName (string)
EmailConfirmed (bool)
TwoFactorEnabled (bool)
IsActive (bool)
FailedLoginAttempts (int)
LockoutEnd (datetime, nullable)
PasswordResetToken (string, nullable)
PasswordResetTokenExpiry (datetime, nullable)
EmailConfirmationToken (string, nullable)
EmailConfirmationTokenExpiry (datetime, nullable)
TwoFactorCode (string, nullable)
TwoFactorCodeExpiry (datetime, nullable)
LastLoginAt (datetime, nullable)
SessionExpiresAt (datetime, nullable)
CreatedAt (datetime)
UpdatedAt (datetime)
```

## NuGet Dependencies
- **BCrypt.Net-Core** (1.6.0) - Password hashing
- **Microsoft.EntityFrameworkCore.SqlServer** (10.0.5) - Database access
- **FluentValidation** (12.1.1) - DTO validation
- **AutoMapper** (16.1.1) - DTO mapping (ready for use)
- **MediatR** (14.1.0) - CQRS pattern (ready for use)

## Next Steps - Razor Pages Implementation

You'll need to create these Razor Pages:

1. **Auth/Login.cshtml** - Login form → calls `AuthenticationService.LoginAsync()`
2. **Auth/LoginTwoFactor.cshtml** - 2FA code entry → calls `AuthenticationService.VerifyTwoFactorCodeAsync()`
3. **Auth/Register.cshtml** - Registration form → calls `AuthenticationService.RegisterAsync()`
4. **Auth/ConfirmEmail.cshtml** - Email confirmation handler → calls `AuthenticationService.ConfirmEmailAsync()`
5. **Auth/ForgotPassword.cshtml** - Password reset request → calls `AuthenticationService.RequestPasswordResetAsync()`
6. **Auth/ResetPassword.cshtml** - New password form → calls `AuthenticationService.ResetPasswordAsync()`
7. **Auth/Enable2FA.cshtml** - 2FA settings → calls `AuthenticationService.EnableTwoFactorAsync()` / `DisableTwoFactorAsync()`
8. **Account/Dashboard.cshtml** - Protected page (requires authentication middleware)

## Email Service Implementation
Currently uses Console.WriteLine placeholders. To implement actual email:

**Option 1: SMTP**
```csharp
using System.Net.Mail;
// Configure SMTP settings in appsettings.json
```

**Option 2: Azure SendGrid**
```csharp
// Add NuGet: SendGrid
using SendGrid;
using SendGrid.Helpers.Mail;
```

**Option 3: Azure Communication Services**
```csharp
// Add NuGet: Azure.Communication.Email
```

## Security Considerations

✅ **Implemented:**
- BCrypt password hashing (industry standard)
- Secure random token generation
- Account lockout protection
- Time-limited tokens
- Email verification

⚠️ **TODO:**
- Add authentication middleware/cookies for Razor Pages
- Add HTTPS enforcement
- Implement CSRF protection
- Add rate limiting on login endpoint
- Implement actual email sending
- Add logging and monitoring
- Add password complexity rules to configuration
- Implement refresh tokens if using APIs

## File Structure
```
PMTool.Domain/
  ├── Entities/
  │   └── User.cs
  └── Enums/
      └── AuthResult.cs

PMTool.Application/
  ├── DTOs/
  │   └── Auth/
  │       ├── RegisterRequest.cs
  │       ├── LoginRequest.cs
  │       ├── LoginResponse.cs
  │       └── TwoFactorRequest.cs
  ├── Validators/
  │   └── Auth/
  │       ├── RegisterRequestValidator.cs
  │       └── LoginRequestValidator.cs
  └── Services/
      └── Auth/
          ├── IAuthenticationService.cs
          └── AuthenticationService.cs

PMTool.Infrastructure/
  ├── Data/
  │   └── AppDbContext.cs
  ├── Repositories/
  │   ├── Interfaces/
  │   │   └── IUserRepository.cs
  │   └── UserRepository.cs
  └── Services/
      ├── Interfaces/
      │   ├── IEmailService.cs
      │   └── ITokenService.cs
      ├── EmailService.cs
      └── TokenService.cs

PMTool.Web/
  ├── Program.cs (configured with all services)
  ├── appsettings.json (AppUrl configured)
  └── Pages/ (to be created)
```

## How to Use

1. **Inject `IAuthenticationService` into Razor Pages:**
```csharp
public class LoginModel : PageModel
{
    private readonly IAuthenticationService _authService;
    
    public LoginModel(IAuthenticationService authService)
    {
        _authService = authService;
    }
    
    public async Task<IActionResult> OnPostAsync(LoginRequest request)
    {
        var result = await _authService.LoginAsync(request);
        // Handle result...
    }
}
```

2. **Validate DTOs:**
```csharp
var validator = new LoginRequestValidator();
var validationResult = await validator.ValidateAsync(request);
```

3. **Handle responses appropriately** for Razor Pages (sessions, cookies, redirects)

---

Clean architecture ensures:
- ✅ Business logic isolated from frameworks
- ✅ Easy testing of core functionality
- ✅ Technology can be swapped without changing business logic
- ✅ Clear separation of concerns
- ✅ Scalability and maintainability
