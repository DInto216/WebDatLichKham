using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using WebDatLichKham.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add(new Microsoft.AspNetCore.Mvc.AutoValidateAntiforgeryTokenAttribute());
});
builder.Services.AddAntiforgery(options => options.HeaderName = "RequestVerificationToken");

// Register SQL Server database context
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register Memory Cache for Performance Optimization
builder.Services.AddMemoryCache();

// Register Business Logic Services (Clean Architecture)
builder.Services.AddScoped<WebDatLichKham.Services.ISpecialtyService, WebDatLichKham.Services.SpecialtyService>();
builder.Services.AddScoped<WebDatLichKham.Services.IDoctorService, WebDatLichKham.Services.DoctorService>();
builder.Services.AddScoped<WebDatLichKham.Services.IAppointmentService, WebDatLichKham.Services.AppointmentService>();
builder.Services.AddScoped<WebDatLichKham.Services.IMedicineService, WebDatLichKham.Services.MedicineService>();
builder.Services.AddScoped<WebDatLichKham.Services.IDashboardService, WebDatLichKham.Services.DashboardService>();
builder.Services.AddScoped<WebDatLichKham.Services.IAccountService, WebDatLichKham.Services.AccountService>();
builder.Services.AddScoped<WebDatLichKham.Services.IMedicalHistoryService, WebDatLichKham.Services.MedicalHistoryService>();
builder.Services.AddScoped<WebDatLichKham.Services.IDoctorReviewService, WebDatLichKham.Services.DoctorReviewService>();
builder.Services.AddScoped<WebDatLichKham.Services.IEmailService, WebDatLichKham.Services.SmtpEmailService>();

// Add Cookie Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Home/Index";
        options.Cookie.HttpOnly = true;
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
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
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
    app.UseHttpsRedirection();
}
app.UseStaticFiles(); // Serve files from wwwroot
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
