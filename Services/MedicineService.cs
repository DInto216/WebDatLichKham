using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using WebDatLichKham.Data;
using WebDatLichKham.Models;

namespace WebDatLichKham.Services
{
    public interface IMedicineService
    {
        Task<IEnumerable<Medicine>> GetAllAsync();
        Task<Medicine> AddAsync(Medicine medicine);
        Task<Medicine> UpdateAsync(string id, Medicine data);
        Task<bool> DeleteAsync(string id);
    }

    public class MedicineService : IMedicineService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMemoryCache _cache;
        private const string CacheKey = "MedicinesList";

        public MedicineService(ApplicationDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<IEnumerable<Medicine>> GetAllAsync()
        {
            if (!_cache.TryGetValue(CacheKey, out IEnumerable<Medicine> medicines))
            {
                medicines = await _context.Medicines.AsNoTracking().ToListAsync();
                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromHours(1));
                _ = _cache.Set(CacheKey, medicines, cacheOptions);
            }
            return medicines;
        }

        public async Task<Medicine> AddAsync(Medicine medicine)
        {
            medicine.Id = "med_" + Guid.NewGuid().ToString("N").Substring(0, 8);
            _ = _context.Medicines.Add(medicine);
            _ = await _context.SaveChangesAsync();
            _cache.Remove(CacheKey);
            return medicine;
        }

        public async Task<Medicine> UpdateAsync(string id, Medicine data)
        {
            var med = await _context.Medicines.FindAsync(id);
            if (med == null) return null;

            med.Name = data.Name;
            med.Ingredients = data.Ingredients;
            med.Unit = data.Unit;
            med.ExpiryDate = data.ExpiryDate;

            _ = await _context.SaveChangesAsync();
            _cache.Remove(CacheKey);
            return med;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var med = await _context.Medicines.FindAsync(id);
            if (med == null) return false;

            _ = _context.Medicines.Remove(med);
            _ = await _context.SaveChangesAsync();
            _cache.Remove(CacheKey);
            return true;
        }
    }
}

