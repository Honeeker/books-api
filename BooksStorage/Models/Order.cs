namespace BooksStorage.Models
{
    public class Order
    {
        public Guid OrderId { get; init; }

        public IEnumerable<OrderLine> OrderLines { get; init; }
    }
}
