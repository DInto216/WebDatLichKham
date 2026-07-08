using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace WebDatLichKham.Services
{
    public class SmtpEmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<SmtpEmailService> _logger;

        public SmtpEmailService(IConfiguration configuration, ILogger<SmtpEmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        private async Task SendEmailAsync(string toEmail, string subject, string htmlBody)
        {
            if (string.IsNullOrWhiteSpace(toEmail))
                return;

            try
            {
                var server = _configuration["SmtpSettings:Server"];
                var port = int.Parse(_configuration["SmtpSettings:Port"] ?? "587");
                var senderName = _configuration["SmtpSettings:SenderName"];
                var senderEmail = _configuration["SmtpSettings:SenderEmail"];
                var username = _configuration["SmtpSettings:Username"];
                var password = _configuration["SmtpSettings:Password"];

                using var client = new SmtpClient(server, port)
                {
                    Credentials = new NetworkCredential(username, password),
                    EnableSsl = true
                };

                using var mailMessage = new MailMessage
                {
                    From = new MailAddress(senderEmail, senderName),
                    Subject = subject,
                    Body = htmlBody,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(toEmail);

                await client.SendMailAsync(mailMessage);
                _logger.LogInformation("Email sent to {ToEmail} successfully.", toEmail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {ToEmail}", toEmail);
            }
        }

        public async Task SendBookingConfirmationAsync(string toEmail, string patientName, string bookingId, string date, string time, string doctorName, string specialty)
        {
            if (string.IsNullOrWhiteSpace(toEmail)) return;

            var subject = $"[MediBooking] Đặt lịch khám thành công - Mã vé: {bookingId}";
            var body = $@"
                <div style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
                    <h2 style='color: #0ea5e9;'>Xác Nhận Đặt Lịch Khám</h2>
                    <p>Xin chào <strong>{patientName}</strong>,</p>
                    <p>Bạn đã đặt lịch khám thành công tại phòng khám MediBooking. Dưới đây là thông tin chi tiết:</p>
                    <ul>
                        <li><strong>Mã đặt lịch:</strong> <span style='color: #0ea5e9; font-weight: bold;'>{bookingId}</span></li>
                        <li><strong>Chuyên khoa:</strong> {specialty}</li>
                        <li><strong>Bác sĩ:</strong> {doctorName}</li>
                        <li><strong>Thời gian:</strong> {time} ngày {date}</li>
                    </ul>
                    <p>Vui lòng có mặt trước giờ khám 15 phút và mang theo mã đặt lịch này để làm thủ tục.</p>
                    <p>Cảm ơn bạn đã tin tưởng dịch vụ của chúng tôi!</p>
                </div>";

            await SendEmailAsync(toEmail, subject, body);
        }

        public async Task SendExamResultAsync(string toEmail, string patientName, string bookingId, string doctorName, string diagnosis, string prescription)
        {
            if (string.IsNullOrWhiteSpace(toEmail)) return;

            var subject = $"[MediBooking] Kết quả khám bệnh - Mã vé: {bookingId}";
            var body = $@"
                <div style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
                    <h2 style='color: #10b981;'>Kết Quả Khám Bệnh</h2>
                    <p>Xin chào <strong>{patientName}</strong>,</p>
                    <p>Bác sĩ <strong>{doctorName}</strong> đã hoàn tất việc khám bệnh và cập nhật hồ sơ của bạn (Mã: {bookingId}).</p>
                    <div style='background-color: #f9fafb; padding: 15px; border-radius: 8px; margin: 15px 0;'>
                        <h4 style='margin-top: 0; color: #333;'>Chẩn đoán:</h4>
                        <p style='color: #dc2626; font-weight: bold;'>{diagnosis}</p>
                        
                        <h4 style='color: #333;'>Đơn thuốc:</h4>
                        <p>{(string.IsNullOrWhiteSpace(prescription) ? "Không có" : prescription)}</p>
                    </div>
                    <p>Bạn có thể tra cứu chi tiết bệnh án và đơn thuốc trên website của chúng tôi bằng mã đặt lịch hoặc số điện thoại.</p>
                    <p>Chúc bạn nhiều sức khỏe!</p>
                </div>";

            await SendEmailAsync(toEmail, subject, body);
        }
    }
}
