﻿using LibraryManagementSystem.Domain;
using LibraryManagementSystem.Interfaces;
using LibraryManagmentSystem.Contract.Requests;
using LibraryManagmentSystem.Contract.Responses;
using LibraryManagmentSystem.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LibraryBranchController : ControllerBase
    {
        private readonly ILibraryBranchService _libraryBranchService;

        public LibraryBranchController(ILibraryBranchService libraryBranchService)
        {
            _libraryBranchService = libraryBranchService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetLibraryBranchResponse>>> GetAllBranches()
        {
            var branches = await _libraryBranchService.GetAllBranchesAsync();
            return Ok(branches);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GetLibraryBranchResponse>> GetBranchById(int id)
        {
            var branch = await _libraryBranchService.GetBranchByIdAsync(id);
            if (branch == null)
            {
                return NotFound();
            }
            return Ok(branch);
        }

        [HttpPost]
        [Authorize] // Only admins can add branches
        public async Task<ActionResult<AddLibraryBranchResponse>> AddBranch(AddLibraryBranchRequest branch)
        {
            var newBranch = await _libraryBranchService.AddBranchAsync(branch);
            return Ok(newBranch);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")] // Only admins can update branches
        public async Task<IActionResult> UpdateBranch(int id, UpdateLibraryBranchRequest branch)
        {
            var updatedBranch = await _libraryBranchService.UpdateBranchAsync(id, branch);
            if (updatedBranch == null)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")] // Only admins can delete branches
        public async Task<IActionResult> DeleteBranch(int id)
        {
            var result = await _libraryBranchService.DeleteBranchAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpPatch("{id}/soft-delete")]
        [Authorize(Roles = "Admin")] // Only admins can soft delete branches
        public async Task<IActionResult> SoftDeleteBranch(int id)
        {
            await _libraryBranchService.SoftDeleteBranchAsync(id);
            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("assignBookToBranch")]
        public async Task<IActionResult> AssignBookToBranch(int branchId, int bookId)
        {
            try
            {
                await _libraryBranchService.AssignBookToBranchAsync(branchId, bookId);
                return Ok(new { message = "Book assigned to branch successfully." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("branches/{branchId}/books")]
        public async Task<ActionResult<IEnumerable<GetAuthorsBookResponse>>> GetBooksInBranch(int branchId)
        {
            try
            {
                var books = await _libraryBranchService.GetBooksInBranchAsync(branchId);
                return Ok(books);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("remove-from-branch")]
        [Authorize(Roles = "Admin")] // Ensure only Admin can perform this operation
        public async Task<IActionResult> RemoveBookFromBranch(int bookId, int branchId)
        {
            var result = await _libraryBranchService.RemoveBookFromBranchAsync(bookId, branchId);

            if (!result)
            {
                return NotFound(new { Message = "Book not found in the specified branch." });
            }

            return Ok(new { Message = "Book successfully removed from branch." });
        }

    }
}
