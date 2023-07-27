using System.ComponentModel.DataAnnotations;

namespace CourseManagementSystem.Models
{
    public class MyCart
    {
        [Key]
        public int cartId { get; set; }
        public string? ProductName { get; set; }
        public string? ProductImage { get; set; }
        public int? ProductQuantity { get; set; }
        public decimal ProductPrice { get; set; }

        public ICollection<Course>? Courses { get; set; }
    }
}
