using TalentBridge.API.DTOs.CommunityDTOs;

public class UniversityDetailDto : UniversityDto
{
    public string ContactPersonName { get; set; } = string.Empty;
    public string ContactPersonEmail { get; set; } = string.Empty;
    public string ContactPersonPhone { get; set; } = string.Empty;
    public List<UniversityCareerDto> Careers { get; set; } = new();
    public List<CommunityDto> Communities { get; set; } = new();
}