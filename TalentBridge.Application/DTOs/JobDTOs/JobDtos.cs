using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalentBridge.Application.DTOs.JobDTOs
{
    public class JobDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Company { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Requirements { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string SalaryRange { get; set; } = string.Empty;
        public string JobType { get; set; } = string.Empty;
        public string Benefits { get; set; } = string.Empty;
        public string ContactEmail { get; set; } = string.Empty;
        public string ContactPhone { get; set; } = string.Empty;
        public string ExperienceLevel { get; set; } = string.Empty;
        public string EducationLevel { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime PostedDate { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int ApplicationCount { get; set; }
    }

    public class JobSummaryDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Company { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string SalaryRange { get; set; } = string.Empty;
        public string JobType { get; set; } = string.Empty;
        public DateTime PostedDate { get; set; }
        public bool IsActive { get; set; }
    }
}