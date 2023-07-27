using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using CourseManagementSystem.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CourseManagementSystem.Models;

namespace PasswordEncryption.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserRegistrationsController : ControllerBase
    {
        private readonly CourseQuizDbContext _context;

        public UserRegistrationsController(CourseQuizDbContext context)
        {
            _context = context;
        }

        // GET: api/UserRegistrations
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserRegistration>>> GetUserRegistrations()
        {
            if (_context.UserRegistrations == null)
            {
                return NotFound();
            }

            var userRegistrations = await _context.UserRegistrations.ToListAsync();

            // Decrypt the stored passwords before returning the user registrations
           /* foreach (var registration in userRegistrations)
            {
                registration.Password = Decrypt(registration.Password);
            }*/

            return userRegistrations;
        }

        // GET: api/UserRegistrations/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserRegistration>> GetUserRegistration(int id)
        {
            if (_context.UserRegistrations == null)
            {
                return NotFound();
            }

            var userRegistration = await _context.UserRegistrations.FindAsync(id);

            if (userRegistration == null)
            {
                return NotFound();
            }

            // Decrypt the stored password before returning the user registration
            userRegistration.Password = Decrypt(userRegistration.Password);

            return userRegistration;
        }

        // PUT: api/UserRegistrations/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUserRegistration(int id, UserRegistration userRegistration)
        {
            if (id != userRegistration.UserId)
            {
                return BadRequest();
            }

            _context.Entry(userRegistration).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserRegistrationExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/UserRegistrations
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<UserRegistration>> PostUserRegistration(UserRegistration userRegistration)
        {
            if (_context.UserRegistrations == null)
            {
                return Problem("Entity set 'EncryptionDbContext.UserRegistrations' is null.");
            }

            // Encrypt the password before storing it
            userRegistration.Password = Encrypt(userRegistration.Password);

            _context.UserRegistrations.Add(userRegistration);
            await _context.SaveChangesAsync();

            // Return the created user registration with the encrypted password
            return CreatedAtAction("GetUserRegistration", new { id = userRegistration.UserId }, userRegistration);
        }

        // DELETE: api/UserRegistrations/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<UserRegistration>> DeleteUserRegistration(int id)
        {
            if (_context.UserRegistrations == null)
            {
                return Ok(new Response { Status = "Failed", Message = "No users found!" });
            }

            var userRegistration = await _context.UserRegistrations.FindAsync(id);

            if (userRegistration == null)
            {
                return Ok(new Response { Status = "Failed", Message = "No users found!" });
            }

            _context.UserRegistrations.Remove(userRegistration);
            await _context.SaveChangesAsync();

            return Ok(new Response { Status = "Sucess", Message = "Deleted Successfully" } );
        }

        private bool UserRegistrationExists(int id)
        {
            return (_context.UserRegistrations?.Any(e => e.UserId == id)).GetValueOrDefault();
        }

        private string Encrypt(string password)
        {
            // Example key and IV generation using hashing
            string passphrase = "your-passphrase";

            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] key = sha256.ComputeHash(Encoding.UTF8.GetBytes(passphrase));
                byte[] iv = sha256.ComputeHash(Encoding.UTF8.GetBytes(passphrase)).Take(16).ToArray();

                using (Aes aes = Aes.Create())
                {
                    aes.Key = key;
                    aes.IV = iv;

                    ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                        {
                            using (StreamWriter writer = new StreamWriter(cryptoStream))
                            {
                                writer.Write(password);
                            }
                        }

                        byte[] encryptedData = memoryStream.ToArray();
                        return Convert.ToBase64String(encryptedData);
                    }
                }
            }
        }

        private string Decrypt(string encryptedPassword)
        {
            // Example key and IV generation using hashing
            string passphrase = "your-passphrase";

            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] key = sha256.ComputeHash(Encoding.UTF8.GetBytes(passphrase));
                byte[] iv = sha256.ComputeHash(Encoding.UTF8.GetBytes(passphrase)).Take(16).ToArray();

                using (Aes aes = Aes.Create())
                {
                    aes.Key = key;
                    aes.IV = iv;

                    ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                    byte[] encryptedData = Convert.FromBase64String(encryptedPassword);

                    using (MemoryStream memoryStream = new MemoryStream(encryptedData))
                    {
                        using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                        {
                            using (StreamReader reader = new StreamReader(cryptoStream))
                            {
                                return reader.ReadToEnd();
                            }
                        }
                    }
                }
            }
        }
    }
}
