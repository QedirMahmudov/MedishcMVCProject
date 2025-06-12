using MedishcMVCProject.Models;
using Microsoft.EntityFrameworkCore;

namespace MedishcMVCProject.DAL
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        //Doctor
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Degree> Degrees { get; set; }
        public DbSet<OpeningHour> OpeningHours { get; set; }
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




        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<ContactInfo>()
               .Property(c => c.ContactType)
               .HasConversion<string>();

            modelBuilder.Entity<Doctor>()
                .Property(d => d.Gender)
                .HasConversion<string>();
        }
    }



}
