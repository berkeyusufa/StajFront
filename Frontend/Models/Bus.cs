namespace WebApplication6.Models
{
    public class Bus
    {
        public int Id { get; set; }
        public string? Photo { get; set; }
        public string DoorNumber { get; set; }
        public string PlateNumber { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public bool IsDeleted { get; set; }
    }
}