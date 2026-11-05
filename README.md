# 🏥 MediBooking – Website Đặt Lịch Hẹn Khám Bệnh Trực Tuyến Tích Hợp AI

> **Công nghệ:** ASP.NET Core 9.0 · Entity Framework Core · SQL Server · Gemini AI · Chart.js

---

## 📌 Giới thiệu

**MediBooking** là hệ thống đặt lịch hẹn khám bệnh trực tuyến giúp bệnh nhân dễ dàng tìm kiếm bác sĩ, chọn lịch hẹn phù hợp và nhận xác nhận qua email — tất cả chỉ trong vài bước đơn giản.

Hệ thống tích hợp **AI tư vấn sức khỏe** (Google Gemini) và **cơ chế chống spam** đa lớp để đảm bảo trải nghiệm người dùng an toàn và tin cậy.

---

## ✨ Tính năng nổi bật

### 👤 Dành cho Bệnh nhân (Người dùng công khai)
- 🔍 Tìm kiếm bác sĩ theo chuyên khoa, tên, học hàm/học vị
- 📅 Đặt lịch hẹn khám theo bước (Wizard 4 bước)
- 🤖 **AI Tư vấn sức khỏe** – Chat với Gemini AI để được tư vấn triệu chứng
- 🔎 Tra cứu kết quả khám bằng mã đặt lịch
- 📧 Nhận email xác nhận sau khi đặt lịch thành công
- ⭐ Đánh giá bác sĩ sau khám

### 👨‍⚕️ Dành cho Bác sĩ
- 📋 Xem lịch khám trong ngày / tuần
- ✅ Xác nhận / từ chối lịch hẹn
- 📝 Tạo hồ sơ bệnh án, kê đơn thuốc
- 📤 Gửi kết quả khám qua email cho bệnh nhân
- 📊 Xem lịch sử khám của bệnh nhân

### 🛡️ Dành cho Admin
- 📊 **Dashboard thống kê** với biểu đồ Chart.js (doanh thu, lượt đặt lịch, chuyên khoa)
- 👥 Quản lý tài khoản (Bác sĩ / Admin)
- 🏥 Quản lý chuyên khoa
- 📅 Quản lý toàn bộ lịch hẹn
- 💊 Quản lý danh mục thuốc
- 💳 Cấu hình thanh toán
- 🗂️ Xem hồ sơ bệnh án toàn hệ thống

---

## 🛡️ Cơ chế chống spam lịch hẹn (Anti-Spam)

Hệ thống tích hợp **6 lớp bảo vệ** tự động:

| Lớp | Cơ chế | Mô tả |
|-----|--------|-------|
| 1 | 🍯 Honeypot Trap | Phát hiện bot qua trường ẩn |
| 2 | 📱 Kiểm tra SĐT | Chỉ chấp nhận số Việt Nam hợp lệ |
| 3 | ⏱️ Rate Limiting | Chặn gửi liên tục trong 30 giây |
| 4 | 📊 Giới hạn hoạt động | Tối đa 3 lịch hẹn đang chờ/mỗi SĐT |
| 5 | 📆 Ràng buộc ngày | Không đặt ngày quá khứ hoặc >60 ngày |
| 6 | 🛡️ Cloudflare Turnstile | Xác minh tự động 1 chạm (không cần giải toán) |

---

## 🏗️ Kiến trúc hệ thống

```
WebDatLichKham/
├── Controllers/          # Điều hướng request (MVC)
│   ├── HomeController    # Trang công khai
│   ├── AdminController   # Quản trị viên
│   ├── DoctorController  # Bác sĩ
│   ├── AccountController # Đăng nhập / đăng xuất
│   └── ApiController     # REST API (đặt lịch, AI, captcha)
├── Models/               # Cấu trúc dữ liệu
│   ├── Appointment       # Lịch hẹn
│   ├── Doctor            # Bác sĩ
│   ├── Account           # Tài khoản
│   ├── MedicalHistory    # Hồ sơ bệnh án
│   ├── Medicine          # Danh mục thuốc
│   ├── Specialty         # Chuyên khoa
│   └── AiDtos            # DTO cho AI
├── Services/             # Logic nghiệp vụ
│   ├── AppointmentService     # Đặt lịch + chống spam
│   ├── GeminiAiService        # Tích hợp AI Gemini
│   ├── SmtpEmailService       # Gửi email
│   ├── DashboardService       # Thống kê
│   └── ...
├── Data/
│   ├── ApplicationDbContext   # EF Core DbContext
│   └── DbSeeder               # Dữ liệu mẫu
├── Views/                # Giao diện Razor
│   ├── Home/             # Trang công khai
│   ├── Admin/            # Trang quản trị
│   ├── Doctor/           # Trang bác sĩ
│   └── Account/          # Đăng nhập
└── wwwroot/              # Tài nguyên tĩnh (CSS, JS)
```

---

## 📱 Quy trình sử dụng

### Bệnh nhân đặt lịch khám

```
1. Vào trang chủ → Chọn "Đặt lịch ngay"
2. Bước 1: Chọn chuyên khoa
3. Bước 2: Chọn bác sĩ & xem thông tin
4. Bước 3: Điền thông tin bệnh nhân (họ tên, SĐT, email, lý do)
5. Bước 4: Chọn ngày giờ + xác minh Cloudflare Turnstile → Xác nhận
6. Nhận mã đặt lịch + email xác nhận ✅
```

### Bác sĩ xử lý lịch khám

```
1. Đăng nhập → Dashboard bác sĩ
2. Xem danh sách lịch hẹn hôm nay
3. Xác nhận / từ chối lịch hẹn
4. Sau khám: Tạo hồ sơ bệnh án + kê đơn
5. Hệ thống tự gửi email kết quả cho bệnh nhân ✅
```

### Admin quản trị hệ thống

```
1. Đăng nhập → Dashboard Admin
2. Xem thống kê: lượt đặt lịch, doanh thu, chuyên khoa
3. Quản lý bác sĩ, chuyên khoa, thuốc, tài khoản
4. Xem/xử lý toàn bộ lịch hẹn hệ thống
```

---

## 🛠️ Công nghệ sử dụng

| Thành phần | Công nghệ |
|------------|-----------|
| Backend | ASP.NET Core 9.0 (MVC) |
| ORM | Entity Framework Core 9.0 |
| Database | SQL Server / LocalDB |
| Frontend | Bootstrap 5, Vanilla JS |
| Biểu đồ | Chart.js + ChartDataLabels |
| AI | Google Gemini 1.5 Flash |
| Email | SMTP (Gmail) |
| Bảo mật mật khẩu | BCrypt.Net |
| Cache | IMemoryCache (.NET built-in) |

---

## 📊 Mô hình dữ liệu

```
Accounts ──────┐
               ├── Doctors ──── Specialties
               │       └── DoctorReviews
               │
Appointments ──┘
       └── MedicalHistory
                └── Medicines
```

---

<div align="center">

**Made with ❤️ by DInto216**  
*MediBooking – Website Đăng Ký Lịch Hẹn Khám Bệnh Trực Tuyến Tích Hợp AI*

</div>

