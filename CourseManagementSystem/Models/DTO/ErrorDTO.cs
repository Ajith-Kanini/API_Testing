namespace CourseManagementSystem.Models.DTO
{
    public class ErrorDTO
    {
        public int ErrorId { get; set; }
        public int ErrorStatusCode { get; set; } = 0;
        public string ErrorMessage { get; set; }=string.Empty; 
    }
}
