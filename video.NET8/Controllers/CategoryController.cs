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
        private readonly IExcelService _excelService;

        public CategoryController(ICategoryService categoryService , IExcelService excelService)
        {
            _categoryService = categoryService;
            _excelService = excelService;


        }

        [Authorize] // members and admins can access this endpoint
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetCategoryResponse>>> GetActiveCategories()
        {
         
            try
            {
                var categories = await _categoryService.GetActiveCategories();
                return Ok(categories);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
           
        }
        [Authorize] // members and admins can access this endpoint
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetCategoryResponse>>> GetAllCategories()
        {
            try
            {
                var categories = await _categoryService.GetAllCategories();
                return Ok(categories);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
            
        }
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<GetCategoryResponse>> GetCategoryById(int id)
        {
            try
            {
                var category = await _categoryService.GetCategoryById(id);
                return Ok(category);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [Authorize(Roles ="Admin")]
        [HttpPost]
        public async Task<ActionResult<AddCategoryResponse>> AddCategory(AddCategoryRequest request)
        {

            //var token = Request.Headers["Authorization"].ToString();
            //var tokenValue = token?.StartsWith("Bearer ") == true ? token.Substring("Bearer ".Length).Trim() : token;

            //if (!await _userService.ValidateAdminsToken(tokenValue))
            //{
            //    return Unauthorized(new { message = "Invalid token or unauthorized user" });
            //}

            var category = await _categoryService.AddCategory(request);
            return Ok(category);
        }

        [Authorize(Roles = "Admin")] //it actually works without the logic of auhtentication down below
        [HttpPut("{id}")]
        public async Task<ActionResult<UpdateCategoryResponse>> UpdateCategory(int id, UpdateCategoryRequest request)
        {
            //var token = Request.Headers["Authorization"].ToString();
            //var tokenValue = token?.StartsWith("Bearer ") == true ? token.Substring("Bearer ".Length).Trim() : token;

            //if (!await _userService.ValidateAdminsToken(tokenValue))
            //{
            //    return Unauthorized(new { message = "Invalid token or unauthorized user" });
            //}

            try
            {
                var category = await _categoryService.UpdateCategory(id, request);
                return Ok(category);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var success = await _categoryService.DeleteCategoryAsync(id);
            if (!success)
            {
                return NotFound("Category not found.");
            }
            return Ok(new { message = "Category has been deleted successfully" });
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> SoftDeleteCategory(int id)
        {
            try
            {
                await _categoryService.SoftDeleteCategory(id);
                return Ok(new { message = "Category has been soft-deleted successfully" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("{id}/{bookId}")]
        public async Task<IActionResult> AssignBookToCategory(int id, int bookId)
        {
            try
            {
                await _categoryService.AssignBookToCategory(id, bookId);
                return Ok("Book assigned to category successfully.");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [Authorize] //admin and members can access this endpoint
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBooksInCategory(int id)
        {
            try
            {
                var books = await _categoryService.GetBooksInCategory(id);
                return Ok(books);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}/{bookId}")]
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
        [HttpGet]
        public async Task<IActionResult> FilterBooks([FromQuery] int? authorId, [FromQuery] bool? available)
        {
            var books = await _categoryService.FilterBooks(authorId, available);
            return Ok(books);
        }

        [AllowAnonymous] //let anyone search through our books
        [HttpGet]
        public async Task<IActionResult> SearchBooks([FromQuery] string? title, [FromQuery] string? authorName)
        {
            var books = await _categoryService.SearchBooks(title, authorName);
            return Ok(books);
        }


        // Export data to Excel
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ExportCategoriesToExcel()
        {
            var AllCategories = await _categoryService.ExportCategoriesToExcel();

            var fileContent = _excelService.GenerateExcelSheet(AllCategories, "ReportOfAllCategories");

            return File(fileContent, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ReportOfAllCategories.xlsx");
        }


        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ImportCategoriesFromExcel(IFormFile excelFile)
        {
            try
            {
                var (validCategories, validationErrors) = await _categoryService.ImportCategoriesFromExcel(excelFile);

                // Create categories in the database for valid entries
                foreach (var category in validCategories)
                {
                    await _categoryService.AddCategory(category);
                }

                return Ok(new
                {
                    message = "Categories import completed.",
                    successfulCategories = validCategories,
                    validationErrors = validationErrors
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

    }
}
