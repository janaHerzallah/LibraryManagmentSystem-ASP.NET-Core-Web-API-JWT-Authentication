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
using LibraryManagmentSystem.Contract.Requests;


namespace LibraryManagmentSystem.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IBookService _bookService;
        private readonly IUserService _userService;


        public BooksController(IBookService bookService, IUserService userService)
        {
            _bookService = bookService;
            _userService = userService;
        }

        [HttpGet]
        [Authorize(Roles ="Admin")]
        public async Task<ActionResult<IEnumerable<GetAllBooksResponse>>> GetActiveAndInActiveBooksByAdmin() //generic : it requires a special type
        { // IEnumerable is a collection of items that can be iterated over

            var books = await _bookService.GetActiveAndInactive();
            return Ok(books);

        }


        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<GetAllBooksResponse>>> GetAllBooks() //generic : it requires a special type
        { // IEnumerable is a collection of items that can be iterated over
            
            var books = await _bookService.GetAllBooks();
            return Ok(books);
           
        }

        [Authorize]
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
        //[Authorize(Roles = "Admin")] // This attribute will require users with the role "Admin" to access the endpoint
        public async Task<ActionResult<AddBookResponse>> AddBookByAdmin([FromBody] AddBookRequest book)
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

                var createdBook = await _bookService.AddBookByAdmin(book);
                return Ok(createdBook);
            }

            catch (Exception ex)
            {
                //unexpected condition that prevented the server from fulfilling the request.
                return StatusCode(500, new { message = ex.Message });
            }
        }


        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<AddBookResponse>> UpdateBookByAdmin(int id, [FromBody] UpdateBookRequest updatedBook)
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
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteBookByAdmin(int id)
        {
            var success = await _bookService.DeleteBookByAdmin(id);
            if (!success)
            {
                return NotFound(new { message = "Book not found or is inactive." });
            }
            return Ok(new {message = "Book has been successfullty deleted"});
        }

        // PATCH: api/Books/{id}/soft-delete
        [HttpPatch("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> SoftDeleteBookByAdmin(int id)
        {
            try
            {
                await _bookService.SoftDeleteBookByAdmin(id);
                return Ok(new {message = "The book has been successfully soft deleted "});
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}