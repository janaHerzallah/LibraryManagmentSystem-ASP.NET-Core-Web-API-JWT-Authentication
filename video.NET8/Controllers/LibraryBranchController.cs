﻿using LibraryManagementSystem.Domain;
using LibraryManagementSystem.Interfaces;
using LibraryManagmentSystem.Contract.Requests;
using LibraryManagmentSystem.Contract.Responses;
using LibraryManagmentSystem.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementSystem.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class LibraryBranchController : ControllerBase
    {
        private readonly ILibraryBranchService _libraryBranchService;


        public LibraryBranchController(ILibraryBranchService libraryBranchService)
        {
            _libraryBranchService = libraryBranchService;
        }

        [Authorize]
        [HttpGet] // admin and member can access this endpoint
        public async Task<ActionResult<IEnumerable<GetLibraryBranchResponse>>> GetActiveBranches()
        {
            var branches = await _libraryBranchService.GetActiveBranches();
            return Ok(branches);
        }
        [Authorize(Roles ="Admin")]
        [HttpGet] // admin and member can access this endpoint
        public async Task<ActionResult<IEnumerable<GetLibraryBranchResponse>>> GetActiveAndInActiveBranches()
        {
            var branches = await _libraryBranchService.GetActiveAndInActiveBranches();
            return Ok(branches);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<GetLibraryBranchResponse>> GetBranchById(int id)
        {
            try
            {
                var branch = await _libraryBranchService.GetBranchById(id);

                return Ok(branch);
            }
            catch (Exception EX)
            {
                return StatusCode(500,new { message = EX.Message });

            }
           
        }

        [HttpPost]
        [Authorize(Roles = "Admin")] // Only admins can add branches
        public async Task<ActionResult<AddLibraryBranchResponse>> AddBranchByAdmin(AddLibraryBranchRequest branch)
        {
            try
            {
                var newBranch = await _libraryBranchService.AddBranchByAdmin(branch);
                return Ok(newBranch);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
            
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")] // Only admins can update branches
        public async Task<ActionResult<UpdateLibraryBranchResponse>> UpdateBranchByAdmin(int id, UpdateLibraryBranchRequest branch)
        {
            try
            {
                var updatedBranch = await _libraryBranchService.UpdateBranchByAdmin(id, branch);
                return Ok(updatedBranch);
            }
            
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")] // Only admins can delete branches
        public async Task<IActionResult> DeleteBranch(int id)
        {
            try
            {
                var result = await _libraryBranchService.DeleteBranchByAdmin(id);
                return Ok(new { message = "The branch has been successfully  deleted" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
            
        }

        [HttpPatch("{id}")]
        [Authorize(Roles = "Admin")] // Only admins can soft delete branches
        public async Task<IActionResult> SoftDeleteBranchByAdmin(int id)
        {
            try
            {
                await _libraryBranchService.SoftDeleteBranchByAdmin(id);
                return Ok(new { message = "The branch has been successfully soft-deleted" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
            
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> AssignBookToBranchByAdmin(int branchId, int bookId)
        {
            try
            {
                await _libraryBranchService.AssignBookToBranchByAdmin(branchId, bookId);
                return Ok(new { message = "Book assigned to branch successfully." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("branches/{branchId}/books")]
        [Authorize] // members and admins can access this endpoint
        public async Task<ActionResult<IEnumerable<GetBooksDetailsResponse>>> GetBooksInBranch(int branchId)
        {
            try
            {
                var books = await _libraryBranchService.GetBooksInBranch(branchId);
                return Ok(books);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete]
        [Authorize(Roles = "Admin")] // Ensure only Admin can perform this operation
        public async Task<IActionResult> RemoveBookFromBranchByAdmin(int bookId, int branchId)
        {

            try
            {
                var result = await _libraryBranchService.RemoveBookFromBranchByAdmin(bookId, branchId);
                if (result)
                {
                    return Ok(new { Message = "Book successfully removed from branch." });

                }
                else
                {
                    return NotFound(new { Message = "Book not found in branch." });
                }
            }

            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }

        }

    }
}
