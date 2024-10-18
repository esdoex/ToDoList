using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToDoList.Data;
using ToDoList.Models;
using Microsoft.AspNetCore.Authorization;
using ToDoList.ViewModels;
using System.Security.Claims;
using ToDoList.Helpers;
using TaskStatus = ToDoList.Models.TaskStatus;

namespace ToDoList.Controllers
{
    [Route("Tasks")]
    [Authorize]
    public class TasksController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<TasksController> _logger;

        public TasksController(ApplicationDbContext context, ILogger<TasksController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // TASKS: GET
        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var tasks = await _context.TaskItems.Include(t => t.User).ToListAsync();
            return View(tasks);
        }

        // DETAILS: GET
        [HttpGet("Details/{encodedId}")]
        public async Task<IActionResult> Details(string encodedId)
        {
            var taskItem = await FindUserTaskAsync(encodedId);
            if (taskItem == null)
            {
                return NotFound();
            }
            return View(taskItem);
        }

        // CREATE: GET
        [HttpGet("Create")]
        public IActionResult Create()
        {
            return View();
        }

        // CREATE: POST
        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TaskItemViewModel taskItemViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(taskItemViewModel);
            }

            if (!DateTime.TryParse(taskItemViewModel.DueDate, out DateTime parsedDate))
            {
                ModelState.AddModelError("DueDate", "Invalid date format.");
                return View(taskItemViewModel);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                throw new InvalidOperationException("User ID claim not found. Ensure the user is authenticated.");
            }

            var taskItem = new TaskItem
            {
                Title = taskItemViewModel.Title,
                Description = taskItemViewModel.Description,
                PriorityStatus = taskItemViewModel.PriorityStatus,
                DueDate = parsedDate,
                UserId = userId,
                Status = TaskStatus.NotStarted,
                CreatedDate = DateTime.Now
            };

            _context.Add(taskItem);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Dashboard");
        }

        // EDIT: GET
        [HttpGet("Edit/{encodedId}")]
        public async Task<IActionResult> Edit(string encodedId)
        {
            var taskItem = await FindUserTaskAsync(encodedId);
            if (taskItem == null)
            {
                return Forbid();
            }

            var taskItemViewModel = new TaskItemViewModel
            {
                Id = taskItem.Id,
                Title = taskItem.Title,
                Description = taskItem.Description,
                DueDate = taskItem.DueDate.ToString("yyyy-MM-dd"),
                PriorityStatus = taskItem.PriorityStatus,
                Status = taskItem.Status
            };

            return View(taskItemViewModel);
        }

        // EDIT: POST
        [HttpPost("Edit/{encodedId}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string encodedId, TaskItemViewModel taskItemViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(taskItemViewModel);
            }

            var taskItem = await FindUserTaskAsync(encodedId);
            if (taskItem == null)
            {
                return Forbid();
            }

            taskItem.Title = taskItemViewModel.Title;
            taskItem.Description = taskItemViewModel.Description;
            taskItem.PriorityStatus = taskItemViewModel.PriorityStatus;
            taskItem.Status = taskItemViewModel.Status;

            if (DateTime.TryParse(taskItemViewModel.DueDate, out DateTime parsedDate))
            {
                taskItem.DueDate = parsedDate;
            }

            _context.Update(taskItem);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Dashboard");
        }

        // DELETE: GET
        [HttpGet("Delete/{encodedId}")]
        public async Task<IActionResult> Delete(string encodedId)
        {
            var taskItem = await FindUserTaskAsync(encodedId);
            if (taskItem == null)
            {
                return Forbid();
            }

            return View(taskItem);
        }

        // DELETE: POST
        [HttpPost("Delete/{encodedId}"), ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string encodedId)
        {
            var taskItem = await FindUserTaskAsync(encodedId);
            if (taskItem != null)
            {
                _context.TaskItems.Remove(taskItem);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index", "Dashboard");
        }

        // FIND IF TASKS BELONGS TO USER
        private async Task<TaskItem> FindUserTaskAsync(string encodedId)
        {
            if (string.IsNullOrEmpty(encodedId)) return null;

            try
            {
                var taskId = IdEncoderHelper.DecodeId(encodedId);
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                return await _context.TaskItems.FirstOrDefaultAsync(t => t.Id == taskId && t.UserId == userId);
            }
            catch
            {
                return null;
            }
        }
    }
}
