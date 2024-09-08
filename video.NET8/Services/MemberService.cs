using LibraryManagementSystem.Data;
using LibraryManagementSystem.Domain;
using LibraryManagementSystem.Interfaces;
using LibraryManagmentSystem.Contract.Requests;
using LibraryManagmentSystem.Contract.Responses;
using LibraryManagmentSystem.Interfaces;
using Microsoft.EntityFrameworkCore;


namespace LibraryManagementSystem.Services
{
    public class MemberService : IMemberService
    {
        private readonly ApplicationDbContext _context;
        private readonly IUserService _userService;

        public MemberService(ApplicationDbContext context, IUserService userService )
        {
            _context = context;
            _userService = userService;
        }

        

        public async Task<IEnumerable<GetMemberResponse>> GetActiveAndInActiveMembersAsync() { 
            var member = await _context.Members.ToListAsync();

            return member.Select(m => new GetMemberResponse
            {
                Id = m.Id,
                Name = m.Name,
                Email = m.Email,
                Active = m.Active,
                CreatedAt = m.DateCreated,
                UpdatedAt = m.DateModified,
                OverDueCount = m.OverDueCount
            });
        }
        public async Task<IEnumerable<GetMemberResponse>> GetAllMembersAsync()
        {
           var member= await _context.Members.Where(m => m.Active).ToListAsync();

            return member.Select(m => new GetMemberResponse
            {
                Id = m.Id,
                Name = m.Name,
                Email = m.Email,
                Active = m.Active,
                CreatedAt = m.DateCreated,
                UpdatedAt = m.DateModified,
                OverDueCount = m.OverDueCount
            });
        }

        public async Task<GetMemberResponse> GetMemberByIdAsync(int id)
        {
            var member = await _context.Members.FirstOrDefaultAsync(m => m.Id == id && m.Active);
            if (member == null)
            {
                throw new KeyNotFoundException("Member not found or is inactive.");
            }
            return new GetMemberResponse
            {
                Id = member.Id,
                Name = member.Name,
                Email = member.Email,
                Active = member.Active,
                CreatedAt = member.DateCreated,
                UpdatedAt = member.DateModified,
                OverDueCount = member.OverDueCount
            };
        }

        public async Task<GetMemberResponse> AddMemberAsync(AddMemberRequest member)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == member.userId);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found.");
            }

            Member memberDataBase = new Member
            {
                Name = member.Name,
                
                Email = member.Email,
                Active = true, // default value
                DateCreated = DateTime.UtcNow,
                DateModified = DateTime.UtcNow,
                UserId = member.userId
            };

            _context.Members.Add(memberDataBase);
            await _context.SaveChangesAsync();

            return new GetMemberResponse
            {
                Id = memberDataBase.Id,
                Name = memberDataBase.Name,
                Email = memberDataBase.Email,
                Active = memberDataBase.Active,
                CreatedAt = memberDataBase.DateCreated,
                UpdatedAt = memberDataBase.DateModified,
                OverDueCount = memberDataBase.OverDueCount,
                userId = member.userId
            };
        }

        public async Task<GetMemberResponse> UpdateMemberAsync(int id, UpdateMemeberRequest updatedMember)
        {
            var member = await _context.Members.FirstOrDefaultAsync(m => m.Id == id);
            if (member == null)
            {
                throw new KeyNotFoundException("Member not found.");
            }

            member.Name = updatedMember.Name;
            member.Email = updatedMember.Email;
            member.Active = updatedMember.Active;
            member.DateModified = DateTime.UtcNow;
            member.Active = updatedMember.Active;
            member.OverDueCount = updatedMember.OverDueCount;
            member.UserId = updatedMember.userId;


            _context.Members.Update(member);
            await _context.SaveChangesAsync();

            return new GetMemberResponse
            {
                Id = member.Id,
                Name = updatedMember.Name,
                Email = updatedMember.Email,
                Active = updatedMember.Active,
                CreatedAt = member.DateCreated,
                UpdatedAt = member.DateModified,
                OverDueCount = member.OverDueCount,
                userId= updatedMember.userId
                

            };
        }

        public async Task<bool> DeleteMemberAsync(int id)
        {
            var member = await _context.Members.FirstOrDefaultAsync(m => m.Id == id && m.Active);
            if (member == null)
            {
                return false;
            }

            _context.Members.Remove(member);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task SoftDeleteMemberAsync(int id)
        {
            var member = await _context.Members.FirstOrDefaultAsync(m => m.Id == id && m.Active);
            if (member == null)
            {
                throw new KeyNotFoundException("Member not found or is already inactive.");
            }

            member.Active = false;
            _context.Members.Update(member);
            await _context.SaveChangesAsync();
        }
        public async Task<IEnumerable<GetBorrowedBooksForAMemberResponse>> GetBorrowedBooksByMemberAsync(int memberId , string token)
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

                return await _context.BookBorrows
                                 .Where(b => b.MemberId == memberId && b.Active)
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
                                     Title = b.Book.Title,
                                     ClaimedReturnDate = b.ClaimedReturnDate
                                 })
                                 .ToListAsync();
        }


        public async Task<IEnumerable<GetBorrowedBooksForAMemberResponse>> GetBorrowedBooksNotReturnedByMemberAsync(int memberId , string token)
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

            return await _context.BookBorrows
                                 .Where(b => b.MemberId == memberId && b.Active && ( b.ActualReturnDate == DateTime.MinValue))
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
                                     Title = b.Book.Title,
                                     ClaimedReturnDate = b.ClaimedReturnDate
                                 })
                                 .ToListAsync();
        }

        public async Task<IEnumerable<GetBorrowedBooksForAMemberResponse>> GetBorrowedBooksOverDuedByMember(int memberId , string token)
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

            return await _context.BookBorrows
                                 .Where(b => b.MemberId == memberId && b.Active && 
                                 b.ClaimedReturnDate< DateTime.UtcNow &&   // if the claimed date has passed 
                                 ( b.ActualReturnDate == DateTime.MinValue)) // if its infinity it means its not returned yet 
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
                                     Title = b.Book.Title,
                                     ClaimedReturnDate = b.ClaimedReturnDate
                                 })
                                 .ToListAsync();
        }
        public async Task<int> GetOverdueBooksCountByMemberAsync(int memberId , string token)
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

            return await _context.BookBorrows
                                 .Where(b => b.MemberId == memberId && b.Active && (b.ActualReturnDate == DateTime.MinValue) && b.ClaimedReturnDate < DateTime.UtcNow )
                                 .CountAsync();
        }



    }
}
