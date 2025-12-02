using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalentBridge.Application.DTOs.CVDTOs
{
    public class UpdateCVDto
    {
        public int          Id                  { get; set; }
        public string       FileName            { get; set; } = string.Empty;
        public string       AnalysisResult      { get; set; } = string.Empty;
        public decimal      Score               { get; set; }
        public bool         IsActive            { get; set; } = true;
        public string       Skills              { get; set; } = string.Empty;
        public string       Experience          { get; set; } = string.Empty;
        public string       Education           { get; set; } = string.Empty;
        public string       Certifications      { get; set; } = string.Empty;
    }
}