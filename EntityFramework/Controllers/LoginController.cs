using EntityFramework.Data;
using EntityFramework.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Bcpg;
using System.Security.Cryptography;
using System.Text;
using static System.Net.WebRequestMethods;

namespace EntityFramework.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly MyAPIDbContext _dbContext;
        private static string _GlobalHashKey;

        public LoginController(MyAPIDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPost("Create-Login")]
        public async Task<IActionResult> Login(string Email, string Password)
        {
            var checkEmail = await _dbContext.Details.FirstOrDefaultAsync(X => X.Email == Email);
            if (checkEmail != null)
            {
                string Hashkey = checkEmail.HashKey;

                var HasedPassword = SaltKey(Hashkey, Password);

                var checkHashkey = await _dbContext.Details.FirstOrDefaultAsync(x => x.Password == HasedPassword);
                if (checkHashkey != null)
                {
                    var CurrentTime = DateTime.UtcNow;
                    
                    var data = new LoginModel
                    {
                        Email = Email,
                        Password = HasedPassword,
                        Time = Convert.ToDateTime(CurrentTime)
                    };

                    await _dbContext.Login.AddAsync(data);
                    await _dbContext.SaveChangesAsync();
                    return Ok("Login Succesfully");
                }
                else
                {
                    return BadRequest("Login Failed...!!!");
                }
            }
            else
            {
                return BadRequest("Enter Valid Email...!!!");
            }
            return null;
        }

        public static string SaltKey(string Hashkey, string Password)
        {

            string PassHashKey = Hashkey + Password;

            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(PassHashKey));
                string hashKey = Convert.ToBase64String(hashBytes);

                _GlobalHashKey = hashKey;

                return hashKey;
            }
        }


        
    }
}
