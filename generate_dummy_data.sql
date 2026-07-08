-- =========================================================================
-- SQL SERVER T-SQL SCRIPT: TỰ ĐỘNG SINH DỮ LIỆU MẪU (DUMMY DATA) CHO WEBDATLICHKHAM
-- Thời gian phát sinh dữ liệu: Từ 01/01/2026 đến nay (06/07/2026)
-- Tương thích 100% với cấu trúc DB SQL Server được tạo bởi Entity Framework
-- =========================================================================

-- 1. Xóa sạch dữ liệu cũ để tránh trùng khóa chính (PK) hoặc lỗi dữ liệu không đồng nhất
DELETE FROM MedicalHistories;
DELETE FROM Appointments;
DELETE FROM Accounts;
DELETE FROM Doctors;
DELETE FROM Medicines;
DELETE FROM Specialties;

-- 2. Thêm dữ liệu danh mục Chuyên Khoa (Specialties)
INSERT INTO Specialties (Id, Name, Icon, Description, DoctorCount) VALUES
('sp_01', N'Nội tổng quát', N'🫀', N'Khám và điều trị các bệnh nội khoa tổng quát cho người lớn', 1),
('sp_02', N'Nhi khoa', N'👶', N'Chăm sóc sức khỏe, tiêm chủng và điều trị bệnh cho trẻ em', 1),
('sp_03', N'Tim mạch', N'❤️', N'Chẩn đoán, điều trị chuyên sâu bệnh lý tim mạch, huyết áp', 1),
('sp_04', N'Da liễu', N'🧴', N'Khám và điều trị các bệnh về da, tóc, móng và thẩm mỹ da', 1),
('sp_05', N'Tai Mũi Họng', N'👂', N'Khám và điều trị các bệnh lý Tai - Mũi - Họng cấp và mãn tính', 1);

-- 3. Thêm dữ liệu danh mục Bác Sĩ (Doctors)
INSERT INTO Doctors (Id, Name, Degree, SpecialtyId, SpecialtyName, Price, Rating, Reviews, Avatar, Bio, ScheduleJson) VALUES
('doc_01', N'BS. Nguyễn Văn An', N'PGS.TS', 'sp_01', N'Nội tổng quát', 350000, 4.8, 120, '', N'Hơn 15 năm kinh nghiệm trong lĩnh vực Nội khoa tổng quát, nguyên Trưởng khoa BV Bạch Mai', '{"monday":["08:00","08:30","09:00","09:30","10:00","14:00","14:30","15:00","15:30"],"tuesday":["08:00","08:30","09:00","09:30","10:00","14:00","14:30","15:00"],"wednesday":["08:00","08:30","09:00","09:30","14:00","14:30","15:00","15:30"],"thursday":["08:00","08:30","09:00","09:30","10:00"],"friday":["08:00","08:30","09:00","14:00","14:30","15:00","15:30","16:00"],"saturday":["08:00","08:30","09:00","09:30"],"sunday":[]}'),
('doc_02', N'BS. Trần Thị Bích', N'TS.BS', 'sp_02', N'Nhi khoa', 400000, 4.9, 210, '', N'20 năm kinh nghiệm chuyên khoa Nhi, từng tu nghiệp tại Nhật Bản, chuyên gia dinh dưỡng và hô hấp trẻ em', '{"monday":["08:00","08:30","09:00","09:30","10:00","10:30"],"tuesday":["08:00","08:30","09:00","09:30","14:00","14:30","15:00"],"wednesday":[],"thursday":["08:00","08:30","09:00","09:30","10:00","14:00","14:30"],"friday":["08:00","08:30","09:00","14:00","14:30","15:00"],"saturday":["08:00","08:30","09:00","09:30","10:00"],"sunday":[]}'),
('doc_03', N'BS. Lê Minh Tuấn', N'ThS.BS', 'sp_03', N'Tim mạch', 500000, 4.7, 98, '', N'Chuyên gia tim mạch can thiệp hàng đầu, tốt nghiệp Đại học Y Hà Nội, nhiều năm công tác tại Viện Tim mạch Quốc gia', '{"monday":["08:00","08:30","09:00","14:00","14:30","15:00"],"tuesday":["08:00","08:30","09:00","09:30"],"wednesday":["08:00","08:30","09:00","14:00","14:30","15:00","15:30"],"thursday":[],"friday":["08:00","08:30","09:00","09:30","14:00","14:30"],"saturday":[],"sunday":[]}'),
('doc_04', N'BS. Phạm Hồng Nhung', N'BS.CKI', 'sp_04', N'Da liễu', 300000, 4.6, 156, '', N'Chuyên gia Da liễu thẩm mỹ, chuyên điều trị các bệnh viêm da cơ địa, mụn trứng cá nặng và trẻ hóa da', '{"monday":["09:00","09:30","10:00","10:30","14:00","14:30"],"tuesday":["08:00","08:30","09:00","09:30","10:00"],"wednesday":["09:00","09:30","10:00","14:00","14:30","15:00"],"thursday":["08:00","08:30","09:00","09:30","14:00","14:30"],"friday":[],"saturday":["08:00","08:30","09:00","09:30"],"sunday":[]}'),
('doc_05', N'BS. Võ Thanh Hùng', N'PGS.TS', 'sp_05', N'Tai Mũi Họng', 350000, 4.8, 89, '', N'Phó Giáo sư chuyên ngành Tai Mũi Họng, hơn 25 năm kinh nghiệm giảng dạy và điều trị lâm sàng', '{"monday":["08:00","08:30","09:00","09:30","10:00"],"tuesday":["14:00","14:30","15:00","15:30","16:00"],"wednesday":["08:00","08:30","09:00","09:30"],"thursday":["08:00","08:30","09:00","14:00","14:30","15:00"],"friday":["08:00","08:30","09:00","09:30","10:00"],"saturday":[],"sunday":[]}');

