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
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        //private readonly IUserService _userService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
            
        }

        [Authorize] // members and admins can access this endpoint
        [HttpGet("GetActiveCategories")]
        public async Task<ActionResult<IEnumerable<GetCategoryResponse>>> GetAllCategories()
        {
         
            try
            {
                var categories = await _categoryService.GetAllCategoriesAsync();
                return Ok(categories);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
           
        }
        [Authorize] // members and admins can access this endpoint
        [HttpGet("GetActiveAndInActiveCategories")]
        public async Task<ActionResult<IEnumerable<GetCategoryResponse>>> GetActiveAndInActiveCategories()
        {
            try
            {
                var categories = await _categoryService.GetActiveAndInActiveCategoriesAsync();
                return Ok(categories);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
            
        }
        [Authorize]
        [HttpGet("GetCategoryById/{id}")]
        public async Task<ActionResult<GetCategoryResponse>> GetCategoryById(int id)
        {
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
        [HttpPost("AddCategoryByAdmin")]
        public async Task<ActionResult<AddCategoryResponse>> AddCategoryByAdmin(AddCategoryRequest request)
        {

            //var token = Request.Headers["Authorization"].ToString();
            //var tokenValue = token?.StartsWith("Bearer ") == true ? token.Substring("Bearer ".Length).Trim() : token;

            //if (!await _userService.ValidateAdminsToken(tokenValue))
            //{
            //    return Unauthorized(new { message = "Invalid token or unauthorized user" });
            //}

            var category = await _categoryService.AddCategoryAsync(request);
            return Ok(category);
        }

        [Authorize(Roles = "Admin")] //it actually works without the logic of auhtentication down below
        [HttpPut("UpdateCategoryByAdmin/{id}")]
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
        [HttpDelete("DeleteCategoryByAdmin/{id}")]
        public async Task<IActionResult> DeleteCategoryByAdmin(int id)
        {
            var success = await _categoryService.DeleteCategoryAsync(id);
            if (!success)
            {
                return NotFound("Category not found.");
            }
            return Ok(new { message = "Category has been deleted successfully" });
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("SoftDeleteCategoryByAdmin/{id}")]
        public async Task<IActionResult> SoftDeleteCategoryByAdmin(int id)
        {
            try
            {
                await _categoryService.SoftDeleteCategoryAsync(id);
                return Ok(new { message = "Category has been soft-deleted successfully" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("api/category/{id}/books/{bookId}")]
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

        [Authorize] //admin and members can access this endpoint
        [HttpGet("api/category/{id}/books")]
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
        [HttpDelete("api/category/{id}/books/{bookId}")]
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
        [HttpGet("filterBYAuthorID/Availability")]
        public async Task<IActionResult> FilterBooks([FromQuery] int? authorId, [FromQuery] bool? available)
        {
            var books = await _categoryService.FilterBooksAsync(authorId, available);
            return Ok(books);
        }

        [AllowAnonymous] //let anyone search through our books
        [HttpGet("searchBYBookTitle/AuthorName")]
        public async Task<IActionResult> SearchBooks([FromQuery] string? title, [FromQuery] string? authorName)
        {
            var books = await _categoryService.SearchBooksAsync(title, authorName);
            return Ok(books);
        }

    }
}
