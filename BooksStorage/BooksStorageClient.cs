using BooksStorage.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;

[assembly: InternalsVisibleTo("BooksStorage.Tests")]
namespace BooksStorage
{
    public sealed class BooksStorageClient : IBooksStorageClient
    {
        private readonly string _bearerToken = "secret_bearer_token";
        private readonly HttpClient _httpClient;

        public BooksStorageClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _bearerToken);
        }

        public async Task<bool> AddBookAsync(Book book)
        {
            var json = JsonSerializer.Serialize(book);
            var body = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("/api/books", body);

            return response.IsSuccessStatusCode;
        }

        public async Task<IEnumerable<Book>> GetBooksAsync()
        {
            var response = await _httpClient.GetAsync("/api/books");

            response.EnsureSuccessStatusCode();

            var books = await response.Content.ReadFromJsonAsync<List<Book>>();

            return books;
        }

        public async IAsyncEnumerable<Order> GetOrdersAsync(int page, int pageSize, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            bool hasMoreOrders;

            do
            {
                var response = await _httpClient.GetAsync($"/api/orders?page={page}&pageSize={pageSize}", cancellationToken);
                response.EnsureSuccessStatusCode();

                var orders = await response.Content.ReadFromJsonAsync<List<Order>>(cancellationToken);

                if (orders == null || orders.Count == 0)
                {
                    hasMoreOrders = false;
                }
                else
                {
                    foreach (var order in orders)
                    {
                        yield return order;
                    }

                    hasMoreOrders = orders.Count == pageSize;
                    page++;
                }
            }
            while (hasMoreOrders && !cancellationToken.IsCancellationRequested);
        }
    }
}
