using Microsoft.AspNetCore.Mvc.RazorPages;
using System;


namespace WebApplication6.Models
{
    public class Task 
    {
        public int Id { get; set; }
        public int DriverId { get; set; }
        public int BusId { get; set; }
        public int RouteId { get; set; }
        public DateTime TaskDate { get; set; }

        public Driver Driver { get; set; }
        public Bus Bus { get; set; }
        
    }
}
