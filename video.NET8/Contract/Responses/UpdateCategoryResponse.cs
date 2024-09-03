namespace LibraryManagmentSystem.Contract.Responses
{
    public class UpdateCategoryResponse 
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }

        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
