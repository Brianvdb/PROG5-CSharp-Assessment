using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainModel
{
    public class HotelRoom
    {
        [Key]
        public int Id { get; set; }

        public int NumberOfPersons { get; set; }

        public float MinPrice { get; set; }

        public virtual ICollection<HotelRoomPrice> RoomPrices { get; set; }

        public virtual ICollection<Booking> Bookings { get; set; }

        public DateTime OpenDate { get; set; }

        public DateTime CloseDate { get; set; }
    }
}
