namespace _Framework.Aggr
{
    public class OrderValidation
    {
        public Guid? OrderEntryById { get; set; }
        public string? OrderEntryByCode { get; set; }
        public List<ProductValidation>? Products { get; set; }
        public string? SupervisorDeviceId { get; set; }
        public long OrderNumber { get; set; }
        public Guid NotificationReceiverId { get; set; }
    }
}
