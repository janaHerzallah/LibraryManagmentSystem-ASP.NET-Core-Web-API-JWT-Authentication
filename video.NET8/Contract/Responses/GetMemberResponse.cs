namespace LibraryManagmentSystem.Contract.Responses
{
    public class GetMemberResponse
    {
        public int Id { get; set; }
        public string? Name { get; set; } // no problem if its null 

        public string? Email { get; set; }

        public bool Active { get; set; } = true; // Default to true

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
