namespace MedishcMVCProject.Models.Pharmacy
{
    public class Order : Base
    {
        public string UserId { get; set; }
        public AppUser User { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public OrderStatus Status { get; set; }

        public List<OrderItem> Items { get; set; }
    }
    public enum OrderStatus
    {
        Pending,
        Paid,
        Cancelled
    }
}
