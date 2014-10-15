using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainModel
{
    public class Address
    {
        [Key]
        public int AddressId { get; set;  }
        public string Street { get; set; }

        public string PostalCode { get; set; }

        public string HomeTown { get; set; }

    }
}
