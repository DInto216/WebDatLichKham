$sql = "USE [WebDatLichKham];`r`nGO`r`n`r`n"
$sql += "-- Xóa dữ liệu cũ của Appointments và MedicalHistories nếu cần`r`n"
$sql += "DELETE FROM [MedicalHistories];`r`n"
$sql += "DELETE FROM [Appointments];`r`n"
$sql += "GO`r`n`r`n"

$reasons = @("Đau đầu, chóng mặt", "Khám tổng quát", "Đau bụng vùng thượng vị", "Ho, sốt kéo dài", "Nổi mẩn đỏ ngứa", "Khám định kỳ", "Đau nhức xương khớp", "Mất ngủ kéo dài", "Tim đập nhanh", "Viêm họng")
$names = @("Nguyễn Văn A", "Trần Thị B", "Lê Văn C", "Phạm Thị D", "Hoàng Văn E", "Vũ Thị F", "Đặng Văn G", "Bùi Thị H", "Đỗ Văn I", "Hồ Thị K", "Đinh Văn L", "Mai Thị M")
$doctors = @(
    @{id="doc_01"; name="BS. Nguyễn Văn An"; specialty="Nội tổng quát"},
    @{id="doc_02"; name="BS. Trần Thị Bích"; specialty="Nhi khoa"},
    @{id="doc_03"; name="BS. Lê Minh Tuấn"; specialty="Tim mạch"},
    @{id="doc_04"; name="BS. Phạm Hồng Nhung"; specialty="Da liễu"},
    @{id="doc_05"; name="BS. Võ Thanh Hùng"; specialty="Tai Mũi Họng"},
    @{id="doc_06"; name="BS. Đặng Thị Mai"; specialty="Sản phụ khoa"}
)

$startDate = Get-Date -Year 2025 -Month 1 -Day 1
$endDate = Get-Date
$totalDays = ($endDate - $startDate).Days

for ($i = 0; $i -lt 500; $i++) {
    $randomDays = Get-Random -Minimum 0 -Maximum $totalDays
    $randomDate = $startDate.AddDays($randomDays)
    $doc = $doctors | Get-Random
    $isPast = $randomDate -lt $endDate.AddDays(-1)
    
    $status = "pending"
    if ($isPast) {
        $rnd = Get-Random -Minimum 0 -Maximum 10
        if ($rnd -lt 7) { $status = "completed" }
        elseif ($rnd -lt 8) { $status = "cancelled" }
        else { $status = "approved" }
    } else {
        if ((Get-Random -Minimum 0 -Maximum 2) -eq 0) { $status = "approved" }
    }
    
    $guid1 = [guid]::NewGuid().ToString().Substring(0,8)
    $guid2 = [guid]::NewGuid().ToString().Substring(0,8).ToUpper()
    $aptId = "apt_gen_$guid1"
    $bookingId = "BK-$guid2"
    $patName = ($names | Get-Random) + " " + (Get-Random -Minimum 1 -Maximum 100)
    $phone = "09" + (Get-Random -Minimum 10000000 -Maximum 99999999)
    $email = "patient${i}@email.com"
    $gender = if ((Get-Random -Minimum 0 -Maximum 2) -eq 0) { "Nam" } else { "Nữ" }
    $birthYear = Get-Random -Minimum 1960 -Maximum 2015
    $reason = $reasons | Get-Random
    $hour = Get-Random -Minimum 8 -Maximum 16
    $minute = if ((Get-Random -Minimum 0 -Maximum 2) -eq 0) { "00" } else { "30" }
    $timeStr = "{0:D2}:{1}" -f $hour, $minute
    $createdAt = $randomDate.AddDays(-(Get-Random -Minimum 1 -Maximum 10))
    
    $dateStr = $randomDate.ToString('yyyy-MM-dd')
    $createdStr = $createdAt.ToString('yyyy-MM-dd HH:mm:ss')
    
    $docId = $doc.id
    $docName = $doc.name
    $docSpec = $doc.specialty
    
    $sql += "INSERT INTO [Appointments] ([Id], [BookingId], [PatientName], [Phone], [Email], [Gender], [BirthYear], [Reason], [DoctorId], [DoctorName], [Specialty], [Date], [Time], [Status], [CreatedAt], [PaymentMethod], [PaymentStatus], [PaymentAmount], [PaidAt]) VALUES ('$aptId', '$bookingId', N'$patName', '$phone', '$email', N'$gender', $birthYear, N'$reason', '$docId', N'$docName', N'$docSpec', '$dateStr', '$timeStr', '$status', '$createdStr', 'cash', 'unpaid', 0, NULL);`r`n"
    
    if ($status -eq "completed") {
        $mabenhan = 5000 + $i
        $sql += "INSERT INTO [MedicalHistories] ([SoDienThoai], [MaBenhAn], [BookingId], [PatientName], [DoctorName], [Specialty], [Symptoms], [Diagnosis], [Prescription], [VisitDate], [Notes], [PrescriptionItemsJson]) VALUES ('$phone', $mabenhan, '$bookingId', N'$patName', N'$docName', N'$docSpec', N'$reason', N'Chẩn đoán mẫu: $reason', N'Thuốc mẫu theo chỉ định', '$dateStr', N'Khám và điều trị ngoại trú', '[]');`r`n"
    }
}

$sql += "GO`r`n"
[IO.File]::WriteAllText("SampleData.sql", $sql, [System.Text.Encoding]::UTF8)
Write-Host "Generated SampleData.sql"
