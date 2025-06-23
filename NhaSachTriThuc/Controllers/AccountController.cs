//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Mvc;
//using NhaSachTriThuc.Models;

//namespace NhaSachTriThuc.Controllers
//{
//    [Authorize]
//    public class AccountController : Controller
//    {
//        private readonly UserManager<ApplicationUser> _userManager;
//        private readonly IWebHostEnvironment _env;

//        public AccountController(UserManager<ApplicationUser> userManager, IWebHostEnvironment env)
//        {
//            _userManager = userManager;
//            _env = env;
//        }

//        public async Task<IActionResult> Profile()
//        {
//            var user = await _userManager.GetUserAsync(User);
//            return View(user);
//        }

//        [HttpPost]
//        public async Task<IActionResult> Profile(ApplicationUser model, IFormFile? avatarFile)
//        {
//            var user = await _userManager.GetUserAsync(User);

//            if (user == null) return NotFound();

//            user.FullName = model.FullName;

//            if (avatarFile != null && avatarFile.Length > 0)
//            {
//                var fileName = $"{Guid.NewGuid()}_{avatarFile.FileName}";
//                var filePath = Path.Combine(_env.WebRootPath, "images", fileName);

//                using var stream = new FileStream(filePath, FileMode.Create);
//                await avatarFile.CopyToAsync(stream);

//                user.Avatar = "/images/" + fileName;
//            }

//            await _userManager.UpdateAsync(user);
//            ViewBag.Message = "Cập nhật thành công!";
//            return View(user);
//        }
//    }
//}
