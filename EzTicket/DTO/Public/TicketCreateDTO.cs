﻿using Models;

namespace EzTickets.DTO.Public
{
    public class TicketCreateDTO
    {
        public int EventID { get; set; }
        public string? UserID { get; set; }
        public TicketType TicketType { get; set; } = TicketType.Regular;
        public decimal Price { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public int? SeatNumber { get; set; }

    }
}
