using MedishcMVCProject.DAL;
using MedishcMVCProject.Models;
using MedishcMVCProject.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MedishcMVCProject.Controllers
{
    public class BlogController : Controller
    {
        private readonly AppDbContext _context;

        public BlogController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            BlogVM vm = new BlogVM()
            {
                Blogs = await _context.Blogs.Include(b => b.Author).Take(6).ToListAsync()
            };

            return View(vm);
        }

        public IActionResult BlogDetail(int? id)
        {
            if (id <= 0) return BadRequest();

            Blog? blog = _context.Blogs
                .Include(b => b.Author)
                .Include(b => b.BlogCategories)
                    .ThenInclude(bc => bc.Category)
                .FirstOrDefault(b => b.Id == id && !b.IsDeleted);

            if (blog is null) return NotFound();

            BlogDetailVM vm = new BlogDetailVM
            {
                Blog = blog,
                Author = blog.Author,
                Categories = blog.BlogCategories
                  .Select(bc => bc.Category)
                  .ToList()
            };

            return View(vm);
        }
    }
}
