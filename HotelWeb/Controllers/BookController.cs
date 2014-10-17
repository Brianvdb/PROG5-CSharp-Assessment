
using HotelWeb.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using ExtensionMethods;

namespace HotelWeb.Controllers
{
    public class BookController : Controller
    {
        private EntityHotelRoomRepository roomRepo;

        [HttpPost]
        public ActionResult test(){
            List<Person> people = new List<Person>{
                   new Person{ID = 1, FirstName = "Scott", LastName = "Gurthie"},
                   new Person{ID = 2, FirstName = "Bill", LastName = "Gates"}
                   };
            
      
            return Json(people.ToJSON());
        }
    }
}
