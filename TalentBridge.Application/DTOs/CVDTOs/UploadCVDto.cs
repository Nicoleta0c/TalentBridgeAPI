using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalentBridge.Application.DTOs
{
    public class UploadCVDto
    {
        public int         UserId             { get; set; }
        public string      FileName           { get; set; } = string.Empty;
        public string      FileType           { get; set; } = string.Empty;
        public long        FileSize           { get; set; }
        public string      Base64Content      { get; set; } = string.Empty; 
    }
}