using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ToDoList.Models
{
    public enum Priority
    {
        Low,
        Medium,
        High,
        Urgent
    }

    public enum TaskStatus
    {
        NotStarted,
        InProgress,
        Completed,
        Overdue
    }

    public class TaskItem
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "The Title field is required.")]
        [StringLength(100)]
        public string Title { get; set; }

        [Required(ErrorMessage = "The Description field is required.")]
        [StringLength(500)]
        public string Description { get; set; }

        [Required(ErrorMessage = "The DueDate field is required.")]
        [DataType(DataType.Date)]
        public DateTime DueDate { get; set; }

        [Required(ErrorMessage = "The PriorityStatus field is required.")]
        public Priority PriorityStatus { get; set; }

        public TaskStatus Status { get; set; } = TaskStatus.NotStarted; // DEFAUT STAT

        [Required]
        public DateTime CreatedDate { get; set; } = DateTime.Now; // AUTO DATE

        [Required]
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }
    }
}
