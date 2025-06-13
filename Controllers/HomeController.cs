using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyDotnetApp.Data;
using MyDotnetApp.Models;
using System.Diagnostics;

namespace MyDotnetApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult Activities()
        {
            // var activities = _context.Activities.ToList();
            // return View(activities);
            return View();
        }

        [AllowAnonymous]
        public IActionResult Profile()
        {
            return View();
        }

        [AllowAnonymous]
        public async Task<IActionResult> MyActivities()
        {
            // 假设我们已经从认证中获取了用户ID
            // 在实际应用中，您需要从认证上下文中获取用户ID
            int userId = 1; // 这里应该是从认证中获取的用户ID

            var activities = await _context.Activities
                .Include(a => a.Registrations)
                .Where(a => a.Registrations.Any(r => r.UserId == userId))
                .ToListAsync();

            return View(activities);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

    public class ErrorViewModel
    {
        public string? RequestId { get; set; }
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}