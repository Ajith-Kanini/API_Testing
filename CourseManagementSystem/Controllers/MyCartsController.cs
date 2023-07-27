using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CourseManagementSystem.Models;
using CourseManagementSystem.Repository.MyCartService;
using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace CourseManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MyCartsController : ControllerBase
    {
        private readonly IMyCartRepository _myCartRepository;

        public MyCartsController(IMyCartRepository myCartRepository)
        {
            _myCartRepository = myCartRepository;
        }

        // GET: api/MyCarts
        [Authorize(Roles = "User,Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MyCart>>> GetMyCarts()
        {
            var myCarts = await _myCartRepository.GetMyCartsAsync();

            if (myCarts == null)
            {
                return NotFound();
            }

            return Ok(myCarts);
        }

        // GET: api/MyCarts/5
        [Authorize(Roles = "User,Admin")]
        [HttpGet("{id}")]
        public async Task<ActionResult<MyCart>> GetMyCart(int id)
        {
            var myCart = await _myCartRepository.GetMyCartByIdAsync(id);

            if (myCart == null)
            {
                return NotFound();
            }

            return Ok(myCart);
        }

        // PUT: api/MyCarts/5
        [Authorize(Roles = "User,Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMyCart(int id, MyCart myCart)
        {
            if (id != myCart.cartId)
            {
                return BadRequest();
            }

            var updatedMyCart = await _myCartRepository.UpdateMyCartAsync(id, myCart);

            if (updatedMyCart == null)
            {
                return NotFound();
            }

            return NoContent();
        }

        // POST: api/MyCarts
        [Authorize(Roles = "User,Admin")]
        [HttpPost]
        public async Task<ActionResult<MyCart>> PostMyCart([FromForm] MyCart myCart, IFormFile imageFile)
        {
            var createdMyCart = await _myCartRepository.CreateMyCartAsync(myCart,imageFile);

            return CreatedAtAction("GetMyCart", new { id = createdMyCart.cartId }, createdMyCart);
        }

        // DELETE: api/MyCarts/5
        [Authorize(Roles = "User,Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMyCart(int id)
        {
            var deletedMyCart = await _myCartRepository.DeleteMyCartAsync(id);

            if (deletedMyCart == null)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
