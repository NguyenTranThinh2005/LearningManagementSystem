namespace PRN232.LAB1.SE193112.Models.Requests
{
    public class CreateSubjectRequest
    {
        public string SubjectCode { get; set; } = null!;
        public string SubjectName { get; set; } = null!;
        public int Credit { get; set; }
    }

    public class UpdateSubjectRequest
    {
        public int SubjectId { get; set; }
        public string SubjectCode { get; set; } = null!;
        public string SubjectName { get; set; } = null!;
        public int Credit { get; set; }
    }
}