-- 4. Thêm dữ liệu danh mục Tài Khoản (Accounts)
INSERT INTO Accounts (Id, Username, Password, Role, DoctorId, Name, Avatar, CreatedAt) VALUES
('acc_admin', 'admin', 'admin123', 'admin', NULL, N'Quản trị viên', NULL, '2026-01-01 00:00:00'),
('acc_doc01', 'doctor01', 'doctor123', 'doctor', 'doc_01', N'BS. Nguyễn Văn An', NULL, '2026-01-01 00:00:00'),
('acc_doc02', 'doctor02', 'doctor123', 'doctor', 'doc_02', N'BS. Trần Thị Bích', NULL, '2026-01-01 00:00:00'),
('acc_doc03', 'doctor03', 'doctor123', 'doctor', 'doc_03', N'BS. Lê Minh Tuấn', NULL, '2026-01-01 00:00:00'),
('acc_doc04', 'doctor04', 'doctor123', 'doctor', 'doc_04', N'BS. Phạm Hồng Nhung', NULL, '2026-01-01 00:00:00'),
('acc_doc05', 'doctor05', 'doctor123', 'doctor', 'doc_05', N'BS. Võ Thanh Hùng', NULL, '2026-01-01 00:00:00');

-- 5. Thêm dữ liệu danh mục Thuốc (Medicines)
INSERT INTO Medicines (Id, Name, Ingredients, Unit, ExpiryDate) VALUES
('med_01', N'Paracetamol 500mg', N'Paracetamol', N'Viên', '2027-12-31'),
('med_02', N'Amoxicillin 250mg', N'Amoxicillin', N'Viên', '2026-05-15'),
('med_03', N'Ibuprofen 400mg', N'Ibuprofen', N'Viên', '2028-01-20'),
('med_04', N'Vitamin C 1000mg', N'Vitamin C', N'Viên sủi', '2026-10-10'),
('med_05', N'Thuốc ho Bảo Thanh', N'Thảo dược', N'Chai', '2026-12-28');

-- 6. Tạo bảng tạm trong bộ nhớ (Memory Tables) phục vụ vòng lặp sinh dữ liệu ngẫu nhiên
DECLARE @Patients TABLE (Id INT IDENTITY(1,1), Name NVARCHAR(100), Phone NVARCHAR(15), Gender NVARCHAR(10), BirthYear INT);
INSERT INTO @Patients (Name, Phone, Gender, BirthYear) VALUES
(N'Nguyễn Thị Hoa', '0912345678', N'Nữ', 1990),
(N'Trần Văn Minh', '0987654321', N'Nam', 1985),
(N'Lê Thị Lan', '0909123456', N'Nữ', 1995),
(N'Phạm Quốc Bảo', '0978111222', N'Nam', 1978),
(N'Hoàng Thị Yến', '0961222333', N'Nữ', 2000),
(N'Đỗ Văn Tùng', '0945333444', N'Nam', 1970),
(N'Nguyễn Tấn Đạt', '0934111222', N'Nam', 1992),
(N'Lê Minh Hoàng', '0923555666', N'Nam', 1988),
(N'Phạm Thị Mai', '0915666777', N'Nữ', 1994),
(N'Vũ Đức Kiên', '0905888999', N'Nam', 1983),
(N'Đinh Tuấn Nghĩa', '0944999000', N'Nam', 1991),
(N'Nguyễn Thị Duyên', '0977222333', N'Nữ', 1997);

