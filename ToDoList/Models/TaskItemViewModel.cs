using ToDoList.Models;

namespace ToDoList.ViewModels
{
    public class TaskItemViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string DueDate { get; set; } // Keep it as a string to handle parsing
        public Priority PriorityStatus { get; set; }
        public Models.TaskStatus Status { get; set; }

    }

}
