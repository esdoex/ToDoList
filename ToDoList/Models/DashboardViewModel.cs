using System.Collections.Generic;
using ToDoList.Models;

namespace ToDoList.Models
{
    public class DashboardViewModel
    {
        public List<TaskItem> UpcomingTasks { get; set; }
        public List<TaskItem> InProgressTasks { get; set; }
        public List<TaskItem> CompletedTasks { get; set; }
        public List<TaskItem> OverdueTasks { get; set; }
    }
}