DECLARE @SymptomsList TABLE (Id INT IDENTITY(1,1), Symptoms NVARCHAR(200), Diagnosis NVARCHAR(200), Prescription NVARCHAR(200), JsonRx NVARCHAR(MAX));
INSERT INTO @SymptomsList (Symptoms, Diagnosis, Prescription, JsonRx) VALUES
(N'Đau đầu dữ dội vùng trán, buồn nôn, nhạy cảm ánh sáng', N'Đau nửa đầu migraine', N'Paracetamol 500mg x20, Ibuprofen 400mg x10', '[{"name":"Paracetamol 500mg","qty":20,"unit":"Viên","dosage":"Ngày 2 lần, mỗi lần 1 viên, sau ăn"},{"name":"Ibuprofen 400mg","qty":10,"unit":"Viên","dosage":"Ngày 1 lần khi đau, sau ăn no"}]'),
(N'Đau bụng vùng thượng vị, ợ chua nhiều lần trong ngày', N'Viêm dạ dày cấp tính', N'Omeprazole 20mg x14, Sucralfate 1g x21', '[{"name":"Omeprazole 20mg","qty":14,"unit":"Viên","dosage":"Ngày 2 lần trước ăn 30 phút"},{"name":"Sucralfate 1g","qty":21,"unit":"Gói","dosage":"Ngày 3 lần trước ăn"}]'),
(N'Bé sốt cao 39 độ, ho khan nhiều về đêm, khó thở nhẹ', N'Viêm phế quản cấp tính', N'Amoxicillin 250mg x21, Thuốc ho Bảo Thanh x1', '[{"name":"Amoxicillin 250mg","qty":21,"unit":"Viên","dosage":"Ngày 3 lần, mỗi lần 1 viên"},{"name":"Thuốc ho Bảo Thanh","qty":1,"unit":"Chai","dosage":"Ngày 3 lần, 10ml mỗi lần"}]'),
(N'Đau ngực trái lan ra sau vai, hồi hộp, nhịp tim nhanh', N'Rối loạn nhịp tim', N'Bisoprolol 2.5mg x30, Aspirin 81mg x30', '[{"name":"Bisoprolol 2.5mg","qty":30,"unit":"Viên","dosage":"Ngày 1 viên, sáng"},{"name":"Aspirin 81mg","qty":30,"unit":"Viên","dosage":"Ngày 1 viên, sau ăn trưa"}]'),
(N'Nổi mẩn đỏ ngứa khắp người sau khi ăn hải sản', N'Mề đay dị ứng cấp tính', N'Loratadine 10mg x10, Vitamin C 1000mg x10', '[{"name":"Loratadine 10mg","qty":10,"unit":"Viên","dosage":"Ngày 1 viên, tối"},{"name":"Vitamin C 1000mg","qty":10,"unit":"Viên sủi","dosage":"Ngày 1 viên, sáng"}]'),
(N'Nghẹt mũi chảy dịch xanh, đau tức vùng xoang trán', N'Viêm xoang mãn tính đợt cấp', N'Amoxicillin 250mg x21, Vitamin C 1000mg x10', '[{"name":"Amoxicillin 250mg","qty":21,"unit":"Viên","dosage":"Ngày 3 lần, mỗi lần 1 viên"},{"name":"Vitamin C 1000mg","qty":10,"unit":"Viên sủi","dosage":"Ngày 1 viên, sáng"}]');

DECLARE @Times TABLE (Id INT IDENTITY(1,1), TimeVal NVARCHAR(10));
INSERT INTO @Times (TimeVal) VALUES ('08:00'), ('08:30'), ('09:00'), ('09:30'), ('10:00'), ('10:30'), ('14:00'), ('14:30'), ('15:00'), ('15:30'), ('16:00');

DECLARE @Reasons TABLE (Id INT IDENTITY(1,1), ReasonVal NVARCHAR(200));
INSERT INTO @Reasons (ReasonVal) VALUES (N'Đau đầu chóng mặt kéo dài'), (N'Đau bụng thượng vị âm ỉ'), (N'Sốt cao ho đờm nhiều'), (N'Khám sức khỏe tổng quát'), (N'Đau tim khó thở nhẹ'), (N'Nổi mẩn đỏ dị ứng da');

