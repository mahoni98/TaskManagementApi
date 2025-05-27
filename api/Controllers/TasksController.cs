using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Dtos.Task;
using api.Interfaces;
using api.Mappers;
using api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using api.Helpers;
using api.Services; 

namespace api.Controllers
{
    [Route("api/task")]
    [ApiController]
    [Authorize]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService; // Yeni servis eklendi
        /// <summary>
        /// TasksController sınıfının yeni bir örneğini başlatır.
        /// </summary>
        /// <param name="taskService">Görev işlemleri için kullanılan ITaskService servisi.</param>
        public TasksController(ITaskService taskService)
        {
            _taskService = taskService;
            //_context = context;
        }

        /// <summary>
        /// Kimliği doğrulanmış kullanıcının tüm görevlerini belirtilen sorgu kriterlerine göre listeler.
        /// </summary>
        /// <param name="query">Görevleri filtrelemek, sıralamak ve sayfalamak için kullanılan sorgu nesnesi.</param>
        /// <returns>Belirtilen kriterlere uyan görevlerin listesini içeren bir Ok (200) yanıtı;
        /// doğrulama hatası durumunda BadRequest (400) veya Unauthorized (401) yanıtı döner.</returns>
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] QueryObject query)
        {
            if (!ModelState.IsValid)// validation kurallarına uygun mu  ? 
                return BadRequest(ModelState); 

            // JWT token'dan kullanıcı ID'sini al
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // user ıd buldu 
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Kullanıcı kimliği bulunamadı.");
            }
            query.UserId = userId; // Query nesnesine kullanıcı ID'sini ata

            var tasksDto = await _taskService.GetAllTasksAsync(query, userId); // Servisi kullan

            return Ok(tasksDto);
        }
        /// <summary>
        /// Belirtilen ID'ye sahip görevi, kimliği doğrulanmış kullanıcıya ait olması koşuluyla getirir.
        /// </summary>
        /// <param name="id">Getirilecek görevin benzersiz ID'si.</param>
        /// <returns>İstenen görevi içeren bir Ok (200) yanıtı;
        /// doğrulama hatası durumunda BadRequest (400), görev bulunamazsa NotFound (404) veya erişim izni yoksa Unauthorized (401) yanıtı döner.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Kullanıcı kimliği bulunamadı.");
            }

            var taskDto = await _taskService.GetTaskByIdAsync(id, userId); // Servisi kullan

            if (taskDto == null)
            {
                return NotFound("Görev bulunamadı veya bu göreve erişim izniniz yok.");
            }

            return Ok(taskDto);
        }
        /// <summary>
        /// Kimliği doğrulanmış kullanıcı için yeni bir görev oluşturur.
        /// </summary>
        /// <param name="taskDto">Oluşturulacak görevin detaylarını içeren DTO.</param>
        /// <returns>Başarılı oluşturma durumunda oluşturulan görevin detaylarını içeren CreatedAtAction (201) yanıtı;
        /// doğrulama hatası durumunda BadRequest (400), Unauthorized (401) veya sunucu hatası durumunda StatusCode (500) yanıtı döner.</returns>
        [HttpPost]
        public async Task<IActionResult> CreateTask([FromBody] CreateTaskRequestDto taskDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // JWT token'dan kullanıcı ID'sini al ve modele ata
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Kullanıcı kimliği bulunamadı.");
            }

            var createdTaskDto = await _taskService.CreateTaskAsync(taskDto, userId); // Servisi kullan

            if (createdTaskDto == null)
            {
                return StatusCode(500, "Görev oluşturulurken bir hata oluştu.");
            }

            return CreatedAtAction(nameof(GetById), new { id = createdTaskDto.Id }, createdTaskDto);
        }

        /// <summary>
        /// Belirtilen ID'ye sahip görevi, kimliği doğrulanmış kullanıcıya ait olması koşuluyla günceller.
        /// </summary>
        /// <param name="id">Güncellenecek görevin benzersiz ID'si.</param>
        /// <param name="updateDto">Görevin güncellenmiş detaylarını içeren DTO.</param>
        /// <returns>Başarılı güncelleme durumunda güncellenen görevin detaylarını içeren Ok (200) yanıtı;
        /// doğrulama hatası durumunda BadRequest (400), Unauthorized (401), görev bulunamazsa NotFound (404) veya erişim izni yoksa NotFound (404) yanıtı döner.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask([FromRoute] int id, [FromBody] UpdateTaskRequestDto updateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Kullanıcı kimliği bulunamadı.");
            }

            var updatedTaskDto = await _taskService.UpdateTaskAsync(id, updateDto, userId); // Servisi kullan

            if (updatedTaskDto == null)
            {
                return NotFound("Görev bulunamadı veya bu görevi güncelleme izniniz yok.");
            }

            return Ok(updatedTaskDto);
        }

        /// <summary>
        /// Belirtilen ID'ye sahip görevi, kimliği doğrulanmış kullanıcıya ait olması koşuluyla siler.
        /// </summary>
        /// <param name="id">Silinecek görevin benzersiz ID'si.</param>
        /// <returns>Başarılı silme durumunda NoContent (204) yanıtı;
        /// doğrulama hatası durumunda BadRequest (400), Unauthorized (401), görev bulunamazsa NotFound (404) veya erişim izni yoksa NotFound (404) yanıtı döner.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask([FromRoute] int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Kullanıcı kimliği bulunamadı.");
            }

            var isDeleted = await _taskService.DeleteTaskAsync(id, userId); // Servisi kullan

            if (!isDeleted)
            {
                return NotFound("Görev bulunamadı veya bu görevi silme izniniz yok.");
            }

            return NoContent();
        }
    }
}