namespace PRN232.LAB1.SE193112.Models.Responses
{
    public class CourseResponse
    {
        public int CourseId { get; set; }
        public string CourseName { get; set; } = null!;
        public int SemesterId { get; set; }
    }

    public class CourseDetailResponse
    {
        public int CourseId { get; set; }
        public string CourseName { get; set; } = null!;
        public int SemesterId { get; set; }
        public SemesterResponse Semester { get; set; } = null!;
        public List<EnrollmentSummaryResponse> Enrollments { get; set; } = new();
    }
}
