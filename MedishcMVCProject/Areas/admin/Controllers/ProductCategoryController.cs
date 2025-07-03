using MedishcMVCProject.DAL;
using MedishcMVCProject.Models;
using MedishcMVCProject.Utilities.Helpers;
using MedishcMVCProject.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MedishcMVCProject.Areas.admin.Controllers
{
    [Area("Admin")]
    public class ProductCategoryController : Controller
    {
        private readonly AppDbContext _context;

        public ProductCategoryController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult List(string name = null)
        {
            var categories = _context.ProductCategories
                .Where(c => string.IsNullOrEmpty(name) || c.Name.Contains(name))
                .Select(c => new ProductCategoryVM
                {
                    Id = c.Id,
                    CategoryName = c.Name
                }).ToList();

            return View(categories);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductCategoryVM vm)
        {
            if (string.IsNullOrWhiteSpace(vm.CategoryName))
            {
                ModelState.AddModelError(nameof(vm.CategoryName), "Category name is required.");
                return View(vm);
            }

            bool isCategoryExist = await _context.ProductCategories
                .AnyAsync(c => c.Name == vm.CategoryName);

            if (isCategoryExist)
            {
                ModelState.AddModelError(nameof(vm.CategoryName), "A category with this name already exists.");
                return View(vm);
            }

            var category = new ProductCategory
            {
                Name = vm.CategoryName.Capitalize()
            };

            _context.ProductCategories.Add(category);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(List));
        }

        public async Task<IActionResult> Update(int id)
        {
            var category = await _context.ProductCategories
                .Where(c => c.Id == id)
                .Select(c => new ProductCategoryVM
                {
                    Id = c.Id,
                    CategoryName = c.Name
                })
                .FirstOrDefaultAsync();

            if (category == null) return NotFound();

            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int? id, ProductCategoryVM vm)
        {
            if (string.IsNullOrWhiteSpace(vm.CategoryName))
            {
                ModelState.AddModelError(nameof(vm.CategoryName), "Category name is required.");
                return View(vm);
            }

            var category = await _context.ProductCategories.FindAsync(vm.Id);

            if (category == null) return NotFound();


            bool isCategoryExist = await _context.ProductCategories
                .AnyAsync(c => c.Name == vm.CategoryName && c.Id != id);

            if (isCategoryExist)
            {
                ModelState.AddModelError(nameof(vm.CategoryName), "A category with this name already exists.");
                return View(vm);
            }

            category.Name = vm.CategoryName;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(List));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var category = await _context.ProductCategories.FindAsync(id);
            if (category == null) return NotFound();

            _context.ProductCategories.Remove(category);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(List));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteSelected(List<int> selectedIds)
        {
            if (selectedIds == null || !selectedIds.Any())
            {
                TempData["Warning"] = "Please select at least one category to delete.";
                return RedirectToAction(nameof(List));
            }

            var categories = await _context.ProductCategories
                .Where(c => selectedIds.Contains(c.Id))
                .ToListAsync();

            _context.ProductCategories.RemoveRange(categories);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(List));
        }
    }
}
