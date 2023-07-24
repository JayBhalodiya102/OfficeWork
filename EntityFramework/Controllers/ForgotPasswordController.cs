using EntityFramework.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ForgotPasswordController : ControllerBase
    {
        private readonly MyAPIDbContext _dbContext;
        private static string _GlobalSaltKey;
        private static int _GlobalRandomNumber;

        public ForgotPasswordController(MyAPIDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        [HttpPost("Verify-Email")]
        public async Task<IActionResult> VerifyEmail(string Email)
        {
            if (string.IsNullOrWhiteSpace(Email))
            {
                return BadRequest();
            }
            else
            {
                var Data = await _dbContext.Details.FirstOrDefaultAsync(X => X.Email == Email);

                if (Data != null)
                {
                    GenerateOTP();

                    Data.OTP = _GlobalRandomNumber;

                    await _dbContext.SaveChangesAsync();

                    SendEmail(Email);

                    return Ok("OTP has been sent to your Mail");
                }

                else
                {
                    return NotFound("Please Enter Valid Email...!!!");
                }

            }
            return Ok();
        }

        [HttpPost("Forgot-Password")]
        public async Task<IActionResult> ForgotPassword(string Email, int OTP, string Password)
        {
            var Data = await _dbContext.Details.FirstOrDefaultAsync(X => X.Email == Email && X.OTP == OTP);
            if (Data != null)
            {
                Data.Password = SaltKey(Password);
                Data.HashKey = _GlobalSaltKey;

                await _dbContext.SaveChangesAsync();
                return Ok("Password Updated SuccessFully");
            }
            else
            {
                return BadRequest("Failed...!!!");
            }

            return null;

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

        public static string SendEmail(string Email)
        {
            var SMTPclient = new SmtpClient("sandbox.smtp.mailtrap.io", 2525)
            {
                Credentials = new NetworkCredential("f9ae5a786cbd44", "28b27b2ca30311"),
                EnableSsl = true
            };

            try
            {
                var From_Address = "JAYPatel2510@gmail.com";
                var To_Address = Email;
                var Subject = "OTP Test Mail";
                var Body = (_GlobalRandomNumber).ToString();

                using (var Data_Message = new MailMessage(From_Address, To_Address, Subject, Body))
                {
                    SMTPclient.Send(Data_Message);
                }

                return "Your Email Sent successfully";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public static int GenerateOTP()
        {
            Random random = new Random();

            int RandomNumber = random.Next(100000, 1000000);

            _GlobalRandomNumber = RandomNumber;

            return RandomNumber;
        }
    }
}
