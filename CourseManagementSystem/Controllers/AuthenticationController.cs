using CourseManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;   

namespace CourseManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [SwaggerTag("Authentication")]
    public class AuthenticationController : ControllerBase
    {
            public IConfiguration _configuration;
            private readonly CourseQuizDbContext _context;
            private const string AdminRole = "Admin";
            private const string UserRole = "User";

            public AuthenticationController(IConfiguration config, CourseQuizDbContext context)
            {
                _configuration = config;
                _context = context;
            }

            [HttpPost("Admin")]
        public async Task<IActionResult> Post(AdminRegistration _userData)
            {
                if (_userData != null && _userData.AdminMailId != null && _userData.AdminPassword != null)
                {
                    var user = await GetAdmin(_userData.AdminMailId,Encrypt(_userData.AdminPassword));

                    if (user != null)
                    {
                        //create claims details based on the user information
                        var claims = new[] {
                        new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),

                         new Claim("AdminMailId", user.AdminMailId),
                        new Claim("AdminPassword",user.AdminPassword),
                        new Claim(ClaimTypes.Role,AdminRole),

                    };

                        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                        var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                        var token = new JwtSecurityToken(
                            _configuration["Jwt:Issuer"],
                            _configuration["Jwt:Audience"],
                            claims,
                            expires: DateTime.UtcNow.AddMinutes(10),
                           signingCredentials: signIn);

                        return Ok(new JwtSecurityTokenHandler().WriteToken(token));
                    }
                    else
                    {
                        return BadRequest("Invalid credentials");
                    }
                }
                else
                {
                    return BadRequest();
                }

            }
            private async Task<AdminRegistration> GetAdmin(string email, string password)
            {
                return await _context.AdminRegistrations.FirstOrDefaultAsync(u => u.AdminMailId == email && u.AdminPassword == password);
            }


        [HttpPost("User")]
        public async Task<IActionResult> Post(UserRegistration _userData)
            {
                if (_userData != null && _userData.MailId != null && _userData.Password != null)
                {
                    var user = await GetUser(_userData.MailId, Encrypt(_userData.Password));

                    if (user != null)
                    {
                        //create claims details based on the user information
                        var claims = new[] {
                        new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),

                         new Claim("MailId", user.MailId),
                        new Claim("Password",user.Password),
                        new Claim(ClaimTypes.Role,UserRole),

                    };

                        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                        var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                        var token = new JwtSecurityToken(
                            _configuration["Jwt:Issuer"],
                            _configuration["Jwt:Audience"],
                            claims,
                            expires: DateTime.UtcNow.AddMinutes(10),
                           signingCredentials: signIn);

                        return Ok(new JwtSecurityTokenHandler().WriteToken(token));
                    }
                    else
                    {
                        return BadRequest("Invalid credentials");
                    }
                }
                else
                {
                    return BadRequest();
                }

            }
            private async Task<UserRegistration> GetUser(string MailId, string password)
            {
                return await _context.UserRegistrations.FirstOrDefaultAsync(u => u.MailId == MailId && u.Password == password);
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

    }
    }

