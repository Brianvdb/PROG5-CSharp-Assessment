using DomainModel;
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
        }

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

        public ActionResult EditRoomPrice(int id, int priceId) {
            ViewBag.RoomId = id;
            return View(roomPriceRepo.Get(priceId));
        }

        public ActionResult DeleteRoomPrice(int id, int priceId)
        {
            ViewBag.RoomId = id;
            return View(roomPriceRepo.Get(priceId));
        }

        [HttpPost]
        public ActionResult EditRoomPrice(FormCollection form)
        {
            int roomId = Int32.Parse(form["RoomId"]);
            int roomPriceId = Int32.Parse(form["HotelRoomPriceId"]);

            ViewBag.RoomId = roomId;

            // CHECK OF ROOM EN ROOMPRICE BESTAAT EN CHECK DE RELATIE

            HotelRoomPrice price = roomPriceRepo.Get(roomPriceId);
            if (price == null)
            {
                ViewBag.Error = "De opgegeven prijstarief bestaat niet meer.";
                return View();
            }
            HotelRoom room = roomRepo.Get(roomId);
            if (room == null)
            {
                ViewBag.Error = "De opgegeven hotelkamer bestaat niet meer.";
                return View();
            }

            if (!room.RoomPrices.Contains(price))
            {
                ViewBag.Error = "Het prijstarief is geen van de opgegeven hotelkamer.";
                return View();
            }

            // CHECK VALID START DATE

            DateTime start = DateTime.Today;
            bool startDateSet = false;
            if (form["startdate"] == null || form["startdate"].Length == 0)
            {
            }
            else if (!DateTime.TryParseExact(form["startdate"], "yyyy-MM-dd", CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out start))
            {
                ViewBag.Error = "Startdatum is niet valide.";
                return View(price);
            }
            else if(start.Date != price.StartDate.Date)
            {
                startDateSet = true;
            }

            // CHECK VALID END DATE

            DateTime end = DateTime.Today;
            bool endDateSet = false;
            if (form["enddate"] == null || form["enddate"].Length == 0)
            {
            }
            else if (!DateTime.TryParseExact(form["enddate"], "yyyy-MM-dd", CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out end))
            {
                ViewBag.Error = "Einddatum is niet valide";
                return View(price);
            }
            else if (end.Date != price.EndDate.Date)
            {
                endDateSet = true;
            }


            // CHECK BOTH DATES

            DateTime s = price.StartDate;
            DateTime e = price.EndDate;
            if (startDateSet)
            {
                s = start;
            }
            if (endDateSet)
            {
                e = end;
            }

            if (s > e)
            {
                ViewBag.Error = "Startdatum is na de einddatum.";
                return View(price);
            }

            if (startDateSet && s < DateTime.Today)
            {
                ViewBag.Error = "Startdatum is in het verleden.";
                return View(price);
            }

            // CHECK CONFLICTS

            if (room.RoomPrices != null)
            {
                foreach (HotelRoomPrice p in room.RoomPrices)
                {
                    if (p == price)
                    {
                        continue;
                    }
                    if (p.StartDate < end && start < p.EndDate)
                    {
                        ViewBag.Error = "De start en eind periode overlapt een ander prijstarief.";
                        return View(price);
                    }
                }
            }

            // CHECK PRICE

            double priceValue = double.Parse(form["price"].Replace(',', '.'), System.Globalization.CultureInfo.InvariantCulture);

            if (priceValue < 0)
            {
                ViewBag.Error = "De prijs mag niet lager dan 0 zijn.";
                return View(price);
            }

            price.StartDate = s;
            price.EndDate = e;
            price.Price = priceValue;

            roomPriceRepo.UpdateDatabase();

            ViewBag.Success = "De wijzigingen zijn successvol doorgevoerd.";
            return View(price);
        }

        public ActionResult EditRoom(int id)
        {
            return View(roomRepo.Get(id));
        }

        public ActionResult DeleteRoom(int id)
        {
            return View(roomRepo.Get(id));
        }

        [HttpPost]
        public ActionResult DeleteRoom(FormCollection form)
        {
            int id = Int32.Parse(form["Id"]);
            HotelRoom room = roomRepo.Get(id);
            if (room == null)
            {
                ViewBag.Error = "De opgegeven kamer bestaat niet";
                return View(room);
            }

            foreach (Booking b in room.Bookings)
            {
                if (b.StartDate >= DateTime.Today || b.EndDate >= DateTime.Today)
                {
                    ViewBag.Error = "De kamer kan niet verwijderd omdat er nog boekingen openstaan.";
                    return View(room);
                }
            }

            foreach(HotelRoomPrice p in room.RoomPrices.ToList()) {
                roomPriceRepo.Delete(p);
            }

            roomRepo.Delete(room);

            ViewBag.Success = "De kamer is verwijderd.";

            return View();
        }

        [HttpPost]
        public ActionResult EditRoom(FormCollection form)
        {
            int id = Int32.Parse(form["Id"]);
            HotelRoom room = roomRepo.Get(id);
            if (room == null)
            {
                return View(room);
            }
            float price = 0;
            // CHECK PRICE
            if (form["minprice"] == null || form["minprice"] == "")
            {
                price = room.MinPrice;
            }
            else
            {
                price = float.Parse(form["minprice"].Replace(',', '.'), System.Globalization.CultureInfo.InvariantCulture);
            }


            if (price < 0)
            {
                ViewBag.Error = "Prijs mag niet lager dan 0 zijn.";
                return View(room);
            }

            // CHECK PERSONS

            int personen = Int32.Parse(form["personen"]);

            if (personen != 2 && personen != 3 && personen != 5)
            {
                ViewBag.Error = "Aantal personen moet 2, 3 of 5 zijn.";
                return View(room);
            }


            // CHECK OPEN DATE

            DateTime start = DateTime.Now;
            bool startDate = false;
            if (form["startdate"] == null || form["startdate"].Length == 0)
            {
         
            }
            else if (!DateTime.TryParseExact(form["startdate"], "yyyy-MM-dd", CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out start))
            {
                ViewBag.Error = "Openingsdatum is niet valide.";
                return View(room);
            }
            else if(room.OpenDate.Date != start.Date)
            {
                startDate = true;
            }

            if (startDate && start < DateTime.Today)
            {
                ViewBag.Error = "Openingsdatum mag niet in het verleden zijn.";
                return View(room);
            }

            // CHECK END DATE

            DateTime close = DateTime.Now;
            bool closeDate = false;
            if (form["closedate"] == null || form["closedate"].Length == 0)
            {

            }
            else if (!DateTime.TryParseExact(form["closedate"], "yyyy-MM-dd", CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out close))
            {
                ViewBag.Error = "Sluitdatum is niet valide.";
                return View(room);
            }
            else if(room.CloseDate.Date != close.Date)
            {
                closeDate = true;
            }

            if (closeDate && close < DateTime.Today)
            {
                ViewBag.Error = "Sluitdatum mag niet in het verleden zijn.";
                return View(room);
            }

            if (startDate && closeDate && close < start)
            {
                ViewBag.Error = "Sluitdatum mag niet voor de openingsdatum zijn.";
                return View(room);
            }
            else if (closeDate && close < room.OpenDate)
            {
                ViewBag.Error = "Sluitdatum mag niet voor de openingsdatum zijn.";
                return View(room);
            }

            // NU AANPASSEN

            if (startDate)
            {
                room.OpenDate = start;
            }
            if (closeDate)
            {
                room.CloseDate = close;
            }
            room.NumberOfPersons = personen;
            room.MinPrice = price;

            roomRepo.UpdateDatabase();

            ViewBag.Success = "De wijzigingen zijn succesvol doorgevoerd.";

            return View(room);
        }

        public ActionResult RoomPrices(int id)
        {
            return View(roomRepo.Get(id));
        }

        [HttpPost]
        public ActionResult Rooms(FormCollection form)
        {
            if (form["minprice"] == null || form["minprice"] == "") {
                ViewBag.Error = "U moet een prijs opgeven";
                return View(roomRepo.GetAll());
            }
            float price = float.Parse(form["minprice"].Replace(',', '.'), System.Globalization.CultureInfo.InvariantCulture);
            if (price < 0)
            {
                ViewBag.Error = "Prijs mag niet lager dan 0 zijn";
                return View(roomRepo.GetAll());
            }
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

        public ActionResult AddRoomPrice()
        {
            return View("Rooms", roomRepo.GetAll());
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
