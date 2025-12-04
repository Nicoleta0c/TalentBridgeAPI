public class UniversityCareerDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string AreaOfStudy { get; set; } = string.Empty;
    public int DurationYears { get; set; }
    public string DegreeType { get; set; } = string.Empty;
    public int TotalStudents { get; set; }
    public bool IsActive { get; set; }
}