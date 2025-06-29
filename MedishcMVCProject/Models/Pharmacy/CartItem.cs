namespace MedishcMVCProject.Models.Pharmacy
{
    public class CartItem : Base
    {
        public string UserId { get; set; }
        public AppUser User { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice => Quantity * Product.Price;
    }
}
