using System;
using System.ComponentModel.DataAnnotations;

namespace taskDz.Models
{
    public class CreateTask
    {
        [Required(ErrorMessage = "Title is required")]
        [StringLength(100, ErrorMessage = "Title must have 100 symbols maximum")]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required(ErrorMessage = "Deadline date is required")]
        public DateTime Deadline { get; set; }
    }
}