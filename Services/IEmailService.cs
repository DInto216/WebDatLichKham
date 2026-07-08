
namespace WebDatLichKham.Services
{
    public interface IEmailService
    {
        Task SendBookingConfirmationAsync(string toEmail, string patientName, string bookingId, string date, string time, string doctorName, string specialty);
        Task SendExamResultAsync(string toEmail, string patientName, string bookingId, string doctorName, string diagnosis, string prescription);
    }
}
