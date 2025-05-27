using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.Task;
using api.Models;
using Microsoft.AspNetCore.Http.HttpResults;

namespace api.Mappers
{
    public static class TaskMappers
    {

        /// <summary>
        /// Bir UserTask nesnesini TaskDto nesnesine dönüþtürür.
        /// </summary>
        /// <param name="task">Dönüþtürülecek UserTask nesnesi.</param>
        /// <returns>Dönüþtürülmüþ TaskDto nesnesi veya eðer giriþ null ise null.</returns>
        public static TaskDto ToTaskDto(this UserTask task)
        {
            if (task == null) return null;
            return new TaskDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                IsCompleted = task.IsCompleted,
                DueDate = task.DueDate,
                UserId = task.UserId
            };
        }

        /// <summary>
        /// Bir CreateTaskRequestDto nesnesini UserTask nesnesine dönüþtürür.
        /// </summary>
        /// <param name="taskDto">Dönüþtürülecek CreateTaskRequestDto nesnesi.</param>
        /// <returns>Dönüþtürülmüþ UserTask nesnesi veya eðer giriþ null ise null.</returns>
        public static UserTask ToTaskFromCreateDto(this CreateTaskRequestDto taskDto)
        {
            if (taskDto == null) return null;
            return new UserTask
            {
                Title = taskDto.Title,
                Description = taskDto.Description,
                IsCompleted = taskDto.IsCompleted,
                DueDate = taskDto.DueDate,
            };
        }
    }
}
