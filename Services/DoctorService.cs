using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using WebDatLichKham.Data;
using WebDatLichKham.Models;

namespace WebDatLichKham.Services
{
    public interface IDoctorService
    {
        Task<IEnumerable<Doctor>> GetAllAsync();
        Task<Doctor> GetByIdAsync(string id);
        Task<Doctor> AddAsync(Doctor doctor);
        Task<Doctor> UpdateAsync(string id, JsonElement data);
        Task<bool> DeleteAsync(string id);
        Task<List<string>> GetDoctorSlotsAsync(string doctorId, string date);
    }

    public class DoctorService : IDoctorService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMemoryCache _cache;
        private const string CacheKeyPrefix = "Doctors_";

        public DoctorService(ApplicationDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<IEnumerable<Doctor>> GetAllAsync()
        {
            var cacheKey = CacheKeyPrefix + "All";
            if (!_cache.TryGetValue(cacheKey, out IEnumerable<Doctor> doctors))
            {
                doctors = await _context.Doctors
                    .Include(d => d.Specialty)
                    .AsNoTracking()
                    .ToListAsync();
                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(15));
                _ = _cache.Set(cacheKey, doctors, cacheOptions);
            }
            return doctors;
        }

        public async Task<Doctor> GetByIdAsync(string id)
        {
            var cacheKey = CacheKeyPrefix + id;
            if (!_cache.TryGetValue(cacheKey, out Doctor doctor))
            {
                doctor = await _context.Doctors
                    .Include(d => d.Specialty)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(d => d.Id == id);
                if (doctor != null)
                {
                    var cacheOptions = new MemoryCacheEntryOptions()
                        .SetAbsoluteExpiration(TimeSpan.FromMinutes(15));
                    _ = _cache.Set(cacheKey, doctor, cacheOptions);
                }
            }
            return doctor;
        }

        public async Task<Doctor> AddAsync(Doctor doctor)
        {
            doctor.Id = "doc_" + Guid.NewGuid().ToString("N").Substring(0, 8);
            if (string.IsNullOrEmpty(doctor.ScheduleJson))
            {
                doctor.ScheduleJson = "{}";
            }
            _ = _context.Doctors.Add(doctor);

            // Update specialty count
            if (!string.IsNullOrEmpty(doctor.SpecialtyName))
            {
                var spec = await _context.Specialties.FirstOrDefaultAsync(s => s.Name == doctor.SpecialtyName);
                if (spec != null) spec.DoctorCount++;
            }

            _ = await _context.SaveChangesAsync();
            InvalidateCache();
            return doctor;
        }

        public async Task<Doctor> UpdateAsync(string id, JsonElement data)
        {
            var doctor = await _context.Doctors.FindAsync(id);
            if (doctor == null) return null;

            if (data.TryGetProperty("specialtyName", out var specNameProp))
            {
                var newSpecName = specNameProp.GetString();
                if (!string.IsNullOrEmpty(doctor.SpecialtyName) && doctor.SpecialtyName != newSpecName)
                {
                    var oldSpec = await _context.Specialties.FirstOrDefaultAsync(s => s.Name == doctor.SpecialtyName);
                    if (oldSpec != null && oldSpec.DoctorCount > 0) oldSpec.DoctorCount--;
                    var newSpec = await _context.Specialties.FirstOrDefaultAsync(s => s.Name == newSpecName);
                    if (newSpec != null) newSpec.DoctorCount++;
                }
                doctor.SpecialtyName = newSpecName ?? doctor.SpecialtyName;
            }

            if (data.TryGetProperty("name", out var nameProp)) doctor.Name = nameProp.GetString() ?? doctor.Name;
            if (data.TryGetProperty("avatar", out var avatarProp)) doctor.Avatar = avatarProp.GetString() ?? doctor.Avatar;
            if (data.TryGetProperty("bio", out var bioProp)) doctor.Bio = bioProp.GetString() ?? doctor.Bio;
            if (data.TryGetProperty("academicTitle", out var acadProp)) doctor.AcademicTitle = acadProp.GetString() ?? doctor.AcademicTitle;
            if (data.TryGetProperty("degree", out var degreeProp)) doctor.Degree = degreeProp.GetString() ?? doctor.Degree;

            if (data.TryGetProperty("price", out var priceProp) && priceProp.TryGetDecimal(out var price))
                doctor.Price = price;

            if (data.TryGetProperty("scheduleJson", out var scheduleProp))
                doctor.ScheduleJson = scheduleProp.GetString() ?? doctor.ScheduleJson;

            _ = await _context.SaveChangesAsync();
            InvalidateCache();
            _cache.Remove(CacheKeyPrefix + id);
            return doctor;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var doctor = await _context.Doctors.FindAsync(id);
            if (doctor == null) return false;

            if (!string.IsNullOrEmpty(doctor.SpecialtyName))
            {
                var spec = await _context.Specialties.FirstOrDefaultAsync(s => s.Name == doctor.SpecialtyName);
                if (spec != null && spec.DoctorCount > 0) spec.DoctorCount--;
            }

            _ = _context.Doctors.Remove(doctor);

            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.DoctorId == id);
            if (account != null) _ = _context.Accounts.Remove(account);

            _ = await _context.SaveChangesAsync();
            InvalidateCache();
            _cache.Remove(CacheKeyPrefix + id);
            return true;
        }

        public async Task<List<string>> GetDoctorSlotsAsync(string doctorId, string date)
        {
            var doctor = await _context.Doctors.AsNoTracking().FirstOrDefaultAsync(d => d.Id == doctorId);
            if (doctor == null) return new List<string>();

            var schedule = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(doctor.ScheduleJson ?? "{}");

            if (DateTime.TryParse(date, out DateTime parsedDate))
            {
                string dayOfWeek = parsedDate.DayOfWeek.ToString().ToLower();
                if (schedule != null && schedule.ContainsKey(dayOfWeek))
                {
                    return schedule[dayOfWeek];
                }
            }
            return new List<string>();
        }

        private void InvalidateCache()
        {
            _cache.Remove(CacheKeyPrefix + "All");
        }
    }
}
