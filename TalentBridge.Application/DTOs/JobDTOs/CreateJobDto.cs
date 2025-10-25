using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalentBridge.Application.DTOs
{
    public class CreateJobDto
    {
        public string      Title             { get; set; } = string.Empty;
        public string      Company           { get; set; } = string.Empty;
        public string      Description       { get; set; } = string.Empty;
        public string      Requirements      { get; set; } = string.Empty;
        public string      Location          { get; set; } = string.Empty;
        public string      SalaryRange       { get; set; } = string.Empty;
        public string      JobType           { get; set; } = string.Empty;
    }
}
