using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalentBridge.Application.DTOs.UserDTOs 
{
    public class UpdateUserDto 
    {
        public string     FullName      { get; set; } = string.Empty;
        public string     Email         { get; set; } = string.Empty;
        public bool       IsActive      { get; set; }
    }
}
