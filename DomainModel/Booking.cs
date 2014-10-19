using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainModel
{
    public class Booking
    {
        [Key]
        public int BookingId { get; set; }

        public virtual ICollection<Guest> Guests { get; set; }

        public virtual HotelRoom HotelRoom { get; set; }

       
        public string Email { get; set; }

        public virtual Address GuestAddress { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set;  }
    }
}
