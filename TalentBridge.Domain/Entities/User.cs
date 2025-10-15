using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalentBridge.Domain.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; } = default!;
        public string PasswordHash { get; set; } = default!;
        public string FullName { get; set; } = default!;
        public bool IsActive { get; set; } = true;
    }
}
