using LibraryManagementSystem.Contract.Requests;
using LibraryManagementSystem.Domain;
using LibraryManagementSystem.Services;
using LibraryManagmentSystem.Contract.Requests;
using LibraryManagmentSystem.Contract.Responses;
using LibraryManagmentSystem.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;

namespace LibraryManagmentSystem.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthorsController : ControllerBase
    {
        private readonly IAuthorService _authorService;
        private readonly IUserService _userService;
        private readonly IExcelService _excelService;


        public AuthorsController(IAuthorService authorService, IUserService userService, IExcelService excelService)
        {
            _authorService = authorService;
            _userService = userService;
            _excelService = excelService;
        }

        // GET: api/authors
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<GetAllAuthorsResponse>>> GetActiveAuthors()
        {
            var authors = await _authorService.GetActiveAuthors();
            return Ok(authors);
        }

        [HttpGet]   
        [Authorize(Roles ="Admin")]
        public async Task<ActionResult<IEnumerable<GetAllAuthorsResponse>>> GetAllAuthors()
        {
            var authors = await _authorService.GetAllAuthors();
            return Ok(authors);
        }


        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<GetAuthorByIdResponse>> GetAuthorById(int id)
        {
            try
            {
                var author = await _authorService.GetAuthorById(id);
                return Ok(author);
            }

            catch (KeyNotFoundException ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // POST: api/authors
        [HttpPost]
        [Authorize(Roles = "Admin")] // This attribute will require users with the role "Admin" to access the endpoint

        public async Task<ActionResult<AddAuthorResponse>> AddAuthor([FromBody] AddAuthorRequest authorRequest)
        {
            try
            {

                // get the token fro the authorization header

                var token = Request.Headers["Authorization"].ToString();
                // Extract the token value from the "Bearer " prefix
                var tokenValue = token?.StartsWith("Bearer ") == true ? token.Substring("Bearer ".Length).Trim() : token;

                // Validate the token using the service method
                if (!await _userService.ValidateAdminsToken(tokenValue))
                {
                    return Unauthorized(new { message = "Invalid token or unauthorized user" });
                }

                if (authorRequest == null)
                {
                    return BadRequest("Author data is required.");
                }
                var authorResponse = await _authorService.AddAuthor(authorRequest);
                return Ok(authorResponse);
            }

            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // PUT: api/authors/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UpdateAuthorResponse>> UpdateAuthor(int id, [FromBody] UpdateAuthorRequest authorRequest)
        {
            try
            {
                var token = Request.Headers["Authorization"].ToString();
                var tokenValue = token?.StartsWith("Bearer ") == true ? token.Substring("Bearer ".Length).Trim() : token;

                if (!await _userService.ValidateAdminsToken(tokenValue))
                {
                    return Unauthorized(new { message = "Invalid token or unauthorized user" });
                }

                if (authorRequest == null)
                {
                    return BadRequest("Author data is required.");
                }

                var authorResponse = await _authorService.UpdateAuthor(id, authorRequest);
                return Ok(authorResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }


        // DELETE: api/authors/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteAuthor(int id)
        {
            try
            {
                var token = Request.Headers["Authorization"].ToString();
                var tokenValue = token?.StartsWith("Bearer ") == true ? token.Substring("Bearer ".Length).Trim() : token;

                if (!await _userService.ValidateAdminsToken(tokenValue))
                {
                    return Unauthorized(new { message = "Invalid token or unauthorized user" });
                }


                var result = await _authorService.DeleteAuthor(id);
                if (!result)
                {
                    return NotFound(new { message = "Author not found or is inactive." });
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }


        // PATCH: api/authors/{id}/softdelete
        [HttpPatch("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SoftDeleteAuthor(int id)
        {
            try
            {
                var token = Request.Headers["Authorization"].ToString();
                var tokenValue = token?.StartsWith("Bearer ") == true ? token.Substring("Bearer ".Length).Trim() : token;

                if (!await _userService.ValidateAdminsToken(tokenValue))
                {
                    return Unauthorized(new { message = "Invalid token or unauthorized user" });
                }

               
                await _authorService.SoftDeleteAuthor(id);
                return Ok(new {message = "The Author has been successfully soft deleted"} );
            }
            catch (KeyNotFoundException ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
            
        }

        // Export all authors data to Excel
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ExportAllAuthorsToExcel()
        {
            try
            {
                var AllAuthors = await _authorService.ExportAllAuthorsToExcel();

                var fileContent = _excelService.GenerateExcelSheet(AllAuthors, "ReportOfAllAuthors");

                return File(fileContent, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ReportOfAllAuthors.xlsx");
            }

            catch (Exception ex)
            {

                return StatusCode(500, new { message = ex.Message });
            }
        }

        // Export data to Excel
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ExportActiveAuthorsToExcel()
        {
            try
            {

                var ActiveAuthors = await _authorService.ExportActiveAuthorsToExcel();

                var fileContent = _excelService.GenerateExcelSheet(ActiveAuthors, "ReportOfActiveAuthors");

                return File(fileContent, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ReportOfActiveAuthors.xlsx");

            }
            
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }

        }


        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ImportAuthorsFromExcel(IFormFile excelFile)
        {
            try
            {
                var (validAuthors, validationErrors) = await _authorService.ImportAuthorsFromExcel(excelFile);

                // Create authors in the database for valid entries
                foreach (var author in validAuthors)
                {
                    await _authorService.AddAuthor(author);
                }

                return Ok(new
                {
                    message = "Authors import completed.",
                    successfulAuthors = validAuthors,
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
