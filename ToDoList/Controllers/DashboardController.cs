using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToDoList.Data;
using ToDoList.Models;


[Authorize]
public class DashboardController : Controller
{
    private readonly ApplicationDbContext _context;

    public DashboardController(ApplicationDbContext context)
    {
        _context = context;
    }

    // CREATING THE TASK
    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    // POST: CREATING THE TASK
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TaskItem task)
    {
        if (ModelState.IsValid)
        {
            task.Status = ToDoList.Models.TaskStatus.NotStarted; // DEFAULT STATUS
            task.CreatedDate = DateTime.Now; // Auto date
            _context.TaskItems.Add(task);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index)); // Redirect to dashboard after creating a task
        }
        return View(task); 
    }

    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value;

        var tasks = await _context.TaskItems
            .Where(t => t.UserId == userId)
            .ToListAsync();


        // OVERDUE UPDATE
        foreach (var task in tasks)
        {
            if (task.Status != ToDoList.Models.TaskStatus.Completed && task.DueDate < DateTime.Now)
            {
                task.Status = ToDoList.Models.TaskStatus.Overdue;
            }
        }
        await _context.SaveChangesAsync();

        // CATEGORIZING

        var upcomingTasks = tasks.Where(t => t.Status == ToDoList.Models.TaskStatus.NotStarted && t.DueDate >= DateTime.Now).ToList();
        var inProgressTasks = tasks.Where(t => t.Status == ToDoList.Models.TaskStatus.InProgress).ToList();
        var completedTasks = tasks.Where(t => t.Status == ToDoList.Models.TaskStatus.Completed).ToList();
        var overdueTasks = tasks.Where(t => t.Status != ToDoList.Models.TaskStatus.Completed && t.DueDate < DateTime.Now).ToList();

        var dashboardViewModel = new ToDoList.Models.DashboardViewModel
        {
            UpcomingTasks = upcomingTasks,
            InProgressTasks = inProgressTasks,
            CompletedTasks = completedTasks,
            OverdueTasks = overdueTasks
        };

        return View(dashboardViewModel);
    }
}
