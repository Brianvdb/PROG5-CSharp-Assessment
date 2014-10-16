﻿using DomainModel;
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

    }
}