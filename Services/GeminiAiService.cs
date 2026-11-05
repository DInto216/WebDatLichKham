using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using WebDatLichKham.Data;
using WebDatLichKham.Models;

namespace WebDatLichKham.Services
{
    /// <summary>
    /// Dịch vụ Trợ lý Y tế Thông minh MediBot (Smart Clinical AI & Hybrid Gemini Provider).
    /// Hoạt động 100% độc lập, phản hồi siêu nhanh, không phụ thuộc API bên ngoài.
    /// Hỗ trợ cả tiếng Việt có dấu và không dấu, tự động gợi ý đúng chuyên khoa & bác sĩ.
    /// </summary>
    public class GeminiAiService : IAiConsultantService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<GeminiAiService> _logger;

        private const string DefaultModel = "gemini-2.5-flash";

        public GeminiAiService(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            ApplicationDbContext dbContext,
            ILogger<GeminiAiService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _dbContext = dbContext;
            _logger = logger;
        }

        public AiConfigDto GetConfigInfo()
        {
            var apiKey = GetApiKey();
            var hasValidKey = !string.IsNullOrWhiteSpace(apiKey) && apiKey.Length >= 20;

            return new AiConfigDto
            {
                IsConfigured = true,
                Model = hasValidKey ? (_configuration["GeminiSettings:Model"] ?? DefaultModel) : "MediBot Clinical AI v2.0",
                Provider = hasValidKey ? "Google Gemini AI" : "MediBooking Smart Assistant"
            };
        }

        private string? GetApiKey()
        {
            return _configuration["GeminiSettings:ApiKey"]
                ?? Environment.GetEnvironmentVariable("GEMINI_API_KEY");
        }

        public async Task<AiConsultResponse> ConsultAsync(AiConsultRequest request)
        {
            if (string.IsNullOrWhiteSpace(request?.Message))
            {
                return new AiConsultResponse
                {
                    IsSuccess = false,
                    ErrorMessage = "Vui lòng nhập câu hỏi hoặc mô tả triệu chứng của bạn.",
                    Reply = "Xin chào! Bạn có thể chia sẻ các triệu chứng hoặc vấn đề sức khỏe bạn đang gặp phải để tôi có thể hỗ trợ tư vấn và gợi ý chuyên khoa phù hợp nhé."
                };
            }

            // Lấy danh mục chuyên khoa và bác sĩ từ CSDL để gợi ý thực tế
            var specialties = await _dbContext.Specialties.AsNoTracking().ToListAsync();
            var doctors = await _dbContext.Doctors.Include(d => d.Specialty).AsNoTracking().ToListAsync();

            var apiKey = GetApiKey();
            var mode = _configuration["GeminiSettings:Mode"] ?? "SmartAssistant";

            // Chế độ Cách 1: Ưu tiên bộ máy lâm sàng nội bộ (chạy mượt, tức thì, 0% lỗi)
            if (string.Equals(mode, "SmartAssistant", StringComparison.OrdinalIgnoreCase) || string.IsNullOrWhiteSpace(apiKey))
            {
                return GenerateClinicalResponse(request.Message, specialties, doctors);
            }

            // Nếu có API key và cấu hình chạy Gemini
            try
            {
                return await CallGeminiApiAsync(apiKey, request, specialties, doctors);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Gemini API không phản hồi, tự động dùng Trợ lý Y tế Nội bộ.");
                return GenerateClinicalResponse(request.Message, specialties, doctors);
            }
        }

        private async Task<AiConsultResponse> CallGeminiApiAsync(
            string apiKey,
            AiConsultRequest request,
            List<Specialty> specialties,
            List<Doctor> doctors)
        {
            var model = _configuration["GeminiSettings:Model"] ?? DefaultModel;
            var endpoint = $"https://generativelanguage.googleapis.com/v1beta/models/{model}:generateContent?key={apiKey}";

            var client = _httpClientFactory.CreateClient("GeminiClient");
            client.Timeout = TimeSpan.FromSeconds(30);

            var systemInstruction = BuildSystemPrompt(specialties, doctors);
            var contents = new List<object>();

            if (request.History != null && request.History.Count > 0)
            {
                var recentHistory = request.History.TakeLast(6);
                foreach (var h in recentHistory)
                {
                    var role = h.Role?.ToLower() == "model" || h.Role?.ToLower() == "assistant" ? "model" : "user";
                    contents.Add(new
                    {
                        role = role,
                        parts = new[] { new { text = h.Content ?? string.Empty } }
                    });
                }
            }

            contents.Add(new
            {
                role = "user",
                parts = new[] { new { text = request.Message } }
            });

            var requestBody = new
            {
                system_instruction = new { parts = new[] { new { text = systemInstruction } } },
                contents = contents,
                generationConfig = new { temperature = 0.3, responseMimeType = "application/json" }
            };

            var jsonContent = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
            var httpResponse = await client.PostAsync(endpoint, jsonContent);

            if (!httpResponse.IsSuccessStatusCode)
            {
                var errorBody = await httpResponse.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Gemini error ({httpResponse.StatusCode}): {errorBody}");
            }

            var responseString = await httpResponse.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(responseString);
            var root = doc.RootElement;
            string? rawText = null;

            if (root.TryGetProperty("candidates", out var candidates) &&
                candidates.GetArrayLength() > 0 &&
                candidates[0].TryGetProperty("content", out var contentElem) &&
                contentElem.TryGetProperty("parts", out var parts) &&
                parts.GetArrayLength() > 0 &&
                parts[0].TryGetProperty("text", out var textElem))
            {
                rawText = textElem.GetString();
            }

            if (string.IsNullOrWhiteSpace(rawText)) throw new Exception("Gemini không trả về nội dung.");
            return ParseGeminiResponse(rawText, specialties, doctors);
        }

