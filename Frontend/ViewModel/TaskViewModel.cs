namespace WebApplication6.Models
{
    public class TaskViewModel
    {
        public int Id { get; set; }
        public int SelectedDriverId { get; set; }
        public int SelectedBusId { get; set; }
        public DateTime TaskDate { get; set; }

        public Driver Driver { get; set; }
        public Bus Bus { get; set; }

        public IEnumerable<Driver> Drivers { get; set; }
        public IEnumerable<Bus> Buses { get; set; }
        public IEnumerable<TaskViewModel> Tasks { get; set; }
    }
}