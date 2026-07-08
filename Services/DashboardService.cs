using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using WebDatLichKham.Data;

namespace WebDatLichKham.Services
{
    public interface IDashboardService
    {
        Task<object> GetStatsAsync();
        Task<object> GetRevenueStatsAsync(int year);
    }

    public class DashboardService : IDashboardService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMemoryCache _cache;
        private const string CacheKey = "DashboardStats";

        public DashboardService(ApplicationDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<object> GetStatsAsync()
        {
            if (!_cache.TryGetValue(CacheKey, out object stats))
            {
                var today = DateTime.Now.ToString("yyyy-MM-dd");
                
                var totalAppointments = await _context.Appointments.CountAsync();
                var todayAppointments = await _context.Appointments.CountAsync(a => a.Date == today);
                
                var totalDoctors = await _context.Doctors.CountAsync();
                var totalSpecialties = await _context.Specialties.CountAsync();

                var pending = await _context.Appointments.CountAsync(a => a.Status == "pending");
                var approved = await _context.Appointments.CountAsync(a => a.Status == "approved");
                var completed = await _context.Appointments.CountAsync(a => a.Status == "completed");
                var cancelled = await _context.Appointments.CountAsync(a => a.Status == "cancelled");

                var monthlyData = new List<object>();
                for (int i = 5; i >= 0; i--)
                {
                    var d = DateTime.Now.AddMonths(-i);
                    var monthStart = d.ToString("yyyy-MM");
                    var label = d.ToString("MM/yyyy");

                    var monthAppts = await _context.Appointments
                        .CountAsync(a => a.Date != null && a.Date.StartsWith(monthStart));
                    var monthCompleted = await _context.Appointments
                        .CountAsync(a => a.Date != null && a.Date.StartsWith(monthStart) && a.Status == "completed");

                    var avgPrice = await _context.Doctors.AverageAsync(d2 => (decimal?)d2.Price) ?? 350000;
                    var monthRevenue = (long)(monthCompleted * avgPrice);

                    monthlyData.Add(new { label, appointments = monthAppts, revenue = monthRevenue });
                }

                var specialties = await _context.Specialties.Take(6).ToListAsync();
                var specialtyStats = new List<object>();
                foreach (var s in specialties)
                {
                    var count = await _context.Appointments.CountAsync(a => a.Specialty == s.Name);
                    specialtyStats.Add(new { name = s.Name, count });
                }

                var globalAvgPrice = await _context.Doctors.AverageAsync(d2 => (decimal?)d2.Price) ?? 350000;

                stats = new
                {
                    totalPatients = totalAppointments,
                    todayAppointments,
                    totalDoctors,
                    totalSpecialties,
                    pending,
                    approved,
                    completed,
                    cancelled,
                    monthlyData,
                    specialtyStats,
                    revenue = (long)(completed * globalAvgPrice)
                };

                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(5));
                _cache.Set(CacheKey, stats, cacheOptions);
            }
            return stats;
        }

        public async Task<object> GetRevenueStatsAsync(int year)
        {
            var monthlyData = new List<object>();
            var avgPrice = await _context.Doctors.AverageAsync(d2 => (decimal?)d2.Price) ?? 350000;

            for (int i = 1; i <= 12; i++)
            {
                var monthStart = $"{year}-{i:D2}";
                var label = $"Tháng {i}";

                var monthAppts = await _context.Appointments
                    .CountAsync(a => a.Date != null && a.Date.StartsWith(monthStart));
                var monthCompleted = await _context.Appointments
                    .CountAsync(a => a.Date != null && a.Date.StartsWith(monthStart) && a.Status == "completed");

                var monthRevenue = (long)(monthCompleted * avgPrice);

                monthlyData.Add(new { label, appointments = monthAppts, revenue = monthRevenue });
            }
            
            return monthlyData;
        }
    }
}
