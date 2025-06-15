using MedishcMVCProject.DAL;
using MedishcMVCProject.Models;
using MedishcMVCProject.Utilities;
using MedishcMVCProject.Utilities.Extensions;
using MedishcMVCProject.Utilities.Helpers;
using MedishcMVCProject.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MedishcMVCProject.Areas.admin.Controllers
{
    [Area("Admin")]

    public class StaffController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public StaffController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public IActionResult List(string staffName = null, string designation = null)
        {
            List<ContactInfo>? contactInfos = _context.ContactInfos
                    .Where(ci => ci.OwnerType == OwnerType.Staff)
                    .ToList();

            List<Staff> staffEntities = _context.Staffs
                .Include(s => s.Designation)
                .ToList();

            List<GetStaffVM> staffs = staffEntities.Select(s => new GetStaffVM
            {
                Id = s.Id,
                Name = s.Name,
                Surname = s.Surname,
                DesignationName = s.Designation.Name,
                Image = s.Image,
                Email = contactInfos
                    .FirstOrDefault(c => c.OwnerId == s.Id && c.ContactType == ContactType.Email)?.Value,
                PhoneNumber = contactInfos
                    .FirstOrDefault(c => c.OwnerId == s.Id && c.ContactType == ContactType.Phone)?.Value
            }).ToList();

            if (!string.IsNullOrWhiteSpace(staffName))
                staffs = Helpers.FilterByText(staffs, s => s.Name + " " + s.Surname, staffName);

            if (!string.IsNullOrWhiteSpace(designation))
                staffs = Helpers.FilterByText(staffs, s => s.DesignationName, designation);

            return View(staffs);
        }

        public async Task<IActionResult> Create()
        {
            CreateStaffVM staffVM = new CreateStaffVM()
            {
                Designations = await _context.Designations.ToListAsync()
            };

            return View(staffVM);
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateStaffVM staffVM)
        {
            staffVM.Designations = await _context.Designations.ToListAsync();

            if (!Helpers.HasDigit(staffVM.Name))
            {
                ModelState.AddModelError(nameof(staffVM.Name), "Name cannot contain digits");
            }

            if (!Helpers.HasDigit(staffVM.Surname))
            {
                ModelState.AddModelError(nameof(staffVM.Surname), "Surname cannot contain digits");
            }


            if (!ModelState.IsValid)
            {
                return View(staffVM);
            }

            bool result = staffVM.Designations.Any(d => d.Id == staffVM.DesignationId);

            if (!result)
            {
                ModelState.AddModelError(nameof(staffVM.DesignationId), "Designation does not exist");
                return View(staffVM);
            }



            if (!staffVM.MainPhoto.ValidateType("image/"))
            {
                ModelState.AddModelError(nameof(CreateStaffVM.MainPhoto), "file type is incorrect");
                return View(staffVM);
            }

            if (!staffVM.MainPhoto.ValidateSize(FileType.MB, 1))
            {
                ModelState.AddModelError(nameof(CreateStaffVM.MainPhoto), "file must be less than 1MB");
                return View(staffVM);
            }

            string image = await staffVM.MainPhoto.CreateFileAsync(_env.WebRootPath, "assets", "images", "staff");


            Staff staff = new Staff()
            {
                Name = staffVM.Name.Capitalize(),
                Surname = staffVM.Surname.Capitalize(),
                Gender = staffVM.Gender.Value,
                DesignationId = staffVM.DesignationId.Value,
                Image = image,
            };


            await _context.Staffs.AddAsync(staff);
            await _context.SaveChangesAsync();


            var contactInfos = new List<(ContactType, string?)>
            {
                (ContactType.Email, staffVM.Email),
                (ContactType.Phone, staffVM.PhoneNumber),
            };

            Helpers.AddContactInfos(_context, OwnerType.Staff, staff.Id, contactInfos);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(List));
        }


        public async Task<IActionResult> Update(int? id)
        {

            if (id is null || id <= 0) return BadRequest();

            Staff? staff = await _context.Staffs.Include(s => s.Designation).FirstOrDefaultAsync(s => s.Id == id);
            if (staff is null) return NotFound();

            List<ContactInfo>? contactInfos = await _context.ContactInfos
                        .Where(c => c.OwnerType == OwnerType.Staff && c.OwnerId == staff.Id)
                        .ToListAsync();

            UpdateStaffVM staffVM = new UpdateStaffVM
            {
                Name = staff.Name.Capitalize(),
                Surname = staff.Surname.Capitalize(),
                Gender = staff.Gender,
                Email = contactInfos.FirstOrDefault(e => e.ContactType == ContactType.Email)?.Value,
                PhoneNumber = contactInfos.FirstOrDefault(x => x.ContactType == ContactType.Phone)?.Value,
                DesignationId = staff.DesignationId,
                Designations = await _context.Designations.ToListAsync(),
                Image = staff.Image,
            };

            return View(staffVM);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int? id, UpdateStaffVM staffVM)
        {
            staffVM.Designations = await _context.Designations.ToListAsync();

            if (!Helpers.HasDigit(staffVM.Name))
            {
                ModelState.AddModelError(nameof(staffVM.Name), "Name cannot contain digits");
            }

            if (!Helpers.HasDigit(staffVM.Surname))
            {
                ModelState.AddModelError(nameof(staffVM.Surname), "Surname cannot contain digits");
            }


            if (!ModelState.IsValid)
            {
                return View(staffVM);
            }

            Staff? existedStaff = await _context.Staffs
                .Include(d => d.Designation)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (existedStaff is null) return NotFound();


            if (staffVM.MainPhoto is not null)
            {
                if (!staffVM.MainPhoto.ValidateType("image/"))
                {
                    ModelState.AddModelError(nameof(UpdateStaffVM.MainPhoto), "File type is incorrect!");
                    return View(staffVM);
                }

                if (!staffVM.MainPhoto.ValidateSize(FileType.MB, 1))
                {
                    ModelState.AddModelError(nameof(UpdateStaffVM.MainPhoto), "File should be less than 1MB");
                    return View(staffVM);
                }
                string newImage = await staffVM.MainPhoto.CreateFileAsync(_env.WebRootPath, "assets", "images", "staff");
                existedStaff.Image.DeleteFile(_env.WebRootPath, "assets", "images", "staff");
                existedStaff.Image = newImage;
            }
            bool result = staffVM.Designations.Any(c => c.Id == staffVM.DesignationId);

            if (!result)
            {
                ModelState.AddModelError(nameof(staffVM.DesignationId), "Designation does not exist");
                return View(staffVM);
            }

            existedStaff.Name = staffVM.Name.Capitalize();
            existedStaff.Surname = staffVM.Surname.Capitalize();
            existedStaff.Gender = staffVM.Gender.Value;
            existedStaff.DesignationId = staffVM.DesignationId.Value;


            List<ContactInfo>? contactInfos = await _context.ContactInfos
                        .Where(c => c.OwnerType == OwnerType.Staff && c.OwnerId == existedStaff.Id)
                        .ToListAsync();


            var contactValues = new List<(ContactType Type, string? Value)>
            {
                (ContactType.Email, staffVM.Email),
                (ContactType.Phone, staffVM.PhoneNumber),
            };

            foreach (var (type, value) in contactValues)
            {
                ContactInfo? existing = contactInfos.FirstOrDefault(c => c.ContactType == type);

                if (!string.IsNullOrWhiteSpace(value))
                {
                    if (existing != null)
                    {
                        existing.Value = value;
                    }
                    else
                    {
                        _context.ContactInfos.Add(new ContactInfo
                        {
                            ContactType = type,
                            Value = value,
                            OwnerType = OwnerType.Doctor,
                            OwnerId = existedStaff.Id
                        });
                    }
                }
                else if (existing != null)
                {
                    _context.ContactInfos.Remove(existing);
                }
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(List));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null || id <= 0) return BadRequest();

            Staff? staff = await _context.Staffs
                .Include(d => d.Designation)
                .FirstOrDefaultAsync(d => d.Id == id);
            if (staff is null) return NotFound();
            staff.Image.DeleteFile(_env.WebRootPath, "assets", "images", "staff");


            List<ContactInfo> contactInfos = await _context.ContactInfos
                    .Where(ci => ci.OwnerType == OwnerType.Staff && ci.OwnerId == staff.Id)
                    .ToListAsync();

            _context.ContactInfos.RemoveRange(contactInfos);


            _context.Staffs.Remove(staff);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(List));

        }

        [HttpPost]
        public async Task<IActionResult> DeleteSelected(List<int> selectedIds)
        {

            if (selectedIds == null || !selectedIds.Any())
            {
                TempData["Warning"] = "Please select at least one staff member to delete.";
                return RedirectToAction(nameof(List));
            }



            List<Staff> staffs = await _context.Staffs
                .Where(s => selectedIds.Contains(s.Id))
                .Include(s => s.Designation)
                .ToListAsync();

            List<ContactInfo> contactInfos = await _context.ContactInfos
                .Where(ci => ci.OwnerType == OwnerType.Staff && selectedIds.Contains(ci.OwnerId))
                .ToListAsync();

            foreach (var staff in staffs)
            {
                staff.Image.DeleteFile(_env.WebRootPath, "assets", "images", "staff");
            }

            _context.ContactInfos.RemoveRange(contactInfos);
            _context.Staffs.RemoveRange(staffs);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(List));
        }



    }
}
