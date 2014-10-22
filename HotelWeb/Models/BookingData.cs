using System;
using System.Collections.Generic;

namespace HotelWeb.Models
{
    [Serializable]
    public class BookingData
    {
        private List<BookingGuest> _adressList = new List<BookingGuest>();

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Nights { get; set; }
        public int RoomId { get; set; }
        public List<BookingGuest> GuestList { get{ return _adressList; } }
        public string Street { get; set; }
        public string PostalCode { get; set; }
        public string HomeTown { get; set; }
        public string EMail { get; set; }

        public void resetGuests()
        {
            _adressList = new List<BookingGuest>();
        }
    }
}