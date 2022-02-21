using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Competition3.Data;
using Competition3.Helper;
using Competition3.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Competition3.Controllers
{
    [ApiController]
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _config;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, IConfiguration config)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _config = config;
        }

        /// <summary>
        /// create user
        /// </summary>
        /// <param name="userVm"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("sign-up")]
        public async Task<IActionResult> SignUp([FromBody] UserVm userVm)
        {
            if (!ModelState.IsValid) return BadRequest();

            var user = new User
            {
                UserName = userVm.Email,
                FullName = userVm.FullName,
                Email = userVm.Email,
                PhoneNumber = userVm.MobileNumber
            };
            var result =await _userManager.CreateAsync(user, userVm.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "User");

                return Ok(new ResultObject
                {       
                    Success = true,
                    Id = user.Id,
                    Message = "کاربر با موفقیت ایجاد شد."
                });
            }

            var errors = GetIdentityError(result);
            return Json(new ResultObject{Success = false, Errors = errors});
        }

        /// <summary>
        /// sign in to program
        /// </summary>
        /// <param name="vm"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("sign-in")]
        public async Task<IActionResult> SigIn([FromBody] SignInVm vm)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var user = await _userManager.FindByEmailAsync(vm.Email);
            if (user == null)
            {
                return Json(new ResultObject {Success = false, Message = "ایمیل یا رمز عبور صحیح نمی باشد"});
            }

            var result = await _signInManager.PasswordSignInAsync(user, vm.Password, false, false);
            if (result.Succeeded)
            {
                var token =await GetTokenAsync(user);

                return Ok(new {vm.Email, userName = user.UserName, Token = token});
            }

            return Json(new ResultObject {Success = false, Message = "ایمیل یا رمز عبور صحیح نمی باشد"});
        }
        
        [NonAction]
        private List<string> GetIdentityError(IdentityResult result)
        {
            var list = new List<string>();
            result.Errors.ToList().ForEach(x=>list.Add(x.Description));

            return list;
        }

        [NonAction]
        private async Task<List<Claim>> GetClaimsAsync(User user)
        {
            var list = new List<Claim>();

            list.AddRange(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName)
            });
            var roles = await _userManager.GetRolesAsync(user);
            list.AddRange(roles.Select(x => new Claim("role", x)));

            var claims = await _userManager.GetClaimsAsync(user);
            list.AddRange(claims);

            return list;
        }

        [NonAction]
        private async Task<string> GetTokenAsync(User user)
        {
            var claims = await GetClaimsAsync(user);
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            var token = new JwtSecurityToken(_config["Jwt:Issuer"], _config["Jwt:Issuer"], claims,
                expires: DateTime.UtcNow.AddMinutes(double.Parse(_config["Jwt:Expire"])),
                signingCredentials: credentials);
            var accessToken = new JwtSecurityTokenHandler().WriteToken(token);

            return accessToken;
        }
    }
}