namespace SellerService.Entities;
public class Seller
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public string StoreName { get; set; }
    public string Description { get; set; }
    public decimal Balance { get; set; }
    public string StoreLogoUrl { get; set; }
}