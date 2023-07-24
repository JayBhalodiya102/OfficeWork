using EntityFramework.Data;
using EntityFramework.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Org.BouncyCastle.Security;
using System.Globalization;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;

namespace EntityFramework.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DetailsController : ControllerBase
    {
        private readonly MyAPIDbContext _dbContext;
        private static string _GlobalSaltKey;

        public DetailsController(MyAPIDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPost("Insert-Details")]
        public async Task<IActionResult> Create(DetailsModel Dm)
        {
            var existsData = await _dbContext.Details.FirstOrDefaultAsync(X => X.Email == Dm.Email);

            if (existsData != null)
            {
                return BadRequest("Email already exists");
            }
            else
            {
                var newData = new DetailsModel
                {
                    Name = Dm.Name,
                    Email = Dm.Email,
                    Password = SaltKey(Dm.Password),
                    HashKey = _GlobalSaltKey,
                    Address = Dm.Address,
                    Phoneno = Dm.Phoneno,
                };

                await _dbContext.Details.AddAsync(newData);
                await _dbContext.SaveChangesAsync();

                return Ok(newData);
            }
        }


        [HttpPut("Update-Details")]
        public async Task<IActionResult> Update(DetailsModel Dm)
        {
            var existsData = await _dbContext.Details.FindAsync(Dm.Id);

            if (existsData != null)
            {
                existsData.Name = Dm.Name;
                existsData.Email = Dm.Email;
                existsData.Address = Dm.Address;
                existsData.Phoneno = Dm.Phoneno;

                await _dbContext.SaveChangesAsync();

                return Ok(existsData);
            }

            return NotFound();
        }


        [HttpDelete("Delete-Details")]
        public async Task<IActionResult> Delete(int id)
        {
            var existsData = await _dbContext.Details.FindAsync(id);

            if (existsData != null)
            {
                _dbContext.Details.Remove(existsData);

                await _dbContext.SaveChangesAsync();

                return Ok("Deleted Record Id : " + id);
            }

            return NotFound();
        }

        [HttpGet("GetAll-Details")]
        public async Task<IActionResult> GetAll()
        {
            var AllData = await _dbContext.Details.ToListAsync();

            return Ok(AllData);
        }


        [HttpGet("GetSingle-Details")]
        public async Task<IActionResult> GetSingle(int id)
        {
            var FindId = await _dbContext.Details.FindAsync(id);
            if (FindId != null)
            {

                return Ok(FindId);

            }
            return NotFound("Please Enter Valid Id...!!!");
        }


        [NonAction]
        public static string SaltKey(string password)
        {
            byte[] salt = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            string Saltkey = Convert.ToBase64String(salt);

            _GlobalSaltKey = Saltkey;

            string PassHashKey = Saltkey + password;

            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(PassHashKey));
                string hashKey = Convert.ToBase64String(hashBytes);
                return hashKey;
            }
            
        }

    }
}

