using api.Data;
using api.Dtos.Task;
using api.Helpers;
using api.Interfaces;
using api.Mappers;

namespace api.Services
{
    public class TaskService :ITaskService
    {
        private readonly ITaskRepository _taskRepository;
        private readonly ApplicationDBContext _context; // Hala kullanılıyor, ileride refactor edebiliriz

        /// <summary>
        /// TaskService sınıfının yeni bir örneğini başlatır.
        /// </summary>
        /// <param name="taskRepository">Görev veritabanı işlemleri için kullanılan ITaskRepository servisi.</param>
        /// <param name="context">Veritabanı bağlamı. (Not: Eğer TaskRepository tüm erişimi kapsıyorsa bu kaldırılabilir.)</param>
        public TaskService(ITaskRepository taskRepository, ApplicationDBContext context)
        {
            _taskRepository = taskRepository;
            _context = context; // Eğer TaskRepository her şeyi kapsıyorsa bu kaldırılabilir
        }

        /// <summary>
        /// Belirtilen sorgu parametrelerine göre, belirli bir kullanıcıya ait tüm görevleri asenkron olarak getirir.
        /// </summary>
        /// <param name="query">Görevleri filtrelemek, sıralamak ve sayfalamak için kullanılan sorgu nesnesi.</param>
        /// <param name="userId">Görevi olan kullanıcının ID'si.</param>
        /// <returns>Belirtilen kriterlere uyan görevlerin DTO listesini döner.</returns>
        public async Task<List<TaskDto>> GetAllTasksAsync(QueryObject query, string userId)
        {
            var tasks = await _taskRepository.GetAllAsync(query);

            return tasks.Select(t => t.ToTaskDto()).ToList();
        }

        /// <summary>
        /// Belirtilen ID'ye sahip görevi, belirli bir kullanıcıya ait olması koşuluyla asenkron olarak getirir.
        /// </summary>
        /// <param name="id">Getirilecek görevin benzersiz ID'si.</param>
        /// <param name="userId">Görevi olan kullanıcının ID'si.</param>
        /// <returns>Görev bulunursa görev DTO'su, aksi takdirde null döner.</returns>
        public async Task<TaskDto?> GetTaskByIdAsync(int id, string userId)
        {
            var task = await _taskRepository.GetByIdAsync(id);

            if (task == null || task.UserId != userId)
            {
                return null; // Görev bulunamadı veya kullanıcıya ait değil
            }

            return task.ToTaskDto();
        }

        /// <summary>
        /// Yeni bir görev oluşturur ve belirtilen kullanıcıya atar.
        /// </summary>
        /// <param name="taskDto">Oluşturulacak görevin detaylarını içeren DTO.</param>
        /// <param name="userId">Görevi oluşturacak kullanıcının ID'si.</param>
        /// <returns>Oluşturulan görevin DTO'sunu döner.</returns>
        public async Task<TaskDto?> CreateTaskAsync(CreateTaskRequestDto taskDto, string userId)
        {
            var taskModel = taskDto.ToTaskFromCreateDto();
            taskModel.UserId = userId; // Görevi oluşturan kullanıcıya ata

            var createdTask = await _taskRepository.CreateAsync(taskModel);
            return createdTask.ToTaskDto();
        }

        /// <summary>
        /// Belirtilen ID'ye sahip görevi, belirli bir kullanıcıya ait olması koşuluyla günceller.
        /// </summary>
        /// <param name="id">Güncellenecek görevin benzersiz ID'si.</param>
        /// <param name="updateDto">Görevin güncellenmiş detaylarını içeren DTO.</param>
        /// <param name="userId">Görevi güncelleyecek kullanıcının ID'si.</param>
        /// <returns>Başarılı olursa güncellenmiş görevin DTO'sunu, görev bulunamazsa veya erişim izni yoksa null döner.</returns>
        public async Task<TaskDto?> UpdateTaskAsync(int id, UpdateTaskRequestDto updateDto, string userId)
        {
            var existingTask = await _taskRepository.GetByIdAsync(id);

            if (existingTask == null || existingTask.UserId != userId)
            {
                return null; // Görev bulunamadı veya kullanıcıya ait değil
            }

            // DTO'dan gelen verilerle modeli güncelle
            existingTask.Title = updateDto.Title;
            existingTask.Description = updateDto.Description;
            existingTask.IsCompleted = updateDto.IsCompleted;
            existingTask.DueDate = updateDto.DueDate;
            // UserId burada güncellenmez, görev başka bir kullanıcıya devredilemez.

            var updatedTask = await _taskRepository.UpdateAsync(id, existingTask);
            return updatedTask?.ToTaskDto();
        }

        /// <summary>
        /// Belirtilen ID'ye sahip görevi, belirli bir kullanıcıya ait olması koşuluyla asenkron olarak siler.
        /// </summary>
        /// <param name="id">Silinecek görevin benzersiz ID'si.</param>
        /// <param name="userId">Görevi silecek kullanıcının ID'si.</param>
        /// <returns>Silme işlemi başarılıysa true, görev bulunamazsa veya erişim izni yoksa false döner.</returns>
        public async Task<bool> DeleteTaskAsync(int id, string userId)
        {
            var taskToDelete = await _taskRepository.GetByIdAsync(id);

            if (taskToDelete == null || taskToDelete.UserId != userId)
            {
                return false; // Görev bulunamadı veya kullanıcıya ait değil
            }

            var deletedTask = await _taskRepository.DeleteAsync(id);
            return deletedTask != null;
        }

        /// <summary>
        /// Belirtilen ID'ye sahip bir görevin veritabanında mevcut olup olmadığını kontrol eder.
        /// </summary>
        /// <param name="id">Kontrol edilecek görevin benzersiz ID'si.</param>
        /// <returns>Görev mevcutsa true, aksi takdirde false döner.</returns>
        public async Task<bool> TaskExists(int id)
        {
            return await _taskRepository.TaskExists(id);
        }
    }
}
