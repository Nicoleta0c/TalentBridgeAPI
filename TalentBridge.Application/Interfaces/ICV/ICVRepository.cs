using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalentBridge.Domain.Entities;

namespace TalentBridge.Application.Interfaces
{
    public interface ICVRepository
    {
        Task<CV?> GetByIdAsync(int id);
        Task<IEnumerable<CV>> GetByUserIdAsync(int userId);
        Task<CV?> GetActiveCVByUserIdAsync(int userId);
        Task<IEnumerable<CV>> GetAllAsync(); 
        Task AddAsync(CV cv);
        Task UpdateAsync(CV cv);
        Task DeleteAsync(CV cv);
    }
}