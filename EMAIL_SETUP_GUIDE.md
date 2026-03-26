# Email Service Setup Guide

## Option 1: Gmail SMTP (Currently Configured) ✅ RECOMMENDED

### Step 1: Enable 2-Step Verification
1. Go to [Google Account](https://myaccount.google.com)
2. Click **Security** (left sidebar)
3. Enable **2-Step Verification** if not already enabled

### Step 2: Generate App Password
1. Go to [Google Account](https://myaccount.google.com) → Security
2. Scroll to "App passwords" (appears only if 2FA is enabled)
3. Select **Mail** and **Windows Computer**
4. Google generates a 16-character password
5. Copy this password

### Step 3: Configure appsettings.json
Update your `PMTool.Web/appsettings.json`:

```json
{
  "Email": {
    "SmtpServer": "smtp.gmail.com",
    "SmtpPort": 587,
    "SenderEmail": "your-email@gmail.com",
    "SenderPassword": "your-16-char-app-password",
    "SenderName": "PMTool",
    "UseSSL": true,
    "IsEnabled": true
  }
}
```

**Replace:**
- `your-email@gmail.com` → Your Gmail address
- `your-16-char-app-password` → The 16-character password from Step 2

### Step 4: Test Email Service
1. Run the app
2. Register a new account
3. You'll receive confirmation email at the registered address
4. Check spam folder if not in inbox

**Pros:**
- ✅ Free (uses your Gmail account)
- ✅ No third-party service
- ✅ Secure (credentials stay on server)
- ✅ Already implemented

**Cons:**
- ❌ Limited to ~500 emails/day (Gmail limit)
- ❌ May be flagged as suspicious for high volume

---

## Option 2: SendGrid (Professional - FREE TIER)

### Step 1: Create SendGrid Account
1. Go to [SendGrid](https://sendgrid.com)
2. Sign up for free account
3. Verify email address

### Step 2: Generate API Key
1. Go to Settings → API Keys
2. Create new API Key
3. Copy the key (shown only once)

### Step 3: Install NuGet Package
```powershell
# In Package Manager Console
Install-Package SendGrid -Version 9.28.1
```

### Step 4: Update appsettings.json
```json
{
  "Email": {
    "Provider": "SendGrid",
    "SendGridApiKey": "your-sendgrid-api-key",
    "SenderEmail": "noreply@pmtool.com",
    "SenderName": "PMTool",
    "IsEnabled": true
  }
}
```

### Step 5: Create SendGrid Email Service
```csharp
// PMTool.Infrastructure/Services/SendGridEmailService.cs
using SendGrid;
using SendGrid.Helpers.Mail;
using Microsoft.Extensions.Configuration;
using PMTool.Infrastructure.Services.Interfaces;

public class SendGridEmailService : IEmailService
{
    private readonly SendGridClient _sendGridClient;
    private readonly string _senderEmail;
    private readonly string _senderName;

    public SendGridEmailService(IConfiguration configuration)
    {
        var apiKey = configuration["Email:SendGridApiKey"];
        _sendGridClient = new SendGridClient(apiKey);
        _senderEmail = configuration["Email:SenderEmail"] ?? "noreply@pmtool.com";
        _senderName = configuration["Email:SenderName"] ?? "PMTool";
    }

    public async Task<bool> SendPasswordResetAsync(string email, string resetLink)
    {
        var subject = "Password Reset Request - PMTool";
        var htmlContent = $@"
            <h2>Password Reset Request</h2>
            <p>You requested a password reset for your PMTool account.</p>
            <p><a href='{resetLink}'>Reset Password</a></p>
        ";
        
        return await SendEmailAsync(email, subject, htmlContent);
    }

    // ... similar methods for other email types

    private async Task<bool> SendEmailAsync(string to, string subject, string htmlContent)
    {
        try
        {
            var from = new EmailAddress(_senderEmail, _senderName);
            var toAddress = new EmailAddress(to);
            var msg = MailHelper.CreateSingleEmail(from, toAddress, subject, "", htmlContent);
            
            var response = await _sendGridClient.SendEmailAsync(msg);
            return response.StatusCode == System.Net.HttpStatusCode.Accepted;
        }
        catch
        {
            return false;
        }
    }
}
```

**Pros:**
- ✅ Professional service
- ✅ Free tier: 100 emails/day
- ✅ Excellent deliverability
- ✅ Good for production

**Cons:**
- ❌ Requires account setup
- ❌ Limited free tier

---

## Option 3: EmailJS (Client-Side) ⚠️ NOT RECOMMENDED FOR BACKEND

EmailJS is **primarily frontend JavaScript**, but we could add it to your Razor Pages. However, this is **NOT recommended** because:
- ❌ API keys exposed in browser
- ❌ Security risk
- ❌ Better to handle email on server

**Only use if you want frontend fallback.** Here's how:

### Step 1: Setup EmailJS Account
1. Go to [EmailJS](https://www.emailjs.com)
2. Sign up free
3. Create email service (Gmail, Outlook, etc.)
4. Get your **Service ID**, **Template ID**, **Public Key**

### Step 2: Add EmailJS to Your Razor Page
In `PMTool.Web/Pages/Auth/Register.cshtml`:

```html
<script type="text/javascript" src="https://cdn.jsdelivr.net/npm/@emailjs/browser@4/dist/index.min.js"></script>
<script type="text/javascript">
    emailjs.init("YOUR_PUBLIC_KEY");
</script>

<script>
function sendEmailViaEmailJS(email, confirmationLink) {
    emailjs.send("YOUR_SERVICE_ID", "YOUR_TEMPLATE_ID", {
        to_email: email,
        confirmation_link: confirmationLink,
        reply_to: "noreply@pmtool.com"
    }).then(function(response) {
        console.log('SUCCESS!', response.status, response.text);
    }, function(error) {
        console.log('FAILED...', error);
    });
}
</script>
```

**Pros:**
- ✅ Very simple to setup
- ✅ Free tier available

**Cons:**
- ❌ API keys visible in browser (security risk)
- ❌ Limited control from backend
- ❌ Not ideal for sensitive operations

---

## My Recommendation

**For your PMTool application:**

1. **Development**: Use Gmail SMTP (Option 1)
   - Already configured ✅
   - Just add your Gmail credentials
   - Perfect for testing

2. **Production**: Use SendGrid (Option 2)
   - Professional service
   - Better deliverability
   - Scalable pricing

**Do NOT use EmailJS for email authentication** because passwords/tokens should never go through the browser.

---

## Quick Setup - Gmail (5 minutes)

### 1. Enable 2FA on Google Account
- Go to [myaccount.google.com/security](https://myaccount.google.com/security)
- Enable 2-Step Verification

### 2. Generate App Password
- Go to App passwords (in Security)
- Select Mail + Windows Computer
- Copy the 16-character password

### 3. Update appsettings.json
```json
"Email": {
  "SmtpServer": "smtp.gmail.com",
  "SmtpPort": 587,
  "SenderEmail": "YOUR_EMAIL@gmail.com",
  "SenderPassword": "YOUR-16-CHAR-PASSWORD",
  "SenderName": "PMTool",
  "UseSSL": true,
  "IsEnabled": true
}
```

### 4. Test
- Run app
- Register account
- Check your email inbox

**That's it!** Emails should work immediately.

---

## Troubleshooting

### "Authentication failed"
- ❌ Wrong email/password
- ✅ Solution: Regenerate App password

### "Account locked by Google"
- ❌ Gmail thinks login is suspicious
- ✅ Solution: Visit [gmail.com](https://gmail.com), login normally, then try app again

### Emails not arriving
- ❌ Check spam folder
- ✅ May need to whitelist `noreply@pmtool.com` in Gmail

### "Less secure app access"
- ❌ Old Gmail restriction (deprecated)
- ✅ Use App passwords method instead

---

## Environment-Specific Configuration

Create `appsettings.Development.json` for local testing:

```json
{
  "Email": {
    "SmtpServer": "smtp.gmail.com",
    "SmtpPort": 587,
    "SenderEmail": "your-email@gmail.com",
    "SenderPassword": "your-app-password",
    "SenderName": "PMTool",
    "UseSSL": true,
    "IsEnabled": true
  }
}
```

Create `appsettings.Production.json` for production:

```json
{
  "Email": {
    "Provider": "SendGrid",
    "SendGridApiKey": "SG.xxxxxxxxxxxx",
    "SenderEmail": "noreply@pmtool.com",
    "SenderName": "PMTool",
    "IsEnabled": true
  }
}
```

---

## Summary

| Feature | Gmail SMTP | SendGrid | EmailJS |
|---------|-----------|----------|---------|
| Setup Time | 5 min | 15 min | 10 min |
| Cost | Free | Free/Paid | Free/Paid |
| Security | ✅ Server-side | ✅ Server-side | ❌ Client-side |
| Volume Limit | 500/day | 100/day free | Varies |
| Best For | Development | Production | Frontend only |
| Recommended | ✅ | ✅✅ | ❌ |

**Start with Gmail SMTP** - it's already configured and takes 5 minutes!
