using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalentBridge.Domain.Entities
{
    public class JobApplication
    {
        public int           Id               { get; set; }
        public int           JobId            { get; set; }
        public int           UserId           { get; set; }
        public int           CVId             { get; set; }
        public string        Status           { get; set; } = "Applied"; 
        public DateTime      AppliedDate      { get; set; } = DateTime.UtcNow;
        public string?       CoverLetter      { get; set; }
                                              
        public Job           Job              { get; set; } = default!;
        public User          User             { get; set; } = default!;
    }
}
