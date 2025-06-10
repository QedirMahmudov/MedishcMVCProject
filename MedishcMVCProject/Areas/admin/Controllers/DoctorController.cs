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
        public IActionResult List()
        {
            var doctors = _context.Doctors
         .Include(x => x.OpeningHours)
         .Include(x => x.Specialist)
         .Select(d => new GetDoctorVM
         {
             Id = d.Id,
             Name = d.Name,
             Surname = d.Surname,
             SpecialistName = d.Specialist.Name,
             Image = d.Image,
             WorkingHours = d.OpeningHours.Select(wh => new WorkingHourVM
             {
                 DayOfWeek = wh.DayOfWeek,
                 OpenTime = wh.OpenTime,
                 CloseTime = wh.CloseTime
             }).ToList()
         }).ToList();

            return View(doctors);
        }
        public IActionResult Cards()
        {
            return View();
        }

        public IActionResult Profile()
        {
            return View();
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

            if (!ModelState.IsValid)
            {
                return View(doctorVM);
            }

            bool result = doctorVM.Specialists.Any(c => c.Id == doctorVM.SpecialistId);

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

            if (!doctorVM.MainPhoto.ValidateSize(FileSize.KB, 500))
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
                Email = doctorVM.Email,
                Phone = doctorVM.PhoneNumber,
                SpecialistId = doctorVM.SpecialistId.Value,
                Image = image,

                AdditionalDescription = doctorVM.AdditionalDescription,
                MainDescription = doctorVM.MainDescription,
                ReviewCount = doctorVM.ReviewCount,
                SocialMediaFacebook = doctorVM.SocialMediaFacebook,
                SocialMediaTwitter = doctorVM.SocialMediaTwitter,
                ZodocRating = doctorVM.ZodocRating,

                OpeningHours = Enum.GetValues(typeof(DayOfWeekEnum))
                                    .Cast<DayOfWeekEnum>()
                                    .Select(day =>
                                    {
                                        WorkingHourVM input = doctorVM.OpeningHours?.FirstOrDefault(x => x.DayOfWeek == day);
                                        return new OpeningHour
                                        {
                                            DayOfWeek = day,
                                            OpenTime = input?.OpenTime,
                                            CloseTime = input?.CloseTime
                                        };
                                    }).ToList()
            };

            await _context.Doctors.AddAsync(doctor);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(List));
        }



        public async Task<IActionResult> Update(int? id)
        {
            if (id is null || id <= 0) return BadRequest();

            Doctor? doctor = await _context.Doctors.Include(p => p.Specialist).FirstOrDefaultAsync(p => p.Id == id);
            if (doctor is null) return NotFound();

            UpdateDoctorVM doctorVM = new UpdateDoctorVM
            {
                Name = doctor.Name.Capitalize(),
                Surname = doctor.Surname.Capitalize(),
                Age = doctor.Age,
                Gender = doctor.Gender,
                Email = doctor.Email,
                PhoneNumber = doctor.Phone,
                SpecialistId = doctor.SpecialistId,
                Image = doctor.Image,

                AdditionalDescription = doctor.AdditionalDescription,
                MainDescription = doctor.MainDescription,
                ReviewCount = doctor.ReviewCount,
                SocialMediaFacebook = doctor.SocialMediaFacebook,
                SocialMediaTwitter = doctor.SocialMediaTwitter,
                ZodocRating = doctor.ZodocRating,

            };
            return View(doctorVM);
        }




    }
}
