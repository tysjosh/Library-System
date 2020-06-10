using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using GraceChapelLibraryWebApp.Core.Dtos;
using GraceChapelLibraryWebApp.Core.Enumerations;
using GraceChapelLibraryWebApp.Core.Models;
using GraceChapelLibraryWebApp.Core.Repositories.RepositoryInterfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace GraceChapelLibraryWebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : AppController
    {
        private readonly IRepositoryWrapper _repoWrapper;
        private UserManager<ApplicationUser> _userManager;
        private readonly ApplicationSettings _appSettings;

        public AuthController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,
            IOptions<ApplicationSettings> appSettings, IRepositoryWrapper repoWrapper)
        {
            _repoWrapper = repoWrapper;
            _userManager = userManager;
            _appSettings = appSettings.Value;
        }

        [HttpPost]
        [Route("Register")]
        //POST : /api/Auth/Register
        public async Task<IActionResult> PostApplicationUser(BookUser model)
        {
            var applicationUser = new ApplicationUser()
            {
                UserName = model.UserName,
                Email = model.Email,
                FullName = model.FullName,
                Created = model.Created,
                Modified = model.Modified,
                Role = model.Role,
                PhoneNumber = model.PhoneNumber

            };

            try
            {
                var result = await _userManager.CreateAsync(applicationUser, model.Password);
                return Ok(result);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }



        [HttpPost]
        [Route("Login")]
        //POST : /api/Auth/Login
        public async Task<IActionResult> Login([FromBody]LoginDto model)
        {
            if (model == null)
            {
                return BadRequest("Invalid client request");
            }

            //if (model.UserName == "admin" && model.Password == "Paramedic22=")
            //{

            //    var signinCredentials = new SigningCredentials(new SymmetricSecurityKey(
            //            Encoding.UTF8.GetBytes(_appSettings.Jwt_Secret)),
            //        SecurityAlgorithms.HmacSha256Signature);
            //    List<Claim> claimsDetailsAdmin;

            //    claimsDetailsAdmin = new List<Claim>{
            //        new Claim(ClaimTypes.Name, model.UserName),
            //        new Claim(ClaimTypes.Role, "Admin"),
            //        new Claim("isAdmin", "yes"),
            //        new Claim("user", model.UserName),
            //        new Claim("fullName", "Adeolu Samuel"),
            //    };

            //    var adminTokenOptions = new JwtSecurityToken(
            //        issuer: "http://localhost:52032",
            //        audience: "http://localhost:52032",
            //        claims: claimsDetailsAdmin,
            //        expires: DateTime.UtcNow.AddHours(2),
            //        signingCredentials: signinCredentials
            //    );

            //    var tokenString = new JwtSecurityTokenHandler().WriteToken(adminTokenOptions);
            //    return Ok(new { Token = tokenString });
            //}



            //var user = await _userManager.FindByNameAsync(model.UserName);
            var userInfo = await _repoWrapper.User.GetUserByUserNameAsync(model.UserName);

            if (userInfo != null && await _userManager.CheckPasswordAsync(userInfo, model.Password))
            {
                var signinCredentials = new SigningCredentials(new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(_appSettings.Jwt_Secret)),
                    SecurityAlgorithms.HmacSha256Signature);
                List<Claim> claimsDetails;

                if ((userInfo.Role == "Admin" && UserStatus.Active == userInfo.Status))
                {
                    claimsDetails = new List<Claim>
                    {
                        new Claim(ClaimTypes.Sid, userInfo.Id.ToString()),
                        new Claim(ClaimTypes.Name, model.UserName),
                        new Claim(ClaimTypes.Role, "Admin"),
                        new Claim("isAdmin", "yes"),
                        new Claim("user", model.UserName),
                        new Claim("fullName", userInfo.FullName),
                    };
                }
                else if (model.UserName == "admin" && model.Password == "Paramedic22=")
                {
                    claimsDetails = new List<Claim>{
                        new Claim(ClaimTypes.Sid, userInfo.Id.ToString()),
                        new Claim(ClaimTypes.Name, model.UserName),
                        new Claim(ClaimTypes.Role, "Admin"),
                        new Claim("isAdmin", "yes"),
                        new Claim("user", model.UserName),
                        new Claim("fullName", userInfo.FullName),
                    };
                }
                else
                {
                    claimsDetails = new List<Claim>{
                        new Claim(ClaimTypes.Sid, userInfo.Id.ToString()),
                        new Claim(ClaimTypes.Name, model.UserName),
                        new Claim(ClaimTypes.Role, "User"),
                        new Claim("isAdmin", "no"),
                        new Claim("user", model.UserName),
                        new Claim("fullName", userInfo.FullName),
                    };
                }



                var tokeOptions = new JwtSecurityToken(
                    issuer: "http://localhost:63568/",
                    audience: "http://localhost:63568/",
                    claims: claimsDetails,
                    expires: DateTime.UtcNow.AddHours(2),
                    signingCredentials: signinCredentials
                );

                var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);
                return Ok(new { Token = tokenString });
            }

            return BadRequest("Invalid username or password");

        }

        public static int Validate(HttpContext context)
        {
            var jwt = new StringBuilder(context.Request.Headers["Authorization"].ToString());
            jwt.Replace("Bearer ", "");
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(jwt.ToString());
            var id = token.Claims.FirstOrDefault(opt => opt.Type == ClaimTypes.Sid).Value;
            return int.Parse(id);


        }

    }
}