using CourseManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CourseManagementSystem.Repository.MyCartService
{
    public class MyCartRepository: IMyCartRepository
    {
        private readonly CourseQuizDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public MyCartRepository(CourseQuizDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IEnumerable<MyCart>> GetMyCartsAsync()
        {
            return await _context.MyCarts.ToListAsync();
        }

        public async Task<MyCart> GetMyCartByIdAsync(int id)
        {
            return await _context.MyCarts.FindAsync(id);
        }

        public async Task<MyCart> UpdateMyCartAsync(int id, MyCart myCart)
        {
            if (id != myCart.cartId)
            {
                return null;
            }

            _context.Entry(myCart).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MyCartExists(id))
                {
                    return null;
                }
                else
                {
                    throw;
                }
            }

            return myCart;
        }

        public async Task<MyCart> CreateMyCartAsync([FromForm] MyCart myCart, IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
            {
                throw new ArgumentException("Invalid file");
            }

            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
            var filePath = Path.Combine(uploadsFolder, fileName);
            try
            {
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                myCart.ProductImage = fileName;

                _context.MyCarts.Add(myCart);
                _context.SaveChanges();

                return myCart;
            }
            catch (Exception ex)
            {
                // Rethrow the exception with additional information
                throw new Exception("Error occurred while posting the room.", ex);
            }
        }

        public async Task<MyCart> DeleteMyCartAsync(int id)
        {
            var myCart = await _context.MyCarts.FindAsync(id);
            if (myCart == null)
            {
                return null;
            }

            _context.MyCarts.Remove(myCart);
            await _context.SaveChangesAsync();

            return myCart;
        }

        private bool MyCartExists(int id)
        {
            return (_context.MyCarts?.Any(e => e.cartId == id)).GetValueOrDefault();
        }
    }
}
