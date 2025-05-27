using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Interfaces;
using api.Models;
using Microsoft.EntityFrameworkCore;
using api.Helpers;

namespace api.Repository
{
    public class TaskRepository : ITaskRepository
    {
        private readonly ApplicationDBContext _context;

        /// <summary>
        /// TaskRepository sınıfının yeni bir örneğini başlatır.
        /// </summary>
        /// <param name="context">Veritabanı işlemleri için kullanılan ApplicationDBContext nesnesi.</param>
        public TaskRepository(ApplicationDBContext context)
        {
            _context = context;
        }
        /// <summary>
        /// Tüm görevleri belirtilen sorgu parametrelerine göre asenkron olarak getirir.
        /// Kullanıcıya özel görev filtrelemesi (UserId), başlığa göre arama, sıralama ve sayfalama destekler.
        /// </summary>
        /// <param name="query">Görevleri filtrelemek, sıralamak ve sayfalamak için kullanılan sorgu nesnesi.</param>
        /// <returns>Belirtilen kriterlere uyan görevlerin bir listesini döner.</returns>
        public async Task<List<UserTask>> GetAllAsync(QueryObject query)
        {
            var tasks = _context.Tasks.AsQueryable();

            // Kullanıcıya özel görev filtrelemesi (Authorization için)
            if (!string.IsNullOrWhiteSpace(query.UserId))
            {
                tasks = tasks.Where(t => t.UserId == query.UserId);
            }

            // Arama/Filtreleme örnekleri (ileride eklenebilir)
            if (!string.IsNullOrWhiteSpace(query.Title))
            {
                tasks = tasks.Where(t => t.Title.Contains(query.Title));
            }

            // Sıralama örnekleri (ileride eklenebilir)
            if (!string.IsNullOrWhiteSpace(query.SortBy))
            {
                if (query.SortBy.Equals("Title", StringComparison.CurrentCultureIgnoreCase))
                {
                    tasks = query.IsDecsending ? tasks.OrderByDescending(t => t.Title) : tasks.OrderBy(t => t.Title);
                }
                // Diğer alanlara göre sıralama eklenebilir
            }

            // Sayfalama (Pagination)
            var skipNumber = (query.PageNumber - 1) * query.PageSize;

            return await tasks.Skip(skipNumber).Take(query.PageSize).ToListAsync();
        }

        /// <summary>
        /// Belirtilen ID'ye sahip görevi asenkron olarak getirir.
        /// </summary>
        /// <param name="id">Getirilecek görevin benzersiz ID'si.</param>
        /// <returns>Bulunursa görevi, aksi takdirde null döner.</returns>
        public async Task<UserTask?> GetByIdAsync(int id)
        {
            return await _context.Tasks.FirstOrDefaultAsync(t => t.Id == id);
        }

        /// <summary>
        /// Yeni bir görev oluşturur ve veritabanına kaydeder.
        /// </summary>
        /// <param name="taskModel">Oluşturulacak görevin verilerini içeren görev modeli.</param>
        /// <returns>Oluşturulan görev modelini döner.</returns>
        public async Task<UserTask> CreateAsync(UserTask taskModel)
        {
            await _context.Tasks.AddAsync(taskModel);
            await _context.SaveChangesAsync();
            return taskModel;
        }

        /// <summary>
        /// Belirtilen ID'ye sahip görevi, sağlanan görev modeli ile günceller.
        /// </summary>
        /// <param name="id">Güncellenecek görevin benzersiz ID'si.</param>
        /// <param name="taskModel">Görevin güncellenmiş verilerini içeren görev modeli.</param>
        /// <returns>Başarılı olursa güncellenmiş görevi, görev bulunamazsa null döner.</returns>
        public async Task<UserTask?> UpdateAsync(int id, UserTask taskModel)
        {
            var existingTask = await _context.Tasks.FirstOrDefaultAsync(x => x.Id == id);
            if (existingTask == null)
            {
                return null;
            }

            existingTask.Title = taskModel.Title;
            existingTask.Description = taskModel.Description;
            existingTask.IsCompleted = taskModel.IsCompleted;
            existingTask.DueDate = taskModel.DueDate;
            // UserId güncellenmez, görev başka bir kullanıcıya devredilemez.

            await _context.SaveChangesAsync();
            return existingTask;
        }

        /// <summary>
        /// Belirtilen ID'ye sahip görevi veritabanından siler.
        /// </summary>
        /// <param name="id">Silinecek görevin benzersiz ID'si.</param>
        /// <returns>Başarılı olursa silinen görevi, görev bulunamazsa null döner.</returns>
        public async Task<UserTask?> DeleteAsync(int id)
        {
            var taskModel = await _context.Tasks.FirstOrDefaultAsync(x => x.Id == id);
            if (taskModel == null)
            {
                return null;
            }

            _context.Tasks.Remove(taskModel);
            await _context.SaveChangesAsync();
            return taskModel;
        }

        /// <summary>
        /// Belirtilen ID'ye sahip bir görevin veritabanında mevcut olup olmadığını kontrol eder.
        /// </summary>
        /// <param name="id">Kontrol edilecek görevin benzersiz ID'si.</param>
        /// <returns>Görev mevcutsa true, aksi takdirde false döner.</returns>
        public async Task<bool> TaskExists(int id)
        {
            return await _context.Tasks.AnyAsync(x => x.Id == id);
        }
    }
}