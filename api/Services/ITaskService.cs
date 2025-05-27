using api.Dtos.Task;
using api.Helpers;

namespace api.Services
{
    public interface ITaskService
    {
        Task<List<TaskDto>> GetAllTasksAsync(QueryObject query, string userId);
        Task<TaskDto?> GetTaskByIdAsync(int id, string userId);
        Task<TaskDto?> CreateTaskAsync(CreateTaskRequestDto taskDto, string userId);
        Task<TaskDto?> UpdateTaskAsync(int id, UpdateTaskRequestDto updateDto, string userId);
        Task<bool> DeleteTaskAsync(int id, string userId);
        Task<bool> TaskExists(int id);
    }
}
