using LibraryManagementSystem.Contract.Responses;
using LibraryManagementSystem.Domain;
using LibraryManagmentSystem.Contract.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace LibraryManagmentSystem.Interfaces
{
    public interface IBookMarketingInfoService
    {
        Task<List<BookMarketingInfo>> GetAllMarketingInfo();
        Task AddMarketingInfo(BookMarketingInfo info);

        Task<GetBookMarketingInfoResponse> addRecordUsingBookID(int bookId);

        Task<BookMarketingInfo> GetMarketingInfoByIdAsync(string objectId);

        Task UpdateMarketingInfoAsync(string objectId, BookMarketingInfo updatedInfo);

        Task DeleteMarketingInfoAsync(string objectId);
        
        Task<List<BookMarketingInfo>> FindByTitleAsync(string title);
        Task<List<BookMarketingInfo>> FindByBranchAsync(string branch);
        Task<List<BookMarketingInfo>> FindByAvailableCopiesAsync(int minCopies);
        
    }
}
