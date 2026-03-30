using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PMTool.Application.Services.User;
using PMTool.Infrastructure.Repositories.Interfaces;
using PMTool.Infrastructure.Services.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace PMTool.Web.Pages.Auth;

public class SetupAccountModel : PageModel
{
    private readonly IUserAdminRepository _userRepository;
    private readonly ITokenService _tokenService;
    private readonly ILogger<SetupAccountModel> _logger;

    public SetupAccountModel(
        IUserAdminRepository userRepository,
        ITokenService tokenService,
        ILogger<SetupAccountModel> logger)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
        _logger = logger;
    }

    [BindProperty]
    public SetupAccountInput Input { get; set; } = new();

    [FromQuery]
    public string? Token { get; set; }

    [FromQuery]
    public string? Email { get; set; }

    public bool? IsTokenValid { get; set; }
    public bool IsLoading { get; set; } = true;
    public string? ErrorMessage { get; set; }
    public string? SuccessMessage { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        // Check if token and email are provided
        if (string.IsNullOrEmpty(Token) || string.IsNullOrEmpty(Email))
        {
            IsTokenValid = false;
            _logger.LogWarning("Setup account attempt with missing token or email");
            return Page();
        }

        // Get the user by email
        var user = await _userRepository.GetByEmailAsync(Email);
        if (user == null)
        {
            IsTokenValid = false;
            _logger.LogWarning("Setup account attempt for non-existent email: {Email}", Email);
            return Page();
        }

        // Check if invitation token has expired
        if (user.InvitationTokenExpiry < DateTime.UtcNow)
        {
            IsTokenValid = false;
            _logger.LogWarning("Setup account attempt with expired token for email: {Email}", Email);
            return Page();
        }

        // Verify the token
        if (user.InvitationToken == null || !_tokenService.VerifyPassword(Token, user.InvitationToken))
        {
            IsTokenValid = false;
            _logger.LogWarning("Setup account attempt with invalid token for email: {Email}", Email);
            return Page();
        }

        // Token is valid
        IsTokenValid = true;
        IsLoading = false;

        // Pre-fill the names if already set
        if (!string.IsNullOrEmpty(user.FirstName))
            Input.FirstName = user.FirstName;
        if (!string.IsNullOrEmpty(user.LastName))
            Input.LastName = user.LastName;

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        // Validate model state
        if (!ModelState.IsValid)
        {
            ErrorMessage = "Please fix the errors below and try again.";
            IsTokenValid = true;
            IsLoading = false;
            return Page();
        }

        // Check if token and email are provided
        if (string.IsNullOrEmpty(Token) || string.IsNullOrEmpty(Email))
        {
            ErrorMessage = "Invalid request. Please use the link from your invitation email.";
            IsTokenValid = false;
            return Page();
        }

        // Get the user by email
        var user = await _userRepository.GetByEmailAsync(Email);
        if (user == null)
        {
            ErrorMessage = "User not found. Please contact your administrator.";
            IsTokenValid = false;
            _logger.LogWarning("Setup account submission for non-existent email: {Email}", Email);
            return Page();
        }

        // Check if invitation token has expired
        if (user.InvitationTokenExpiry < DateTime.UtcNow)
        {
            ErrorMessage = "Your invitation link has expired. Please contact your administrator for a new invitation.";
            IsTokenValid = false;
            _logger.LogWarning("Setup account submission with expired token for email: {Email}", Email);
            return Page();
        }

        // Verify the token
        if (user.InvitationToken == null || !_tokenService.VerifyPassword(Token, user.InvitationToken))
        {
            ErrorMessage = "Invalid invitation link. Please use the link from your invitation email.";
            IsTokenValid = false;
            _logger.LogWarning("Setup account submission with invalid token for email: {Email}", Email);
            return Page();
        }

        try
        {
            // Update user information
            user.FirstName = Input.FirstName;
            user.LastName = Input.LastName;
            user.PasswordHash = _tokenService.HashPassword(Input.Password);
            user.EmailConfirmed = true;
            user.AccountSetupCompleted = true;
            user.InvitationToken = null;
            user.InvitationTokenExpiry = null;
            user.CreatedAt = DateTime.UtcNow;

            // Save user
            var updated = await _userRepository.UpdateAsync(user);
            if (!updated)
            {
                ErrorMessage = "Failed to complete account setup. Please try again.";
                IsTokenValid = true;
                IsLoading = false;
                _logger.LogError("Failed to update user during setup account for email: {Email}", Email);
                return Page();
            }

            _logger.LogInformation("User {Email} completed account setup successfully", Email);

            // Redirect to login with success message
            TempData["SuccessMessage"] = "Account setup completed successfully! You can now log in with your credentials.";
            return RedirectToPage("/Auth/Login");
        }
        catch (Exception ex)
        {
            ErrorMessage = "An unexpected error occurred. Please try again later.";
            IsTokenValid = true;
            IsLoading = false;
            _logger.LogError(ex, "Exception during account setup for email: {Email}", Email);
            return Page();
        }
    }

    public class SetupAccountInput
    {
        [Required(ErrorMessage = "First name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "First name must be between 2 and 100 characters")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Last name must be between 2 and 100 characters")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [StringLength(128, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters long")]
        [RegularExpression(
            @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*])[a-zA-Z\d!@#$%^&*]{8,}$",
            ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character (!@#$%^&*)"
        )]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please confirm your password")]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
