using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalentBridge.Application.DTOs.CVDTOs
{
    public class UploadCVDto
    {
        public int         UserId             { get; set; }
        public string      FileName           { get; set; } = string.Empty;
        public string      FileType           { get; set; } = string.Empty;
        public long        FileSize           { get; set; }
        public byte[]      FileContent        { get; set; } = Array.Empty<byte>();
        public string      Base64Content      { get; set; } = string.Empty;
        public string      Skills             { get; set; } = string.Empty;
        public string      Experience         { get; set; } = string.Empty;
        public string      Education          { get; set; } = string.Empty;
        public string      Certifications     { get; set; } = string.Empty;
        public string      Languages          { get; set; } = string.Empty;
        public bool        IsDefault          { get; set; } = false;
        public string      Summary            { get; set; } = string.Empty;
    }
}