namespace PRN232.LAB1.SE193112.Models.Responses
{
    public class EnrollmentResponse
    {
        public int EnrollmentId { get; set; }
        public int StudentId { get; set; }
        public int CourseId { get; set; }
        public DateTime EnrollDate { get; set; }
        public string Status { get; set; } = null!;
        public StudentResponse? Student { get; set; }
        public CourseResponse? Course { get; set; }
    }

    public class EnrollmentDetailResponse
    {
        public int EnrollmentId { get; set; }
        public DateTime EnrollDate { get; set; }
        public string Status { get; set; } = null!;
        public StudentResponse? Student { get; set; }
        public CourseResponse? Course { get; set; }
    }

    public class EnrollmentSummaryResponse
    {
        public int EnrollmentId { get; set; }
        public int StudentId { get; set; }
        public int CourseId { get; set; }
        public DateTime EnrollDate { get; set; }
        public string Status { get; set; } = null!;
    }
}
