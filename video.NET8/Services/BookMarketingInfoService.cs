using LibraryManagementSystem.Contract.Responses;
using LibraryManagementSystem.Data;
using LibraryManagementSystem.Domain;
using LibraryManagmentSystem.Contract.Responses;
using LibraryManagmentSystem.Interfaces;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;

namespace LibraryManagmentSystem.Services
{
    public class BookMarketingInfoService : IBookMarketingInfoService
    {
        private readonly IMongoCollection<BookMarketingInfo> _bookMarketingInfoCollection;
        private readonly ApplicationDbContext _context;


        public BookMarketingInfoService(MongoDbService mongoDbService, ApplicationDbContext context)
        {
            _bookMarketingInfoCollection = mongoDbService.BookMarketingInfo;
            _context = context;
        }

        public async Task<List<BookMarketingInfo>> GetAllMarketingInfo()
        {
            return await _bookMarketingInfoCollection.Find(_ => true).ToListAsync();
        }

        public async Task AddMarketingInfo(BookMarketingInfo info)
        {
            await _bookMarketingInfoCollection.InsertOneAsync(info);
        }


    public async Task<GetBookMarketingInfoResponse> addRecordUsingBookID(int bookId)

    {
        var book = await _context.Books
                .Include(b => b.Author) // Include the Author entity
                .Include(b => b.Category)
                .Include(b => b.LibraryBranch)
                .FirstOrDefaultAsync(b => b.Id == bookId);

            if (book == null)
            {
                throw new KeyNotFoundException("Book not found or is inactive.");
            }


            // Insert the BookMarketingInfo into MongoDB
            var bookMarketingInfo = new BookMarketingInfo
            {
                BookId = book.Id,
                Title = book.Title,
                Author = book.Author.Name,
                Category = book.Category.Name,
                LibraryBranch = book.LibraryBranch.Name,
                AvailableCopies = book.AvailableCopies,
                TimesBorrowed = 0
            };

            // Insert and await the result to get the ObjectId
            await _bookMarketingInfoCollection.InsertOneAsync(bookMarketingInfo);

            // Retrieve the ObjectId from the inserted document
            var objectId = bookMarketingInfo.Id;

            int timesBorrowed = _context.BookBorrows.Where(b => b.BookId == bookId).Count();


            return new GetBookMarketingInfoResponse
            {
                Id = objectId,
                Category = book.Category.Name,
                Author = book.Author.Name,
                Title = book.Title,
                AvailableCopies = book.AvailableCopies,
                LibraryBranch = book.LibraryBranch.Name,
                TimesBorrowed = timesBorrowed, // This is not implemented yet
                BookId = book.Id

            };
        }
        // Find BookMarketingInfo by ObjectId (MongoDB _id)
        public async Task<BookMarketingInfo> GetMarketingInfoByIdAsync(string objectId)
        {
            var filter = Builders<BookMarketingInfo>.Filter.Eq(b => b.Id, objectId);
            var result = await _bookMarketingInfoCollection.Find(filter).FirstOrDefaultAsync();

            if (result == null)
            {
                throw new KeyNotFoundException("BookMarketingInfo not found.");
            }

            return result;
        }

        // Update BookMarketingInfo by ObjectId
        public async Task UpdateMarketingInfoAsync(string objectId, BookMarketingInfo updatedInfo)
        {
            //the Builders<T> class helps construct  filters,
            var filter = Builders<BookMarketingInfo>.Filter.Eq(b => b.Id, objectId);
            var update = Builders<BookMarketingInfo>.Update
                .Set(b => b.Title, updatedInfo.Title)
                .Set(b => b.Author, updatedInfo.Author)
                .Set(b => b.Category, updatedInfo.Category)
                .Set(b => b.LibraryBranch, updatedInfo.LibraryBranch)
                .Set(b => b.TimesBorrowed, updatedInfo.TimesBorrowed)
                .Set(b => b.AvailableCopies, updatedInfo.AvailableCopies);

            var result = await _bookMarketingInfoCollection.UpdateOneAsync(filter, update);

            if (result.MatchedCount == 0)
            {
                throw new KeyNotFoundException("BookMarketingInfo not found or not updated.");
            }
        }

        // Delete BookMarketingInfo by ObjectId
        public async Task DeleteMarketingInfoAsync(string objectId)
        {
            var filter = Builders<BookMarketingInfo>.Filter.Eq(b => b.Id, objectId);
            var result = await _bookMarketingInfoCollection.DeleteOneAsync(filter);

            if (result.DeletedCount == 0)
            {
                throw new KeyNotFoundException("BookMarketingInfo not found or not deleted.");
            }
        }


        public async Task<List<BookMarketingInfo>> FindByTitleAsync(string title)
        {
            var filter = Builders<BookMarketingInfo>.Filter.Regex(b => b.Title, new MongoDB.Bson.BsonRegularExpression(title, "i"));
            return await _bookMarketingInfoCollection.Find(filter).ToListAsync();
        }

        public async Task<List<BookMarketingInfo>> FindByBranchAsync(string branch)
        {
            var filter = Builders<BookMarketingInfo>.Filter.Eq(b => b.LibraryBranch, branch);
            return await _bookMarketingInfoCollection.Find(filter).ToListAsync();
        }

        public async Task<List<BookMarketingInfo>> FindByAvailableCopiesAsync(int minCopies)
        {
            var filter = Builders<BookMarketingInfo>.Filter.Gte(b => b.AvailableCopies, minCopies);
            return await _bookMarketingInfoCollection.Find(filter).ToListAsync();
        }

    }
}