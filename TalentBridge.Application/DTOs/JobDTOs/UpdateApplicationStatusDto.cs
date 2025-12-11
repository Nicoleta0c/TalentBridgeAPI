using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalentBridge.Application.DTOs.JobDTOs
{
    public class UpdateApplicationStatusDto
    {
        public string Status { get; set; } = string.Empty;
        public string Feedback { get; set; } = string.Empty;
    }
}