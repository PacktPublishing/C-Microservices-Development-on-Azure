using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyToDoList.DTOs;
using MyToDoList.Models;
using AutoMapper;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;
using MyToDoList.Database;
using Microsoft.EntityFrameworkCore;

namespace MyToDoList.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class AccountController : Controller
    {
        AppDbContext _context;
        SignInManager<ApplicationUser> _signInManager;
        UserManager<ApplicationUser> _userManager;
        IConfiguration _configuration;

        public AccountController(AppDbContext context, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _configuration = configuration;
            _context = context;
        }

        [HttpPost("signup")]
        public async Task<IActionResult> SignUp([FromBody] UserSignUpDTO user)
        {
            ApplicationUser appUser = Mapper.Map<UserSignUpDTO, ApplicationUser>(user);
            appUser.UserName = user.Email;
            IdentityResult result = await _userManager.CreateAsync(appUser, user.Password);
            if(result == IdentityResult.Success)
            {
                await _signInManager.SignInAsync(appUser, false);
                string token = GenerateAccessToken(appUser);
                return Ok(token);
            }
            else
            {
                throw new Exception("Sign-Up Failed");
            }
        }

        [HttpPost("signin")]
        public async Task<IActionResult> SignIn([FromBody] UserSignInDTO user)
        {
            var result = await _signInManager.PasswordSignInAsync(user.Email, user.Password, false, false);
            if (result == SignInResult.Success)
            {
                var appUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == user.Email);
                string token = GenerateAccessToken(appUser);
                return Ok(token);
            }
            else
            {
                return Unauthorized();
            }
        }

        private string GenerateAccessToken(ApplicationUser appUser)
        {
            Claim[] claims = new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, appUser.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("Email", appUser.Email),
                new Claim("FirstName", appUser.FirstName),
                new Claim("LastName", appUser.LastName)
            };
            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSecret"]));
            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor()
            {
                Audience = _configuration["Audience"],
                Issuer = _configuration["Issuer"],
                IssuedAt = DateTime.UtcNow,
                Expires = DateTime.UtcNow.AddHours(12),
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature),
                Subject = new ClaimsIdentity(claims)
            };
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            var token = handler.CreateToken(tokenDescriptor);
            return handler.WriteToken(token);
        }
    }
}