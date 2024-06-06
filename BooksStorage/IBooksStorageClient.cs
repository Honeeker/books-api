using BooksStorage.Models;

namespace BooksStorage
{
    public interface IBooksStorageClient
    {
        Task<IEnumerable<Book>> GetBooksAsync();

        IAsyncEnumerable<Order> GetOrdersAsync(int page, int pageSize, CancellationToken cancellationToken);

        Task<bool> AddBookAsync(Book book);
    }
}
