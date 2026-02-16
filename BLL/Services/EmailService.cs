using BLL.Interfaces;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using OfficeTicketSystemBackend.Entities.DTO;

namespace BLL.Services
{
	public class EmailService : IEmailService
	{
		private readonly EmailSettingsDto _emailSettings;

		public EmailService(IOptions<EmailSettingsDto> emailSettings)
		{
			_emailSettings = emailSettings.Value;
		}

		public async Task SendEmailAsync(string to, string subject, string body)
		{
			var email = new MimeMessage();
			email.From.Add(new MailboxAddress(_emailSettings.SenderName, _emailSettings.SenderDefaultEmail));
			email.To.Add(MailboxAddress.Parse(to));
			email.Subject = subject;

			email.Body = new TextPart(MimeKit.Text.TextFormat.Html)
			{
				Text = body
			};

			using var smtp = new SmtpClient();
			await smtp.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.Port, MailKit.Security.SecureSocketOptions.StartTls);
			await smtp.AuthenticateAsync(_emailSettings.SenderDefaultEmail, _emailSettings.Password);
			await smtp.SendAsync(email);
			await smtp.DisconnectAsync(true);
		}
	}
}
