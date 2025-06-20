namespace MedishcMVCProject.ViewModels
{
    public class GetAppointmentVM
    {
        public int Id { get; set; }

        public int PatientId { get; set; }
        public string PatientName { get; set; }
        public string PatientImage { get; set; }

        public int DoctorId { get; set; }
        public string DoctorName { get; set; }
        public string DoctorImage { get; set; }
        public string Department { get; set; }

        public DateTime Date { get; set; }
        public string Time { get; set; }
        public string? Disease { get; set; }
    }
}
