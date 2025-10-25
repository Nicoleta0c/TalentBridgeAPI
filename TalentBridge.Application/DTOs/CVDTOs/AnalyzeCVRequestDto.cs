using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalentBridge.Application.DTOs
{
    public class AnalyzeCVRequestDto
    {
        public int          CVId                { get; set; }
        public string?      JobDescription      { get; set; } 
    }
}