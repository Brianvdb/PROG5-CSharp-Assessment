﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainModel
{
    public class Invoice
    {
        [Key]
        public int InvoiceId { get; set; }
        public Booking Booking { get; set; }
        public Address BillingAddress { get; set; }

        public float TotalPrice { get; set; }

        public int BankAccountNumber { get; set; }
    }
}