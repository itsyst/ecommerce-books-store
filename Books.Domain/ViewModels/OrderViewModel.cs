using Books.Domain.Entities;

namespace Books.Domain.ViewModels
{
    public class OrderViewModel
    {
        public OrderHeader OrderHeader { get; set; } = new();
        public IEnumerable<OrderDetail>? OrderDetail { get; set; }
    }
}
