using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using WebDatLichKham.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews(options =>
{
    // BẢO MẬT: Bật bộ lọc chống giả mạo yêu cầu chéo (CSRF - Cross-Site Request Forgery) TOÀN CẦU.
    // Tất cả các POST, PUT, DELETE form (đặt lịch, bệnh án) đều PHẢI có ValidateAntiForgeryToken hợp lệ từ Frontend.
    options.Filters.Add(new Microsoft.AspNetCore.Mvc.AutoValidateAntiforgeryTokenAttribute());
}).AddJsonOptions(options =>
{
    // Cấu hình không escape ký tự Unicode tiếng Việt
    options.JsonSerializerOptions.Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
});
// BẢO MẬT: Cấu hình Header nhận CSRF Token từ AJAX Fetch requests
builder.Services.AddAntiforgery(options => options.HeaderName = "RequestVerificationToken");

// [CLEAN CODE] TẦNG DATA ACCESS: Đăng ký DbContext với chuỗi kết nối SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    options.ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
});

// [HIỆU NĂNG]: Đăng ký Memory Cache giúp tăng tốc độ truy xuất dữ liệu không thay đổi thường xuyên
builder.Services.AddMemoryCache();

// [CLEAN CODE] TẦNG BUSINESS LOGIC (Services): Đăng ký Dependency Injection theo chuẩn Clean Architecture
// Sử dụng AddScoped để đảm bảo mỗi HTTP request sẽ khởi tạo một instance độc lập, an toàn luồng dữ liệu (thread-safe).
builder.Services.AddScoped<WebDatLichKham.Services.ISpecialtyService, WebDatLichKham.Services.SpecialtyService>();
builder.Services.AddScoped<WebDatLichKham.Services.IDoctorService, WebDatLichKham.Services.DoctorService>();
builder.Services.AddScoped<WebDatLichKham.Services.IAppointmentService, WebDatLichKham.Services.AppointmentService>();
builder.Services.AddScoped<WebDatLichKham.Services.IMedicineService, WebDatLichKham.Services.MedicineService>();
builder.Services.AddScoped<WebDatLichKham.Services.IDashboardService, WebDatLichKham.Services.DashboardService>();
builder.Services.AddScoped<WebDatLichKham.Services.IAccountService, WebDatLichKham.Services.AccountService>();
builder.Services.AddScoped<WebDatLichKham.Services.IMedicalHistoryService, WebDatLichKham.Services.MedicalHistoryService>();
builder.Services.AddScoped<WebDatLichKham.Services.IDoctorReviewService, WebDatLichKham.Services.DoctorReviewService>();
builder.Services.AddScoped<WebDatLichKham.Services.IEmailService, WebDatLichKham.Services.SmtpEmailService>();
builder.Services.AddHttpClient("GeminiClient");
builder.Services.AddScoped<WebDatLichKham.Services.IAiConsultantService, WebDatLichKham.Services.GeminiAiService>();

// BẢO MẬT: Cấu hình Cookie Authentication cho quản lý phiên đăng nhập
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login"; // Chuyển hướng khi chưa đăng nhập
        options.AccessDeniedPath = "/Home/Index"; // Chuyển hướng khi không đủ quyền
        options.Cookie.HttpOnly = true; // Ngăn chặn XSS tấn công lấy cắp Cookie
        options.ExpireTimeSpan = TimeSpan.FromDays(7); // Duy trì phiên đăng nhập 7 ngày
    });

var app = builder.Build();

// Seed Database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        DbSeeder.Seed(context);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

// Configure the HTTP request pipeline.
// [CLEAN CODE]: Quy trình Pipeline Middleware tuân thủ tuyệt đối chuẩn bảo mật và hiệu năng.
if (!app.Environment.IsDevelopment())
{
    _ = app.UseExceptionHandler("/Home/Error");
    _ = app.UseHsts(); // Cấu hình Strict-Transport-Security (Bảo mật giao thức)
    _ = app.UseHttpsRedirection();
}

app.UseStaticFiles(); // 1. Trả file tĩnh (wwwroot) trước tiên để tiết kiệm tài nguyên
app.UseRouting();     // 2. Định tuyến URL để tìm Controller tương ứng

app.UseAuthentication(); // 3. Xác thực Cookie/Token (Who are you?)
app.UseAuthorization();  // 4. Phân quyền truy cập theo Roles (What can you do?)

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

