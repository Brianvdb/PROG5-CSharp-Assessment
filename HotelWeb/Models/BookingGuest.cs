using System;
namespace HotelWeb.Models
{
    [Serializable]
    public class BookingGuest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public int Gender { get; set; }
    }
}