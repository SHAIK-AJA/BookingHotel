 
 
using MailKit.Net.Smtp;
using MimeKit;

namespace BookingHotel.Services
{ 
 public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress("BookingHotel", "shaikaja9@gmail.com"));
            email.To.Add(MailboxAddress.Parse(toEmail));
            email.Subject = subject;

            email.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = body
            };

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync("smtp.yourprovider.com", 587, false);
            await smtp.AuthenticateAsync("shaikaja9@gmail.com", "fexh bfjr eppl ssxz");
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
    }
}

