using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GraceChapelLibraryWebApp.Core.Dtos;
using GraceChapelLibraryWebApp.Core.Models;
using GraceChapelLibraryWebApp.Core.Repositories.RepositoryInterfaces;
using GraceChapelLibraryWebApp.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace GraceChapelLibraryWebApp.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IRepositoryWrapper _repoWrapper;
        private UserManager<ApplicationUser> _userManager;

        public UsersController(IRepositoryWrapper repositoryWrapper, UserManager<ApplicationUser> userManager)
        {
            _repoWrapper = repositoryWrapper;
            _userManager = userManager;
        }

        // GET: api/Users
        [HttpGet, Authorize(Roles = "Admin")]
        public async Task<IEnumerable<ApplicationUser>> GetAllUsers()
        {
            try
            {
                return await _repoWrapper.User.GetAllUsersAsync();

            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        // GET: api/Users/5
        [HttpGet("{id}"), Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApplicationUser>> GetUser()
        {
            var id = AuthController.Validate(HttpContext);

            //int tokenId = int.Parse(User.FindFirst(ClaimTypes.Name).Value);

            var user = await _repoWrapper.User.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return user;
        }


        [HttpPut("deactivate/{id}"), Authorize(Roles = "Admin")]
        public async Task<IActionResult> PutDeactivateUser(int id)
        {
            try
            {
                var dbUser = await _repoWrapper.User.GetUserByIdAsync(id);
                if (dbUser == null)
                {
                    return NotFound();
                }

                await _repoWrapper.User.DeactivateUserAsync(dbUser);

                return Ok(new { status = "success", message = "Successfully Deactivated the BookUser" });
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal server error");
            }
        }
        // PUT: api/Users/activate/5
        [HttpPut("activate/{id}"), Authorize(Roles = "Admin")]
        public async Task<IActionResult> PutActivateUser(int id)
        {
            try
            {
                var dbUser = await _repoWrapper.User.GetUserByIdAsync(id);
                if (dbUser == null)
                {
                    return NotFound();
                }

                await _repoWrapper.User.ActivateUserAsync(dbUser);
                return Ok(new { status = "success", message = "Successfully Activated the BookUser" });
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal server error");
            }
        }


        // POST: api/Users
        [HttpPost, Authorize(Roles = "Admin")]
        public async Task<IActionResult> PostUser(ApplicationUser user)
        {
            var isExists = _repoWrapper.User.IsExists(user);
            if (isExists)
            {
                var result = new ObjectResult("BookUser Already exists");
                result.StatusCode = 409;
                return result;
            }

            var person = _userManager.FindByNameAsync(user.UserName);
            user.Email = person.Result.Email;

            await _repoWrapper.User.AddUserAsync(user);

            return Ok(new { status = "success", message = "Successfully added the user as admin" });
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}"), Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                var user = await _repoWrapper.User.GetUserByIdAsync(id);
                if (user == null)
                {
                    return NotFound();
                }
                await _repoWrapper.User.DeleteUserAsync(user);
                return Ok(new { status = "success", message = "Successfully deleted the admin user" });
            }
            catch (Exception)
            {

                return StatusCode(500, "Internal server error");
            }
        }


    }
}
