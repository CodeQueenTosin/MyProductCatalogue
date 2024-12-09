using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Concurrent;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace Authentication.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountController(IConfiguration config) : ControllerBase
    {
   
        private static ConcurrentDictionary<string, string> UserData {  get; set; } = new ConcurrentDictionary<string, string>();

        //api/account/login/{email}/{password}
        [HttpPost("Login/{email}/{password}")]
        public async Task<IActionResult> Login(string email, string password)
        {
            await Task.Delay(500);
            var getEmail = UserData!.Keys.Where( x => x.Equals(email)).FirstOrDefault();
            if(!string.IsNullOrEmpty(getEmail) )
            {
                UserData.TryGetValue(getEmail, out string? dbPassword);
                if (!Equals(dbPassword, password))
                    return BadRequest("Invalid Credentials");

                string jwtToken = GenerateToken(getEmail);
                return Ok(jwtToken);

            }
            return NotFound("Email not found");
        }

        private string GenerateToken(string getEmail)
        {
            var key = Encoding.UTF8.GetBytes(config["Authentication:Key"]!);
            var securityKey = new SymmetricSecurityKey(key);
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[] { new Claim(ClaimTypes.Email, getEmail!) };

            var token = new JwtSecurityToken(
                issuer: config["Authentication:Issuer"],
                audience: config["authentication:Audience"],
                claims: claims,
                expires: null,
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);

        }

        [HttpPost("register/{email}/{password}")]
        public async Task<IActionResult> Register(string email, string password)
        {
            await Task.Delay(500);
            var getEmail = UserData!.Keys.Where(e=> e.Equals(email)).FirstOrDefault();

            if (!string.IsNullOrEmpty(getEmail))
                return BadRequest("User already exist");

            UserData[email] = password;
            return Ok("User Created Succesfully");

          }

    }

}
