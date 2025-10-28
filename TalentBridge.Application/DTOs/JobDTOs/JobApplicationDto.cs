using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalentBridge.Application.DTOs
{
    public class JobApplicationDto
    {
        public int           Id               { get; set; }
        public int           JobId            { get; set; }
        public int           UserId           { get; set; }
        public int           CVId             { get; set; }
        public string        Status           { get; set; } = string.Empty;
        public DateTime      AppliedDate      { get; set; }
        public string?       CoverLetter      { get; set; }
        public string        JobTitle         { get; set; } = string.Empty;
        public string        Company          { get; set; } = string.Empty;
        public string        UserName         { get; set; } = string.Empty;
    }
}