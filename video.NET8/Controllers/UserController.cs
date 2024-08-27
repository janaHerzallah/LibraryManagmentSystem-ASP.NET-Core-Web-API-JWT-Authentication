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
    [Route("api/[controller]")]
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

        [HttpGet("ViewMembersByAdmin")]
        [Authorize(Roles = "Admin")] // This attribute will require users with the role "Admin" to access the endpoint

        public async Task<IActionResult> GetUsers()
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

                // Retrieve and return the list of members
                var members = await _userService.GetMembersByAdminOnly();
                return Ok(members);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }







    }
}
