
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
        public ActionResult test()
        {
            IEnumerable<HotelRoom> list = roomRepo.GetAll();
            return Json("{}");
        }
        [HttpPost]
        public ActionResult Dates(FormCollection form)
        {
            DateTime startDate = DateTime.Parse(form["StartDate"]);
            int amountOfDatesInTable = int.Parse(form["AmountOfDatesInTable"]);
            bool dateCorrect = startDate.Date >= DateTime.Now.Date;
            string defaultDateFormat = ("yyyy-MM-dd");
            IEnumerable<int> roomIds;

            if (!dateCorrect)
            {
                return Json("{error:\"De ingestelde datum kan niet plaats vinden in het verleden\"}");
            }
            
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

            //what bookings are active during the period that will be showed
            List<AvailabilityJson> bookings = bookRepo.GetAll()
                .Where(booking => roomIds.Contains(booking.HotelRoom.Id) &&
                    booking.StartDate <= startDate.AddDays(amountOfDatesInTable).AddDays(1) &&
                    booking.EndDate >= startDate)
                .Select(booking => new AvailabilityJson() { 
                    startDate = booking.StartDate.Date.ToString(defaultDateFormat),
                    endDate = booking.EndDate.Date.ToString(defaultDateFormat),
                    message = "taken",
                    roomID = booking.HotelRoom.Id
                }).ToList();

            IEnumerable<HotelRoom> rooms = roomRepo.GetAll().Where(room => roomIds.Contains(room.Id));

            foreach (HotelRoom room in rooms)
            {
                if (room.OpenDate >= startDate)
                {
                    bookings.Add(new AvailabilityJson() { 
                        roomID = room.Id,
                        startDate = startDate.Date.ToString(defaultDateFormat),
                        endDate = room.OpenDate.Date.ToString(defaultDateFormat), 
                        message = "closed" 
                    });
                }

                if (room.CloseDate > room.OpenDate && room.CloseDate <= startDate.AddDays(amountOfDatesInTable))
                {
                    bookings.Add(new AvailabilityJson(){
                        roomID = room.Id,
                        startDate = room.CloseDate.ToString(defaultDateFormat),
                        message = "closed"
                    });
                }
            }
            return Json(bookings.ToJSON());
        }

        public ActionResult PlanCorrect(FormCollection plan)
        {
            bool correct = false;

            DateTime startDate = new DateTime();
            DateTime endDate = new DateTime();
            int nights = -1;
            int roomId = -1;

            try
            {
                startDate = DateTime.Parse(plan["StartDate"]);
                nights = int.Parse(plan["Nights"]);
                roomId = int.Parse(plan["RoomId"]);

                //calculate enddate
                endDate = startDate.AddDays(nights);
            }
            catch
            {
                return Json("{\"PlanCorrect\":\"false\"}");
            }

            //check room == free
            correct = RoomIsFree(startDate, endDate, roomId);

            return Json("{\"PlanCorrect\":\"" + correct + "\",\"StartDate\":\"" + startDate.ToString("yyyy-MM-dd") + "\",\"EndDate\":\"" + endDate.ToString("yyyy-MM-dd") + "\"}");
        }

        public bool RoomIsFree(DateTime start, DateTime end, int roomId)
        {
            HotelRoom thisRoom = roomRepo.Get(roomId);

            //is room open (not closed)
            if ((thisRoom.CloseDate > thisRoom.OpenDate && end >= thisRoom.CloseDate) || start <= thisRoom.OpenDate)
            {
                return false;
            }

            //is room available
            List<Booking> bookings = bookRepo.GetAll()
                .Where(booking => 
                    booking.HotelRoom.Id == roomId &&
                    booking.StartDate <= end &&
                    booking.EndDate >= start).ToList();

            if (bookings.Count > 0)
            {
                return false;
            }

            return true;
        }
    }
}
