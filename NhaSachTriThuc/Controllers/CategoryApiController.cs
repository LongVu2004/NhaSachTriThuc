using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using NhaSachTriThuc.Data;
using System.Linq;

namespace NhaSachTriThuc.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryApiController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CategoryApiController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetCategories()
        {
            var categories = _context.Categories
                .Select(c => new { c.CategoryId, c.Name })
                .ToList();

            return Ok(categories);
        }
    }
}
