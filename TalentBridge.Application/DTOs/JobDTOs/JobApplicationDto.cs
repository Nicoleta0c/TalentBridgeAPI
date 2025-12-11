public class JobApplicationDto
{
    public int Id { get; set; }
    public int JobId { get; set; }
    public int UserId { get; set; }
    public int CVId { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime AppliedDate { get; set; }
    public string CoverLetter { get; set; } = string.Empty;
    public string JobTitle { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public string UserFullName { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;
    public string CVFileName { get; set; } = string.Empty;
}