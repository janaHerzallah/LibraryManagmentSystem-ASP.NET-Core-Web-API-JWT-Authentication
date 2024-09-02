using LibraryManagementSystem.Domain;
using LibraryManagementSystem.Interfaces;
using LibraryManagementSystem.Services;
using LibraryManagmentSystem.Contract.Requests;
using LibraryManagmentSystem.Contract.Responses;
using LibraryManagmentSystem.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Member>>> GetAllMembersByAdmin()
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

            var member = await _memberService.GetMemberByIdAsync(id);
            return Ok(member);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<GetMemberResponse>> AddMemberByAdmin([FromBody] AddMemberRequest member)
        {
            var token = Request.Headers["Authorization"].ToString();
            var tokenValue = token?.StartsWith("Bearer ") == true ? token.Substring("Bearer ".Length).Trim() : token;

            if (!await _userService.ValidateAdminsToken(tokenValue))
            {
                return Unauthorized(new { message = "Invalid token or unauthorized user" });
            }

            var addedMember = await _memberService.AddMemberAsync(member);
            return CreatedAtAction(nameof(GetMemberByIdByAdmin), new { id = addedMember.Id }, addedMember);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<GetMemberResponse>> UpdateMemberByAdmin(int id, [FromBody] UpdateMemeberRequest updatedMember)
        {
            var token = Request.Headers["Authorization"].ToString();
            var tokenValue = token?.StartsWith("Bearer ") == true ? token.Substring("Bearer ".Length).Trim() : token;

            if (!await _userService.ValidateAdminsToken(tokenValue))
            {
                return Unauthorized(new { message = "Invalid token or unauthorized user" });
            }

            var member = await _memberService.UpdateMemberAsync(id, updatedMember);
            return Ok(member);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteMemberByAdmin(int id)
        {
            var token = Request.Headers["Authorization"].ToString();
            var tokenValue = token?.StartsWith("Bearer ") == true ? token.Substring("Bearer ".Length).Trim() : token;

            if (!await _userService.ValidateAdminsToken(tokenValue))
            {
                return Unauthorized(new { message = "Invalid token or unauthorized user" });
            }

            var result = await _memberService.DeleteMemberAsync(id);
            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> SoftDeleteMemberByAdmin(int id)
        {
            var token = Request.Headers["Authorization"].ToString();
            var tokenValue = token?.StartsWith("Bearer ") == true ? token.Substring("Bearer ".Length).Trim() : token;

            if (!await _userService.ValidateAdminsToken(tokenValue))
            {
                return Unauthorized(new { message = "Invalid token or unauthorized user" });
            }

            await _memberService.SoftDeleteMemberAsync(id);
            return NoContent();
        }

        [HttpGet("borrowed-books")]
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
                return StatusCode(500, new { message = ex.Message });
            }
            
        }

        [HttpGet("borrowed-books/overdue")]
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
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
            
        }

        [HttpGet("borrowed-books/overdue-count")]
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
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
            
        }

    }
}
