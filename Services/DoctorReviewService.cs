using Microsoft.EntityFrameworkCore;
using WebDatLichKham.Data;
using WebDatLichKham.Models;

namespace WebDatLichKham.Services
{
    public interface IDoctorReviewService
    {
        Task<IEnumerable<DoctorReview>> GetAllAsync();
        Task<DoctorReview> AddAsync(DoctorReview review);
    }

    public class DoctorReviewService : IDoctorReviewService
    {
        private readonly ApplicationDbContext _context;

        public DoctorReviewService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DoctorReview>> GetAllAsync()
        {
            // TỐI ƯU HÓA: Sử dụng AsNoTracking cho List truy vấn
            return await _context.DoctorReviews.AsNoTracking().OrderByDescending(r => r.Date).ToListAsync();
        }

        public async Task<DoctorReview> AddAsync(DoctorReview review)
        {
            review.Id = "rev_" + Guid.NewGuid().ToString("N").Substring(0, 8);
            if (string.IsNullOrEmpty(review.Date))
            {
                review.Date = DateTime.Now.ToString("yyyy-MM-dd");
            }

            _ = _context.DoctorReviews.Add(review);
            _ = await _context.SaveChangesAsync();
            return review;
        }
    }
}

