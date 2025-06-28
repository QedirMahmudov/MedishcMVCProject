using MedishcMVCProject.DAL;
using MedishcMVCProject.Models;
using MedishcMVCProject.Utilities;
using MedishcMVCProject.Utilities.Helpers;
using MedishcMVCProject.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MedishcMVCProject.Areas.admin.Controllers
{
    [Area("Admin")]
    public class SpecialistController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public SpecialistController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public IActionResult List(string specialistName = null)
        {
            List<ContactInfo>? contactInfos = _context.ContactInfos
                    .Where(ci => ci.OwnerType == OwnerType.Specialist)
                    .ToList();

            List<Specialist> specialistEntities = _context.Specialists
                .Include(s => s.Doctors)
                .ToList();

            List<GetSpecialistVM> specialists = specialistEntities.Select(s => new GetSpecialistVM
            {
                Id = s.Id,
                DepartmentName = s.Name,
                HeadDoctorName = s.HeadDoctor.Name,
                HeadDoctorSurname = s.HeadDoctor.Surname,
                Image = s.HeadDoctor.Image,
                DepartmentEmail = contactInfos
                    .FirstOrDefault(c => c.OwnerId == s.Id && c.ContactType == ContactType.Email)?.Value,
                DepartmentPhoneNumber = contactInfos
                    .FirstOrDefault(c => c.OwnerId == s.Id && c.ContactType == ContactType.Phone)?.Value
            }).ToList();

            if (!string.IsNullOrWhiteSpace(specialistName))
                specialists = Helpers.FilterByText(specialists, s => s.DepartmentName, specialistName);

            return View(specialists);
        }

        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateSpecialistVM vm)
        {
            if (string.IsNullOrWhiteSpace(vm.HeadDoctorFullName))
            {
                ModelState.AddModelError(nameof(vm.HeadDoctorFullName), "Full name is required.");
            }
            else
            {
                string trimmed = vm.HeadDoctorFullName.Trim();
                if (trimmed.Length < 5)
                    ModelState.AddModelError(nameof(vm.HeadDoctorFullName), "Full name must be at least 5 characters.");
                else if (trimmed.Length > 30)
                    ModelState.AddModelError(nameof(vm.HeadDoctorFullName), "Full name must be less than 30 characters.");

                var nameParts = trimmed.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (nameParts.Length < 2)
                {
                    ModelState.AddModelError(nameof(vm.HeadDoctorFullName), "Please enter both first and last name.");
                }
                else
                {
                    string name = nameParts[0];
                    string surname = string.Join(" ", nameParts.Skip(1));

                    if (!Helpers.HasDigit(name))
                        ModelState.AddModelError(nameof(vm.HeadDoctorFullName), "First name cannot contain digits.");
                    if (!Helpers.HasDigit(surname))
                        ModelState.AddModelError(nameof(vm.HeadDoctorFullName), "Last name cannot contain digits.");

                    if (ModelState.IsValid)
                    {
                        var headDoctor = await _context.Doctors
                            .FirstOrDefaultAsync(d =>
                                d.Name.ToLower() == name.ToLower() &&
                                d.Surname.ToLower() == surname.ToLower());

                        if (headDoctor is null)
                        {
                            ModelState.AddModelError(nameof(vm.HeadDoctorFullName), "Doctor not found with the given full name.");
                            return View(vm);
                        }

                        AppUser? appUser = await _userManager.FindByIdAsync(headDoctor.AppUserId);
                        if (appUser != null)
                        {
                            var oldRoles = await _userManager.GetRolesAsync(appUser);
                            if (oldRoles.Contains("Doctor"))
                                await _userManager.RemoveFromRoleAsync(appUser, "Doctor");

                            if (!oldRoles.Contains("HeadDoctor"))
                                await _userManager.AddToRoleAsync(appUser, "HeadDoctor");
                        }

                        Specialist specialist = new Specialist
                        {
                            Name = vm.DepartmentName,
                            HeadDoctorId = headDoctor.Id
                        };

                        await _context.Specialists.AddAsync(specialist);
                        await _context.SaveChangesAsync();

                        var contactInfos = new List<(ContactType, string?)>
                        {
                            (ContactType.Email, vm.DepartmentEmail),
                            (ContactType.Phone, vm.DepartmentPhoneNumber),
                        };

                        Helpers.AddContactInfos(_context, OwnerType.Specialist, specialist.Id, contactInfos);
                        await _context.SaveChangesAsync();

                        return RedirectToAction(nameof(List));
                    }
                }
            }

            return View(vm);
        }

        public async Task<IActionResult> Update(int? id)
        {
            if (id is null || id <= 0) return BadRequest();

            Specialist? specialist = await _context.Specialists
                .Include(s => s.HeadDoctor)
                .Include(s => s.Doctors)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (specialist is null) return NotFound();


            List<ContactInfo> contactInfos = await _context.ContactInfos
                   .Where(ci => ci.OwnerType == OwnerType.Specialist && ci.OwnerId == specialist.Id)
                   .ToListAsync();

            UpdateSpecialistVM vm = new UpdateSpecialistVM
            {
                DepartmentName = specialist.Name,
                HeadDoctorFullName = $"{specialist.HeadDoctor?.Name} {specialist.HeadDoctor?.Surname}",
                DepartmentEmail = contactInfos.FirstOrDefault(c => c.ContactType == ContactType.Email)?.Value,
                DepartmentPhoneNumber = contactInfos.FirstOrDefault(c => c.ContactType == ContactType.Phone)?.Value
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int? id, UpdateSpecialistVM specialistVM)
        {
            if (id is null || id <= 0) return BadRequest();

            Specialist? specialist = await _context.Specialists
                .Include(s => s.HeadDoctor)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (specialist is null) return NotFound();

            if (string.IsNullOrWhiteSpace(specialistVM.HeadDoctorFullName))
            {
                ModelState.AddModelError(nameof(UpdateSpecialistVM.HeadDoctorFullName), "Full name is required.");
            }
            else
            {
                var nameParts = specialistVM.HeadDoctorFullName.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);

                if (nameParts.Length < 2)
                {
                    ModelState.AddModelError(nameof(UpdateSpecialistVM.HeadDoctorFullName), "Please enter both name and surname.");
                }
                else
                {
                    string firstName = nameParts[0];
                    string surname = string.Join(' ', nameParts.Skip(1));

                    if (!Helpers.HasDigit(firstName))
                        ModelState.AddModelError(nameof(UpdateSpecialistVM.HeadDoctorFullName), "First name cannot contain digits.");

                    if (!Helpers.HasDigit(surname))
                        ModelState.AddModelError(nameof(UpdateSpecialistVM.HeadDoctorFullName), "Surname cannot contain digits.");
                }
            }

            if (!ModelState.IsValid)
                return View(specialistVM);

            var namePartsFinal = specialistVM.HeadDoctorFullName.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            string firstNameCap = namePartsFinal[0].Capitalize();
            string surnameCap = string.Join(" ", namePartsFinal.Skip(1)).Capitalize();

            Doctor? existingDoctor = await _context.Doctors
                .FirstOrDefaultAsync(d => d.Name == firstNameCap && d.Surname == surnameCap);

            if (existingDoctor is null)
            {
                ModelState.AddModelError(nameof(UpdateSpecialistVM.HeadDoctorFullName), "This doctor does not exist in the system.");
                return View(specialistVM);
            }

            AppUser? appUser = await _userManager.FindByIdAsync(existingDoctor.AppUserId);
            if (appUser != null)
            {
                var oldRoles = await _userManager.GetRolesAsync(appUser);
                if (oldRoles.Contains("Doctor"))
                    await _userManager.RemoveFromRoleAsync(appUser, "Doctor");

                if (!oldRoles.Contains("HeadDoctor"))
                    await _userManager.AddToRoleAsync(appUser, "HeadDoctor");
            }

            specialist.HeadDoctorId = existingDoctor.Id;
            specialist.Name = specialistVM.DepartmentName;

            List<ContactInfo> contactInfos = await _context.ContactInfos
                .Where(ci => ci.OwnerType == OwnerType.Specialist && ci.OwnerId == specialist.Id)
                .ToListAsync();

            var contactValues = new List<(ContactType, string?)>
            {
                (ContactType.Email, specialistVM.DepartmentEmail),
                (ContactType.Phone, specialistVM.DepartmentPhoneNumber),
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
                            OwnerType = OwnerType.Specialist,
                            OwnerId = specialist.Id
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

            Specialist? specialist = await _context.Specialists
                .Include(d => d.Doctors)
                .FirstOrDefaultAsync(d => d.Id == id);
            if (specialist is null) return NotFound();

            List<ContactInfo> contactInfos = await _context.ContactInfos
                    .Where(ci => ci.OwnerType == OwnerType.Specialist && ci.OwnerId == specialist.Id)
                    .ToListAsync();

            _context.ContactInfos.RemoveRange(contactInfos);


            _context.Specialists.Remove(specialist);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(List));

        }



        [HttpPost]
        public async Task<IActionResult> DeleteSelected(List<int> selectedIds)
        {

            if (selectedIds == null || !selectedIds.Any())
            {
                TempData["Warning"] = "Please select at least one specialist to delete.";
                return RedirectToAction(nameof(List));
            }

            List<Specialist> specialists = await _context.Specialists
                .Where(s => selectedIds.Contains(s.Id))
                .Include(s => s.Doctors)
                .ToListAsync();

            List<ContactInfo> contactInfos = await _context.ContactInfos
                .Where(ci => ci.OwnerType == OwnerType.Specialist && selectedIds.Contains(ci.OwnerId))
                .ToListAsync();

            _context.ContactInfos.RemoveRange(contactInfos);
            _context.Specialists.RemoveRange(specialists);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(List));
        }
    }
}
