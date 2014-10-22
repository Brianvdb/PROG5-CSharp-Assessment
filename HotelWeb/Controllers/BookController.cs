
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
        private EntityAddressRepository addressRepo;
        private EntityInvoiceRepository invoiceRepo;
        private EntityGuestRepository guestRepo;
        private EntityHotelRoomPriceRepository priceRepo;

        public BookController()
        {
            DatabaseContext db = new DatabaseContext();
            roomRepo = new EntityHotelRoomRepository(db);
            bookRepo = new EntityBookingRepository(db);
            addressRepo = new EntityAddressRepository(db);
            invoiceRepo = new EntityInvoiceRepository(db);
            guestRepo = new EntityGuestRepository(db);
            priceRepo = new EntityHotelRoomPriceRepository(db);
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
            if ((thisRoom.CloseDate > thisRoom.OpenDate && end >= thisRoom.CloseDate) || start <= thisRoom.OpenDate || start < DateTime.Now.Date)
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

        [HttpPost]
        public ActionResult RegisterGuests(FormCollection planData)
        {
            bool correct = false;

            DateTime startDate = new DateTime();
            DateTime endDate = new DateTime();
            int nights = -1;
            int roomId = -1;

            try
            {
                startDate = DateTime.Parse(planData["start-date-input"]);
                nights = int.Parse(planData["nights-input"]);
                roomId = int.Parse(planData["room-id-input"]);

                //calculate enddate
                endDate = startDate.AddDays(nights);
            }
            catch
            {
                return Json("Er trad een fout op tijdens het parsen van uw planning!");
            }

            //check room == free
            correct = RoomIsFree(startDate, endDate, roomId);

            //register the information that is available after the planning has been sent
            if (correct)
            {
                BookingData data = new BookingData() {
                    StartDate = startDate,
                    EndDate = endDate,
                    Nights = nights,
                    RoomId = roomId
                };
                Session["BookingData"] = data;
            }
            
            //return the view 
            ViewBag.StartDate = startDate.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endDate.ToString("yyyy-MM-dd");
            ViewBag.RoomId = roomId;
            ViewBag.MaxPersons = roomRepo.Get(roomId).NumberOfPersons;

            return View();
        }

        [HttpPost]
        public ActionResult RegisterGuestInfo(PersonalInfo[] input)
        {
            BookingData data = Session["BookingData"] as BookingData;
            int adressCount = input.Length;

            if (adressCount > roomRepo.Get(data.RoomId).NumberOfPersons)
            {
                return Json("{\"error\":\"Er zijn meer klanten opgegeven dan is toegestaan voor de kamer (" + data.RoomId + " m " + roomRepo.Get(data.RoomId).NumberOfPersons + ")\"}");
            }

            //make sure the list is empty
            data.resetGuests();
            for (int x = 0; x < adressCount; x++)
            {
                try
                {
                    data.GuestList.Add(new BookingGuest() { FirstName = input[x].firstName, LastName = input[x].lastName, BirthDate = DateTime.Parse(input[x].birthDate), Gender = input[x].sex == "male" ? 0 : 1 });

                }
                catch
                {
                    return Json("{\"error\":\"Form velden zijn vergeten\"}");
                }
            }

            Session["BookingData"] = data;

            return Json(adressCount);
        }

        [HttpPost]
        public ActionResult AdressForm()
        {
            return View();
        }

        [HttpPost]
        public ActionResult RegisterAdress(FormCollection form)
        {
            BookingData data = Session["BookingData"] as BookingData;

            //assign invoice data
            data.Street = form["adress"];
            data.PostalCode = form["postalcode"];
            data.HomeTown = form["hometown"];

            if (data.Street == "" || data.PostalCode == "" || data.HomeTown == "")
            {
                return RedirectToRoute("AdressForm");
            }

            Session["BookingData"] = data;

            return View(data);
        }

        public ActionResult RegisterBooking(FormCollection form)
        {
            BookingData data = Session["BookingData"] as BookingData;

            HotelRoom room = roomRepo.Get(data.RoomId);

            //assign invoice data
            string bankAccount = form["bankaccount"];
            string mail = form["mail"];

            string invoiceAdress = form["adress"];
            string invoicePostalcode = form["postalcode"];
            string invoiceHometown = form["hometown"];

            if (mail == "" || bankAccount == "" || invoiceAdress == "" || invoicePostalcode == "" || invoiceHometown == "" || data.TotalPrice <= 0)
            {

                ViewBag.Error = "Helaas zijn niet alle gegevens aanwezig. De boeking is mislukt.";
                return View();
            }

            Address address = new Address()
            {
                Street = data.Street,
                HomeTown = data.HomeTown,
                PostalCode = data.PostalCode
            };

            Address billingAddress;
            if (invoiceAdress == data.Street && invoicePostalcode == data.PostalCode && invoiceHometown == data.HomeTown)
            {
                billingAddress = address;
            }
            else
            {
                billingAddress = new Address()
                {
                    Street = invoiceAdress,
                    HomeTown = invoiceHometown,
                    PostalCode = invoicePostalcode
                };
            }

            Invoice invoice = new Invoice()
            {
                BillingAddress = billingAddress,
                TotalPrice = (float)data.TotalPrice, 
                BankAccountNumber = bankAccount
            };

            Booking booking = new Booking()
            {
                Guests = new List<Guest>(),
                HotelRoom = room,
                Email = mail,
                GuestAddress = address, // NORMAAL GAST ADRES
                StartDate = data.StartDate,
                EndDate = data.EndDate,
                Invoice = invoice
            };

            foreach (BookingGuest g in data.GuestList)
            {
                Guest guest = new Guest()
                {
                    FirstName = g.FirstName,
                    LastName = g.LastName,
                    BirthDate = g.BirthDate,
                    Gender = g.Gender
                };
                booking.Guests.Add(guest);
            }

            foreach(Guest g in booking.Guests) {
                guestRepo.Add(g);
            }

            addressRepo.Add(address);
            if (billingAddress != address)
            {
                addressRepo.Add(billingAddress);
            }
            invoiceRepo.Add(invoice);
            bookRepo.Add(booking);

            room.Bookings.Add(booking);

            try
            {
                roomRepo.UpdateDatabase();
                ViewBag.Succes = "De booking is opgeslagen!";
            }
            catch
            {
                ViewBag.Error = "Kon de booking niet opslaan. Booking mislukt";
            }


            Session["BookingData"] = null;

            //return View(data);
            return View();
        }

        [HttpPost]
        public ActionResult GetPrices()
        {
            BookingData data = Session["BookingData"] as BookingData;

            DateTime startDate = data.StartDate;
            int roomId = data.RoomId;
            double minPrice = roomRepo.Get(roomId).MinPrice;
            int nights = data.Nights;

            List<NightPriceJson> priceList = new List<NightPriceJson>();
            IEnumerable<HotelRoomPrice> prices = priceRepo.GetAll().Where(price => price.StartDate < data.EndDate && price.EndDate >= data.StartDate);

            for (int night = 0; night < nights; night++)
            {
                double altPrice = -1;
                foreach (HotelRoomPrice sprice in prices)
                {
                    if (sprice.StartDate <= startDate.AddDays(night) && sprice.EndDate >= startDate.AddDays(night))
                    {
                        altPrice = sprice.Price;
                    }
                }

                priceList.Add(new NightPriceJson() { date = startDate.AddDays(night).ToString("yyyy-MM-dd"), price = altPrice == -1 ? minPrice : altPrice });
            }

            //calculate total price
            double totalPrice = 0;
            foreach (NightPriceJson dayPrice in priceList)
            {
                totalPrice += dayPrice.price;
            }

            data.TotalPrice = totalPrice;
            Session["BookingData"] = data;

            return Json(priceList.ToJSON());
        }
        
        public ActionResult SessionTest()
        {
            BookingData data = Session["BookingData"] as BookingData;
            return Content(data.StartDate.ToString("yyyy-MM-dd"));
        }

        public ActionResult GuestTest()
        {
            BookingData data = Session["BookingData"] as BookingData;
            return Content(data.GuestList[1].FirstName);
        }
    }
}
