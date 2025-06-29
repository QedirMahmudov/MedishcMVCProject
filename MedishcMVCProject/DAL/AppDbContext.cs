using MedishcMVCProject.Models;
using MedishcMVCProject.Models.Pharmacy;
using MedishcMVCProject.Utilities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MedishcMVCProject.DAL
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        //Doctor
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Degree> Degrees { get; set; }
        public DbSet<WorkingHours> WorkingHours { get; set; }
        public DbSet<PriceList> PriceLists { get; set; }
        public DbSet<Specialist> Specialists { get; set; }
        public DbSet<University> Universities { get; set; }
        //Clinics

        public DbSet<Clinic> Clinics { get; set; }
        public DbSet<ClinicService> ClinicServices { get; set; }
        //Blog

        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<BlogCategory> BlogCategories { get; set; }
        public DbSet<Author> Authors { get; set; }

        //MedicalService
        public DbSet<MedicalService> MedicalServices { get; set; }
        //ContactInfoProperties
        public DbSet<ContactInfo> ContactInfos { get; set; }
        //Patients
        public DbSet<Patient> Patients { get; set; }
        public DbSet<BloodGroup> BloodGroups { get; set; }
        public DbSet<Disease> Diseases { get; set; }
        public DbSet<PatientReport> PatientReports { get; set; }
        //Staff
        public DbSet<Staff> Staffs { get; set; }
        public DbSet<Designation> Designations { get; set; }
        //Appointment
        public DbSet<Appointment> Appointments { get; set; }
        //Pharmacy
        public DbSet<Product> Products { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Payment> Payments { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<ContactInfo>()
              .Property(c => c.ContactType)
              .HasConversion<string>();

            modelBuilder.Entity<Doctor>()
               .Property(d => d.Gender)
               .HasConversion<string>();

            modelBuilder.Entity<ContactInfo>()
               .Property(e => e.OwnerType)
               .HasConversion(
                   v => v.ToString(),
                   v => (OwnerType)Enum.Parse(typeof(OwnerType), v));

            modelBuilder.Entity<Doctor>()
                .HasOne(d => d.Specialist)
                .WithMany(s => s.Doctors)
                .HasForeignKey(d => d.SpecialistId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Specialist>()
                .HasOne(s => s.HeadDoctor)
                .WithMany()
                .HasForeignKey(s => s.HeadDoctorId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }



}
