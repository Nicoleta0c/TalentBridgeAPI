using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalentBridge.Application.DTOs
{
    public class AuthResultDto
    {
        public bool        Success     { get; set; }
        public string?     Token       { get; set; }
        public string?     Message     { get; set; }
    }
}
