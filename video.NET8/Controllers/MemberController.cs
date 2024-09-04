using LibraryManagementSystem.Domain;
using LibraryManagementSystem.Interfaces;
using LibraryManagementSystem.Services;
using LibraryManagmentSystem.Contract.Requests;
using LibraryManagmentSystem.Contract.Responses;
using LibraryManagmentSystem.Interfaces;
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

        public MemberController(IMemberService memberService, IUserService userService)
        {
            _memberService = memberService;
            _userService = userService;
        }
        // authorize ? will it allow members even though i have a check for admin in the method ?
        // since there is an admin validation in the method it will outweight the authorize attribute 
        // and only allow admins to access the method
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Member>>> GetActiveAndInActiveMembersByAdmin()
        {
            try
            {
                var token = Request.Headers["Authorization"].ToString();
                var tokenValue = token?.StartsWith("Bearer ") == true ? token.Substring("Bearer ".Length).Trim() : token;

                if (!await _userService.ValidateAdminsToken(tokenValue))
                {
                    return Unauthorized(new { message = "Invalid token or unauthorized user" });
                }

                var members = await _memberService.GetActiveAndInActiveMembersAsync();
                return Ok(members);
            }
            catch (KeyNotFoundException  ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }


        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Member>>> GetAllMembersByAdmin()
        {
            try
            {
                var token = Request.Headers["Authorization"].ToString();
                var tokenValue = token?.StartsWith("Bearer ") == true ? token.Substring("Bearer ".Length).Trim() : token;

                if (!await _userService.ValidateAdminsToken(tokenValue))
                {
                    return Unauthorized(new { message = "Invalid token or unauthorized user" });
                }

                var members = await _memberService.GetAllMembersAsync();
                return Ok(members);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<Member>> GetMemberByIdByAdmin(int id)
        {
            var token = Request.Headers["Authorization"].ToString();
            var tokenValue = token?.StartsWith("Bearer ") == true ? token.Substring("Bearer ".Length).Trim() : token;

            if (!await _userService.ValidateAdminsToken(tokenValue))
            {
                return Unauthorized(new { message = "Invalid token or unauthorized user" });
            }

            try
            {

                var member = await _memberService.GetMemberByIdAsync(id);
                return Ok(member);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<GetMemberResponse>> AddMemberByAdmin([FromBody] AddMemberRequest member)
        {
        
            var addedMember = await _memberService.AddMemberAsync(member);
            return CreatedAtAction(nameof(GetMemberByIdByAdmin), new { id = addedMember.Id }, addedMember);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<GetMemberResponse>> UpdateMemberByAdmin(int id, [FromBody] UpdateMemeberRequest updatedMember)
        {
            

            var member = await _memberService.UpdateMemberAsync(id, updatedMember);
            return Ok(member);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteMemberByAdmin(int id)
        {
            
            var result = await _memberService.DeleteMemberAsync(id);
            if (!result)
            {
                return NotFound();
            }

            return Ok(new { message = "The member got successfully deleted , you can update its status from the update method" });
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> SoftDeleteMemberByAdmin(int id)
        {
            try
            {
                await _memberService.SoftDeleteMemberAsync(id);
                return Ok(new {message = "The member got successfully soft-deleted , you can update its status from the update method"});
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            
        }

        [HttpGet("borrowed-books")]
        [Authorize]
        public async Task<IActionResult> GetBorrowedBooksByMember(int id)
        {
            try
            {
                // Extract and validate token
                var token = Request.Headers["Authorization"].ToString();
                var tokenValue = token?.StartsWith("Bearer ") == true ? token.Substring("Bearer ".Length).Trim() : token;
                if (!await _userService.ValidateUsersToken(tokenValue))
                {
                    return Unauthorized(new { message = "Invalid token or unauthorized user" });
                }

                var books = await _memberService.GetBorrowedBooksByMemberAsync(id);
                return Ok(books);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
            
        }

        [HttpGet("borrowed-books/NOTReturnedYet")]
        [Authorize]
        public async Task<IActionResult> GetBorrowedBooksNotReturnedByMember(int id)
        {
            try
            {
                // Extract and validate token
                var token = Request.Headers["Authorization"].ToString();
                var tokenValue = token?.StartsWith("Bearer ") == true ? token.Substring("Bearer ".Length).Trim() : token;
                if (!await _userService.ValidateUsersToken(tokenValue))
                {
                    return Unauthorized(new { message = "Invalid token or unauthorized user" });
                }

                var books = await _memberService.GetBorrowedBooksNotReturnedByMemberAsync(id);
                return Ok(books);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            
        }
        [HttpGet("borrowed-books/overdue")]
        [Authorize]
        public async Task<IActionResult> GetBorrowedBooksOverDueByMember(int id)
        {
            try
            {
                // Extract and validate token
                var token = Request.Headers["Authorization"].ToString();
                var tokenValue = token?.StartsWith("Bearer ") == true ? token.Substring("Bearer ".Length).Trim() : token;
                if (!await _userService.ValidateUsersToken(tokenValue))
                {
                    return Unauthorized(new { message = "Invalid token or unauthorized user" });
                }

                var books = await _memberService.GetBorrowedBooksOverDuedByMember(id);
                return Ok(books);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }

        }

        [HttpGet("borrowed-books/overdue-count")]
        [Authorize]
        public async Task<IActionResult> GetOverdueBooksCountByMember(int id)
        {
            try
            {
                // Extract and validate token
                var token = Request.Headers["Authorization"].ToString();
                var tokenValue = token?.StartsWith("Bearer ") == true ? token.Substring("Bearer ".Length).Trim() : token;
                if (!await _userService.ValidateUsersToken(tokenValue))
                {
                    return Unauthorized(new { message = "Invalid token or unauthorized user" });
                }

                var count = await _memberService.GetOverdueBooksCountByMemberAsync(id);
                return Ok(count);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            
        }

    }
}
