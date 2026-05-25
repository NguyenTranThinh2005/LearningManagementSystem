namespace PRN232.LMS.Services.Models
{
    public class SemesterBusinessModel
    {
        public int SemesterId { get; set; }
        public string SemesterName { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<int> CourseIds { get; set; } = new();
        public bool IsActive { get; set; }
    }
}
