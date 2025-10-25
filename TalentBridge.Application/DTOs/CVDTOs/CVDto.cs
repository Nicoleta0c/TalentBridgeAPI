using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalentBridge.Application.DTOs
{
    public class CVDto
    {
        public int           Id                  { get; set; }
        public int           UserId              { get; set; }
        public string        FileName            { get; set; } = string.Empty;
        public string        FileType            { get; set; } = string.Empty;
        public long          FileSize            { get; set; }
        public string        AnalysisResult      { get; set; } = string.Empty;
        public decimal       Score               { get; set; }
        public DateTime      UploadDate          { get; set; }
        public string        UserName            { get; set; } = string.Empty;
    }
}
