using MedishcMVCProject.DAL;
using MedishcMVCProject.Models;
using MedishcMVCProject.Utilities;
using MedishcMVCProject.Utilities.Extensions;
using MedishcMVCProject.Utilities.Helpers;
using MedishcMVCProject.ViewModels;
using MedishcMVCProject.ViewModels.ProductVM;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MedishcMVCProject.Areas.admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly UserManager<AppUser> _userManager;

        public ProductController(AppDbContext context, IWebHostEnvironment env, UserManager<AppUser> userManager)
        {
            _context = context;
            _env = env;
            _userManager = userManager;
        }

        public async Task<IActionResult> List(string name = null, decimal? minPrice = null, decimal? maxPrice = null, string specialistName = null)
        {
            List<GetProductVM> shops = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Specialist)
                .Select(p => new GetProductVM
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    SKU = p.SKU,
                    CategoryName = p.Category.Name,
                    Image = p.ImageUrl,
                    SpecialistName = p.Specialist != null ? p.Specialist.Name : "",
                    SpecialistId = p.SpecialistId
                }).ToListAsync();

            if (!string.IsNullOrWhiteSpace(name))
            {
                shops = shops
                    .Where(p => p.Name.Contains(name.Trim(), StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            if (minPrice.HasValue)
            {
                shops = shops
                    .Where(p => p.Price >= minPrice.Value)
                    .ToList();
            }

            if (maxPrice.HasValue)
            {
                shops = shops
                    .Where(p => p.Price <= maxPrice.Value)
                    .ToList();
            }

            if (!string.IsNullOrWhiteSpace(specialistName))
            {
                shops = shops
                    .Where(p => p.SpecialistName.Contains(specialistName.Trim(), StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            return View(shops);
        }


        public async Task<IActionResult> Detail(int? id)
        {
            if (id is null || id <= 0) return BadRequest();

            Product? product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product is null) return NotFound();

            GetProductVM vm = new GetProductVM
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                SKU = product.SKU,
                Image = product.ImageUrl,
                Description = product.Description,
                CategoryName = product.Category?.Name ?? "Unknown"
            };

            return View(vm);
        }
        public async Task<IActionResult> Create()
        {
            CreateProductVM vm = new CreateProductVM
            {
                ProductCategories = await _context.ProductCategories.ToListAsync(),
                Specialists = await _context.Specialists.ToListAsync()
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateProductVM vm)
        {
            vm.ProductCategories = await _context.ProductCategories.ToListAsync();
            vm.Specialists = await _context.Specialists.ToListAsync();

            if (!vm.MainPhoto.ValidateType("image/"))
            {
                ModelState.AddModelError(nameof(vm.MainPhoto), "File type is incorrect");
                return View(vm);
            }

            if (!vm.MainPhoto.ValidateSize(FileType.MB, 1))
            {
                ModelState.AddModelError(nameof(vm.MainPhoto), "File must be less than 1MB");
                return View(vm);
            }

            if (!ModelState.IsValid) return View(vm);

            bool categoryExists = await _context.ProductCategories.AnyAsync(c => c.Id == vm.CategoryId);
            bool specialistExist = await _context.Specialists.AnyAsync(c => c.Id == vm.SpecialistId);

            if (!categoryExists)
            {
                ModelState.AddModelError(nameof(vm.CategoryId), "Selected category does not exist");
                return View(vm);
            }

            if (!specialistExist)
            {
                ModelState.AddModelError(nameof(vm.SpecialistId), "Selected specialist does not exist");
                return View(vm);
            }

            string fileName = await vm.MainPhoto.CreateFileAsync(_env.WebRootPath, "assets", "images", "shop", "full");

            Product product = new Product
            {
                Name = vm.Name.Capitalize(),
                SKU = vm.SKU,
                Price = vm.Price,
                Description = vm.Description,
                ImageUrl = fileName,
                CategoryId = vm.CategoryId,
                SpecialistId = vm.SpecialistId
            };

            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(List));
        }


        public async Task<IActionResult> Update(int? id)
        {
            if (id == null || id <= 0) return BadRequest();

            Product? product = await _context.Products.Include(p => p.Specialist).Include(p => p.Category).FirstOrDefaultAsync(p => p.Id == id);
            if (product == null) return NotFound();

            UpdateProductVM vm = new UpdateProductVM
            {
                Id = product.Id,
                Name = product.Name,
                SKU = product.SKU,
                Price = product.Price,
                Description = product.Description,
                ImageUrl = product.ImageUrl,
                CategoryId = product.CategoryId,
                SpecialistId = product.SpecialistId,
                ProductCategories = await _context.ProductCategories.ToListAsync(),
                Specialists = await _context.Specialists.ToListAsync()
            };

            return View(vm);
        }



        [HttpPost]
        public async Task<IActionResult> Update(int? id, UpdateProductVM vm)
        {
            if (id == null || id <= 0 || id != vm.Id)
                return BadRequest();

            vm.ProductCategories = await _context.ProductCategories.ToListAsync();
            vm.Specialists = await _context.Specialists.ToListAsync();
            if (!ModelState.IsValid)
                return View(vm);

            Product? existedProduct = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);

            if (existedProduct is null)
                return NotFound();

            bool categoryExists = vm.ProductCategories.Any(c => c.Id == vm.CategoryId);
            if (!categoryExists)
            {
                ModelState.AddModelError(nameof(vm.CategoryId), "Selected category does not exist");
                return View(vm);
            }

            if (vm.MainPhoto is not null)
            {
                if (!vm.MainPhoto.ValidateType("image/"))
                {
                    ModelState.AddModelError(nameof(vm.MainPhoto), "File type is incorrect");
                    return View(vm);
                }

                if (!vm.MainPhoto.ValidateSize(FileType.MB, 1))
                {
                    ModelState.AddModelError(nameof(vm.MainPhoto), "File must be less than 1MB");
                    return View(vm);
                }

                string newImage = await vm.MainPhoto.CreateFileAsync(_env.WebRootPath, "assets", "images", "shop", "full");
                existedProduct.ImageUrl.DeleteFile(_env.WebRootPath, "assets", "images", "shop", "full");
                existedProduct.ImageUrl = newImage;
            }

            existedProduct.Name = vm.Name.Capitalize();
            existedProduct.SKU = vm.SKU;
            existedProduct.Price = vm.Price;
            existedProduct.Description = vm.Description;
            existedProduct.CategoryId = vm.CategoryId;
            existedProduct.SpecialistId = vm.SpecialistId;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(List));
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null || id <= 0) return BadRequest();

            Product? product = await _context.Products
                .Include(p => p.Specialist)
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product is null) return NotFound();

            if (!string.IsNullOrEmpty(product.ImageUrl))
            {
                product.ImageUrl.DeleteFile(_env.WebRootPath, "assets", "images", "shop", "full");
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(List));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteSelected(List<int> selectedIds)
        {
            if (selectedIds == null || !selectedIds.Any())
            {
                TempData["Warning"] = "Please select at least one product to delete.";
                return RedirectToAction(nameof(List));
            }

            List<Product> products = await _context.Products
                .Where(p => selectedIds.Contains(p.Id))
                .Include(p => p.Specialist)
                .Include(p => p.Category)
                .ToListAsync();



            foreach (var product in products)
            {
                if (!string.IsNullOrEmpty(product.ImageUrl))
                {
                    product.ImageUrl.DeleteFile(_env.WebRootPath, "assets", "images", "shop", "full");
                }
            }

            _context.Products.RemoveRange(products);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"{products.Count} product(s) deleted successfully.";

            return RedirectToAction(nameof(List));
        }

    }
}
