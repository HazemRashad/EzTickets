﻿
namespace EzTickets.Services
{
    public class FakePaymentService
    {
        private static readonly Random _random = new();

        //public bool ProcessPayment(PaymentViewModel payment)
        //{
        //    // Simulate failure in 5% of cases 
        //    return _random.Next(1, 101) > 5;
        //}
    }
}
