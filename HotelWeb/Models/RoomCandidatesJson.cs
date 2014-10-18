using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HotelWeb.Models
{
    public class RoomCandidatesJson
    {
        public int ID { get; set; }
        public int NumberOfPersons { get; set; }
        public float MinPrice { get; set; }
    }
}