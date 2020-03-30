using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using cloudSite.Forms;
using cloudSite.Helpers;
using cloudSite.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace cloudSite.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public AuthController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpPost, Route("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult>  SignIn([FromBody]UserForm userForm)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => 
                u.Email.Equals(userForm.Email) && u.Password.Equals(userForm.Password));
            
            if (user == null) 
                return NotFound("User not found");

            _db.LoggerUserInfos.Add(new LoggerUserInfo
            {
                User = user,
                DateTime = DateTime.Now
            });
            await _db.SaveChangesAsync();
            
            return Ok(new { token = GenerateToken(user.Email), user = user.Firstname });
        }

        [HttpPost, Route("signUp")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SignUp([FromBody] UserForm userForm)
        {
            var userExist = await _db.Users.AnyAsync(u => u.Email == userForm.Email);

            if (userExist) return BadRequest("User already exist");

            var user = new User
            {
                Firstname = userForm.Firstname,
                Surname = userForm.Surname,
                Email = userForm.Email,
                Password = userForm.Password
            };
            
            await _db.Users.AddAsync(user);
            await _db.LoggerUserInfos.AddAsync(new LoggerUserInfo
                {
                    User = user,
                    DateTime = DateTime.Now
                });
            
            await _db.SaveChangesAsync();
            return Ok(new { token = GenerateToken(userForm.Email), user = user.Firstname });
        }
        
        private static string GenerateToken(string email)
        {
            var identity = GetIdentity(email);
            var now = DateTime.UtcNow;

            var jwt = new JwtSecurityToken(
                AuthOptions.Issuer,
                AuthOptions.Audience,
                notBefore: now,
                claims: identity.Claims,
                expires: now.Add(TimeSpan.FromMinutes(AuthOptions.Lifetime)),
                signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), 
                    SecurityAlgorithms.HmacSha256));
            
            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }
        
        private static ClaimsIdentity GetIdentity(string email)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, email),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, "User")
            };
                
            var claimsIdentity =
                new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                    ClaimsIdentity.DefaultRoleClaimType);
            return claimsIdentity;
        }
    }
}