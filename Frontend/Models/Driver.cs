using System;

namespace WebApplication6.Models
{
    public class Driver
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public DateTime BirthDate { get; set; }
        public int Age
        {
            get
            {
                return CalculateAge(BirthDate);
            }
            set
            {
                BirthDate = CalculateBirthDate(value);
            }
        }
        public DateTime CreatedDate { get; set; } = DateTime.Today; // Varsayılan olarak sadece tarihi alır.
        public bool IsDeleted { get; set; } = false; // Yeni eklenen özellik
        public bool IsAvailable { get; set; } = true; // Yeni eklenen özellik

        private DateTime CalculateBirthDate(int age)
        {
            var today = DateTime.Today;
            var birthDate = today.AddYears(-age);
            if (birthDate > today)
            {
                birthDate = birthDate.AddYears(-1);
            }
            return birthDate;
        }

        private int CalculateAge(DateTime birthDate)
        {
            int age = DateTime.Now.Year - birthDate.Year;
            if (DateTime.Now.DayOfYear < birthDate.DayOfYear)
                age = age - 1;

            return age;
        }
    }
}
