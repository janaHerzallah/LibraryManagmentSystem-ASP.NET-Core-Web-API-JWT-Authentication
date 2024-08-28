using LibraryManagmentSystem.Interfaces;
using LibraryManagementSystem.Domain;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using NpgsqlTypes;
using LibraryManagementSystem.Contract.Requests;
using LibraryManagmentSystem.Contract.Responses;
using Microsoft.AspNetCore.Authorization;



using LibraryManagementSystem.Services;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;


namespace LibraryManagmentSystem.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IBookService _bookService;
        private readonly IUserService _userService;


        public BooksController(IBookService bookService , IUserService userService)
        {
            _bookService = bookService;
            _userService = userService;
        }

        [HttpGet("GetAllBooks")]
        public async Task<ActionResult<IEnumerable<GetAllBooksResponse>>> GetAllBooks()
        {
            var books = await _bookService.GetAllBooks();
            return Ok(books);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<GetBookByIdResponse>> GetBookById(int id)
        {
            try
            {
                var book = await _bookService.GetBookById(id);

               

                return Ok(book);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }



        [HttpPost("AddBookByAdmin")]
        [Authorize(Roles = "Admin")] // This attribute will require users with the role "Admin" to access the endpoint
        public async Task<ActionResult<AddBookResponse>> AddBook([FromBody] AddBookRequest book)
        {

            try
            {

                // get the token fro the authorization header

                var token = Request.Headers["Authorization"].ToString();
                // Extract the token value from the "Bearer " prefix
                var tokenValue = token?.StartsWith("Bearer ") == true ? token.Substring("Bearer ".Length).Trim() : token;

                // Validate the token using the service method
                if (!await _userService.ValidateToken(tokenValue))
                {
                    return Unauthorized(new { message = "Invalid token or unauthorized user" });
                }

                var createdBook = await _bookService.AddBookByAdmin(book);
                return Ok(createdBook);
            }

            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }


        [HttpPut("{id}")]
        public async Task<ActionResult<Book>> UpdateBook(int id, [FromBody] Book updatedBook)
        {
            try
            {
                var book = await _bookService.UpdateBookByAdmin(id, updatedBook);
                return Ok(book);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }


        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteBook(int id)
        {
            var success = await _bookService.DeleteBookByAdmin(id);
            if (!success)
            {
                return NotFound(new { message = "Book not found or is inactive." });
            }
            return NoContent();
        }

        // PATCH: api/Books/{id}/soft-delete
        [HttpPatch("{id}")]
        public async Task<ActionResult> SoftDeleteBook(int id)
        {
            try
            {
                await _bookService.SoftDeleteBookByAdmin(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}
