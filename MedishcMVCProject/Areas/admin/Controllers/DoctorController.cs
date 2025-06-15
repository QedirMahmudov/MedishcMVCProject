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
    public class DoctorController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public DoctorController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public IActionResult List(string doctorName = null, string department = null)
        {
            List<GetDoctorVM> doctors = _context.Doctors
         .Include(x => x.WorkingHours)
         .Include(x => x.Specialist)
         .Select(d => new GetDoctorVM
         {
             Id = d.Id,
             Name = d.Name,
             Surname = d.Surname,
             SpecialistName = d.Specialist.Name,
             Image = d.Image,
             WorkingHours = d.WorkingHours.Select(wh => new WorkingHourVM
             {
                 DayOfWeek = wh.DayOfWeek,
                 OpenTime = wh.OpenTime,
                 CloseTime = wh.CloseTime
             }).ToList()
         }).ToList();
            doctors = Helpers.FilterByText(doctors, d => d.Name + " " + d.Surname, doctorName);
            doctors = Helpers.FilterByText(doctors, d => d.SpecialistName, department);
            return View(doctors);
        }
        public IActionResult Cards(string doctorName = null, string department = null)
        {
            List<GetDoctorVM> doctors = _context.Doctors
                .Include(x => x.Specialist)
                .Select(d => new GetDoctorVM
                {
                    Id = d.Id,
                    Name = d.Name,
                    Surname = d.Surname,
                    SpecialistName = d.Specialist.Name,
                    Image = d.Image,
                }).ToList();

            doctors = Helpers.FilterByText(doctors, d => d.Name + " " + d.Surname, doctorName);
            doctors = Helpers.FilterByText(doctors, d => d.SpecialistName, department);

            return View(doctors);
        }
        //ajax real search time...

        //public IActionResult FilterDoctors(string doctorName = null, string department = null)
        //{
        //    List<GetDoctorVM> doctors = _context.Doctors
        //        .Include(x => x.Specialist)
        //        .Select(d => new GetDoctorVM
        //        {
        //            Id = d.Id,
        //            Name = d.Name,
        //            Surname = d.Surname,
        //            SpecialistName = d.Specialist.Name,
        //            Image = d.Image,
        //        }).ToList();

        //    doctors = Helpers.FilterByText(doctors, d => d.Name + " " + d.Surname, doctorName);
        //    doctors = Helpers.FilterByText(doctors, d => d.SpecialistName, department);

        //    return PartialView("FilterDoctors", doctors); // yalnız kartları qaytarır
        //}

        public async Task<IActionResult> Profile(int? id)
        {
            if (id is null || id <= 0) return BadRequest();

            Doctor? doctor = await _context.Doctors
                .Include(d => d.Specialist)
                .Include(d => d.Degree)
                .Include(d => d.PriceLists)
                .Include(d => d.WorkingHours)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (doctor is null) return NotFound();
            GetDoctorVM vm = new GetDoctorVM()
            {
                Image = doctor.Image,
                Name = doctor.Name,
                Surname = doctor.Surname,
                Review = doctor.ReviewCount,
                MainDescription = doctor.MainDescription,
                DegreeName = doctor.Degree?.Name,
                SpecialistName = doctor.Specialist?.Name,
                WorkingHours = doctor.WorkingHours.Select(oh => new WorkingHourVM
                {
                    DayOfWeek = oh.DayOfWeek,
                    OpenTime = oh.OpenTime,
                    CloseTime = oh.CloseTime
                }).ToList(),
                PriceLists = doctor.PriceLists.ToList()
            };

            return View(vm);
        }
        //CRUD


        public async Task<IActionResult> Create()
        {
            CreateDoctorVM doctorVM = new CreateDoctorVM()
            {
                Specialists = await _context.Specialists.ToListAsync()
            };

            return View(doctorVM);
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateDoctorVM doctorVM)
        {
            doctorVM.Specialists = await _context.Specialists.ToListAsync();

            if (!Helpers.HasDigit(doctorVM.Name))
            {
                ModelState.AddModelError(nameof(doctorVM.Name), "Name cannot contain digits");
            }

            if (!Helpers.HasDigit(doctorVM.Surname))
            {
                ModelState.AddModelError(nameof(doctorVM.Surname), "Surname cannot contain digits");
            }

            //**********BASLAMA VE BITME SAATININ 1 i DOLANDA OBRIDE DOLMALIDI AMMA SPANDA CIXMIR ERROR!*************

            if (doctorVM.WorkingHours != null)
            {
                foreach (WorkingHourVM? item in doctorVM.WorkingHours)
                {
                    bool hasOpen = item.OpenTime.HasValue;
                    bool hasClose = item.CloseTime.HasValue;

                    if (hasOpen != hasClose)
                    {
                        ModelState.AddModelError(
                            string.Empty,
                            $"{item.DayOfWeek} start or end time are missing!"
                        );
                    }
                }
            }






            if (!ModelState.IsValid)
            {
                return View(doctorVM);
            }

            bool result = doctorVM.Specialists.Any(s => s.Id == doctorVM.SpecialistId);

            if (!result)
            {
                ModelState.AddModelError(nameof(doctorVM.SpecialistId), "Specialist does not exist");
                return View(doctorVM);
            }



            if (!doctorVM.MainPhoto.ValidateType("image/"))
            {
                ModelState.AddModelError(nameof(CreateDoctorVM.MainPhoto), "file type is incorrect");
                return View(doctorVM);
            }

            if (!doctorVM.MainPhoto.ValidateSize(FileType.MB, 1))
            {
                ModelState.AddModelError(nameof(CreateDoctorVM.MainPhoto), "file must be less than 500kb");
                return View(doctorVM);
            }

            string image = await doctorVM.MainPhoto.CreateFileAsync(_env.WebRootPath, "assets", "images", "team", "full");


            Doctor doctor = new Doctor()
            {
                Name = doctorVM.Name.Capitalize(),
                Surname = doctorVM.Surname.Capitalize(),
                Age = doctorVM.Age,
                Gender = doctorVM.Gender,
                SpecialistId = doctorVM.SpecialistId.Value,
                Image = image,
                AdditionalDescription = doctorVM.AdditionalDescription,
                MainDescription = doctorVM.MainDescription,
                ReviewCount = doctorVM.ReviewCount,
                ZodocRating = doctorVM.ZodocRating,

                WorkingHours = Enum.GetValues(typeof(DayOfWeekEnum))
                                    .Cast<DayOfWeekEnum>()
                                    .Select(day =>
                                    {
                                        WorkingHourVM input = doctorVM.WorkingHours?.FirstOrDefault(x => x.DayOfWeek == day);
                                        return new WorkingHours
                                        {
                                            DayOfWeek = day,
                                            OpenTime = input?.OpenTime,
                                            CloseTime = input?.CloseTime
                                        };
                                    }).ToList(),

            };




            await _context.Doctors.AddAsync(doctor);
            await _context.SaveChangesAsync();


            var contactInfos = new List<(ContactType, string?)>
            {
                (ContactType.Email, doctorVM.Email),
                (ContactType.Phone, doctorVM.PhoneNumber),
                (ContactType.Facebook, doctorVM.SocialMediaFacebook),
                (ContactType.Twitter, doctorVM.SocialMediaTwitter)
            };

            Helpers.AddContactInfos(_context, OwnerType.Doctor, doctor.Id, contactInfos);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(List));
        }



        public async Task<IActionResult> Update(int? id)
        {

            if (id is null || id <= 0) return BadRequest();

            Doctor? doctor = await _context.Doctors.Include(d => d.Specialist).Include(d => d.WorkingHours).FirstOrDefaultAsync(d => d.Id == id);
            if (doctor is null) return NotFound();

            List<ContactInfo>? contactInfos = await _context.ContactInfos
                        .Where(c => c.OwnerType == OwnerType.Doctor && c.OwnerId == doctor.Id)
                        .ToListAsync();

            UpdateDoctorVM doctorVM = new UpdateDoctorVM
            {
                Name = doctor.Name.Capitalize(),
                Surname = doctor.Surname.Capitalize(),
                Age = doctor.Age,
                Gender = doctor.Gender,
                Email = contactInfos.FirstOrDefault(e => e.ContactType == ContactType.Email)?.Value,
                PhoneNumber = contactInfos.FirstOrDefault(x => x.ContactType == ContactType.Phone)?.Value,
                SocialMediaFacebook = contactInfos.FirstOrDefault(x => x.ContactType == ContactType.Facebook)?.Value,
                SocialMediaTwitter = contactInfos.FirstOrDefault(x => x.ContactType == ContactType.Twitter)?.Value,
                SpecialistId = doctor.SpecialistId,
                Specialists = await _context.Specialists.ToListAsync(),
                Image = doctor.Image,

                AdditionalDescription = doctor.AdditionalDescription,
                MainDescription = doctor.MainDescription,
                ReviewCount = doctor.ReviewCount,

                ZodocRating = doctor.ZodocRating,
                WorkingHours = Enum.GetValues(typeof(DayOfWeekEnum))
                                .Cast<DayOfWeekEnum>()
                                .Select(day =>
                                {
                                    WorkingHours? existing = doctor.WorkingHours.FirstOrDefault(x => x.DayOfWeek == day);
                                    return new WorkingHourVM
                                    {
                                        DayOfWeek = day,
                                        OpenTime = existing?.OpenTime,
                                        CloseTime = existing?.CloseTime
                                    };
                                }).ToList()
            };

            return View(doctorVM);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int? id, UpdateDoctorVM doctorVM)
        {
            doctorVM.Specialists = await _context.Specialists.ToListAsync();

            if (!Helpers.HasDigit(doctorVM.Name))
            {
                ModelState.AddModelError(nameof(doctorVM.Name), "Name cannot contain digits");
            }

            if (!Helpers.HasDigit(doctorVM.Surname))
            {
                ModelState.AddModelError(nameof(doctorVM.Surname), "Surname cannot contain digits");
            }


            if (!ModelState.IsValid)
            {
                return View(doctorVM);
            }

            Doctor? existedDoctor = await _context.Doctors
                .Include(d => d.Specialist)
                .Include(d => d.WorkingHours)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (existedDoctor is null) return NotFound();


            if (doctorVM.MainPhoto is not null)
            {
                if (!doctorVM.MainPhoto.ValidateType("image/"))
                {
                    ModelState.AddModelError(nameof(UpdateDoctorVM.MainPhoto), "File type is incorrect!");
                    return View(doctorVM);
                }

                if (!doctorVM.MainPhoto.ValidateSize(FileType.MB, 1))
                {
                    ModelState.AddModelError(nameof(UpdateDoctorVM.MainPhoto), "File should be less than 1MB");
                    return View(doctorVM);
                }
                string newImage = await doctorVM.MainPhoto.CreateFileAsync(_env.WebRootPath, "assets", "images", "team", "full");
                existedDoctor.Image.DeleteFile(_env.WebRootPath, "assets", "images", "team", "full");
                existedDoctor.Image = newImage;
            }
            bool result = doctorVM.Specialists.Any(c => c.Id == doctorVM.SpecialistId);

            if (!result)
            {
                ModelState.AddModelError(nameof(doctorVM.SpecialistId), "Specialist does not exist");
                return View(doctorVM);
            }

            existedDoctor.Name = doctorVM.Name.Capitalize();
            existedDoctor.Surname = doctorVM.Surname.Capitalize();
            existedDoctor.Age = doctorVM.Age;
            existedDoctor.Gender = doctorVM.Gender;
            existedDoctor.SpecialistId = doctorVM.SpecialistId.Value;
            existedDoctor.AdditionalDescription = doctorVM.AdditionalDescription;
            existedDoctor.MainDescription = doctorVM.MainDescription;
            existedDoctor.ReviewCount = doctorVM.ReviewCount;
            existedDoctor.ZodocRating = doctorVM.ZodocRating;

            if (doctorVM.WorkingHours is not null && doctorVM.WorkingHours.Any())
            {
                _context.WorkingHours.RemoveRange(existedDoctor.WorkingHours);

                existedDoctor.WorkingHours = Enum.GetValues(typeof(DayOfWeekEnum))
                    .Cast<DayOfWeekEnum>()
                    .Select(day =>
                    {
                        WorkingHourVM input = doctorVM.WorkingHours.FirstOrDefault(x => x.DayOfWeek == day);
                        return new WorkingHours
                        {
                            DayOfWeek = day,
                            OpenTime = input?.OpenTime,
                            CloseTime = input?.CloseTime,
                            DoctorId = existedDoctor.Id
                        };
                    }).ToList();
            }


            List<ContactInfo>? contactInfos = await _context.ContactInfos
                        .Where(c => c.OwnerType == OwnerType.Doctor && c.OwnerId == existedDoctor.Id)
                        .ToListAsync();


            var contactValues = new List<(ContactType Type, string? Value)>
            {
                (ContactType.Email, doctorVM.Email),
                (ContactType.Phone, doctorVM.PhoneNumber),
                (ContactType.Facebook, doctorVM.SocialMediaFacebook),
                (ContactType.Twitter, doctorVM.SocialMediaTwitter)
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
                            OwnerId = existedDoctor.Id
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

            Doctor? doctor = await _context.Doctors
                .Include(d => d.Specialist)
                .Include(d => d.WorkingHours)
                .FirstOrDefaultAsync(d => d.Id == id);
            if (doctor is null) return NotFound();
            doctor.Image.DeleteFile(_env.WebRootPath, "assets", "images", "team", "full");


            List<ContactInfo> contactInfos = await _context.ContactInfos
                    .Where(ci => ci.OwnerType == OwnerType.Doctor && ci.OwnerId == doctor.Id)
                    .ToListAsync();

            _context.ContactInfos.RemoveRange(contactInfos);


            _context.Doctors.Remove(doctor);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(List));

        }
        [HttpPost]
        public async Task<IActionResult> DeleteSelected(List<int> selectedIds)
        {
            if (selectedIds == null || !selectedIds.Any())
            {
                TempData["Warning"] = "Please select at least one doctor to delete.";
                return RedirectToAction(nameof(List));
            }

            List<Doctor> doctors = await _context.Doctors
                .Where(d => selectedIds.Contains(d.Id))
                .Include(d => d.Specialist)
                .Include(d => d.WorkingHours)
                .ToListAsync();

            List<ContactInfo> contactInfos = await _context.ContactInfos
                .Where(ci => ci.OwnerType == OwnerType.Doctor && selectedIds.Contains(ci.OwnerId))
                .ToListAsync();

            foreach (var doctor in doctors)
            {
                doctor.Image.DeleteFile(_env.WebRootPath, "assets", "images", "team", "full");
            }

            _context.ContactInfos.RemoveRange(contactInfos);
            _context.Doctors.RemoveRange(doctors);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Selected doctors deleted successfully.";
            return RedirectToAction(nameof(List));
        }
    }
}
