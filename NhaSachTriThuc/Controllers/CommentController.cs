using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NhaSachTriThuc.Data;
using NhaSachTriThuc.Models;
using System.Security.Claims;

namespace NhaSachTriThuc.Controllers
{
    [Authorize]
    public class CommentsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public CommentsController(AppDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int bookId, string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                TempData["Error"] = "Nội dung bình luận không được để trống.";
                return RedirectToAction("Details", "Product", new { id = bookId });
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var comment = new Comment
            {
                BookId = bookId,
                Content = content,
                UserId = userId
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            TempData["Message"] = "Bình luận của bạn đã được đăng.";
            return RedirectToAction("Details", "Product", new { id = bookId });
        }
    }
}
