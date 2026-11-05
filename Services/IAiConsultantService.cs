using WebDatLichKham.Models;

namespace WebDatLichKham.Services
{
    public interface IAiConsultantService
    {
        Task<AiConsultResponse> ConsultAsync(AiConsultRequest request);
        AiConfigDto GetConfigInfo();
    }
}

