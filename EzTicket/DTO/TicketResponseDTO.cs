﻿using Models;

namespace EzTickets.DTO
{
    public class TicketResponseDTO
    {
        public string TicketID { get; set; }
        public string EventID { get; set; }
        public string EventName { get; set; }
        public string? UserID { get; set; }
        public string? UserFullName { get; set; }
        public TicketType TicketType { get; set; }
        public decimal Price { get; set; }
        public DateTime? PurchaseDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public TicketStatus TicketStatus { get; set; }
        public int? SeatNumber { get; set; }
        public string? QRCode { get; set; }
        public bool IsDeleted { get; set; }
    }
}