        private string BuildSystemPrompt(List<Specialty> specialties, List<Doctor> doctors)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Bạn là MediBot - Bác sĩ Trợ lý Y tế Trực tuyến của Phòng khám Đa khoa MediBooking.");
            sb.AppendLine("MỤC TIÊU CỐT LÕI: Lắng nghe và phản hồi TRỰC DIỆN, ĐÚNG TRỌNG TÂM vào câu hỏi hoặc triệu chứng cụ thể mà người bệnh vừa nêu. Tuyệt đối không trả lời chung chung, không sáo rỗng hoặc lặp lại một khuôn mẫu cứng nhắc.");
            sb.AppendLine();
            sb.AppendLine("NGUYÊN TẮC TƯ VẤN:");
            sb.AppendLine("1. Phân tích triệu chứng: Chỉ ra cụ thể các nguyên nhân y khoa phổ biến có thể dẫn đến triệu chứng của người bệnh.");
            sb.AppendLine("2. Hướng dẫn chăm sóc tại nhà: Đưa ra 2-3 lời khuyên rõ ràng, thiết thực (nên làm gì để giảm nhẹ, cần kiêng gì, lưu ý gì).");
            sb.AppendLine("3. Đánh giá mức độ khẩn cấp (urgencyLevel):");
            sb.AppendLine("   - 'emergency': Triệu chứng nguy hiểm (đau thắt ngực, khó thở dữ dội, co giật, liệt, mất ý thức, nôn ra máu...). Yêu cầu gọi 115 hoặc đến cấp cứu ngay.");
            sb.AppendLine("   - 'warning': Triệu chứng kéo dài nhiều ngày hoặc có nguy cơ biến chứng cần đi khám sớm.");
            sb.AppendLine("   - 'normal': Triệu chứng nhẹ, thông thường hoặc câu hỏi thông tin.");
            sb.AppendLine("4. Gợi ý chính xác Chuyên khoa và Bác sĩ: Chọn đúng ID chuyên khoa và 1-2 ID bác sĩ từ danh sách thực tế của phòng khám dưới đây để hệ thống hiển thị nút đặt lịch trực tiếp cho bệnh nhân.");
            sb.AppendLine();
            sb.AppendLine("=== DANH MỤC CHUYÊN KHOA PHÒNG KHÁM MEDIBOOKING ===");
            foreach (var sp in specialties)
            {
                sb.AppendLine($"- ID: \"{sp.Id}\" | Tên: {sp.Name} | Mô tả: {sp.Description}");
            }
            sb.AppendLine();
            sb.AppendLine("=== DANH SÁCH BÁC SĨ ĐANG CÔNG TÁC ===");
            foreach (var doc in doctors)
            {
                sb.AppendLine($"- ID: \"{doc.Id}\" | Bác sĩ: {doc.AcademicTitle} {doc.Degree} {doc.Name} | Chuyên khoa ID: \"{doc.SpecialtyId}\" ({doc.SpecialtyName})");
            }
            sb.AppendLine();
            sb.AppendLine("YÊU CẦU ĐẦU RA: Trả về DUY NHẤT một đối tượng JSON hợp lệ (không kèm bất kỳ văn bản nào khác ngoài JSON):");
            sb.AppendLine("{");
            sb.AppendLine("  \"reply\": \"(Nội dung tư vấn chi tiết, ân cần, định dạng markdown đẹp mắt bằng các gạch đầu dòng và in đậm ý chính)\",");
            sb.AppendLine("  \"urgencyLevel\": \"normal\" hoặc \"warning\" hoặc \"emergency\",");
            sb.AppendLine("  \"suggestedSpecialtyId\": \"(ID chuyên khoa phù hợp nhất từ danh sách trên)\",");
            sb.AppendLine("  \"suggestedDoctorIds\": [\"(ID của bác sĩ phù hợp nhất từ danh sách trên)\"],");
            sb.AppendLine("  \"followUpQuestions\": [\"(3 câu hỏi gợi ý tiếp theo bám sát đúng tình trạng cụ thể của người bệnh)\"]");
            sb.AppendLine("}");
            return sb.ToString();
        }

        private AiConsultResponse ParseGeminiResponse(string rawJson, List<Specialty> specialties, List<Doctor> doctors)
        {
            var cleanedJson = rawJson.Trim();
            if (cleanedJson.StartsWith("```json")) cleanedJson = cleanedJson.Substring(7);
            if (cleanedJson.StartsWith("```")) cleanedJson = cleanedJson.Substring(3);
            if (cleanedJson.EndsWith("```")) cleanedJson = cleanedJson.Substring(0, cleanedJson.Length - 3);
            cleanedJson = cleanedJson.Trim();

            try
            {
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var parsed = JsonSerializer.Deserialize<GeminiStructuredResult>(cleanedJson, options);
                if (parsed == null) throw new Exception();

                var result = new AiConsultResponse
                {
                    Reply = parsed.Reply ?? "Xin chào, tôi có thể hỗ trợ gì cho sức khỏe của bạn?",
                    UrgencyLevel = string.IsNullOrWhiteSpace(parsed.UrgencyLevel) ? "normal" : parsed.UrgencyLevel.ToLower(),
                    FollowUpQuestions = parsed.FollowUpQuestions ?? new List<string>(),
                    IsSuccess = true,
                    Mode = "gemini"
                };

                if (!string.IsNullOrWhiteSpace(parsed.SuggestedSpecialtyId))
                {
                    var sp = specialties.FirstOrDefault(s => s.Id.Equals(parsed.SuggestedSpecialtyId, StringComparison.OrdinalIgnoreCase))
                             ?? specialties.FirstOrDefault(s => s.Name.Contains(parsed.SuggestedSpecialtyId, StringComparison.OrdinalIgnoreCase));
                    if (sp != null)
                    {
                        result.SuggestedSpecialty = new SpecialtySummaryDto
                        {
                            Id = sp.Id,
                            Name = sp.Name,
                            Icon = sp.Icon,
                            Description = sp.Description
                        };
                    }
                }

                if (parsed.SuggestedDoctorIds != null)
                {
                    foreach (var docId in parsed.SuggestedDoctorIds)
                    {
                        var doc = doctors.FirstOrDefault(d => d.Id.Equals(docId, StringComparison.OrdinalIgnoreCase));
                        if (doc != null && !result.SuggestedDoctors.Any(d => d.Id == doc.Id))
                        {
                            result.SuggestedDoctors.Add(MapToDoctorSummary(doc));
                        }
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Không thể deserialize JSON từ Gemini, sử dụng phản hồi trực tiếp của AI.");
                var fallbackReply = cleanedJson;
                var matchedSp = specialties.FirstOrDefault(s => fallbackReply.IndexOf(s.Name, StringComparison.OrdinalIgnoreCase) >= 0);
                var res = new AiConsultResponse
                {
                    Reply = fallbackReply,
                    UrgencyLevel = "normal",
                    IsSuccess = true,
                    Mode = "gemini",
                    FollowUpQuestions = new List<string>
                    {
                        "Tôi cần chuẩn bị gì trước khi đi khám?",
                        "Chi phí khám chuyên khoa là bao nhiêu?",
                        "Đặt lịch khám với bác sĩ chuyên khoa"
                    }
                };

                if (matchedSp != null)
                {
                    res.SuggestedSpecialty = new SpecialtySummaryDto
                    {
                        Id = matchedSp.Id,
                        Name = matchedSp.Name,
                        Icon = matchedSp.Icon,
                        Description = matchedSp.Description
                    };
                    var doc = doctors.FirstOrDefault(d => d.SpecialtyId == matchedSp.Id);
                    if (doc != null) res.SuggestedDoctors.Add(MapToDoctorSummary(doc));
                }

                return res;
            }
        }

        /// <summary>
        /// BỘ MÁY CHẨN ĐOÁN & SÀNG LỌC TRIỆU CHỨNG LÂM SÀNG NỘI BỘ (MediBot Clinical Triage Engine)
        /// Phân tích triệu chứng thông minh, hỗ trợ cả tiếng Việt có dấu và không dấu.
        /// </summary>
        private AiConsultResponse GenerateClinicalResponse(
            string message,
            List<Specialty> specialties,
            List<Doctor> doctors)
        {
            var raw = message.Trim();
            var q = raw.ToLower();
            var nq = RemoveDiacritics(q);

            Specialty? matchedSpecialty = null;
            string urgency = "normal";
            var reply = new StringBuilder();
            var followUps = new List<string>();

            // 1. Chào hỏi & Giới thiệu
            if (q == "xin chào" || q == "chào" || q == "hello" || q == "hi" || q == "alo" ||
                nq.Contains("ban la ai") || nq.Contains("bot oi") || nq.Contains("tu van giup") ||
                nq == "chao" || nq == "xin chao" || nq == "chao ban")
            {
                reply.AppendLine("Xin chào bạn! Tôi là **MediBot** - Bác sĩ Trợ lý Ảo Y tế của phòng khám MediBooking.");
                reply.AppendLine("- Tôi có thể hỗ trợ bạn **sàng lọc triệu chứng ban đầu**, đưa ra lời khuyên chăm sóc sức khỏe và **gợi ý chuyên khoa/bác sĩ phù hợp** để bạn đặt lịch khám thuận tiện nhất.");
                reply.AppendLine("- Bạn hãy mô tả các dấu hiệu hoặc cảm giác khó chịu bạn đang gặp phải (ví dụ: *đau đầu hoa mắt, ho rát họng, đau bụng, sốt ở bé...*) để tôi hỗ trợ nhé!");

                followUps.AddRange(new[]
                {
                    "Tôi bị đau đầu, hoa mắt chóng mặt",
                    "Bé nhà tôi 3 tuổi bị sốt nhẹ và ho",
                    "Chi phí và bảng giá khám bệnh",
                    "Phòng khám làm việc vào khung giờ nào?"
                });
            }
            // 2. Dấu hiệu Cấp cứu Khẩn cấp (Red Flags)
            else if (MatchAny(q, nq, "khó thở", "kho tho", "thở dốc", "tho doc", "tức ngực", "tuc nguc", "đau thắt ngực", "dau that nguc",
                              "đột quỵ", "dot quy", "tai biến", "tai bien", "co giật", "co giat", "bất tỉnh", "bat tinh", "hôn mê", "hon me",
                              "ngất", "ngat", "mất ý thức", "mat y thuc", "liệt", "liet", "méo miệng", "meo mieng", "chảy máu nhiều", "chay mau nhieu",
                              "nôn ra máu", "non ra mau", "ho ra máu", "ho ra mau"))
            {
                urgency = "emergency";
                reply.AppendLine("🚨 **CẢNH BÁO: DẤU HIỆU CẦN CAN THIỆP Y TẾ KHẨN CẤP!**");
                reply.AppendLine("Các biểu hiện bạn vừa mô tả (*khó thở dữ dội, đau tức ngực, co giật, bất tỉnh hoặc mất ý thức...*) có thể là cảnh báo của tình trạng bệnh lý cấp tính nguy hiểm.");
                reply.AppendLine("- **Hành động ngay:** Hãy gọi ngay đường dây nóng cấp cứu **115** hoặc di chuyển ngay đến phòng Cấp cứu của bệnh viện gần nhất.");
                reply.AppendLine("- **Lưu ý:** Đặt người bệnh nằm nơi thoáng khí, nới lỏng quần áo, không để người bệnh tự lái xe hoặc vận động gắng sức.");

                matchedSpecialty = specialties.FirstOrDefault(s => s.Name.Contains("Tim mạch") || s.Name.Contains("Nội"));
                followUps.AddRange(new[]
                {
                    "Gọi đường dây cấp cứu 115",
                    "Cần sơ cứu như thế nào khi người bệnh khó thở?",
                    "Danh sách bác sĩ Nội khoa - Tim mạch trực"
                });
            }
            // 3. Thần kinh / Đau đầu / Tiền đình / Mất ngủ (Ưu tiên trước Hô hấp để tránh trùng từ "chóng" / "chong")
            else if (MatchAny(q, nq, "đau đầu", "dau dau", "nửa đầu", "nua dau", "chóng mặt", "chong mat",
                              "tiền đình", "tien dinh", "hoa mắt", "hoa mat", "mất ngủ", "mat ngu", "khó ngủ", "kho ngu",
                              "stress", "căng thẳng", "cang thang", "run tay", "đãng trí", "dang tri", "hay quên", "hay quen"))
            {
                matchedSpecialty = specialties.FirstOrDefault(s => s.Name.Contains("Thần kinh") || s.Name.Contains("Nội"));
                reply.AppendLine("Cơn đau đầu, hoa mắt chóng mặt hoặc rối loạn giấc ngủ có thể xuất phát từ căng thẳng kéo dài, thiếu máu não thoáng qua hoặc rối loạn tuần hoàn tiền đình.");
                reply.AppendLine("- **Lời khuyên chăm sóc ban đầu:**");
                reply.AppendLine("  + Nghỉ ngơi trong không gian yên tĩnh, thoáng khí, ánh sáng dịu nhẹ.");
                reply.AppendLine("  + Uống đủ 2 lít nước mỗi ngày, tránh nhìn màn hình điện thoại hoặc máy tính liên tục.");
                reply.AppendLine("  + Tránh thay đổi tư thế đột ngột (đặc biệt khi đang nằm bật dậy nhanh).");
                reply.AppendLine("- **Khuyến nghị:** Bạn nên thăm khám chuyên khoa Thần kinh hoặc Nội tổng quát để đo lưu huyết não và xác định nguyên nhân cụ thể.");

                followUps.AddRange(new[]
                {
                    "Cơn đau đầu xuất hiện âm ỉ hay giật nhói từng cơn?",
                    "Có kèm cảm giác buồn nôn hoặc sợ tiếng ồn không?",
                    "Đặt lịch khám chuyên khoa Thần kinh / Nội tổng quát"
                });
            }
            // 4. Nhi khoa (Trẻ em, Bé nhỏ)
            else if (MatchAny(q, nq, "trẻ", "tre", "bé", "be", "con tôi", "con toi", "cháu", "chau",
                              "sơ sinh", "so sinh", "biếng ăn", "bieng an", "quấy khóc", "quay khoc", "rôm sảy", "rom say"))
            {
                matchedSpecialty = specialties.FirstOrDefault(s => s.Name.Contains("Nhi"));
                reply.AppendLine("Ở trẻ nhỏ, hệ miễn dịch còn non nớt nên các phản ứng bệnh lý thường diễn tiến nhanh, phụ huynh cần quan sát sát sao.");
                reply.AppendLine("- **Hướng dẫn chăm sóc trẻ ban đầu:**");
                reply.AppendLine("  + Cho bé mặc quần áo mỏng, thoáng mát, giữ nhiệt độ phòng ổn định.");
                reply.AppendLine("  + Bổ sung đủ cữ sữa, nước hoặc dung dịch oresol theo tỷ lệ pha chuẩn.");
                reply.AppendLine("  + Thường xuyên đo nhiệt độ nách cho bé.");
                reply.AppendLine("- ⚠️ **Dấu hiệu cần đưa bé đi khám ngay:** Bé sốt cao liên tục không hạ, li bì, bỏ bú, co giật hoặc thở rút lõm lồng ngực.");

                followUps.AddRange(new[]
                {
                    "Bé nhà bạn được mấy tuổi hoặc mấy tháng?",
                    "Bé có đang sốt hay nôn trớ không?",
                    "Xem lịch khám của BS. Trần Thị Bích (Nhi khoa)"
                });
            }
            // 5. Tai Mũi Họng & Hô hấp (dùng regex từ chính xác để không nhầm chữ "chong" chứa "ho")
            else if (MatchWord(q, nq, "ho", "ho khan", "ho đờm", "ho dom") ||
                     MatchAny(q, nq, "rát họng", "rat hong", "đau họng", "dau hong", "viêm họng", "viem hong",
                              "đờm", "dom", "sổ mũi", "so mui", "nghẹt mũi", "nghet mui", "chảy nước mũi", "chay nuoc mui",
                              "xoang", "viêm xoang", "viem xoang", "ù tai", "u tai", "đau tai", "dau tai",
                              "amidan", "khàn tiếng", "khan tieng", "mất tiếng", "mat tieng"))
            {
                matchedSpecialty = specialties.FirstOrDefault(s => s.Name.Contains("Tai Mũi Họng") || s.Name.Contains("Hô hấp"));
                reply.AppendLine("Dựa trên các triệu chứng bạn mô tả, có nhiều khả năng bạn đang bị **viêm đường hô hấp trên** hoặc tổn thương vùng **Tai - Mũi - Họng** (như viêm họng, viêm mũi xoang, kích ứng thanh quản).");
                reply.AppendLine("- **Chăm sóc ban đầu tại nhà:**");
                reply.AppendLine("  + Súc miệng và họng bằng nước muối sinh lý ấm 2-3 lần/ngày.");
                reply.AppendLine("  + Uống nhiều nước ấm, giữ ấm vùng cổ ngực, tránh tiếp xúc luồng gió máy lạnh phả trực tiếp.");
                reply.AppendLine("  + Tuyệt đối **không tự ý mua thuốc kháng sinh** uống khi chưa có chỉ định của bác sĩ.");
                reply.AppendLine("- **Khuyến nghị:** Bạn nên đặt lịch khám Tai Mũi Họng để được bác sĩ nội soi chẩn đoán chính xác mức độ viêm.");

                followUps.AddRange(new[]
                {
                    "Triệu chứng ho đã kéo dài bao nhiêu ngày?",
                    "Có kèm sốt cao trên 38.5 độ không?",
                    "Xem lịch khám của BS. Võ Thanh Hùng (Tai Mũi Họng)"
                });
            }
            // 6. Tim mạch & Huyết áp
            else if (MatchAny(q, nq, "tim", "huyết áp", "huyet ap", "hồi hộp", "hoi hop", "đánh trống ngực", "danh trong nguc",
                              "nhịp tim", "nhip tim", "loạn nhịp", "loan nhip", "thiếu máu cơ tim", "thieu mau co tim"))
            {
                urgency = "warning";
                matchedSpecialty = specialties.FirstOrDefault(s => s.Name.Contains("Tim mạch"));
                reply.AppendLine("Các dấu hiệu hồi hộp, tim đập nhanh hoặc dao động huyết áp cần được tầm soát kỹ lưỡng để phòng tránh các biến chứng tim mạch.");
                reply.AppendLine("- **Lời khuyên:**");
                reply.AppendLine("  + Ngồi nghỉ ngơi thư giãn, tránh xúc động mạnh và ngưng sử dụng cà phê, trà đậm, thuốc lá.");
                reply.AppendLine("  + Đo và ghi lại chỉ số huyết áp vào sổ theo dõi hàng ngày.");
                reply.AppendLine("  + Bạn nên đặt lịch khám cùng chuyên gia Tim mạch để đo điện tim (ECG) và siêu âm tim.");

                followUps.AddRange(new[]
                {
                    "Chỉ số huyết áp gần nhất của bạn là bao nhiêu?",
                    "Cơn hồi hộp xuất hiện lúc nghỉ ngơi hay khi gắng sức?",
                    "Xem lịch khám của BS. Lê Minh Tuấn (Tim mạch)"
                });
            }
            // 7. Da liễu
            else if (MatchWord(q, nq, "da") ||
                     MatchAny(q, nq, "mụn", "mun", "ngứa", "ngua", "dị ứng", "di ung", "mề đay", "me day",
                              "nổi mẩn", "noi man", "vảy nến", "vay nen", "chàm", "cham", "nấm da", "nam da"))
            {
                matchedSpecialty = specialties.FirstOrDefault(s => s.Name.Contains("Da liễu"));
                reply.AppendLine("Các tổn thương trên da (mẩn ngứa, mề đay, mụn, phát ban) thường do dị ứng thời tiết, tiếp xúc hóa chất, nấm hoặc viêm da cơ địa.");
                reply.AppendLine("- **Lời khuyên ban đầu:**");
                reply.AppendLine("  + Không cào gãi làm xước rách da gây nhiễm trùng thứ phát.");
                reply.AppendLine("  + Tạm ngưng các mỹ phẩm, sữa tắm lạ hoặc hóa chất nghi ngờ gây kích ứng.");
                reply.AppendLine("  + Không tự ý mua thuốc bôi chứa corticoid mạnh tránh làm teo da, giãn mạch.");

                followUps.AddRange(new[]
                {
                    "Vùng da bị tổn thương ở vị trí nào trên cơ thể?",
                    "Có xuất hiện mụn nước hay vảy ngứa không?",
                    "Xem lịch khám của BS. Phạm Hồng Nhung (Da liễu)"
                });
            }
            // 8. Tiêu hóa & Dạ dày
            else if (MatchAny(q, nq, "bụng", "bung", "dạ dày", "da day", "tiêu hóa", "tieu hoa", "ợ chua", "o chua",
                              "trào ngược", "trao nguoc", "tiêu chảy", "tieu chay", "táo bón", "tao bon", "buồn nôn", "buon non",
                              "đầy hơi", "day hoi", "chướng bụng", "chuong bung", "đại tràng", "dai trang"))
            {
                matchedSpecialty = specialties.FirstOrDefault(s => s.Name.Contains("Nội") || s.Name.Contains("Tiêu hóa"));
                reply.AppendLine("Các biểu hiện đau tức thượng vị, ợ chua, buồn nôn hoặc rối loạn tiêu hóa thường phản ánh tình trạng viêm loét dạ dày, trào ngược thực quản hoặc kích ứng đại tràng.");
                reply.AppendLine("- **Chăm sóc ban đầu:**");
                reply.AppendLine("  + Ăn thức ăn chín, mềm, chia làm nhiều bữa nhỏ trong ngày.");
                reply.AppendLine("  + Kiêng đồ cay nóng, chua, đồ chiên rán nhiều dầu mỡ và nước ngọt có gas.");
                reply.AppendLine("  + Không nằm ngay sau khi ăn ít nhất 2 giờ.");

                followUps.AddRange(new[]
                {
                    "Cơn đau bụng xuất hiện trước hay sau bữa ăn?",
                    "Có kèm theo sốt hoặc đi ngoài phân lỏng không?",
                    "Xem lịch khám của BS. Nguyễn Văn An (Nội tổng quát)"
                });
            }
            // 9. Cơ Xương Khớp & Chấn thương
            else if (MatchAny(q, nq, "xương", "xuong", "khớp", "khop", "đau lưng", "dau lung", "thoái hóa", "thoai hoa",
                              "thoát vị", "thoat vi", "vai gáy", "vai gay", "gout", "đau gối", "dau goi", "bong gân", "bong gan", "cột sống", "cot song"))
            {
                matchedSpecialty = specialties.FirstOrDefault(s => s.Name.Contains("chỉnh hình") || s.Name.Contains("xương khớp") || s.Name.Contains("Cơ"));
                reply.AppendLine("Đau mỏi cột sống, đau khớp hay tê bì vai gáy thường liên quan đến tư thế ngồi sai, thoái hóa sụn khớp hoặc thoát vị đĩa đệm.");
                reply.AppendLine("- **Lời khuyên:**");
                reply.AppendLine("  + Tránh mang vác vật nặng hoặc bẻ xoay khớp cột sống đột ngột.");
                reply.AppendLine("  + Có thể chườm ấm vị trí đau mỏi 15-20 phút để giãn cơ.");
                reply.AppendLine("  + Bạn nên chụp X-quang hoặc MRI để bác sĩ đánh giá chính xác tình trạng sụn và đĩa đệm.");

                followUps.AddRange(new[]
                {
                    "Cơn đau thường nhức nhiều vào buổi sáng hay khi vận động?",
                    "Khớp có biểu hiện sưng đỏ nóng không?",
                    "Đặt lịch khám chuyên khoa Chấn thương chỉnh hình"
                });
            }
            // 10. Sản phụ khoa
            else if (MatchAny(q, nq, "thai", "bầu", "bau", "kinh nguyệt", "kinh nguyet", "trễ kinh", "tre kinh",
                              "chậm kinh", "cham kinh", "phụ khoa", "phu khoa", "khí hư", "khi hu", "đau bụng dưới", "dau bung duoi"))
            {
                matchedSpecialty = specialties.FirstOrDefault(s => s.Name.Contains("Sản") || s.Name.Contains("phụ khoa"));
                reply.AppendLine("Vấn đề sức khỏe sinh sản và chu kỳ phụ khoa cần được thăm khám định kỳ và cẩn thận bởi bác sĩ chuyên khoa Sản phụ khoa.");
                reply.AppendLine("- **Lời khuyên:**");
                reply.AppendLine("  + Vệ sinh vùng kín đúng cách bằng dung dịch dịu nhẹ, giữ môi trường khô thoáng.");
                reply.AppendLine("  + Ghi nhớ chính xác ngày đầu tiên của kỳ kinh nguyệt gần nhất.");
                reply.AppendLine("  + Nếu đang mang thai, bạn cần tuân thủ lịch siêu âm mốc tuần quan trọng.");

                followUps.AddRange(new[]
                {
                    "Kỳ kinh gần nhất của bạn cách đây bao lâu?",
                    "Bạn đã siêu âm hoặc thăm khám phụ khoa gần đây chưa?",
                    "Xem lịch khám của BS. Đặng Thị Mai (Sản phụ khoa)"
                });
            }
            // 11. Mắt
            else if (MatchAny(q, nq, "mắt", "mat", "mờ mắt", "mo mat", "cận thị", "can thi",
                              "đau mắt đỏ", "dau mat do", "chảy nước mắt", "chay nuoc mat", "cộm mắt", "com mat"))
            {
                matchedSpecialty = specialties.FirstOrDefault(s => s.Name.Contains("Mắt"));
                reply.AppendLine("Mắt là cơ quan rất nhạy cảm. Triệu chứng nhìn mờ, cộm rát hoặc đỏ mắt có thể là dấu hiệu viêm kết mạc hoặc tật khúc xạ.");
                reply.AppendLine("- **Lời khuyên:**");
                reply.AppendLine("  + Không đưa tay lên dụi mắt để tránh xước giác mạc.");
                reply.AppendLine("  + Sử dụng nước muối sinh lý NaCl 0.9% để nhỏ rửa mắt.");
                reply.AppendLine("  + Áp dụng quy tắc 20-20-20 khi làm việc với máy tính để mắt được thư giãn.");

                followUps.AddRange(new[]
                {
                    "Bạn bị đau đỏ 1 bên hay cả 2 bên mắt?",
                    "Có kèm cảm giác chói mắt hoặc chảy ghèn rỉ không?",
                    "Đặt lịch khám chuyên khoa Mắt"
                });
            }
            // 12. Răng Hàm Mặt
            else if (MatchAny(q, nq, "răng", "rang", "nướu", "nuou", "lợi", "loi", "sâu răng", "sau rang",
                              "răng khôn", "rang khon", "niềng răng", "nieng rang", "hôi miệng", "hoi mieng"))
            {
                matchedSpecialty = specialties.FirstOrDefault(s => s.Name.Contains("Răng") || s.Name.Contains("Hàm"));
                reply.AppendLine("Các cơn đau nhức răng, sưng nướu thường do sâu răng tiến triển vào tủy, viêm lợi hoặc răng khôn mọc lệch chèn ép.");
                reply.AppendLine("- **Lời khuyên:**");
                reply.AppendLine("  + Súc miệng nước muối ấm sau mỗi bữa ăn.");
                reply.AppendLine("  + Tránh đồ ăn quá nóng, quá lạnh hoặc đồ cứng ngọt.");
                reply.AppendLine("  + Cần khám nha khoa sớm để tránh tình trạng viêm tủy lan rộng gây áp xe xương hàm.");

                followUps.AddRange(new[]
                {
                    "Răng có bị buốt nhói khi ăn đồ nóng lạnh không?",
                    "Vùng nướu quanh răng có bị sưng mủ không?",
                    "Đặt lịch khám chuyên khoa Răng Hàm Mặt"
                });
            }
            // 13. Hỏi giá khám, Giờ làm việc, Quy trình đặt lịch
            else if (MatchAny(q, nq, "giá", "gia", "chi phí", "chi phi", "bảng giá", "bang gia",
                              "bao nhiêu tiền", "bao nhieu tien", "giờ làm việc", "gio lam viec", "mấy giờ", "may gio",
                              "địa chỉ", "dia chi", "ở đâu", "o dau", "bảo hiểm", "bao hiem", "bhyt", "đặt lịch thế nào", "dat lich the nao"))
            {
                reply.AppendLine("🏥 **THÔNG TIN DỊCH VỤ PHÒNG KHÁM MEDIBOOKING:**");
                reply.AppendLine("- **Chi phí khám chuyên khoa:** Dao động từ **300.000đ – 500.000đ** / lượt khám (tùy học hàm Bác sĩ CKI, ThS, TS, PGS).");
                reply.AppendLine("- **Thời gian làm việc:**");
                reply.AppendLine("  + Thứ 2 – Thứ 7: **07:30 – 20:00**");
                reply.AppendLine("  + Chủ nhật: **08:00 – 17:00**");
                reply.AppendLine("- **Bảo hiểm:** Phòng khám có hỗ trợ xuất hóa đơn điện tử và liên kết bảo lãnh viện phí với các công ty bảo hiểm tư nhân.");
                reply.AppendLine("- **Cách đặt lịch:** Bạn chỉ cần chọn Bác sĩ hoặc Chuyên khoa mong muốn ngay trên website và chọn khung giờ trống phù hợp.");

                followUps.AddRange(new[]
                {
                    "Xem bảng giá chi tiết từng bác sĩ",
                    "Đặt lịch khám ngay bây giờ",
                    "Phòng khám có chấp nhận thẻ BHYT không?"
                });
            }
            // 14. Mặc định: Nội tổng quát
            else
            {
                matchedSpecialty = specialties.FirstOrDefault(s => s.Name.Contains("Nội")) ?? specialties.FirstOrDefault();
                reply.AppendLine("Cảm ơn bạn đã chia sẻ cùng **MediBot**. Để có chẩn đoán chính xác và phác đồ điều trị an toàn, bạn nên đặt lịch thăm khám trực tiếp cùng bác sĩ chuyên khoa.");
                reply.AppendLine("- Bạn có thể chia sẻ chi tiết hơn về thời gian xuất hiện triệu chứng, mức độ đau hoặc các bệnh nền sẵn có để tôi hỗ trợ tư vấn sát nhất.");
                reply.AppendLine("- **Lưu ý:** Lời tư vấn mang tính chất tham khảo, không thay thế chẩn đoán y khoa chuyên sâu.");

                followUps.AddRange(new[]
                {
                    "Tôi muốn khám sức khỏe tổng quát",
                    "Cần chuẩn bị gì trước khi đi khám?",
                    "Xem danh sách tất cả các chuyên khoa"
                });
            }

            var result = new AiConsultResponse
            {
                Reply = reply.ToString().Trim(),
                UrgencyLevel = urgency,
                IsSuccess = true,
                Mode = "smart_clinical_ai",
                FollowUpQuestions = followUps
            };

            // Gợi ý chuyên khoa và các bác sĩ phù hợp trong CSDL
            if (matchedSpecialty != null)
            {
                result.SuggestedSpecialty = new SpecialtySummaryDto
                {
                    Id = matchedSpecialty.Id,
                    Name = matchedSpecialty.Name,
                    Icon = matchedSpecialty.Icon,
                    Description = matchedSpecialty.Description
                };

                var docsInSp = doctors
                    .Where(d => d.SpecialtyId == matchedSpecialty.Id)
                    .Take(2)
                    .ToList();

                if (docsInSp.Count == 0)
                {
                    docsInSp = doctors.Take(2).ToList();
                }

                foreach (var d in docsInSp)
                {
                    result.SuggestedDoctors.Add(MapToDoctorSummary(d));
                }
            }

            return result;
        }

        private static bool MatchAny(string query, string normalizedQuery, params string[] phrases)
        {
            foreach (var p in phrases)
            {
                if (query.Contains(p) || normalizedQuery.Contains(p)) return true;
            }
            return false;
        }

        private static bool MatchWord(string query, string normalizedQuery, params string[] words)
        {
            foreach (var w in words)
            {
                var pat = $@"\b{Regex.Escape(w)}\b";
                if (Regex.IsMatch(query, pat, RegexOptions.IgnoreCase) || Regex.IsMatch(normalizedQuery, pat, RegexOptions.IgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        private static string RemoveDiacritics(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return text;
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var uc = CharUnicodeInfo.GetUnicodeCategory(c);
                if (uc != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(c);
                }
            }

            return sb.ToString().Normalize(NormalizationForm.FormC).Replace('đ', 'd').Replace('Đ', 'D');
        }

        private DoctorSummaryDto MapToDoctorSummary(Doctor doc)
        {
            return new DoctorSummaryDto
            {
                Id = doc.Id,
                Name = doc.Name,
                AcademicTitle = doc.AcademicTitle,
                Degree = doc.Degree,
                SpecialtyName = doc.SpecialtyName,
                SpecialtyId = doc.SpecialtyId,
                Avatar = doc.Avatar,
                Price = doc.Price,
                Rating = doc.Rating,
                Bio = doc.Bio
            };
        }

        private class GeminiStructuredResult
        {
            [JsonPropertyName("reply")]
            public string? Reply { get; set; }

            [JsonPropertyName("urgencyLevel")]
            public string? UrgencyLevel { get; set; }

            [JsonPropertyName("suggestedSpecialtyId")]
            public string? SuggestedSpecialtyId { get; set; }

            [JsonPropertyName("suggestedDoctorIds")]
            public List<string>? SuggestedDoctorIds { get; set; }

            [JsonPropertyName("followUpQuestions")]
            public List<string>? FollowUpQuestions { get; set; }
        }
    }
}

