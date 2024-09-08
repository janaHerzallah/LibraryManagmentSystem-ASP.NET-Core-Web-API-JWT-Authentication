using LibraryManagementSystem.Domain;
using LibraryManagementSystem.Services;
using LibraryManagmentSystem.Contract.Requests;
using LibraryManagmentSystem.Contract.Responses;
using LibraryManagmentSystem.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagmentSystem.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthorsController : ControllerBase
    {
        private readonly IAuthorService _authorService;
        private readonly IUserService _userService;


        public AuthorsController(IAuthorService authorService, IUserService userService)
        {
            _authorService = authorService;
            _userService = userService;
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



    }
}
