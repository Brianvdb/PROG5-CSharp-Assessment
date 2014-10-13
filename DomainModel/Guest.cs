using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainModel
{
    class Guest
    {
        [Key]
        public int GuestId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public DateTime BirthDate { get; set; }

        public int Gender { get; set; }

        public ICollection<Booking> Bookings { get; set; }


    }
}
