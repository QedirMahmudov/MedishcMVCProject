namespace MedishcMVCProject.Models.Pharmacy
{
    public class Payment : Base
    {
        public int OrderId { get; set; }
        public Order Order { get; set; }

        public decimal Amount { get; set; }
        public DateTime PaidAt { get; set; }

        public PaymentMethod Method { get; set; }
        public PaymentStatus Status { get; set; }
        public string FailureReason { get; set; }
    }
    public enum PaymentMethod
    {
        Stripe,
        Cash,
        Card
    }
    public enum PaymentStatus
    {
        Completed,
        Failed,
        Pending
    }
}
