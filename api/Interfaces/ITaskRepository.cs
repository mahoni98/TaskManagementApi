using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;
using api.Helpers;

namespace api.Interfaces
{
    public interface ITaskRepository
    {
        Task<List<UserTask>> GetAllAsync(QueryObject query); // Kullanıcıya özel filtreleme ve sorgulama için QueryObject eklendi
        Task<UserTask?> GetByIdAsync(int id);
        Task<UserTask> CreateAsync(UserTask taskModel);
        Task<UserTask?> UpdateAsync(int id, UserTask taskModel);
        Task<UserTask?> DeleteAsync(int id);
        Task<bool> TaskExists(int id);
    }
}