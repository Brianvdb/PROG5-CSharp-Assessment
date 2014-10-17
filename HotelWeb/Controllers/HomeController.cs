﻿using DomainModel;
using HotelWeb.Models;
using HotelWeb.Repositories;
using System;
using System.Collections.Generic;
using System.Globalization;
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

        public ActionResult RoomPrices(int id)
        {
            return View(roomRepo.Get(id));
        }

        [HttpPost]
        public ActionResult Rooms(FormCollection form)
        {
            float price = float.Parse(form["minprice"].Replace(',', '.'), System.Globalization.CultureInfo.InvariantCulture);
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

            ViewBag.Success = "De kamer is toegevoegd";

            return View(roomRepo.GetAll());
        }

        [HttpPost]
        public ActionResult AddRoomPrice(FormCollection form)
        {
            // CHECK VALID START DATE
            DateTime start;
            if (form["startdate"] == null || form["startdate"].Length == 0)
            {
                ViewBag.Error = "Startdatum is leeg.";
                return View("Rooms", roomRepo.GetAll());
            }
            else if (!DateTime.TryParseExact(form["startdate"], "yyyy-MM-dd", CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out start)) {
                ViewBag.Error = "Startdatum is niet valide.";
                return View("Rooms", roomRepo.GetAll());
            } 
            
            // CHECK VALID END DATE
            DateTime end;
            if (form["enddate"] == null || form["enddate"].Length == 0)
            {
                ViewBag.Error = "Einddatum is leeg.";
                return View("Rooms", roomRepo.GetAll());
            }
            else if (!DateTime.TryParseExact(form["enddate"], "yyyy-MM-dd", CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out end))
            {
                ViewBag.Error = "Einddatum is niet valide";
                return View("Rooms", roomRepo.GetAll());
            }

            int id = Int32.Parse(form["room"]);

            HotelRoom room = roomRepo.Get(id);

            // CHECK BOTH DATES

            if (start > end)
            {
                ViewBag.Error = "Startdatum is na de einddatum.";
                return View("Rooms", roomRepo.GetAll());
            }

            if (start < DateTime.Now)
            {
                ViewBag.Error = "Startdatum is in het verleden.";
                return View("Rooms", roomRepo.GetAll());
            }

            // CHECK CONFLICTS

            if (room.RoomPrices != null)
            {
                foreach (HotelRoomPrice p in room.RoomPrices)
                {
                    if (p.StartDate < end && start < p.EndDate)
                    {
                        ViewBag.Error = "De start en eind periode overlapt een ander prijstarief.";
                        return View("Rooms", roomRepo.GetAll());
                    }
                }
            }
            
            // CHECK PRICE

            double price = double.Parse(form["price"].Replace(',', '.'), System.Globalization.CultureInfo.InvariantCulture);
            
            if (price < 0)
            {
                ViewBag.Error = "De prijs mag niet lager dan 0 zijn.";
                return View("Rooms", roomRepo.GetAll());
            }
            
            // NOW WE CAN ADD

            HotelRoomPrice roomPrice = new HotelRoomPrice()
            {
                StartDate = start,
                EndDate = end,
                Price = price,
                Yearly = false
            };

            roomPriceRepo.Add(roomPrice);

            if (room.RoomPrices == null)
            {
                room.RoomPrices = new List<HotelRoomPrice>();
            }
            
            room.RoomPrices.Add(roomPrice);
            roomRepo.UpdateDatabase();

            ViewBag.Success = "De prijstarief voor de opgegeven periode is aangemaakt.";

            return View("Rooms", roomRepo.GetAll());
        }
    }
}
