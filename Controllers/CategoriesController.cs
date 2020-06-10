using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GraceChapelLibraryWebApp.Core.Models;
using GraceChapelLibraryWebApp.Core.Repositories.RepositoryInterfaces;
using Microsoft.AspNetCore.Mvc;

namespace GraceChapelLibraryWebApp.Controllers
{
    // Controller: BookCategories

    /// <summary>
    /// BookCategories Controller.
    /// </summary>
    ///
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : AppController
    {
        private readonly IRepositoryWrapper _repoWrapper;
        // Controller: BookCategories

        /// <summary>
        /// BookCategories Controller.
        /// </summary>
        ///
        public CategoriesController(IRepositoryWrapper repoWrapper)
        {
            _repoWrapper = repoWrapper;
        }

        // GET: api/BookCategories/all/yes
        /// <summary>
        /// Get all BookCategories.
        /// If Parent and child relation not needed 
        /// send "no" for the relation value
        /// </summary>
        ///
        [HttpGet("all/{relation}")]
        public async Task<IEnumerable<Category>> GetCategories(string relation = "yes")
        {
            try
            {
                if (relation == "no")
                {
                    return await _repoWrapper.Category.GetAllCategoriesWithoutRelationAsync();
                }
                return await _repoWrapper.Category.GetAllCategoriesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }
        //Get: api/Category/1
        /// <summary>
        /// Category Information with Id
        /// </summary>
        ///
        [HttpGet("{id}")]

        public async Task<ActionResult<Category>> GetCategory(int id)
        {
            Category category = await _repoWrapper.Category.GetCategoryByIdAsync(id);
            if (category.Id == 0 || category == null) 
            {
                return NotFound("No Category Found");
            }

            return category;
        }

        // GET: api/BookCategories
        /// <summary>
        /// Post Category.
        /// </summary>
        ///
        [HttpPost]
        public async Task<ActionResult<Category>> PostCategory([FromBody]Category category)
        {
            var isExists = _repoWrapper.Category.CategoryExistsByName(category.Name);
            if (isExists)
            {
                return Conflict("Already exists with the same category name");
            }
            await _repoWrapper.Category.CreateCategoryAsync(category);
            return CreatedAtAction("GetCategory", new { id = category.Id }, category);
        }


        // PUT: api/BookCategories/5
        /// <summary>
        /// Edit the category.
        /// </summary>
        ///
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCategory(int id, [FromBody]Category category)
        {
            try
            {
                if (category == null)
                {
                    return BadRequest("Category object is null");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest("Invalid Category object");
                }

                var dbCategory = await _repoWrapper.Category.GetCategoryByIdAsync(id);
                if (dbCategory == null)
                {
                    return NotFound();
                }

                await _repoWrapper.Category.UpdateCategoryAsync(dbCategory, category);

                return Ok();
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal server error");
            }
        }


        // DELETE: api/BookCategories/5
        /// <summary>
        /// Delete category.
        /// </summary>
        ///
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            try
            {
                var category = await _repoWrapper.Category.GetCategoryByIdAsync(id);
                if (category == null)
                {
                    return NotFound();
                }

                await _repoWrapper.Category.DeleteCategoryAsync(category);

                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal server error");
            }
        }

    }
}