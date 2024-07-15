namespace dotnetAPI.Models
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }

        public bool ActiveState { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public int CustomerId { get; set; }

        public Customer? Customer { get; set; }
        public ICollection<OrderItem>? OrderItems { get; set; }
    }
}
