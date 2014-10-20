using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HotelWeb.Models
{
    public class AvailabilityJson
    {
        public int roomID { get; set; }
        public string startDate { get; set; }
        public string endDate { get; set; }
        public string message { get; set; }
    }
}