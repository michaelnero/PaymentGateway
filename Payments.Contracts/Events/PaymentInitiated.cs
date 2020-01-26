﻿using System;
using Infrastructure.EventSourcing;

namespace Payments.Contracts.Events
{
    public class PaymentInitiated : VersionedEvent
    {
        public Guid ChargeId { get; set; }

        public decimal Amount { get; set; }

        public string Currency { get; set; }

        public string CardNumber { get; set; }

        public string Cvv { get; set; }

        public int ExpiryMonth { get; set; }

        public int ExpiryYear { get; set; }
    }
}
