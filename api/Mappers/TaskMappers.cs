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
        /// Bir UserTask nesnesini TaskDto nesnesine d�n��t�r�r.
        /// </summary>
        /// <param name="task">D�n��t�r�lecek UserTask nesnesi.</param>
        /// <returns>D�n��t�r�lm�� TaskDto nesnesi veya e�er giri� null ise null.</returns>
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
        /// Bir CreateTaskRequestDto nesnesini UserTask nesnesine d�n��t�r�r.
        /// </summary>
        /// <param name="taskDto">D�n��t�r�lecek CreateTaskRequestDto nesnesi.</param>
        /// <returns>D�n��t�r�lm�� UserTask nesnesi veya e�er giri� null ise null.</returns>
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
