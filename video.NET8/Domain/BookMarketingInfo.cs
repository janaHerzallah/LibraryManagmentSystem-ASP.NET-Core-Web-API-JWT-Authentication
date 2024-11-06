using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class BookMarketingInfo
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } // Represents the unique identifier in MongoDB.

    [BsonElement("bookId")]
    public int BookId { get; set; }

    [BsonElement("title")]
    public string Title { get; set; }

    [BsonElement("author")]
    public string Author { get; set; } // author name from author table

    [BsonElement("timesBorrowed")] // how many times in the borrow table this book has been borrowed
    public int TimesBorrowed { get; set; }

    [BsonElement("availableCopies")]
    public int? AvailableCopies { get; set; }

    [BsonElement("category")]

    public string Category { get; set; } // category name from category table

    [BsonElement("libraryBranch")]

    public string LibraryBranch { get; set; } // library branch name from library branch table
}
