using LibraryManagementSystem.Interfaces;
using LibraryManagmentSystem.Contract.Requests;
using LibraryManagmentSystem.Contract.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LibraryManagementSystem.Domain;
using LibraryManagementSystem.Services;
using LibraryManagmentSystem.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly IUserService _userService;

        public CategoryController(ICategoryService categoryService, IUserService userService)
        {
            _categoryService = categoryService;
            _userService = userService;
        }

        [Authorize] // members and admins can access this endpoint
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetCategoryResponse>>> GetAllCategories()
        {
            var token = Request.Headers["Authorization"].ToString();
            var tokenValue = token?.StartsWith("Bearer ") == true ? token.Substring("Bearer ".Length).Trim() : token;

            if (!await _userService.ValidateAdminsToken(tokenValue))
            {
                return Unauthorized(new { message = "Invalid token or unauthorized user" });
            }

            var categories = await _categoryService.GetAllCategoriesAsync();
            return Ok(categories);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<GetCategoryResponse>> GetCategoryById(int id)
        {

            var token = Request.Headers["Authorization"].ToString();
            var tokenValue = token?.StartsWith("Bearer ") == true ? token.Substring("Bearer ".Length).Trim() : token;

            if (!await _userService.ValidateAdminsToken(tokenValue))
            {
                return Unauthorized(new { message = "Invalid token or unauthorized user" });
            }

            try
            {
                var category = await _categoryService.GetCategoryByIdAsync(id);
                return Ok(category);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [Authorize(Roles ="Admin")]
        [HttpPost]
        public async Task<ActionResult<AddCategoryResponse>> AddCategoryByAdmin(AddCategoryRequest request)
        {

            //var token = Request.Headers["Authorization"].ToString();
            //var tokenValue = token?.StartsWith("Bearer ") == true ? token.Substring("Bearer ".Length).Trim() : token;

            //if (!await _userService.ValidateAdminsToken(tokenValue))
            //{
            //    return Unauthorized(new { message = "Invalid token or unauthorized user" });
            //}

            var category = await _categoryService.AddCategoryAsync(request);
            return CreatedAtAction(nameof(GetCategoryById), new { id = category.Id }, category);
        }

        [Authorize(Roles = "Admin")] //it actually works without the logic of auhtentication down below
        [HttpPut("{id}")]
        public async Task<ActionResult<UpdateCategoryResponse>> UpdateCategoryByAdmin(int id, UpdateCategoryRequest request)
        {
            //var token = Request.Headers["Authorization"].ToString();
            //var tokenValue = token?.StartsWith("Bearer ") == true ? token.Substring("Bearer ".Length).Trim() : token;

            //if (!await _userService.ValidateAdminsToken(tokenValue))
            //{
            //    return Unauthorized(new { message = "Invalid token or unauthorized user" });
            //}

            try
            {
                var category = await _categoryService.UpdateCategoryAsync(id, request);
                return Ok(category);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategoryByAdmin(int id)
        {
            var token = Request.Headers["Authorization"].ToString();
            var tokenValue = token?.StartsWith("Bearer ") == true ? token.Substring("Bearer ".Length).Trim() : token;

            if (!await _userService.ValidateAdminsToken(tokenValue))
            {
                return Unauthorized(new { message = "Invalid token or unauthorized user" });
            }

            var success = await _categoryService.DeleteCategoryAsync(id);
            if (!success)
            {
                return NotFound("Category not found.");
            }
            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("soft-delete/{id}")]
        public async Task<IActionResult> SoftDeleteCategoryByAdmin(int id)
        {
            try
            {
                await _categoryService.SoftDeleteCategoryAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("api/categories/{id}/books/{bookId}")]
        public async Task<IActionResult> AssignBookToCategoryByAdmin(int id, int bookId)
        {
            try
            {
                await _categoryService.AssignBookToCategoryAsync(id, bookId);
                return Ok("Book assigned to category successfully.");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("api/categories/{id}/books")]
        public async Task<IActionResult> GetBooksInCategory(int id)
        {
            try
            {
                var books = await _categoryService.GetBooksInCategoryAsync(id);
                return Ok(books);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("api/categories/{id}/books/{bookId}")]
        public async Task<IActionResult> RemoveBookFromCategoryByAdmin(int id, int bookId)
        {
            try
            {
                await _categoryService.RemoveBookFromCategoryAsync(id, bookId);
                return Ok("Book removed from category successfully.");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [Authorize] 
        [HttpGet("api/books/filter")]
        public async Task<IActionResult> FilterBooks([FromQuery] int? authorId, [FromQuery] bool? available)
        {
            var books = await _categoryService.FilterBooksAsync(authorId, available);
            return Ok(books);
        }

        [Authorize]
        [HttpGet("api/books/search")]
        public async Task<IActionResult> SearchBooks([FromQuery] string? title, [FromQuery] string? authorName)
        {
            var books = await _categoryService.SearchBooksAsync(title, authorName);
            return Ok(books);
        }

    }
}
