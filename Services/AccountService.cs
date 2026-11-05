using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using WebDatLichKham.Data;
using WebDatLichKham.Models;

namespace WebDatLichKham.Services
{
    public interface IAccountService
    {
        Task<IEnumerable<object>> GetAllAsync();
        Task<Account> AddAsync(Account account);
        Task<bool> DeleteAsync(string id);
        Task<Account> UpdateProfileAsync(string id, JsonElement data);
        Task<Account> AuthenticateAsync(string username, string password);
        Task<Account?> GetAccountByIdAsync(string id);
    }

    public class AccountService : IAccountService
    {
        private readonly ApplicationDbContext _context;
        private readonly Microsoft.Extensions.Caching.Memory.IMemoryCache _cache;

        public AccountService(ApplicationDbContext context, Microsoft.Extensions.Caching.Memory.IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<IEnumerable<object>> GetAllAsync()
        {
            var accounts = await _context.Accounts.AsNoTracking().ToListAsync();

            // JOIN voi Doctors de lay SpecialtyName
            var doctorIds = accounts
                .Where(a => !string.IsNullOrEmpty(a.DoctorId))
                .Select(a => a.DoctorId!)
                .Distinct()
                .ToList();

            var doctors = await _context.Doctors.AsNoTracking()
                .Where(d => doctorIds.Contains(d.Id))
                .Select(d => new { d.Id, d.SpecialtyName })
                .ToListAsync();

            var doctorMap = doctors.ToDictionary(d => d.Id, d => d.SpecialtyName);

            return accounts.Select(a => new
            {
                a.Id,
                a.Username,
                a.Name,
                password = "        ",
                a.Role,
                a.DoctorId,
                a.CreatedAt,
                avatar = a.Avatar ?? "",
                specialtyName = (a.DoctorId != null && doctorMap.TryGetValue(a.DoctorId, out var sp)) ? sp : ""
            });
        }

        public async Task<Account?> GetAccountByIdAsync(string id)
        {
            return await _context.Accounts.FindAsync(id);
        }

        public async Task<Account> AddAsync(Account acc)
        {
            acc.Id = "acc_" + Guid.NewGuid().ToString("N").Substring(0, 8);
            acc.CreatedAt = DateTime.Now;
            acc.Password = BCrypt.Net.BCrypt.HashPassword(acc.Password);
            _ = _context.Accounts.Add(acc);
            _ = await _context.SaveChangesAsync();
            return acc;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var acc = await _context.Accounts.FindAsync(id);
            if (acc == null) return false;

            _ = _context.Accounts.Remove(acc);
            _ = await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Account> UpdateProfileAsync(string id, JsonElement data)
        {
            var acc = await _context.Accounts.FindAsync(id);
            if (acc == null) return null;

            if (data.TryGetProperty("oldPassword", out var oldPwdProp) && data.TryGetProperty("newPassword", out var newPwdProp))
            {
                bool oldMatch;
                if (acc.Password.StartsWith("$2a$") || acc.Password.StartsWith("$2b$") || acc.Password.StartsWith("$2y$"))
                {
                    oldMatch = BCrypt.Net.BCrypt.Verify(oldPwdProp.GetString(), acc.Password);
                }
                else
                {
                    oldMatch = (acc.Password == oldPwdProp.GetString());
                }

                if (!oldMatch)
                {
                    throw new Exception("Mật khẩu cũ không chính xác");
                }
                acc.Password = BCrypt.Net.BCrypt.HashPassword(newPwdProp.GetString() ?? "");
            }

            if (data.TryGetProperty("name", out var nameProp))
            {
                acc.Name = nameProp.GetString() ?? acc.Name;
            }

            if (data.TryGetProperty("avatar", out var avatarProp))
            {
                acc.Avatar = avatarProp.GetString() ?? acc.Avatar;
            }

            _ = await _context.SaveChangesAsync();

            if (acc.Role == "doctor")
            {
                Doctor doc;
                if (string.IsNullOrEmpty(acc.DoctorId))
                {
                    // Tự động tạo hồ sơ Bác sĩ nếu chưa có
                    doc = new Doctor
                    {
                        Id = "doc_" + Guid.NewGuid().ToString("N").Substring(0, 8),
                        Name = acc.Name,
                        Avatar = acc.Avatar ?? ""
                    };

                    if (data.TryGetProperty("specialtyId", out var defSpecProp))
                    {
                        var sid = defSpecProp.GetString();
                        if (!string.IsNullOrEmpty(sid))
                        {
                            doc.SpecialtyId = sid;
                            var spec = await _context.Specialties.FindAsync(sid);
                            if (spec != null) doc.SpecialtyName = spec.Name;
                        }
                        else
                        {
                            doc.SpecialtyId = "UNKNOWN";
                        }
                    }
                    else
                    {
                        doc.SpecialtyId = "UNKNOWN";
                    }

                    _ = _context.Doctors.Add(doc);
                    acc.DoctorId = doc.Id;
                }
                else
                {
                    doc = await _context.Doctors.FindAsync(acc.DoctorId);
                }

                if (doc != null)
                {
                    if (data.TryGetProperty("name", out var dNameProp)) doc.Name = dNameProp.GetString() ?? doc.Name;
                    if (data.TryGetProperty("degree", out var dDegProp)) doc.Degree = dDegProp.GetString() ?? doc.Degree;
                    if (data.TryGetProperty("academicTitle", out var acadProp)) doc.AcademicTitle = acadProp.GetString() ?? doc.AcademicTitle;
                    if (data.TryGetProperty("biography", out var dBioProp)) doc.Bio = dBioProp.GetString() ?? doc.Bio;
                    if (data.TryGetProperty("avatar", out var dAvatarProp)) doc.Avatar = dAvatarProp.GetString() ?? doc.Avatar;
                    if (data.TryGetProperty("price", out var dPriceProp) && dPriceProp.ValueKind == JsonValueKind.Number) doc.Price = dPriceProp.GetDecimal();
                    if (data.TryGetProperty("email", out var dEmailProp)) doc.Email = dEmailProp.GetString() ?? doc.Email;
                    if (data.TryGetProperty("phone", out var dPhoneProp)) doc.Phone = dPhoneProp.GetString() ?? doc.Phone;
                    if (data.TryGetProperty("experience", out var dExpProp)) doc.Experience = dExpProp.GetString() ?? doc.Experience;
                    if (data.TryGetProperty("specialtyId", out var dSpecProp))
                    {
                        var newSpecialtyId = dSpecProp.GetString();
                        if (!string.IsNullOrEmpty(newSpecialtyId) && doc.SpecialtyId != newSpecialtyId)
                        {
                            var oldSpecialtyId = doc.SpecialtyId;
                            
                            // Giảm số lượng bác sĩ ở chuyên khoa cũ
                            if (!string.IsNullOrEmpty(oldSpecialtyId))
                            {
                                var oldSpec = await _context.Specialties.FindAsync(oldSpecialtyId);
                                if (oldSpec != null && oldSpec.DoctorCount > 0) oldSpec.DoctorCount--;
                            }

                            // Tăng số lượng bác sĩ ở chuyên khoa mới
                            doc.SpecialtyId = newSpecialtyId;
                            var newSpec = await _context.Specialties.FindAsync(newSpecialtyId);
                            if (newSpec != null)
                            {
                                doc.SpecialtyName = newSpec.Name;
                                newSpec.DoctorCount++;
                            }
                        }
                    }
                    _ = await _context.SaveChangesAsync();
                    _cache.Remove("Doctors_All");
                    _cache.Remove("Doctors_" + doc.Id);
                    _cache.Remove("SpecialtiesList");
                }
            }

            return acc;
        }

        public async Task<Account> AuthenticateAsync(string username, string password)
        {
            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Username == username);
            if (account == null) return null;

            bool passwordMatches = false;
            if (account.Password.StartsWith("$2a$") || account.Password.StartsWith("$2b$") || account.Password.StartsWith("$2y$"))
            {
                passwordMatches = BCrypt.Net.BCrypt.Verify(password, account.Password);
            }
            else
            {
                if (account.Password == password)
                {
                    passwordMatches = true;
                    account.Password = BCrypt.Net.BCrypt.HashPassword(password);
                    _ = await _context.SaveChangesAsync();
                }
            }

            return passwordMatches ? account : null;
        }
    }
}

