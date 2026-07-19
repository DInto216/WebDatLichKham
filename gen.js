const fs = require('fs');

let sql = `USE [WebDatLichKham];\nGO\n\n`;
sql += `-- Xóa dữ liệu cũ của Appointments và MedicalHistories nếu cần\n`;
sql += `DELETE FROM [MedicalHistories];\n`;
sql += `DELETE FROM [Appointments];\n`;
sql += `GO\n\n`;

const reasons = ['Đau đầu, chóng mặt', 'Khám tổng quát', 'Đau bụng vùng thượng vị', 'Ho, sốt kéo dài', 'Nổi mẩn đỏ ngứa', 'Khám định kỳ', 'Đau nhức xương khớp', 'Mất ngủ kéo dài', 'Tim đập nhanh', 'Viêm họng'];
const names = ['Nguyễn Văn A', 'Trần Thị B', 'Lê Văn C', 'Phạm Thị D', 'Hoàng Văn E', 'Vũ Thị F', 'Đặng Văn G', 'Bùi Thị H', 'Đỗ Văn I', 'Hồ Thị K', 'Đinh Văn L', 'Mai Thị M'];
const doctors = [
    {id:'doc_01', name:'BS. Nguyễn Văn An', specialty:'Nội tổng quát'},
    {id:'doc_02', name:'BS. Trần Thị Bích', specialty:'Nhi khoa'},
    {id:'doc_03', name:'BS. Lê Minh Tuấn', specialty:'Tim mạch'},
    {id:'doc_04', name:'BS. Phạm Hồng Nhung', specialty:'Da liễu'},
    {id:'doc_05', name:'BS. Võ Thanh Hùng', specialty:'Tai Mũi Họng'},
    {id:'doc_06', name:'BS. Đặng Thị Mai', specialty:'Sản phụ khoa'}
];

const startDate = new Date(2025, 0, 1).getTime();
const endDate = new Date().getTime();

const randomChoice = arr => arr[Math.floor(Math.random() * arr.length)];
const randomInt = (min, max) => Math.floor(Math.random() * (max - min + 1)) + min;
const formatStr = d => d.toISOString().split('T')[0] + ' ' + d.toTimeString().split(' ')[0];
const formatDate = d => d.toISOString().split('T')[0];
const uuid8 = () => Math.random().toString(36).substring(2, 10).toUpperCase();

sql += `SET IDENTITY_INSERT [MedicalHistories] ON;\nGO\n\n`;

for (let i = 0; i < 500; i++) {
    const rndTime = startDate + Math.random() * (endDate - startDate);
    const randomDate = new Date(rndTime);
    const doc = randomChoice(doctors);
    const isPast = rndTime < (endDate - 86400000);
    
    let status = 'pending';
    if (isPast) {
        const rnd = randomInt(0, 10);
        if (rnd < 7) status = 'completed';
        else if (rnd < 8) status = 'cancelled';
        else status = 'approved';
    } else {
        if (randomInt(0, 1) === 0) status = 'approved';
    }
    
    const aptId = 'apt_gen_' + uuid8().toLowerCase();
    const bookingId = 'BK-' + uuid8();
    const patName = randomChoice(names) + ' ' + randomInt(1, 100);
    const phone = '09' + randomInt(10000000, 99999999);
    const email = `patient${i}@email.com`;
    const gender = randomInt(0, 1) === 0 ? 'Nam' : 'Nữ';
    const birthYear = randomInt(1960, 2015);
    const reason = randomChoice(reasons);
    const hour = randomInt(8, 16).toString().padStart(2, '0');
    const minute = randomInt(0, 1) === 0 ? '00' : '30';
    const timeStr = `${hour}:${minute}`;
    
    const createdAt = new Date(rndTime - randomInt(1, 10) * 86400000);
    
    sql += `INSERT INTO [Appointments] ([Id], [BookingId], [PatientName], [Phone], [Email], [Gender], [BirthYear], [Reason], [DoctorId], [DoctorName], [Specialty], [Date], [Time], [Status], [CreatedAt], [PaymentMethod], [PaymentStatus], [PaymentAmount], [PaidAt]) VALUES ('${aptId}', '${bookingId}', N'${patName}', '${phone}', '${email}', N'${gender}', ${birthYear}, N'${reason}', '${doc.id}', N'${doc.name}', N'${doc.specialty}', '${formatDate(randomDate)}', '${timeStr}', '${status}', '${formatStr(createdAt)}', 'cash', 'unpaid', 0, NULL);\n`;
    
    if (status === 'completed') {
        const mabenhan = 5000 + i;
        const malichsu = 10000 + i;
        sql += `INSERT INTO [MedicalHistories] ([MaLichSu], [SoDienThoai], [MaBenhAn], [BookingId], [PatientName], [DoctorName], [Specialty], [Symptoms], [Diagnosis], [Prescription], [VisitDate], [Notes], [PrescriptionItemsJson]) VALUES (${malichsu}, '${phone}', ${mabenhan}, '${bookingId}', N'${patName}', N'${doc.name}', N'${doc.specialty}', N'${reason}', N'Chẩn đoán mẫu: ${reason}', N'Thuốc mẫu theo chỉ định', '${formatDate(randomDate)}', N'Khám và điều trị ngoại trú', '[]');\n`;
    }
}

sql += `\nGO\nSET IDENTITY_INSERT [MedicalHistories] OFF;\nGO\n`;
fs.writeFileSync('SampleData.sql', sql, 'utf8');
console.log('Done!');
