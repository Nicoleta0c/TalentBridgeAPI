using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalentBridge.Domain.Entities
{
    public class CV
    {
        public int            Id                  { get; set; }
        public int            UserId              { get; set; }
        public string         FileName            { get; set; } = string.Empty;
        public string         FilePath            { get; set; } = string.Empty;
        public string         FileType            { get; set; } = string.Empty; 
        public long           FileSize            { get; set; }
        public string         AnalysisResult      { get; set; } = string.Empty; 
        public decimal        Score               { get; set; } 
        public bool           IsActive            { get; set; } = true;
        public DateTime       UploadDate          { get; set; } = DateTime.UtcNow;
        public DateTime?      UpdatedAt           { get; set; }
                                                  
        public User           User                { get; set; } = default!;
    }
}
