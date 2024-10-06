using LibraryManagementSystem.Interfaces;
using LibraryManagementSystem.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LibraryManagmentSystem.Interfaces;
using LibraryManagmentSystem.Contract.Requests;
using LibraryManagmentSystem.Contract.Responses;
using LibraryManagementSystem.Services;
using LibraryManagmentSystem.Services;

namespace LibraryManagementSystem.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class BorrowsController : ControllerBase
    {
        private readonly IBorrowService _borrowService;
        private readonly IExcelService _excelService;
        

        public BorrowsController(IBorrowService borrowService , IExcelService excelService)
        {
            _borrowService = borrowService;
            _excelService = excelService;
            
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



        // Export data to Excel
        [HttpGet("{memberId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ExportAMembersBorrowedBooks(int memberId)
        {

            try
            {
                // Extract and validate token
                var token = Request.Headers["Authorization"].ToString();
                var tokenValue = token?.StartsWith("Bearer ") == true ? token.Substring("Bearer ".Length).Trim() : token;

                var borrows = await _borrowService.GetMembersBorrowedBooks(memberId, tokenValue);

               
                var fileContent = _excelService.GenerateExcelSheet(borrows, "ReportOfMembersBorrowedBooks");

                return File(fileContent, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ReportOfMembersBorrowedBooks.xlsx");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }

       
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ImportBorrowsFromExcel(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new { message = "File is required." });
            }

            try
            {
                // Extract and validate token
                var token = Request.Headers["Authorization"].ToString();
                var tokenValue = token?.StartsWith("Bearer ") == true ? token.Substring("Bearer ".Length).Trim() : token;

                // Import valid borrows and validation errors from the Excel file
                var (validBorrows, validationErrors) = await _borrowService.ImportBorrowsFromExcel(file);

                // Create borrows in the database for valid entries
                foreach (var borrow in validBorrows)
                {
                    // create another function that adds borrow rows to the database
                     await _borrowService.AddBorrowRecordfromExcel(borrow);
                }

                return Ok(new
                {
                    message = "Borrows import completed.",
                    successfulBorrows = validBorrows,
                    validationErrors = validationErrors
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                // Log the exception (consider using a logging framework)
                return StatusCode(500, new { message = "An error occurred while importing borrows.", details = ex.Message });
            }
        }


        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<ExcelBorrowBookResponse>>> ExportAllBorrowsTOExcel()
        {
            try
            {

                var borrows = await _borrowService.ExportAllBorrowsTOExcel();
                var fileContent = _excelService.GenerateExcelSheet(borrows, "ReportOfAllBorrows");

                return File(fileContent, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ReportOfMembersBorrowedBooks.xlsx");
                
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

    }
}
