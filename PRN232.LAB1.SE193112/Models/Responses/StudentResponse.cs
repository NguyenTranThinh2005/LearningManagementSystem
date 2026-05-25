namespace PRN232.LAB1.SE193112.Models.Responses
{
    public class StudentResponse
    {
        public int StudentId { get; set; }
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public DateTime DateOfBirth { get; set; }
    }

    public class StudentDetailResponse
    {
        public int StudentId { get; set; }
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public DateTime DateOfBirth { get; set; }
        public List<EnrollmentSummaryResponse> Enrollments { get; set; } = new();
    }
}
