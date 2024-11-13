using LibraryManagementSystem.Contract.Responses;
using LibraryManagementSystem.Domain;
using LibraryManagementSystem.Services;
using LibraryManagmentSystem.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class MarketingController : ControllerBase
    {
        private readonly IBookMarketingInfoService _bookMarketingInfoService;

        public MarketingController(IBookMarketingInfoService bookMarketingInfoService)
        {
            _bookMarketingInfoService = bookMarketingInfoService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllMarketingInfo()
        {
            var marketingInfo = await _bookMarketingInfoService.GetAllMarketingInfo();
            return Ok(marketingInfo);
        }

        // Add BookMarketingInfo with new information from the user
        [HttpPost]
        public async Task<IActionResult> AddMarketingInfo([FromBody] BookMarketingInfo info)
        {
            await _bookMarketingInfoService.AddMarketingInfo(info);
            return Ok(info);
        }

        // this method is get but it 
        [HttpGet("{id}")]
        public async Task<IActionResult> addRecordInMongoUsingBookID(int id)
        {
            var book = await _bookMarketingInfoService.addRecordUsingBookID(id);

            if (book == null)
                return NotFound();

            return Ok(book);
        }



        // Get BookMarketingInfo by ObjectId (MongoDB _id)
        [HttpGet("{objectId}")]
        public async Task<ActionResult<GetBookMarketingInfoResponse>> GetMarketingInfoByIdAsync(string objectId)
        {
            try
            {
                var bookMarketingInfo = await _bookMarketingInfoService.GetMarketingInfoByIdAsync(objectId);
                var response = new GetBookMarketingInfoResponse
                {
                    Id = bookMarketingInfo.Id,
                    Title = bookMarketingInfo.Title,
                    Author = bookMarketingInfo.Author,
                    Category = bookMarketingInfo.Category,
                    LibraryBranch = bookMarketingInfo.LibraryBranch,
                    AvailableCopies = bookMarketingInfo.AvailableCopies,
                    TimesBorrowed = 0, // Placeholder, modify later if needed
                    BookId = bookMarketingInfo.BookId
                };
                return Ok(response);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // Update BookMarketingInfo by ObjectId
        [HttpPut("{objectId}")]
        public async Task<IActionResult> UpdateMarketingInfoAsync(string objectId, [FromBody] BookMarketingInfo updatedInfo)
        {
            try
            {
                await _bookMarketingInfoService.UpdateMarketingInfoAsync(objectId, updatedInfo);
                return Ok(); // 204 No Content
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // Delete BookMarketingInfo by ObjectId
        [HttpDelete("{objectId}")]
        public async Task<IActionResult> DeleteMarketingInfoAsync(string objectId)
        {
            try
            {
                await _bookMarketingInfoService.DeleteMarketingInfoAsync(objectId);
                return Ok(); // 204 No Content
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }


        [HttpGet("FindByTitle")]
        public async Task<IActionResult> FindByTitle([FromQuery] string title)
        {
            var results = await _bookMarketingInfoService.FindByTitleAsync(title);
            return Ok(results);
        }

        [HttpGet("FindByBranch")]
        public async Task<IActionResult> FindByBranch([FromQuery] string branch)
        {
            var results = await _bookMarketingInfoService.FindByBranchAsync(branch);
            return Ok(results);
        }

        [HttpGet("FindByAvailableCopies")]
        public async Task<IActionResult> FindByAvailableCopies([FromQuery] int minCopies)
        {
            var results = await _bookMarketingInfoService.FindByAvailableCopiesAsync(minCopies);
            return Ok(results);
        }

    }
}
