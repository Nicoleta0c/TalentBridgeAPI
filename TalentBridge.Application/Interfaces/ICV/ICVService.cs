using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalentBridge.Application.DTOs;

namespace TalentBridge.Application.Interfaces
{
    public interface ICVService
    {
        Task<CVDto?> GetCVByIdAsync(int id);
        Task<IEnumerable<CVDto>> GetUserCVsAsync(int userId);
        Task<CVDto> UploadCVAsync(UploadCVDto uploadCVDto);
        Task<CVDto?> UpdateCVAsync(int id, UploadCVDto updateCVDto);
        Task<bool> DeleteCVAsync(int id);
        Task<CVAnalysisResultDto> AnalyzeCVAsync(AnalyzeCVRequestDto analyzeRequest);
    }
}