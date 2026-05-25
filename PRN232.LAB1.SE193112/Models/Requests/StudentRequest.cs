namespace PRN232.LAB1.SE193112.Models.Requests
{
    public class CreateStudentRequest
    {
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public DateTime DateOfBirth { get; set; }
    }

    public class UpdateStudentRequest
    {
        public int StudentId { get; set; }
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public DateTime DateOfBirth { get; set; }
    }
}
