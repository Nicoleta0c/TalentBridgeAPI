using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;

namespace TalentBridge.Domain.Entities
{
    public class Job
    {
        public int            Id                { get; set; }
        public string         Title             { get; set; } = string.Empty;
        public string         Company           { get; set; } = string.Empty;
        public string         Description       { get; set; } = string.Empty;
        public string         Requirements      { get; set; } = string.Empty;
        public string         Location          { get; set; } = string.Empty;
        public string         SalaryRange       { get; set; } = string.Empty;
        public string         JobType           { get; set; } = string.Empty; 
        public bool           IsActive          { get; set; } = true;
        public DateTime       PostedDate        { get; set; } = DateTime.UtcNow;
        public DateTime?      UpdatedAt         { get; set; }
    }
}
