﻿using LibraryManagmentSystem.Contract.Requests;
using LibraryManagementSystem.Domain;
using LibraryManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using LibraryManagmentSystem.Interfaces;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagmentSystem.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize] // This attribute will require authentication to access the controller
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService registerUserService)
        {
            _userService = registerUserService;
        }

        [HttpPost("register")]
        [AllowAnonymous] // Allow anonymous users to access the register endpoint
        public async Task<IActionResult> Register([FromBody] RegisterUserRequest request)
        {
            try
            {
                var user = await _userService.RegisterUser(request);
                return Ok(new { message = "User registered successfully", user });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("login")]
        [AllowAnonymous] // Allow anonymous users to access the login endpoint
        public async Task<IActionResult> LogIn([FromBody] LogInUserRequest request)
        {
            try
            {
                var user = await _userService.LogInUser(request);
                return Ok(new { message = "User logged in successfully", user });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin")] // This attribute will require users with the role "Admin" to access the endpoint

        public async Task<IActionResult> GetActiveMembersByAdmin()
        {
            try
            {
                // Retrieve and return the list of members
                var members = await _userService.GetActiveMembersByAdminOnly();
                return Ok(members);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin")] // no need to pass the token as a parameter since its only allowed to be accessed by admins
        public async Task<IActionResult> GetActiveAndInActiveMembersByAdminOnly()
        {
            var users = await _userService.GetActiveAndInActiveMembersByAdminOnly();
            return Ok(users);
        }

        [HttpPut]
        [Authorize(Roles = "Admin")] // no need to pass the token as a parameter since its only allowed to be accessed by admins
        public async Task<IActionResult> DeactivateUserByAdmin(ActivateDeActivateUserRequest request)
        {
            try
            {
                var response = await _userService.DeactivateUser(request);
                return Ok(response);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPut]
        [Authorize(Roles = "Admin")] // no need to pass the token as a parameter since its only allowed to be accessed by admins
        public async Task<IActionResult> ReActivateUserByAdmin(ActivateDeActivateUserRequest request)
        {
            try
            {
                var response = await _userService.ReActivateUser(request);
                return Ok(response);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

    }
}
