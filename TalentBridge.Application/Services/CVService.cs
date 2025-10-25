using TalentBridge.Application.DTOs;
using TalentBridge.Application.Interfaces;
using TalentBridge.Application.Interfaces.IUser;
using TalentBridge.Domain.Entities;

namespace TalentBridge.Application.Services
{
    public class CVService : ICVService
    {
        private readonly ICVRepository _cvRepository;
        private readonly IUserRepository _userRepository;

        public CVService(ICVRepository cvRepository, IUserRepository userRepository)
        {
            _cvRepository = cvRepository;
            _userRepository = userRepository;
        }

        public async Task<CVDto?> GetCVByIdAsync(int id)
        {
            var cv = await _cvRepository.GetByIdAsync(id);
            return cv != null ? MapToCVDto(cv) : null;
        }

        public async Task<IEnumerable<CVDto>> GetUserCVsAsync(int userId)
        {
            var cvs = await _cvRepository.GetByUserIdAsync(userId);
            return cvs.Select(MapToCVDto);
        }

        public async Task<CVDto> UploadCVAsync(UploadCVDto uploadCVDto)
        {
            var user = await _userRepository.GetByIdAsync(uploadCVDto.UserId);
            if (user == null)
                throw new InvalidOperationException("Usuario no encontrado");

            var cv = new CV
            {
                UserId = uploadCVDto.UserId,
                FileName = uploadCVDto.FileName,
                FileType = uploadCVDto.FileType,
                FileSize = uploadCVDto.FileSize,
                FilePath = $"/cvs/{uploadCVDto.UserId}/{Guid.NewGuid()}_{uploadCVDto.FileName}", // Ruta simulada
                AnalysisResult = string.Empty,
                Score = 0,
                IsActive = true,
                UploadDate = DateTime.UtcNow
            };

            await _cvRepository.AddAsync(cv);
            return MapToCVDto(cv);
        }

        public async Task<CVDto?> UpdateCVAsync(int id, UploadCVDto updateCVDto)
        {
            var cv = await _cvRepository.GetByIdAsync(id);
            if (cv == null) return null;

            cv.FileName = updateCVDto.FileName;
            cv.FileType = updateCVDto.FileType;
            cv.FileSize = updateCVDto.FileSize;
            cv.FilePath = $"/cvs/{cv.UserId}/{Guid.NewGuid()}_{updateCVDto.FileName}";

            await _cvRepository.UpdateAsync(cv);
            return MapToCVDto(cv);
        }

        public async Task<bool> DeleteCVAsync(int id)
        {
            var cv = await _cvRepository.GetByIdAsync(id);
            if (cv == null) return false;

            await _cvRepository.DeleteAsync(cv);
            return true;
        }

        public async Task<CVAnalysisResultDto> AnalyzeCVAsync(AnalyzeCVRequestDto analyzeRequest)
        {
            var cv = await _cvRepository.GetByIdAsync(analyzeRequest.CVId);
            if (cv == null)
                throw new InvalidOperationException("CV no encontrado");

            // MOCK de análisis con IA (por ahora simulado)
            var analysisResult = await MockAIAnalysis(cv, analyzeRequest.JobDescription);

            // Actualizar el CV con el resultado del análisis
            cv.AnalysisResult = System.Text.Json.JsonSerializer.Serialize(analysisResult);
            cv.Score = analysisResult.Score;
            await _cvRepository.UpdateAsync(cv);

            return analysisResult;
        }

        private async Task<CVAnalysisResultDto> MockAIAnalysis(CV cv, string? jobDescription)
        {
            // Simular procesamiento de IA (Aquí integrarías con OpenAI API)
            await Task.Delay(1000); 

            var random = new Random();
            var score = random.Next(60, 95);

            return new CVAnalysisResultDto
            {
                Success = true,
                Score = score,
                Feedback = "Tu CV tiene una buena estructura general, pero hay áreas que puedes mejorar para destacar más ante los reclutadores.",
                Strengths = new List<string>
                {
                    "Formato limpio y profesional",
                    "Experiencia laboral bien detallada",
                    "Habilidades técnicas relevantes"
                },
                Improvements = new List<string>
                {
                    "Agrega más palabras clave relacionadas con el puesto",
                    "Incluye métricas y resultados cuantificables",
                    "Mejora la sección de resumen profesional"
                },
                KeywordSuggestions = new List<string>
                {
                    ".NET Core", "Entity Framework", "SQL Server", "React", "TypeScript",
                    "APIs REST", "Azure", "Scrum", "Git", "CI/CD"
                }
            };
        }

        private static CVDto MapToCVDto(CV cv)
        {
            return new CVDto
            {
                Id = cv.Id,
                UserId = cv.UserId,
                FileName = cv.FileName,
                FileType = cv.FileType,
                FileSize = cv.FileSize,
                AnalysisResult = cv.AnalysisResult,
                Score = cv.Score,
                UploadDate = cv.UploadDate,
                UserName = cv.User?.FullName ?? string.Empty
            };
        }
    }
}