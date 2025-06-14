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
                ModelState.AddModelError(nameof(CreatePatientVM.MainPhoto), "File type is incorrect");
                return View(patientVM);
            }

            if (!patientVM.MainPhoto.ValidateSize(FileType.MB, 1))
            {
                ModelState.AddModelError(nameof(CreatePatientVM.MainPhoto), "File must be less than 500kb");
                return View(patientVM);
            }


            if (patientVM.ReportFiles == null || !patientVM.ReportFiles.Any())
            {
                ModelState.AddModelError(nameof(patientVM.ReportFiles), "At least one PDF file must be uploaded");
                return View(patientVM);
            }

            foreach (IFormFile reportFile in patientVM.ReportFiles)
            {
                if (!reportFile.ValidateType("application/pdf"))
                {
                    ModelState.AddModelError(nameof(patientVM.ReportFiles), "File type is incorrect");
                    return View(patientVM);
                }

                if (!reportFile.ValidateSize(FileType.MB, 2))
                {
                    ModelState.AddModelError(nameof(patientVM.ReportFiles), "File must be less than 2MB");
                    return View(patientVM);
                }
            }





            string image = await patientVM.MainPhoto.CreateFileAsync(_env.WebRootPath, "assets", "images", "patient");

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


            foreach (IFormFile reportFile in patientVM.ReportFiles)
            {
                string reportFileName = await reportFile.CreateFileAsync(_env.WebRootPath, "assets", "uploads", "patientreports");

                PatientReport report = new PatientReport
                {
                    PatientId = patient.Id,
                    FileName = reportFileName,
                    CreatedDate = DateTime.UtcNow
                };

                await _context.PatientReports.AddAsync(report);
            }


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




        public async Task<IActionResult> Update(int? id)
        {
            if (id is null || id <= 0) return BadRequest();

            Patient? patient = await _context.Patients
                .Include(p => p.Disease)
                .Include(p => p.BloodGroup)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (patient is null) return NotFound();

            List<ContactInfo> contactInfos = await _context.ContactInfos
                .Where(c => c.OwnerType == OwnerType.Patient && c.OwnerId == patient.Id)
                .ToListAsync();

            List<PatientReport> patientReports = await _context.PatientReports
                .Where(r => r.PatientId == patient.Id)
                .ToListAsync();

            UpdatePatientVM patientVM = new UpdatePatientVM
            {
                Name = patient.Name.Capitalize(),
                Surname = patient.Surname.Capitalize(),
                Age = patient.Age,
                Gender = patient.Gender,
                Email = contactInfos.FirstOrDefault(e => e.ContactType == ContactType.Email)?.Value,
                PhoneNumber = contactInfos.FirstOrDefault(x => x.ContactType == ContactType.Phone)?.Value,
                SocialMediaFacebook = contactInfos.FirstOrDefault(x => x.ContactType == ContactType.Facebook)?.Value,
                SocialMediaTwitter = contactInfos.FirstOrDefault(x => x.ContactType == ContactType.Twitter)?.Value,
                BloodGroupId = patient.BloodGroupId,
                BloodGroups = await _context.BloodGroups.ToListAsync(),
                DiseaseId = patient.DiseaseId,
                Diseases = await _context.Diseases.ToListAsync(),
                Image = patient.Image,
                MainDescription = patient.MainDescription,

                ExistingReports = patientReports.Select(r => new PatientReportVM
                {
                    Id = r.Id,
                    FileName = r.FileName,
                    FileUrl = $"/assets/uploads/patientreports/{r.FileName}"
                }).ToList()
            };

            return View(patientVM);
        }
        [HttpPost]
        public async Task<IActionResult> Update(int? id, UpdatePatientVM patientVM)
        {
            patientVM.BloodGroups = await _context.BloodGroups.ToListAsync();
            patientVM.Diseases = await _context.Diseases.ToListAsync();

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

            Patient? existedPatient = await _context.Patients
                .Include(p => p.BloodGroup)
                .Include(p => p.Disease)
                .Include(p => p.Reports)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (existedPatient is null) return NotFound();




            if (patientVM.MainPhoto is not null)
            {
                if (!patientVM.MainPhoto.ValidateType("image/"))
                {
                    ModelState.AddModelError(nameof(UpdatePatientVM.MainPhoto), "File type is incorrect!");
                    return View(patientVM);
                }

                if (!patientVM.MainPhoto.ValidateSize(FileType.MB, 1))
                {
                    ModelState.AddModelError(nameof(UpdatePatientVM.MainPhoto), "File should be less than 1MB");
                    return View(patientVM);
                }
                string newImage = await patientVM.MainPhoto.CreateFileAsync(_env.WebRootPath, "assets", "images", "patient");
                existedPatient.Image.DeleteFile(_env.WebRootPath, "assets", "images", "patient");
                existedPatient.Image = newImage;
            }


            if ((patientVM.ReportFiles != null && patientVM.ReportFiles.Any() || (patientVM.RemovedReportIds != null && patientVM.RemovedReportIds.Any())
    ))
            {
                // 1. Silinəcək fayllar varsa sil
                if (patientVM.RemovedReportIds != null)
                {
                    foreach (var reportId in patientVM.RemovedReportIds)
                    {
                        var report = await _context.PatientReports.FindAsync(reportId);
                        if (report != null)
                        {
                            string filePath = Path.Combine(_env.WebRootPath, "assets", "uploads", "patientreports", report.FileName);
                            if (System.IO.File.Exists(filePath))
                                System.IO.File.Delete(filePath);

                            _context.PatientReports.Remove(report);
                        }
                    }
                }

                // 2. Yeni yüklənən fayllar varsa yoxla və əlavə et
                if (patientVM.ReportFiles != null)
                {
                    foreach (var reportFile in patientVM.ReportFiles)
                    {
                        if (!reportFile.ValidateType("application/pdf"))
                        {
                            ModelState.AddModelError(nameof(UpdatePatientVM.ReportFiles), "All report files must be PDF!");
                            return View(patientVM);
                        }
                        if (!reportFile.ValidateSize(FileType.MB, 2))
                        {
                            ModelState.AddModelError(nameof(UpdatePatientVM.ReportFiles), "File must be less than 2MB!");
                            return View(patientVM);
                        }
                    }

                    foreach (var reportFile in patientVM.ReportFiles)
                    {
                        string reportFileName = await reportFile.CreateFileAsync(_env.WebRootPath, "assets", "uploads", "patientreports");

                        PatientReport report = new PatientReport
                        {
                            PatientId = existedPatient.Id,
                            FileName = reportFileName,
                            CreatedDate = DateTime.UtcNow
                        };

                        await _context.PatientReports.AddAsync(report);
                    }
                }
            }

            bool resultBloodGroup = patientVM.BloodGroups.Any(p => p.Id == patientVM.BloodGroupId);
            bool resultDisease = patientVM.Diseases.Any(p => p.Id == patientVM.DiseaseId);

            if (!resultBloodGroup)
            {
                ModelState.AddModelError(nameof(patientVM.BloodGroupId), "BloodGroup does not exist");
                return View(patientVM);
            }

            if (!resultDisease)
            {
                ModelState.AddModelError(nameof(patientVM.DiseaseId), "Diseases does not exist");
                return View(patientVM);
            }

            existedPatient.Name = patientVM.Name.Capitalize();
            existedPatient.Surname = patientVM.Surname.Capitalize();
            existedPatient.Age = patientVM.Age;
            existedPatient.Gender = patientVM.Gender.Value;
            existedPatient.DiseaseId = patientVM.DiseaseId.Value;
            existedPatient.BloodGroupId = patientVM.BloodGroupId.Value;
            existedPatient.MainDescription = patientVM.MainDescription;

            List<ContactInfo>? contactInfos = await _context.ContactInfos
                        .Where(c => c.OwnerType == OwnerType.Patient && c.OwnerId == existedPatient.Id)
                        .ToListAsync();


            var contactValues = new List<(ContactType Type, string? Value)>
            {
                (ContactType.Email, patientVM.Email),
                (ContactType.Phone, patientVM.PhoneNumber),
                (ContactType.Facebook, patientVM.SocialMediaFacebook),
                (ContactType.Twitter, patientVM.SocialMediaTwitter)
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
                            OwnerId = existedPatient.Id
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

            Patient? patient = await _context.Patients
                .Include(p => p.Disease)
                .Include(p => p.BloodGroup)
                .FirstOrDefaultAsync(d => d.Id == id);
            if (patient is null) return NotFound();

            patient.Image.DeleteFile(_env.WebRootPath, "assets", "images", "patient");

            foreach (PatientReport? report in patient.Reports)
            {
                report.FileName.DeleteFile(_env.WebRootPath, "assets", "uploads", "patientreports");
            }


            List<ContactInfo> contactInfos = await _context.ContactInfos
                    .Where(ci => ci.OwnerType == OwnerType.Patient && ci.OwnerId == patient.Id)
                    .ToListAsync();

            _context.ContactInfos.RemoveRange(contactInfos);


            _context.Patients.Remove(patient);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(List));

        }


    }
}