-- 7. Chạy vòng lặp sinh ngẫu nhiên khoảng 90 bản ghi lịch hẹn rải đều từ tháng 1/2026 đến nay
DECLARE @Counter INT = 1;
DECLARE @TotalAppts INT = 90; 

DECLARE @RandPatientId INT;
DECLARE @RandDoctorId VARCHAR(10);
DECLARE @RandSymptomId INT;
DECLARE @RandTimeId INT;
DECLARE @RandReasonId INT;

DECLARE @PatName NVARCHAR(100), @PatPhone NVARCHAR(15), @PatGender NVARCHAR(10), @PatBirth INT;
DECLARE @DocName NVARCHAR(100), @DocSpecialty NVARCHAR(100), @DocPrice DECIMAL(18,2);
DECLARE @SympText NVARCHAR(200), @DiagText NVARCHAR(200), @PresText NVARCHAR(200), @JsonRxText NVARCHAR(MAX);
DECLARE @TimeText NVARCHAR(10), @ReasonText NVARCHAR(200);

DECLARE @RandDays INT;
DECLARE @DateVal NVARCHAR(10);
DECLARE @StatusVal NVARCHAR(20);
DECLARE @ApptId NVARCHAR(50);
DECLARE @BookingId NVARCHAR(20);
DECLARE @PaymentMethod NVARCHAR(20);
DECLARE @PaymentStatus NVARCHAR(20);
DECLARE @PaidDate DATETIME;
DECLARE @MaBenhAn INT = 1001;

