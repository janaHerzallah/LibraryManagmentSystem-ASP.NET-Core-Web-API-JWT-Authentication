using LibraryManagementSystem.Interfaces;
using LibraryManagementSystem.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LibraryManagmentSystem.Interfaces;
using LibraryManagmentSystem.Contract.Requests;
using LibraryManagmentSystem.Contract.Responses;

namespace LibraryManagementSystem.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class BorrowsController : ControllerBase
    {
        private readonly IBorrowService _borrowService;
        

        public BorrowsController(IBorrowService borrowService)
        {
            _borrowService = borrowService;
            
        }

        [HttpPost]
        [Authorize] // Requires the user to be authenticated
        public async Task<ActionResult<BorrowBookResponse>> BorrowBook([FromBody] BorrowBookRequest request)
        {
            try
            {
                // Extract and validate token
                var token = Request.Headers["Authorization"].ToString();
                var tokenValue = token?.StartsWith("Bearer ") == true ? token.Substring("Bearer ".Length).Trim() : token;
                

                var borrow = await _borrowService.BorrowBook(request, tokenValue);
                return Ok(borrow);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost]
        [Authorize] // Requires the user to be authenticated
        public async Task<ActionResult<ReturnBookResponse>> ReturnBook([FromBody] ReturnBookRequest request)
        {
            try
            {
                // Extract and validate token
                var token = Request.Headers["Authorization"].ToString();
                var tokenValue = token?.StartsWith("Bearer ") == true ? token.Substring("Bearer ".Length).Trim() : token;
                
                var borrow = await _borrowService.ReturnBook(request.MemberId, request.BookId, tokenValue);
                return Ok(borrow);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("{memberId}")]
        [Authorize] // Requires the user to be authenticated
        public async Task<ActionResult<IEnumerable<GetBorrowedBooksForAMemberResponse>>> GetMembersBorrowedBooks(int memberId)
        { 
            try
            {
                // Extract and validate token
                var token = Request.Headers["Authorization"].ToString();
                var tokenValue = token?.StartsWith("Bearer ") == true ? token.Substring("Bearer ".Length).Trim() : token;
                
                var borrows = await _borrowService.GetMembersBorrowedBooks(memberId, tokenValue);
                return Ok(borrows);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
