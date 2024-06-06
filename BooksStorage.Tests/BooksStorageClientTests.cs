using BooksStorage.Models;
using Moq;
using Moq.Protected;
using System.Net.Http.Json;

namespace BooksStorage.Tests
{
    public class BooksStorageClientTests
    {
        private readonly Mock<HttpMessageHandler> _handler;
        private readonly HttpClient _httpClient;
        private readonly BooksStorageClient _booksServiceApi;

        public BooksStorageClientTests()
        {
            _handler = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_handler.Object) { BaseAddress = new Uri("https://testapi.com") };
            _booksServiceApi = new BooksStorageClient(_httpClient);
        }

        [Fact]
        public async Task GetBooksAsync_ShouldReturnBooks()
        {
            var books = new List<Book> 
            { 
                new Book { Id = 1, Authors = new[] { new Author { FirstName = "Foo", LastName = "Bar" } }, Bookstand = 1, Price = 20.5m, Shelf = 1, Title = "FooBar"  }
            };

            _handler
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage 
                { 
                        StatusCode = System.Net.HttpStatusCode.OK,
                        Content = JsonContent.Create(books) 
                });

            var result = await _booksServiceApi.GetBooksAsync();

            Assert.NotNull(result);
            Assert.True(result.Any());
        }

        [Fact]
        public async Task GetOrdersAsync_ShouldReturnOrders()
        {
            var ordersPage1 = new List<Order>
            {
                new Order
                {
                    OrderId = Guid.NewGuid(), OrderLines = new List<OrderLine>
                    {
                        new OrderLine { BookId = 1, Quantity = 1 }
                    }
                },
                new Order
                {
                    OrderId = Guid.NewGuid(), OrderLines = new List<OrderLine>
                    {
                        new OrderLine { BookId = 2, Quantity = 2 }
                    }
                }
            };

            _handler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync", 
                    ItExpr.Is<HttpRequestMessage>(r => r.RequestUri == new Uri("https://testapi.com/api/orders?page=1&pageSize=3")), 
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage { StatusCode = System.Net.HttpStatusCode.OK, Content = JsonContent.Create(ordersPage1) });

            var result = _booksServiceApi.GetOrdersAsync(1, 3);

            await foreach (var order in result)
            {
                Assert.NotNull(order);
                Assert.True(order.OrderLines.Any());
            }
        }

        [Fact]
        public async Task AddBookAsync_ShouldBeSuccessful()
        {
            var book = new Book { Id = 1, Authors = new[] { new Author { FirstName = "Foo", LastName = "Bar" } }, Bookstand = 1, Price = 20.5m, Shelf = 1, Title = "FooBar" };

            _handler
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = System.Net.HttpStatusCode.OK
                });

            var result = await _booksServiceApi.AddBookAsync(book);

            Assert.True(result);
        }
    }
}