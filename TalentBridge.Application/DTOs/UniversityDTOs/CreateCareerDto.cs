public class CreateCareerDto
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string AreaOfStudy { get; set; } = string.Empty;
    public int DurationYears { get; set; }
    public string DegreeType { get; set; } = string.Empty;
    public int UniversityId { get; set; }
}