namespace PRN232.LAB1.SE193112.Models.Requests
{
    public class CreateCourseRequest
    {
        public string CourseName { get; set; } = null!;
        public int SemesterId { get; set; }
    }

    public class UpdateCourseRequest
    {
        public int CourseId { get; set; }
        public string CourseName { get; set; } = null!;
        public int SemesterId { get; set; }
    }
}
