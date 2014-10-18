using DomainModel;
using HotelWeb.Models;
using HotelWeb.Repositories;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace HotelWeb.Controllers
{
    public class GuestController : Controller
    {
        private EntityAddressRepository addressRepo;
        private EntityBookingRepository bookingRepo;
        private EntityGuestRepository guestRepo;
        private EntityHotelRoomPriceRepository roomPriceRepo;
        private EntityHotelRoomRepository roomRepo;
        private EntityInvoiceRepository invoiceRepo;

        public GuestController()
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
            return View(guestRepo.GetAll());
        }

        public ActionResult Edit(int id)
        {
            return View(guestRepo.Get(id));
        }

        [HttpPost]
        public ActionResult Edit(FormCollection form)
        {
            int id = Int32.Parse(form["GuestId"]);

            Guest guest = guestRepo.Get(id);

            if (guest == null)
            {
                ViewBag.Error = "De opgegeven gast bestaat niet.";
                return View(guest);
            }

            string firstName = form["FirstName"];
            string lastName = form["LastName"];

            // check voornaam en achternaam

            if (firstName == null || firstName.Length == 0)
            {
                ViewBag.Error = "Voornaam mag niet leeg zijn.";
                return View(guest);
            }


            if (firstName != guest.FirstName && !firstName.All(c => char.IsLetter(c) || char.IsSeparator(c)))
            {
                ViewBag.Error = "Voornaam mag alleen letters en spaties bevatten.";
                return View(guest);
            }

            if (lastName == null || lastName.Length == 0)
            {
                ViewBag.Error = "Achternaam mag niet leeg zijn.";
                return View(guest);
            }

            if (lastName != guest.LastName && !lastName.All(c => char.IsLetter(c) || char.IsSeparator(c)))
            {
                ViewBag.Error = "Achternaam mag alleen letters en spaties bevatten.";
                return View(guest);
            }

            // check geboortedatum

            DateTime birthDate = DateTime.Today;
            bool birthDateSet = false;
            if (form["birthdate"] == null || form["birthdate"].Length == 0)
            {

            }
            else if (!DateTime.TryParseExact(form["birthdate"], "yyyy-MM-dd", CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out birthDate))
            {
                ViewBag.Error = "Geboortedatum is niet valide.";
                return View(guest);
            }
            else if (guest.BirthDate.Date != birthDate.Date)
            {
                birthDateSet = true;
            }

            if (birthDateSet)
            {
                if (birthDate.Date < DateTime.Today.AddYears(-130))
                {
                    ViewBag.Error = "Geboortedatum mag niet ouder dan 30 jaar zijn.";
                    return View(guest);
                }
                if (birthDate.Date > DateTime.Today.Date)
                {
                    ViewBag.Error = "Geboortedatum mag niet in de toekomst zijn.";
                    return View(guest);
                }
            }

            // check geslacht

            int gender = Int32.Parse(form["gender"]);
            if (gender != 0 && gender != 1)
            {
                ViewBag.Error = "Het geslacht moet 0 of 1 zijn.";
                return View(guest);
            }

            guest.FirstName = firstName;
            guest.LastName = lastName;
            if (birthDateSet)
            {
                guest.BirthDate = birthDate;
            }
            guest.Gender = gender;

            guestRepo.UpdateDatabase();

            ViewBag.Success = "De wijzigingen zijn succesvol doorgevoerd.";
            return View(guest);
        }

    }
}
