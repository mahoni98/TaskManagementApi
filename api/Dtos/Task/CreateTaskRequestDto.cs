using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dtos.Task
{
    public class CreateTaskRequestDto
    {
        [Required(ErrorMessage = "Başlık gereklidir.")] // Validasyon eklendi
        [MinLength(3, ErrorMessage = "Başlık en az 3 karakter olmalıdır.")]
        [MaxLength(200, ErrorMessage = "Başlık en fazla 200 karakter olmalıdır.")]
        public string Title { get; set; }
        public string? Description { get; set; }
        [Required(ErrorMessage = "Tamamlanma durumu gereklidir.")] // Validasyon eklendi
        public bool IsCompleted { get; set; }
        [Required(ErrorMessage = "Teslim tarihi gereklidir.")] // Validasyon eklendi
        public DateTime DueDate { get; set; }
        
    }
}