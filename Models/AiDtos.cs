namespace WebDatLichKham.Models
{
    public class AiConsultRequest
    {
        public string Message { get; set; } = string.Empty;
        public List<AiChatMessage>? History { get; set; } = new();
    }

    public class AiChatMessage
    {
        public string Role { get; set; } = "user"; // "user" hoặc "model"
        public string Content { get; set; } = string.Empty;
    }

    public class SpecialtySummaryDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    public class DoctorSummaryDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string AcademicTitle { get; set; } = string.Empty;
        public string Degree { get; set; } = string.Empty;
        public string SpecialtyName { get; set; } = string.Empty;
        public string SpecialtyId { get; set; } = string.Empty;
        public string Avatar { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public double Rating { get; set; }
        public string Bio { get; set; } = string.Empty;
    }

    public class AiConsultResponse
    {
        public string Reply { get; set; } = string.Empty;
        public string UrgencyLevel { get; set; } = "normal"; // "normal" | "warning" | "emergency"
        public SpecialtySummaryDto? SuggestedSpecialty { get; set; }
        public List<DoctorSummaryDto> SuggestedDoctors { get; set; } = new();
        public List<string> FollowUpQuestions { get; set; } = new();
        public bool IsSuccess { get; set; } = true;
        public string? ErrorMessage { get; set; }
        public string Mode { get; set; } = "gemini"; // "gemini" | "fallback"
    }

    public class AiConfigDto
    {
        public bool IsConfigured { get; set; }
        public string Model { get; set; } = "gemini-1.5-flash";
        public string Provider { get; set; } = "Google Gemini";
    }
}

