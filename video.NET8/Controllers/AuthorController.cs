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
        public async Task<ActionResult<IEnumerable<GetAllAuthorsResponse>>> GetAllAuthors()
        {
            var authors = await _authorService.GetAllAuthors();
            return Ok(authors);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GetAuthorByIdResponse>> GetAuthorById(int id)
        {
            
                var author = await _authorService.GetAuthorById(id);
                return Ok(author);
       
            
        }

        // POST: api/authors
        [HttpPost]
        [Authorize(Roles = "Admin")] // This attribute will require users with the role "Admin" to access the endpoint

        public async Task<ActionResult<AddAuthorResponse>> AddAuthorByAdmin([FromBody] AddAuthorRequest authorRequest)
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
                var authorResponse = await _authorService.AddAuthorByAdmin(authorRequest);
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
        public async Task<ActionResult<UpdateAuthorResponse>> UpdateAuthorByAdmin(int id, [FromBody] UpdateAuthorRequest authorRequest)
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

                var authorResponse = await _authorService.UpdateAuthorByAdmin(id, authorRequest);
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
        public async Task<IActionResult> DeleteAuthorByAdmin(int id)
        {
            try
            {
                var token = Request.Headers["Authorization"].ToString();
                var tokenValue = token?.StartsWith("Bearer ") == true ? token.Substring("Bearer ".Length).Trim() : token;

                if (!await _userService.ValidateAdminsToken(tokenValue))
                {
                    return Unauthorized(new { message = "Invalid token or unauthorized user" });
                }

                

                var result = await _authorService.DeleteAuthorByAdmin(id);
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
        public async Task<IActionResult> SoftDeleteAuthorByAdmin(int id)
        {
            try
            {
                var token = Request.Headers["Authorization"].ToString();
                var tokenValue = token?.StartsWith("Bearer ") == true ? token.Substring("Bearer ".Length).Trim() : token;

                if (!await _userService.ValidateAdminsToken(tokenValue))
                {
                    return Unauthorized(new { message = "Invalid token or unauthorized user" });
                }

               
                await _authorService.SoftDeleteAuthorByAdmin(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }



    }
}
