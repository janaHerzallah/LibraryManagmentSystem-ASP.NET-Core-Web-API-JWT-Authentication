﻿namespace LibraryManagmentSystem.Contract.Requests
{
    public class UpdateCategoryRequest
    {
        public string Name { get; set; }
        public string? Description { get; set; }

        public bool Active { get; set; }

    }
}
