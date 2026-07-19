import uuid, random, datetime

sql_lines = []
sql_lines.append("USE [WebDatLichKham];")
sql_lines.append("GO\n")

sample_reasons = ['Đau đầu, chóng mặt', 'Khám tổng quát', 'Đau bụng vùng thượng vị', 'Ho, sốt kéo dài', 'Nổi mẩn đỏ ngứa', 'Khám định kỳ', 'Đau nhức xương khớp', 'Mất ngủ kéo dài', 'Tim đập nhanh', 'Viêm họng']
sample_names = ['Nguyễn Văn A', 'Trần Thị B', 'Lê Văn C', 'Phạm Thị D', 'Hoàng Văn E', 'Vũ Thị F', 'Đặng Văn G', 'Bùi Thị H', 'Đỗ Văn I', 'Hồ Thị K', 'Đinh Văn L', 'Mai Thị M']
statuses = ['pending', 'approved', 'completed', 'cancelled']

doctors = [
    {'id': 'doc_01', 'name': 'BS. Nguyễn Văn An', 'specialty': 'Nội tổng quát'},
    {'id': 'doc_02', 'name': 'BS. Trần Thị Bích', 'specialty': 'Nhi khoa'},
    {'id': 'doc_03', 'name': 'BS. Lê Minh Tuấn', 'specialty': 'Tim mạch'},
    {'id': 'doc_04', 'name': 'BS. Phạm Hồng Nhung', 'specialty': 'Da liễu'},
    {'id': 'doc_05', 'name': 'BS. Võ Thanh Hùng', 'specialty': 'Tai Mũi Họng'},
    {'id': 'doc_06', 'name': 'BS. Đặng Thị Mai', 'specialty': 'Sản phụ khoa'}
]

start_date = datetime.date(2025, 1, 1)
end_date = datetime.date.today()
total_days = (end_date - start_date).days

sql_lines.append("-- Xóa dữ liệu cũ của Appointments và MedicalHistories nếu cần")
sql_lines.append("DELETE FROM [MedicalHistories];")
sql_lines.append("DELETE FROM [Appointments];")
sql_lines.append("GO\n")

for i in range(500):
    random_days = random.randint(0, total_days)
    random_date = start_date + datetime.timedelta(days=random_days)
    doc = random.choice(doctors)
    is_past = random_date < end_date - datetime.timedelta(days=1)
    status = random.choices(['completed', 'cancelled', 'pending', 'approved'], weights=[70, 10, 10, 10])[0] if is_past else random.choices(['pending', 'approved'], weights=[50, 50])[0]
    
    apt_id = 'apt_gen_' + str(uuid.uuid4())[:8]
    booking_id = 'BK-' + str(uuid.uuid4())[:8].upper()
    patient_name = random.choice(sample_names) + ' ' + str(random.randint(1, 100))
    phone = '09' + str(random.randint(10000000, 99999999))
    email = f'patient{i}@email.com'
    gender = random.choice(['Nam', 'Nữ'])
    birth_year = random.randint(1960, 2015)
    reason = random.choice(sample_reasons)
    time_str = f'{random.randint(8, 16):02d}:{random.choice(["00", "30"])}'
    created_at = random_date - datetime.timedelta(days=random.randint(1, 10))
    
    sql_lines.append(f"INSERT INTO [Appointments] ([Id], [BookingId], [PatientName], [Phone], [Email], [Gender], [BirthYear], [Reason], [DoctorId], [DoctorName], [Specialty], [Date], [Time], [Status], [CreatedAt], [PaymentMethod], [PaymentStatus], [PaymentAmount], [PaidAt]) VALUES ('{apt_id}', '{booking_id}', N'{patient_name}', '{phone}', '{email}', N'{gender}', {birth_year}, N'{reason}', '{doc['id']}', N'{doc['name']}', N'{doc['specialty']}', '{random_date.strftime('%Y-%m-%d')}', '{time_str}', '{status}', '{created_at.strftime('%Y-%m-%d %H:%M:%S')}', 'cash', 'unpaid', 0, NULL);")
    
    if status == 'completed':
        mabenhan = 5000 + i
        sql_lines.append(f"INSERT INTO [MedicalHistories] ([SoDienThoai], [MaBenhAn], [BookingId], [PatientName], [DoctorName], [Specialty], [Symptoms], [Diagnosis], [Prescription], [VisitDate], [Notes], [PrescriptionItemsJson]) VALUES ('{phone}', {mabenhan}, '{booking_id}', N'{patient_name}', N'{doc['name']}', N'{doc['specialty']}', N'{reason}', N'Chẩn đoán mẫu: {reason}', N'Thuốc mẫu theo chỉ định', '{random_date.strftime('%Y-%m-%d')}', N'Khám và điều trị ngoại trú', '[]');")

sql_lines.append("GO\n")

with open('SampleData.sql', 'w', encoding='utf-8') as f:
    f.write('\n'.join(sql_lines))

print('Generated SampleData.sql successfully.')
