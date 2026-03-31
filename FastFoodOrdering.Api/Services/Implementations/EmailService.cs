using FastFoodOrdering.Api.Services.Interfaces;
using System.Net;
using System.Net.Mail;

namespace FastFoodOrdering.Api.Services.Implementations;

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;

    // Tiêm IConfiguration vào để lấy thông tin từ appsettings.json
    public EmailService(IConfiguration config)
    {
        _config = config;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        // 1. Lấy cấu hình Email từ file appsettings.json
        var emailSettings = _config.GetSection("EmailSettings");
        string senderEmail = emailSettings["Email"] ?? string.Empty;
        string senderPassword = emailSettings["Password"] ?? string.Empty;
        string host = emailSettings["Host"] ?? "smtp.gmail.com";
        int port = int.Parse(emailSettings["Port"] ?? "587");

        // 2. Cài đặt máy chủ SMTP của Google
        using var client = new SmtpClient(host, port)
        {
            Credentials = new NetworkCredential(senderEmail, senderPassword),
            EnableSsl = true // Bắt buộc phải bật SSL với Gmail
        };

        // 3. Soạn thư
        var mailMessage = new MailMessage
        {
            From = new MailAddress(senderEmail, "FastFood API"), // Tên người gửi hiển thị
            Subject = subject,
            Body = body,
            IsBodyHtml = true // Để true để sau này có thể gửi email bằng code HTML cho đẹp
        };
        mailMessage.To.Add(toEmail);

        // 4. Bấm nút gửi
        await client.SendMailAsync(mailMessage);
    }
}
