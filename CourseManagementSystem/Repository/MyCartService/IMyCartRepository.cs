using CourseManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;

namespace CourseManagementSystem.Repository.MyCartService
{
    public interface IMyCartRepository
    {
        Task<IEnumerable<MyCart>> GetMyCartsAsync();
        Task<MyCart> GetMyCartByIdAsync(int id);
        Task<MyCart> UpdateMyCartAsync(int id, MyCart myCart);
        Task<MyCart> CreateMyCartAsync([FromForm] MyCart myCart, IFormFile imageFile);
        Task<MyCart> DeleteMyCartAsync(int id);
    }
}
