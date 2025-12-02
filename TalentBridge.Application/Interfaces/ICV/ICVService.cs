using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalentBridge.Application.DTOs;
using TalentBridge.Application.DTOs.CVDTOs;

namespace TalentBridge.Application.Interfaces
{
    public interface ICVService
    {
        Task<CVDto?> GetCVByIdAsync(int id);
        Task<IEnumerable<CVDto>> GetUserCVsAsync(int userId);
        Task<CVDto> UploadCVAsync(UploadCVDto uploadCVDto);
        Task<bool> UpdateCVAsync(int id, UpdateCVDto updateCVDto);
        Task<bool> DeleteCVAsync(int id);
        Task<CVAnalysisResultDto> AnalyzeCVAsync(AnalyzeCVRequestDto analyzeRequest);
        Task<IEnumerable<CVDto>> GetAllCVsAsync();
        Task<IEnumerable<CVDto>> GetCVsByUserIdAsync(int userId);
        Task<CVDto?> GetActiveCVByUserIdAsync(int userId);
    }
}