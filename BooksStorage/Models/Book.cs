namespace BooksStorage.Models
{
    public class Book
    {
        public int Id { get; init; }

        public string Title { get; init; }

        public decimal Price { get; init; }

        public int Bookstand { get; init; }

        public int Shelf { get; init; }

        public IEnumerable<Author> Authors { get; init; }
    }
}
