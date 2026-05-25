namespace PRN232.LAB1.SE193112.Models.Requests
{
    public class CreateEnrollmentRequest
    {
        public int StudentId { get; set; }
        public int CourseId { get; set; }
        public DateTime EnrollDate { get; set; }
        public string Status { get; set; } = "Active";
    }

    public class UpdateEnrollmentRequest
    {
        public int EnrollmentId { get; set; }
        public int StudentId { get; set; }
        public int CourseId { get; set; }
        public DateTime EnrollDate { get; set; }
        public string Status { get; set; } = null!;
    }
}
