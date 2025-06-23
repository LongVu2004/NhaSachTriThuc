using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NhaSachTriThuc.Data;
using NhaSachTriThuc.Models;
using NhaSachTriThuc.Repositories;

namespace NhaSachTriThuc.Controllers
{
    //public class ProfileController : Controller
    //{
    //    private readonly UserManager<IdentityUser> _userManager;
    //    private readonly AppDbContext _context;
    //    private readonly IWebHostEnvironment _env;

    //    public ProfileController(UserManager<IdentityUser> userManager, AppDbContext context, IWebHostEnvironment env)
    //    {
    //        _userManager = userManager;
    //        _context = context;
    //        _env = env;
    //    }

    //    [HttpGet]
    //    public async Task<IActionResult> Edit()
    //    {
    //        var user = await _userManager.GetUserAsync(User);
    //        if (user == null) return NotFound();

    //        var profile = await _context.UserProfiles.FindAsync(user.Id);
    //        if (profile == null)
    //        {
    //            profile = new UserProfile
    //            {
    //                UserId = user.Id,
    //                FullName = user.UserName ?? "Người dùng"
    //            };
    //            _context.UserProfiles.Add(profile);
    //            await _context.SaveChangesAsync();
    //        }

    //        return View(profile);
    //    }

    //    [HttpPost]
    //    public async Task<IActionResult> Edit(UserProfile model, IFormFile? avatarFile)
    //    {
    //        if (!ModelState.IsValid)
    //            return View(model);

    //        var profile = await _context.UserProfiles.FindAsync(model.UserId);
    //        if (profile == null) return NotFound();

    //        profile.FullName = model.FullName;

    //        // Upload ảnh
    //        if (avatarFile != null && avatarFile.Length > 0)
    //        {
    //            var uploadsFolder = Path.Combine(_env.WebRootPath, "avatars");
    //            Directory.CreateDirectory(uploadsFolder);
    //            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(avatarFile.FileName);
    //            var filePath = Path.Combine(uploadsFolder, fileName);

    //            using (var fileStream = new FileStream(filePath, FileMode.Create))
    //            {
    //                await avatarFile.CopyToAsync(fileStream);
    //            }

    //            profile.Avatar = "/avatars/" + fileName;
    //        }

    //        _context.Update(profile);
    //        await _context.SaveChangesAsync();

    //        TempData["Success"] = "Cập nhật hồ sơ thành công!";
    //        return RedirectToAction("Edit");
    //    }
    //}
    public class ProfileController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly IUserProfileRepository _userProfileRepository;

        public ProfileController(UserManager<IdentityUser> userManager,
                                 AppDbContext context,
                                 IWebHostEnvironment env, 
                                 IUserProfileRepository userProfileRepository)
        {
            _userManager = userManager;
            _context = context;
            _env = env;
            _userProfileRepository = userProfileRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var profile = await _context.UserProfiles.FindAsync(user.Id);

            if (profile == null)
            {
                profile = new UserProfile
                {
                    UserId = user.Id,
                    FullName = user.UserName ?? "Người dùng"
                };
                _context.UserProfiles.Add(profile);
                await _context.SaveChangesAsync(); // đảm bảo FullName không null
            }

            return View(profile);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, UserProfile user, IFormFile avatarFile)
        {
            //if (!ModelState.IsValid) return View(model);

            //var profile = await _context.UserProfiles.FindAsync(model.UserId);
            //if (profile == null) return NotFound();

            //profile.FullName = model.FullName;

            //// Xử lý upload ảnh
            //if (avatarFile != null && avatarFile.Length > 0)
            //{
            //    var uploads = Path.Combine(_env.WebRootPath, "images", "avatars");
            //    Directory.CreateDirectory(uploads);
            //    var fileName = Guid.NewGuid() + Path.GetExtension(avatarFile.FileName);
            //    var filePath = Path.Combine(uploads, fileName);

            //    Console.WriteLine("Đường dẫn ảnh lưu: " + filePath);

            //    // Xóa ảnh cũ nếu có
            //    if (!string.IsNullOrEmpty(profile.Avatar))
            //    {
            //        var oldPath = Path.Combine(_env.WebRootPath, profile.Avatar.TrimStart('/'));
            //        if (System.IO.File.Exists(oldPath))
            //        {
            //            System.IO.File.Delete(oldPath);
            //        }
            //    }

            //    // Lưu ảnh mới
            //    using (var stream = new FileStream(filePath, FileMode.Create))
            //    {
            //        await avatarFile.CopyToAsync(stream);
            //    }

            //    profile.Avatar = "/images/avatars/" + fileName;
            //}


            //_context.UserProfiles.Update(profile);
            //await _context.SaveChangesAsync();

            //TempData["Success"] = "Cập nhật hồ sơ thành công!";
            //return RedirectToAction("Edit", new { id = model.UserId });

            if (id != user.UserId) return BadRequest();

            if (ModelState.IsValid)
            {
                if (avatarFile != null && avatarFile.Length > 0)
                {
                    var fileName = Path.GetFileName(avatarFile.FileName);
                    var images = Path.Combine(_env.WebRootPath, "images", "avatars");

                    if (!Directory.Exists(images))
                        Directory.CreateDirectory(images);

                    var filePath = Path.Combine(images, fileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await avatarFile.CopyToAsync(fileStream);
                    }

                    user.Avatar = "/images/avatars/" + fileName;
                }

                await _userProfileRepository.UpdateAsync(user);
                _context.SaveChangesAsync();
                return RedirectToAction("Edit", new { id = user.UserId });
            }

            return View(user);
        }
    }


}
