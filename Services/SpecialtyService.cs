using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using WebDatLichKham.Data;
using WebDatLichKham.Models;

namespace WebDatLichKham.Services
{
    public interface ISpecialtyService
    {
        Task<IEnumerable<Specialty>> GetAllAsync();
        Task<Specialty> AddAsync(Specialty specialty);
        Task<Specialty> UpdateAsync(string id, Specialty data);
        Task<bool> DeleteAsync(string id);
    }

    public class SpecialtyService : ISpecialtyService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMemoryCache _cache;
        private const string CacheKey = "SpecialtiesList";

        public SpecialtyService(ApplicationDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<IEnumerable<Specialty>> GetAllAsync()
        {
            if (!_cache.TryGetValue(CacheKey, out IEnumerable<Specialty> specialties))
            {
                specialties = await _context.Specialties.AsNoTracking().ToListAsync();
                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromHours(1));
                _ = _cache.Set(CacheKey, specialties, cacheOptions);
            }
            return specialties;
        }

        public async Task<Specialty> AddAsync(Specialty specialty)
        {
            specialty.Id = "sp_" + Guid.NewGuid().ToString("N").Substring(0, 8);
            specialty.DoctorCount = 0;
            _ = _context.Specialties.Add(specialty);
            _ = await _context.SaveChangesAsync();
            _cache.Remove(CacheKey);
            return specialty;
        }

        public async Task<Specialty> UpdateAsync(string id, Specialty data)
        {
            var specialty = await _context.Specialties.FindAsync(id);
            if (specialty == null) return null;

            specialty.Name = data.Name;
            specialty.Description = data.Description;
            specialty.Icon = data.Icon;

            _ = await _context.SaveChangesAsync();
            _cache.Remove(CacheKey);
            return specialty;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var specialty = await _context.Specialties.FindAsync(id);
            if (specialty == null) return false;

            _ = _context.Specialties.Remove(specialty);
            _ = await _context.SaveChangesAsync();
            _cache.Remove(CacheKey);
            return true;
        }
    }
}
