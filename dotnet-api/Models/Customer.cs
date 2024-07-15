
namespace dotnetAPI.Models
{
    public class Customer
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Description { get; set; }

        public bool ActiveState { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        //public ICollection<Order>? Orders { get; set; }
        /*
       public static implicit operator Customer(List<Customer> v)
       {
           throw new NotImplementedException();
       }*/
    }
}
