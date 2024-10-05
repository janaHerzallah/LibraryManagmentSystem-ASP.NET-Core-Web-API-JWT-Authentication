using LibraryManagementSystem.Data;
using LibraryManagementSystem.Domain;
using LibraryManagementSystem.Interfaces;
using LibraryManagmentSystem.Contract.Requests;
using LibraryManagmentSystem.Contract.Responses;
using LibraryManagmentSystem.Interfaces;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace LibraryManagementSystem.Services
{
    public class BorrowService : IBorrowService
    {
        private readonly ApplicationDbContext _context;
        private readonly IUserService _userService;

        public BorrowService(ApplicationDbContext context , IUserService userService)
        {
            _context = context;
            _userService = userService;
        }

        public async Task<BorrowBookResponse> BorrowBook(BorrowBookRequest request, string token)
        {
            // Check if the token belongs to an admin
            var isAdmin = await _userService.ValidateAdminsToken(token);

            // If not an admin, check if the token belongs to the member
            if (!isAdmin)
            {
                var member = await _context.Members
                    .Include(m => m.User)
                    .FirstOrDefaultAsync(m => m.Id == request.MemberId && m.Active);

                if (member == null)
                {
                    throw new Exception("Member not found.");
                }

                if (!member.User.Token.Equals(token))
                {
                    throw new UnauthorizedAccessException("Unauthorized access. You can only return the book for yourself not for anyone else.");
                }

                // this code segment has been placed here 
                // to check if the member has been late more than 4 times
                // out side the member will out of scope of the member

                if (member.OverDueCount > 4) // Can't borrow if late more than 4 times
                {
                    throw new Exception("You can't borrow a book because you have been late more than 4 times.");
                }
            }

            var book = await _context.Books.FirstOrDefaultAsync(b => b.Id == request.BookId && b.Active);

            if (book == null || book.AvailableCopies < 1)
            {
                throw new Exception("Book not available for borrowing.");
            }

            // **Restriction: Claimed return date must be at least 2 days ahead**
            if ((request.ClaimedReturnDate - DateTime.UtcNow).TotalDays < 2)
            {
                throw new ArgumentException("The claimed return date must be at least 2 days from now.");
            }

            if ((request.ClaimedReturnDate - DateTime.UtcNow).TotalDays > 15)
            {
                throw new ArgumentException("You can't borrow a book for more than 15 days.");
            }

            // **Check if the book was already borrowed by the member and is still active (i.e., not returned)**
            var activeBorrowRecord = await _context.BookBorrows
                .FirstOrDefaultAsync(b => b.BookId == request.BookId && b.MemberId == request.MemberId && b.Active);

            if (activeBorrowRecord != null)
            {
                throw new InvalidOperationException("You already have an active borrow record for this book.");
            }

            // All conditions met, proceed with borrowing the book
            book.AvailableCopies -= 1;

            var borrowRecord = new Borrow
            {
                MemberId = request.MemberId,
                BookId = request.BookId,
                BorrowDate = DateTime.UtcNow,
                Active = true,
                DateCreated = DateTime.UtcNow,
                DateModified = DateTime.UtcNow,
                ClaimedReturnDate = request.ClaimedReturnDate
            };

            _context.BookBorrows.Add(borrowRecord);
            _context.Books.Update(book);
            await _context.SaveChangesAsync();

            return new BorrowBookResponse
            {
                bookId = borrowRecord.Id,
                MemberId = borrowRecord.MemberId,
                BorrowDate = borrowRecord.BorrowDate,
                DateCreated = borrowRecord.DateCreated,
                DateModified = borrowRecord.DateModified,
                availableCopies = book.AvailableCopies,
                Title = book.Title,
                ClaimedReturnDate = borrowRecord.ClaimedReturnDate
            };
        }


        public async Task<ReturnBookResponse> ReturnBook(int memberId, int bookId, string token)
        {
            // Check if the token belongs to an admin
            var isAdmin = await _userService.ValidateAdminsToken(token);

            // If not an admin, check if the token belongs to the member
            if (!isAdmin)
            {
                var member = await _context.Members
                    .Include(m => m.User)
                    .FirstOrDefaultAsync(m => m.Id == memberId && m.Active);

                if (member == null)
                {
                    throw new Exception("Member not found.");
                }

                if (!member.User.Token.Equals(token))
                {
                    throw new UnauthorizedAccessException("Unauthorized access. You can only return the book for yourself not for anyone else.");
                }
            }

            // Proceed with the return book logic
            var borrowRecord = await _context.BookBorrows.FirstOrDefaultAsync(b => b.MemberId == memberId && b.BookId == bookId && b.Active);
            if (borrowRecord == null)
            {
                throw new Exception("Borrow record not found or its already returned");
            }

            var book = await _context.Books.FirstOrDefaultAsync(b => b.Id == bookId && b.Active);
            if (book == null)
            {
                throw new Exception("Book record not found.");
            }

            borrowRecord.ActualReturnDate = DateTime.UtcNow;
            borrowRecord.Active = false; // set active to false means its not borrowed any more so its returned
            book.AvailableCopies += 1;
            book.DateModified = DateTime.UtcNow;

            _context.BookBorrows.Update(borrowRecord);
            _context.Books.Update(book);
            await _context.SaveChangesAsync();

            // Check for overdue
            if (borrowRecord.ActualReturnDate > borrowRecord.ClaimedReturnDate)
            {
                var member = await _context.Members.FirstOrDefaultAsync(m => m.Id == memberId);
                if (member != null)
                {
                    member.OverDueCount += 1;
                    _context.Members.Update(member);
                    await _context.SaveChangesAsync();
                }
            }

          

            return new ReturnBookResponse
            {
                bookId = borrowRecord.Id,
                MemberId = borrowRecord.MemberId,
                BorrowDate = borrowRecord.BorrowDate,
                ReturnDate = borrowRecord.ActualReturnDate == DateTime.MinValue ? "Not Returned" : borrowRecord.ActualReturnDate.ToString("yyyy-MM-dd"),
                DateCreated = borrowRecord.DateCreated,
                DateModified = borrowRecord.DateModified,
                availableCopies = book.AvailableCopies,
                Title = book.Title,
                ClaimedReturnDate = borrowRecord.ClaimedReturnDate,
                
            };
        }


        public async Task<IEnumerable<GetBorrowedBooksForAMemberResponse>> GetMembersBorrowedBooks(int memberId, string token)
        {
            // get all borrowed books either its retruned or not // so even if its active or not
            // validate if its an admin or if its a member it should be the same member requesting the service for 


            // Check if the token belongs to an admin
            var isAdmin = await _userService.ValidateAdminsToken(token);

            // If not an admin, check if the token belongs to the member
            if (!isAdmin)
            {
                var member = await _context.Members
                    .Include(m => m.User)
                    .FirstOrDefaultAsync(m => m.Id == memberId );

                if (member == null)
                {
                    throw new Exception("Member not found.");
                }

                if (!member.User.Token.Equals(token))
                {
                    throw new UnauthorizedAccessException("Unauthorized access. You can only return the book for yourself not for anyone else.");
                }
            }

            return await _context.BookBorrows
                         .Where(b => b.MemberId == memberId )
                         .Include(b => b.Book)
                         .Select(b => new GetBorrowedBooksForAMemberResponse
                         {
                             MemberId = b.MemberId,
                             bookId = b.BookId,
                             BorrowDate = b.BorrowDate,
                             ReturnDate = b.ActualReturnDate == DateTime.MinValue ? "Not Returned" : b.ActualReturnDate.ToString("yyyy-MM-dd"),
                             DateCreated = b.DateCreated,
                             DateModified = b.DateModified,
                             availableCopies = b.Book.AvailableCopies,
                             Title = b.Book.Title ,
                             ClaimedReturnDate = b.ClaimedReturnDate
                         })
                         .ToListAsync();
        }




        public async Task<(List<Borrow> validBorrows, List<ValidationErrorBorrowResponse> validationErrors)> ImportBorrowsFromExcel(IFormFile excelFile)
        {
            if (excelFile == null || excelFile.Length == 0)
            {
                throw new ArgumentException("No file uploaded or file is empty.");
            }

            var validBorrows = new List<Borrow>();
            var validationErrorList = new List<ValidationErrorBorrowResponse>();

            using (var stream = new MemoryStream())
            {
                await excelFile.CopyToAsync(stream);
                using (var package = new ExcelPackage(stream))
                {
                    var worksheet = package.Workbook.Worksheets[0]; // Assume the data is in the first worksheet

                    // Define expected column names
                    var expectedColumnNames = new List<string> { "BookId", "MemberId", "BorrowDate", "ClaimedReturnDate", "ActualReturnDate", "Active" };
                    int expectedColumnCount = expectedColumnNames.Count;

                    // Retrieve column names from the first row (header row)
                    var columnNames = new List<string>();
                    for (int col = 1; col <= expectedColumnCount; col++) // Assuming columns start at index 1
                    {
                        columnNames.Add(worksheet.Cells[1, col].Text.Trim()); // Get the column name and trim any whitespace
                    }

                    // Validate the number of columns
                    if (columnNames.Count != expectedColumnCount)
                    {
                        throw new Exception($"The number of columns in the sheet ({columnNames.Count}) does not match the expected number ({expectedColumnCount}).");
                    }

                    // Validate column names
                    for (int i = 0; i < expectedColumnCount; i++)
                    {
                        if (!columnNames[i].Equals(expectedColumnNames[i], StringComparison.OrdinalIgnoreCase))
                        {
                            throw new Exception($"Column name mismatch at position {i + 1}. Expected '{expectedColumnNames[i]}', but got '{columnNames[i]}'.");
                        }
                    }

                    int rowCount = worksheet.Dimension.Rows;

                    for (int row = 2; row <= rowCount; row++)
                    {
                        var errorMessage = string.Empty;
                        var haveError = false;

                        // Validate BookId
                        var bookIdText = worksheet.Cells[row, 1].Text;
                        if (!int.TryParse(bookIdText, out var bookId))
                        {
                            errorMessage += "BookId is required and must be an integer. ";
                            haveError = true;
                        }

                        // Validate MemberId
                        var memberIdText = worksheet.Cells[row, 2].Text;
                        if (!int.TryParse(memberIdText, out var memberId))
                        {
                            errorMessage += "MemberId is required and must be an integer. ";
                            haveError = true;
                        }

                        // Validate BorrowDate (initialize with a default value)
                        var borrowDateText = worksheet.Cells[row, 3].Text;
                        var borrowDate = DateTime.MinValue;
                        if (!IsValidDateTimeFormat(borrowDateText) || !DateTime.TryParse(borrowDateText, out borrowDate))
                        {
                            errorMessage += "BorrowDate is required and must be in the format 'YYYY-MM-DD' followed by valid time characters (numbers, : or +). ";
                            haveError = true;
                        }

                        // Validate ClaimedReturnDate (initialize with a default value)
                        var claimedReturnDateText = worksheet.Cells[row, 4].Text;
                        var claimedReturnDate = DateTime.MinValue;
                        if (!IsValidDateTimeFormat(claimedReturnDateText) || !DateTime.TryParse(claimedReturnDateText, out claimedReturnDate))
                        {
                            errorMessage += "ClaimedReturnDate is required and must be in the format 'YYYY-MM-DD' followed by valid time characters (numbers, : or +). ";
                            haveError = true;
                        }

                        // Validate ActualReturnDate (optional, initialize with a default value)
                        var actualReturnDateText = worksheet.Cells[row, 5].Text;
                        var actualReturnDate = DateTime.MinValue;
                        if (string.IsNullOrWhiteSpace(actualReturnDateText))
                        {
                            actualReturnDate = DateTime.MinValue; // If no value provided
                        }
                        else
                        {
                            if (!IsValidDateTimeFormat(actualReturnDateText) || !DateTime.TryParse(actualReturnDateText, out actualReturnDate))
                            {
                                errorMessage += "ActualReturnDate must be in the format 'YYYY-MM-DD' followed by valid time characters (numbers, : or +). ";
                                haveError = true;
                            }
                        }

                        // Validate Active (boolean)
                        var activeText = worksheet.Cells[row, 6].Text;
                        if (!bool.TryParse(activeText, out bool active))
                        {
                            errorMessage += "Active is required and must be a boolean. ";
                            haveError = true;
                        }

                        // If there are errors, add to the validation error list
                        if (haveError)
                        {
                            validationErrorList.Add(new ValidationErrorBorrowResponse
                            {
                                RowNumber = row,
                                BookId = bookIdText,
                                MemberId = memberIdText,
                                BorrowDate = borrowDateText,
                                ClaimedReturnDate = claimedReturnDateText,
                                ActualReturnDate = actualReturnDateText,
                                ErrorMessage = errorMessage.Trim()
                            });
                            continue;
                        }

                        // If no errors, create a valid Borrow object
                        var borrow = new Borrow
                        {
                            BookId = bookId,
                            MemberId = memberId,
                            BorrowDate = borrowDate.ToUniversalTime(), // Convert to UTC
                            ClaimedReturnDate = claimedReturnDate.ToUniversalTime(), // Convert to UTC
                            ActualReturnDate = actualReturnDate == DateTime.MinValue ? actualReturnDate : actualReturnDate.ToUniversalTime(), // Convert to UTC if valid
                            Active = active,
                            DateCreated = DateTime.UtcNow,
                            DateModified = DateTime.UtcNow

                        };


                        validBorrows.Add(borrow);
                    }

                }
            }

            return (validBorrows, validationErrorList);
        }

        private bool IsValidDateTimeFormat(string dateTimeText)
        {
            // Regular expression to match the format "YYYY-MM-DD" followed by either a number, ':' or '+'
            var regex = new System.Text.RegularExpressions.Regex(@"^\d{4}-\d{2}-\d{2}[\s](\d+|[:+])");
            return regex.IsMatch(dateTimeText);
        }


        public async Task AddBorrowRecordfromExcel(Borrow borrow)
        {
            // Check if the Book and Member exist
            var book = await _context.Books.FirstOrDefaultAsync(b => b.Id == borrow.BookId && b.Active);
            var member = await _context.Members.FirstOrDefaultAsync(m => m.Id == borrow.MemberId && m.Active);

            if (book == null)
            {
                throw new ArgumentException("Invalid BookId. The book does not exist or is inactive.");
            }

            if (member == null)
            {
                throw new ArgumentException("Invalid MemberId. The member does not exist or is inactive.");
            }

            // Add the borrow record to the database
            _context.BookBorrows.Add(borrow);

            // Save changes to the database
            await _context.SaveChangesAsync();
        }


        public async Task<IEnumerable<ExcelBorrowBookResponse>> ExportAllBorrowsTOExcel()
        {
            var borrow = await _context.BookBorrows.ToListAsync();

            return borrow.Select(m => new ExcelBorrowBookResponse
            {
               bookId= m.Id,
               MemberId = m.MemberId,
               BorrowDate = m.BorrowDate,
               DateCreated = m.DateCreated,
               DateModified = m.DateModified,
               ClaimedReturnDate = m.ClaimedReturnDate
            });
        }

    }
}
