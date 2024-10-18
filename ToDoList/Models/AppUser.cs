using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace ToDoList.Models
{
    public class ApplicationUser : IdentityUser
    {
        public virtual ICollection<TaskItem> Tasks { get; set; }
    }
}
