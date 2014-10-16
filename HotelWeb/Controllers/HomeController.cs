using DomainModel;
using HotelWeb.Models;
using HotelWeb.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HotelWeb.Controllers
{
    public class HomeController : Controller
    {
        private EntityAddressRepository addressRepo;
        private EntityBookingRepository bookingRepo;
        private EntityGuestRepository guestRepo;
        private EntityHotelRoomPriceRepository roomPriceRepo;
        private EntityHotelRoomRepository roomRepo;
        private EntityInvoiceRepository invoiceRepo;


        public HomeController()
        {
            DatabaseContext db = new DatabaseContext();
            this.addressRepo = new EntityAddressRepository(db);
            this.bookingRepo = new EntityBookingRepository(db);
            this.guestRepo = new EntityGuestRepository(db);
            this.roomPriceRepo = new EntityHotelRoomPriceRepository(db);
            this.roomRepo = new EntityHotelRoomRepository(db);
            this.invoiceRepo = new EntityInvoiceRepository(db);
            

            HotelRoom room = new HotelRoom();
            room.MinPrice = 20;
            room.OpenDate = DateTime.Now;
            room.CloseDate = DateTime.Now;
            room.NumberOfPersons = 2;
            room.RoomPrices = new List<HotelRoomPrice>();

            //roomRepo.GetAll();


            //roomRepo.Add(room);
            //db.HotelRooms.Add(room);
           // db.SaveChanges();
        }
        //
        // GET: /Home/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Rooms()
        {
            return View(roomRepo.GetAll());
        }

        public ActionResult Guests()
        {
            return View(guestRepo.GetAll());
        }

        public ActionResult Contact() {
            return View();
        }

        public ActionResult Booking()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Rooms(FormCollection form)
        {
            System.Globalization.NumberFormatInfo nf = new System.Globalization.NumberFormatInfo() { NumberGroupSeparator = "."};
            float price = float.Parse(form["minprice"], nf);
            HotelRoom room = new HotelRoom()
            {
                NumberOfPersons = Int32.Parse(form["personen"]),
                MinPrice = price,
                OpenDate = DateTime.Now,
                CloseDate = DateTime.Now.AddDays(-1),
                Bookings = new List<Booking>(),
                RoomPrices = new List<HotelRoomPrice>()
            };
            roomRepo.Add(room);
            return View(roomRepo.GetAll());
        }

    }
}
