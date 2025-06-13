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
    public class PatientController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public PatientController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public IActionResult List(string patientName = null)
        {
            List<GetPatientVM> patients = _context.Patients
         .Include(x => x.BloodGroup)
         .Include(x => x.Disease)
         .Select(p => new GetPatientVM
         {
             Id = p.Id,
             Name = p.Name,
             Surname = p.Surname,
             Image = p.Image,
             Age = p.Age,
             MainDescription = p.MainDescription,
             BloodName = p.BloodGroup.Name,
             DiseaseName = p.Disease.Name,
             GenderName = p.Gender.ToString()
         }).ToList();

            List<ContactInfo>? contactInfos = _context.ContactInfos
                                        .Where(c => c.OwnerType == OwnerType.Patient &&
                                                   (c.ContactType == ContactType.Email || c.ContactType == ContactType.Phone))
                                        .ToList();

            foreach (GetPatientVM patient in patients)
            {
                var contacts = contactInfos.Where(c => c.OwnerId == patient.Id);

                ContactInfo? emailContact = contacts.FirstOrDefault(c => c.ContactType == ContactType.Email);
                ContactInfo? phoneContact = contacts.FirstOrDefault(c => c.ContactType == ContactType.Phone);

                patient.Email = emailContact?.Value;

                //                             birnece neticeni eyni anda qaytarmaqa yarayir OUT.
                if (int.TryParse(phoneContact?.Value, out int phone))
                {
                    patient.PhoneNumber = phone;
                }
                else
                {
                    patient.PhoneNumber = 0;
                }
            }



            patients = Helpers.FilterByText(patients, p => p.Name + " " + p.Surname, patientName);
            return View(patients);
        }


        public async Task<IActionResult> Profile(int? id)
        {
            if (id is null || id <= 0) return BadRequest();

            Patient? patient = await _context.Patients
                .Include(p => p.BloodGroup)
                .Include(p => p.Disease)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (patient is null) return NotFound();

            List<ContactInfo> contactInfos = await _context.ContactInfos
                .Where(c => c.OwnerType == OwnerType.Patient && c.OwnerId == patient.Id)
                .ToListAsync();

            ContactInfo? emailContact = contactInfos.FirstOrDefault(c => c.ContactType == ContactType.Email);
            ContactInfo? phoneContact = contactInfos.FirstOrDefault(c => c.ContactType == ContactType.Phone);

            List<PatientReport>? reports = await _context.PatientReports
                .Where(r => r.PatientId == patient.Id)
                .ToListAsync();

            GetPatientVM vm = new GetPatientVM()
            {
                Image = patient.Image,
                Name = patient.Name,
                Surname = patient.Surname,
                MainDescription = patient.MainDescription,
                Age = patient.Age,
                BloodName = patient.BloodGroup.Name,
                DiseaseName = patient.Disease.Name,
                GenderName = patient.Gender.ToString(),
                Email = emailContact?.Value,
                PhoneNumber = int.TryParse(phoneContact?.Value, out int phone) ? phone : 0,
                Reports = reports,
            };

            return View(vm);
        }



        public async Task<IActionResult> Create()
        {
            CreatePatientVM patientVM = new CreatePatientVM()
            {
                BloodGroups = await _context.BloodGroups.ToListAsync(),
                Diseases = await _context.Diseases.ToListAsync()
            };

            return View(patientVM);
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreatePatientVM patientVM)
        {
            patientVM.Diseases = await _context.Diseases.ToListAsync();
            patientVM.BloodGroups = await _context.BloodGroups.ToListAsync();

            if (!Helpers.HasDigit(patientVM.Name))
            {
                ModelState.AddModelError(nameof(patientVM.Name), "Name cannot contain digits");
            }

            if (!Helpers.HasDigit(patientVM.Surname))
            {
                ModelState.AddModelError(nameof(patientVM.Surname), "Surname cannot contain digits");
            }

            if (!ModelState.IsValid)
            {
                return View(patientVM);
            }

            bool resultDisease = patientVM.Diseases.Any(d => d.Id == patientVM.DiseaseId);
            bool resultBloodGroup = patientVM.BloodGroups.Any(b => b.Id == patientVM.BloodGroupId);

            if (!resultDisease)
            {
                ModelState.AddModelError(nameof(patientVM.DiseaseId), "Disease does not exist");
                return View(patientVM);
            }
            if (!resultBloodGroup)
            {
                ModelState.AddModelError(nameof(patientVM.BloodGroupId), "Blood Group does not exist");
                return View(patientVM);
            }


            if (!patientVM.MainPhoto.ValidateType("image/"))
            {
                ModelState.AddModelError(nameof(CreatePatientVM.MainPhoto), "file type is incorrect");
                return View(patientVM);
            }

            if (!patientVM.MainPhoto.ValidateSize(FileType.KB, 500))
            {
                ModelState.AddModelError(nameof(CreatePatientVM.MainPhoto), "file must be less than 500kb");
                return View(patientVM);
            }


            if (!patientVM.ReportFile.ValidateType("application/pdf"))
            {
                ModelState.AddModelError(nameof(CreatePatientVM.MainPhoto), "file type is incorrect");
                return View(patientVM);
            }

            if (!patientVM.ReportFile.ValidateSize(FileType.MB, 2))
            {
                ModelState.AddModelError(nameof(CreatePatientVM.MainPhoto), "ReportFile must be less than 2MB");
                return View(patientVM);
            }






            string image = await patientVM.MainPhoto.CreateFileAsync(_env.WebRootPath, "assets", "images", "patient");
            string reportFileName = await patientVM.ReportFile.CreateFileAsync(_env.WebRootPath, "assets", "uploads", "patientreports");

            Patient patient = new Patient()
            {
                Name = patientVM.Name.Capitalize(),
                Surname = patientVM.Surname.Capitalize(),
                Age = patientVM.Age,
                Gender = patientVM.Gender.Value,
                Image = image,
                MainDescription = patientVM.MainDescription,
                BloodGroupId = patientVM.BloodGroupId.Value,
                DiseaseId = patientVM.DiseaseId.Value,
            };




            await _context.Patients.AddAsync(patient);
            await _context.SaveChangesAsync();


            PatientReport report = new PatientReport
            {
                PatientId = patient.Id,
                FileName = reportFileName,
                CreatedDate = DateTime.UtcNow
            };

            await _context.PatientReports.AddAsync(report);


            var contactInfos = new List<(ContactType, string?)>
            {
                (ContactType.Email, patientVM.Email),
                (ContactType.Phone, patientVM.PhoneNumber),
                (ContactType.Facebook, patientVM.SocialMediaFacebook),
                (ContactType.Twitter, patientVM.SocialMediaTwitter)
            };

            Helpers.AddContactInfos(_context, OwnerType.Patient, patient.Id, contactInfos);





            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(List));
        }



    }
}
