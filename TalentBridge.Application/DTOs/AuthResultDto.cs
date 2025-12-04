using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalentBridge.Application.DTOs.UserDTOs;

namespace TalentBridge.Application.DTOs
{
    public class AuthResultDto
    {
        public bool        Success     { get; set; }
        public string      Message     { get; set; } = string.Empty;
        public string?     Token       { get; set; }
        public string?     RefreshToken { get; set; }
        public DateTime?   TokenExpiration { get; set; }
        public UserDto?    User        { get; set; }
    }
}
