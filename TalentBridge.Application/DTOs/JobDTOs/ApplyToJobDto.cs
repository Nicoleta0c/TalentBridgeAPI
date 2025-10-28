using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalentBridge.Application.DTOs
{
    public class ApplyToJobDto
    {
        public int          JobId            { get; set; }
        public int          CVId             { get; set; }
        public string?      CoverLetter      { get; set; }
    }
}
