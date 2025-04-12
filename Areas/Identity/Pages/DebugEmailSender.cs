using System.Diagnostics;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace ProjetERP.Services
{ 
  public class DebugEmailSender : IEmailSender
  {
    public Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        if (email == null) throw new ArgumentNullException(nameof(email));
        if (subject == null) throw new ArgumentNullException(nameof(subject));
        if (htmlMessage == null) throw new ArgumentNullException(nameof(htmlMessage));

        Debug.WriteLine($"Email to: {email}");
        Debug.WriteLine($"Subject: {subject}");
        Debug.WriteLine($"Message: {htmlMessage}");
        return Task.CompletedTask;
    }
  }
}