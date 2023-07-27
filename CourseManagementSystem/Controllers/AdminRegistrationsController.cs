using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CourseManagementSystem.Models;
using System.Security.Cryptography;
using System.Text;

namespace CourseManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminRegistrationsController : ControllerBase
    {
        private readonly CourseQuizDbContext _context;

        public AdminRegistrationsController(CourseQuizDbContext context)
        {
            _context = context;
        }

        // GET: api/AdminRegistrations
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AdminRegistration>>> GetAdminRegistrations()
        {
            if (_context.AdminRegistrations == null)
            {
                return NotFound();
            }

            var adminRegistrations = await _context.AdminRegistrations.ToListAsync();

            // Decrypt the stored passwords before returning the user registrations
            foreach (var registration in adminRegistrations)
            {
                registration.AdminPassword = Decrypt(registration.AdminPassword);
            }

            return adminRegistrations;
        }

        // GET: api/AdminRegistrations/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AdminRegistration>> GetAdminRegistration(int id)
        {
            if (_context.AdminRegistrations == null)
            {
                return NotFound();
            }

            var adminRegistration = await _context.AdminRegistrations.FindAsync(id);

            if (adminRegistration == null)
            {
                return NotFound();
            }

            // Decrypt the stored password before returning the user registration
            adminRegistration.AdminPassword = Decrypt(adminRegistration.AdminPassword);

            return adminRegistration;
        }

        // PUT: api/AdminRegistrations/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAdminRegistration(int id, AdminRegistration adminRegistration)
        {
            if (id != adminRegistration.AdminId)
            {
                return BadRequest();
            }

            _context.Entry(adminRegistration).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AdminRegistrationExists(id))
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

        // POST: api/AdminRegistrations
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<AdminRegistration>> PostAdminRegistration(AdminRegistration adminRegistration)
        {
            if (_context.AdminRegistrations == null)
            {
                return Problem("Entity set 'EncryptionDbContext.AdminRegistrations' is null.");
            }

            // Encrypt the password before storing it
            adminRegistration.AdminPassword = Encrypt(adminRegistration.AdminPassword);

            _context.AdminRegistrations.Add(adminRegistration);
            await _context.SaveChangesAsync();

            // Return the created user registration with the encrypted password
            return CreatedAtAction("GetAdminRegistration", new { id = adminRegistration.AdminId }, adminRegistration);
        }

        // DELETE: api/AdminRegistrations/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAdminRegistration(int id)
        {
            if (_context.AdminRegistrations == null)
            {
                return NotFound();
            }
            var adminRegistration = await _context.AdminRegistrations.FindAsync(id);
            if (adminRegistration == null)
            {
                return NotFound();
            }

            _context.AdminRegistrations.Remove(adminRegistration);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AdminRegistrationExists(int id)
        {
            return (_context.AdminRegistrations?.Any(e => e.AdminId == id)).GetValueOrDefault();
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
