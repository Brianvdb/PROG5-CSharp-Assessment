using System;
using System.Collections.Generic;

namespace HotelWeb.Models
{
    [Serializable]
    public class BookingData
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Nights { get; set; }
        public int RoomId { get; set; }
        public List<BookingGuest> GuestList { get; set; }
        public string Street { get; set; }
        public string PostalCode { get; set; }
        public string HomeTown { get; set; }
        public string EMail { get; set; }
    }
}