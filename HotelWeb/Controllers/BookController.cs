
using HotelWeb.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using ExtensionMethods;
using DomainModel;
using HotelWeb.Models;

namespace HotelWeb.Controllers
{
    public class BookController : Controller
    {
        private EntityHotelRoomRepository roomRepo;
        private EntityBookingRepository bookRepo;

        public BookController()
        {
            DatabaseContext db = new DatabaseContext();
            roomRepo = new EntityHotelRoomRepository(db);
            bookRepo = new EntityBookingRepository(db);
        }

        [HttpPost]
        public ActionResult Header(FormCollection form)
        {
            int id = Int32.Parse(form["PersonRoom"]);
            bool extraBig = form["BiggerRoom"] == "true";

            IEnumerable<RoomCandidatesJson> list = roomRepo.GetAll().Where(item =>
                {
                    if(extraBig){
                        return item.NumberOfPersons >= id;
                    }else{
                        return item.NumberOfPersons == id;
                    }
                })
                .Select(item => new RoomCandidatesJson{ID = item.Id, NumberOfPersons = item.NumberOfPersons, MinPrice = item.MinPrice});
            return Json(list.ToJSON());
        }

        [HttpPost]
        public ActionResult Dates(FormCollection form)
        {
            DateTime startDate = DateTime.Parse(form["StartDate"]);
            bool dateCorrect = startDate.Date >= DateTime.Now.Date;
            IEnumerable<int> roomIds;

            
            roomIds = form["RoomSelection"].Split(',').Select(x => 
            {
                try
                {
                    return int.Parse(x);
                }
                catch(Exception e)
                {
                    return -1;
                }
            });

            if (roomIds.Contains(-1))
            {
                return Json("{error:\"Er zijn geen resultaten voor de opgegeven waardes\"}");
            }

            IEnumerable<int> bookings = bookRepo.GetAll().Where(booking => roomIds.Contains(booking.HotelRoom.Id)).Select(booking => booking.BookingId);
            return Json(bookings.ToJSON());
        }
    }
}
