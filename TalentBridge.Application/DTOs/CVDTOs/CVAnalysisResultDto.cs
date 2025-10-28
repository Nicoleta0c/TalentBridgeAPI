using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalentBridge.Application.DTOs
{
    public class CVAnalysisResultDto
    {
        public bool              Success                 { get; set; }
        public decimal           Score                   { get; set; }
        public string            Feedback                { get; set; } = string.Empty;
        public List<string>      Strengths               { get; set; } = new();
        public List<string>      Improvements            { get; set; } = new();
        public List<string>      KeywordSuggestions      { get; set; } = new();
    }
}