WHILE @Counter <= @TotalAppts
BEGIN
    -- Chọn ngẫu nhiên index
    SET @RandPatientId = 1 + ABS(CHECKSUM(NEWID())) % 12;
    SET @RandDoctorId = 'doc_0' + CAST((1 + ABS(CHECKSUM(NEWID())) % 5) AS VARCHAR(1));
    SET @RandSymptomId = 1 + ABS(CHECKSUM(NEWID())) % 6;
    SET @RandTimeId = 1 + ABS(CHECKSUM(NEWID())) % 11;
    SET @RandReasonId = 1 + ABS(CHECKSUM(NEWID())) % 6;
    
    -- Phát sinh ngày ngẫu nhiên trong vòng 186 ngày trước ngày hôm nay (từ khoảng tháng 1/2026)
    SET @RandDays = ABS(CHECKSUM(NEWID())) % 186;
    DECLARE @CalculatedDate DATE = DATEADD(DAY, -@RandDays, GETDATE());
    SET @DateVal = CONVERT(NVARCHAR(10), @CalculatedDate, 120);

    -- Lấy thông tin tương ứng
    SELECT @PatName = Name, @PatPhone = Phone, @PatGender = Gender, @PatBirth = BirthYear FROM @Patients WHERE Id = @RandPatientId;
    SELECT @DocName = Name, @DocSpecialty = SpecialtyName, @DocPrice = Price FROM Doctors WHERE Id = @RandDoctorId;
    SELECT @SympText = Symptoms, @DiagText = Diagnosis, @PresText = Prescription, @JsonRxText = JsonRx FROM @SymptomsList WHERE Id = @RandSymptomId;
    SELECT @TimeText = TimeVal FROM @Times WHERE Id = @RandTimeId;
    SELECT @ReasonText = ReasonVal FROM @Reasons WHERE Id = @RandReasonId;

    -- Thiết lập mã định danh duy nhất
    SET @ApptId = 'APT' + CONVERT(NVARCHAR(20), DATEADD(SECOND, @Counter, GETDATE()), 112) + RIGHT('00000' + CAST(@Counter AS VARCHAR(5)), 5);
    SET @BookingId = 'BK-' + UPPER(SUBSTRING(REPLACE(CAST(NEWID() as VARCHAR(36)), '-', ''), 1, 8));

    -- Quyết định trạng thái logic
    -- Nếu ngày khám ở quá khứ: 90% là hoàn thành (completed) để sinh bệnh án phục vụ Chart.js, 10% bị hủy (cancelled)
    -- Nếu ngày khám ở hôm nay hoặc tương lai: ngẫu nhiên pending, approved hoặc cancelled
    IF @CalculatedDate < CAST(GETDATE() AS DATE)
    BEGIN
        SET @StatusVal = CASE WHEN ABS(CHECKSUM(NEWID())) % 10 = 0 THEN 'cancelled' ELSE 'completed' END;
    END
    ELSE
    BEGIN
        SET @StatusVal = CASE WHEN ABS(CHECKSUM(NEWID())) % 5 = 0 THEN 'cancelled' 
                              WHEN ABS(CHECKSUM(NEWID())) % 2 = 0 THEN 'approved' 
                              ELSE 'pending' END;
    END

    -- Đồng bộ trạng thái thanh toán theo trạng thái lịch khám
    IF @StatusVal = 'completed'
    BEGIN
        SET @PaymentStatus = 'paid';
        SET @PaymentMethod = CASE WHEN ABS(CHECKSUM(NEWID())) % 3 = 0 THEN 'momo' 
                                  WHEN ABS(CHECKSUM(NEWID())) % 3 = 1 THEN 'bank_transfer' 
                                  ELSE 'cash' END;
        SET @PaidDate = CAST(@DateVal + ' ' + @TimeText AS DATETIME);
    END
    ELSE IF @StatusVal = 'approved'
    BEGIN
        SET @PaymentStatus = CASE WHEN ABS(CHECKSUM(NEWID())) % 2 = 0 THEN 'paid' ELSE 'unpaid' END;
        SET @PaymentMethod = CASE WHEN @PaymentStatus = 'paid' THEN 'bank_transfer' ELSE 'cash' END;
        SET @PaidDate = CASE WHEN @PaymentStatus = 'paid' THEN CAST(@DateVal + ' ' + @TimeText AS DATETIME) ELSE NULL END;
    END
    ELSE
    BEGIN
        SET @PaymentStatus = 'unpaid';
        SET @PaymentMethod = 'cash';
        SET @PaidDate = NULL;
    END

    -- Insert vào bảng Appointments (LichHen)
    INSERT INTO Appointments (Id, BookingId, PatientName, Phone, Email, Gender, BirthYear, Reason, DoctorId, DoctorName, Specialty, Date, Time, Status, CreatedAt, PaymentMethod, PaymentStatus, PaymentAmount, PaidAt)
    VALUES (
        @ApptId, 
        @BookingId, 
        @PatName, 
        @PatPhone, 
        CAST(LOWER(REPLACE(@PatName, ' ', '')) + '@email.com' AS NVARCHAR(100)), 
        @PatGender, 
        @PatBirth, 
        @ReasonText, 
        @RandDoctorId, 
        @DocName, 
        @DocSpecialty, 
        @DateVal, 
        @TimeText, 
        @StatusVal, 
        DATEADD(HOUR, -2, CAST(@DateVal + ' ' + @TimeText AS DATETIME)),
        @PaymentMethod,
        @PaymentStatus,
        @DocPrice,
        @PaidDate
    );

    -- Tự động sinh bản ghi Bệnh Án & Kê Đơn trong MedicalHistories (BenhAn & ChiTietDonThuoc) nếu trạng thái là 'completed'
    IF @StatusVal = 'completed'
    BEGIN
        INSERT INTO MedicalHistories (SoDienThoai, MaBenhAn, BookingId, PatientName, DoctorName, Specialty, Symptoms, Diagnosis, Prescription, VisitDate, Notes, PrescriptionItemsJson)
        VALUES (
            @PatPhone,
            @MaBenhAn,
            @BookingId,
            @PatName,
            @DocName,
            @DocSpecialty,
            @SympText,
            @DiagText,
            @PresText,
            @DateVal,
            N'Uống thuốc đúng liều lượng, tái khám khi có dấu hiệu bất thường.',
            @JsonRxText
        );
        SET @MaBenhAn = @MaBenhAn + 1;
    END

    SET @Counter = @Counter + 1;
END

-- 8. Cập nhật thống kê Reviews và Rating ngẫu nhiên của các Bác Sĩ để đồng bộ dữ liệu hiển thị
UPDATE Doctors
SET Reviews = (SELECT COUNT(*) FROM Appointments WHERE Appointments.DoctorId = Doctors.Id AND Appointments.Status = 'completed'),
    Rating = 4.5 + (ABS(CHECKSUM(Id)) % 6) * 0.1;

-- 9. Cập nhật số lượng Bác Sĩ thuộc mỗi Chuyên Khoa
UPDATE Specialties
SET DoctorCount = (SELECT COUNT(*) FROM Doctors WHERE Doctors.SpecialtyId = Specialties.Id);

PRINT '=========================================================================';
PRINT 'SUCCESSFULLY GENERATED DUMMY DATA FOR WEBDATLICHKHAM (SQL SERVER T-SQL)';
PRINT '=========================================================================';
