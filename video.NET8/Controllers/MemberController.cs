using LibraryManagementSystem.Domain;
using LibraryManagementSystem.Interfaces;
using LibraryManagementSystem.Services;
using LibraryManagmentSystem.Contract.Requests;
using LibraryManagmentSystem.Contract.Responses;
using LibraryManagmentSystem.Interfaces;
using LibraryManagmentSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace LibraryManagementSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class MemberController : ControllerBase
    {
        private readonly IMemberService _memberService;
        private readonly IUserService _userService;
        private readonly IExcelService _excelService;

        public MemberController(IMemberService memberService, IUserService userService, IExcelService excelService)
        {
            _memberService = memberService;
            _userService = userService;
            _excelService = excelService;
        }
        // authorize ? will it allow members even though i have a check for admin in the method ?
        // since there is an admin validation in the method it will outweight the authorize attribute 
        // and only allow admins to access the method
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<Member>>> GetAllMembers()
        {
            try
            {

                var members = await _memberService.GetAllMembers();
                return Ok(members);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }


        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<Member>>> GetActiveMembers()
        {
            try
            {
                var members = await _memberService.GetActiveMembers();
                return Ok(members);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }

        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Member>> GetMemberById(int id)
        {
            try
            {

                var member = await _memberService.GetMemberById(id);
                return Ok(member);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<GetMemberResponse>> AddMember([FromBody] AddMemberRequest member)
        {

            var addedMember = await _memberService.AddMember(member);
            return Ok(addedMember);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<GetMemberResponse>> UpdateMember(int id, [FromBody] UpdateMemeberRequest updatedMember)
        {
            try
            {
                var member = await _memberService.UpdateMember(id, updatedMember);
                return Ok(member);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteMember(int id)
        {
            try
            {
                var result = await _memberService.DeleteMember(id);
                if (!result)
                {
                    return NotFound(new { message = "Member not found." });
                }

                return Ok(new { message = "The member was successfully deleted." });
            }
            catch (Exception ex)
            {
                // Log the exception (you can use a logging framework like Serilog, NLog, etc.)
                // Return a generic error message to avoid exposing details of the exception
                return StatusCode(500, new { message = "An error occurred while processing your request.", details = ex.Message });
            }
        }


        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> SoftDeleteMember(int id)
        {
            try
            {
                await _memberService.SoftDeleteMember(id);
                return Ok(new { message = "The member got successfully soft-deleted , you can update its status from the update method" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }

        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetMembersAllBorrowedBooks(int id)
        {
            try
            {
                // Extract and validate token
                var token = Request.Headers["Authorization"].ToString();
                var tokenValue = token?.StartsWith("Bearer ") == true ? token.Substring("Bearer ".Length).Trim() : token;
               

                var books = await _memberService.GetMembersAllBorrowedBooks(id, tokenValue);
                return Ok(books);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
            
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetNotReturnedBooks(int id)
        {
            try
            {
                // Extract and validate token
                var token = Request.Headers["Authorization"].ToString();
                var tokenValue = token?.StartsWith("Bearer ") == true ? token.Substring("Bearer ".Length).Trim() : token;
                

                var books = await _memberService.GetNotReturnedBooks(id, tokenValue);
                return Ok(books);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            
        }
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetOverDueBorrowedBooks(int id)
        {
            try
            {
                // Extract and validate token
                var token = Request.Headers["Authorization"].ToString();
                var tokenValue = token?.StartsWith("Bearer ") == true ? token.Substring("Bearer ".Length).Trim() : token;
             

                var books = await _memberService.GetOverDueBorrowedBooks(id, tokenValue);
                return Ok(books);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }

        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetOverdueBooksCount(int id)
        {
            try
            {
                // Extract and validate token
                var token = Request.Headers["Authorization"].ToString();
                var tokenValue = token?.StartsWith("Bearer ") == true ? token.Substring("Bearer ".Length).Trim() : token;
               

                var count = await _memberService.GetOverdueBooksCount(id, tokenValue);
                return Ok(count);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            
        }


        // Export data to Excel
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ExportAllMembersToExcel()
        {
            var AllMembers = await _memberService.GetAllMembers();

            var fileContent = _excelService.GenerateExcelSheet(AllMembers, "ReportOfAllMembers");

            return File(fileContent, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ReportOfAllMembers.xlsx");
        }

        // Export data to Excel
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ExportActiveMembersToExcel()
        {
            var ActiveMembers = await _memberService.GetActiveMembers();

            var fileContent = _excelService.GenerateExcelSheet(ActiveMembers, "ReportOfActiveMembers");

            return File(fileContent, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ReportOfActiveMembers.xlsx");
        }
    }
}
