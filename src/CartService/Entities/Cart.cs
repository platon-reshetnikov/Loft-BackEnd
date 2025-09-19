using UserService.Entities;

namespace CartService.Entities;

public class Cart
{
    public long Id { get; set; }
    public long CustomerId { get; set; }
    public User Customer { get; set; }
    public ICollection<CartItem> CartItems { get; set; }
}