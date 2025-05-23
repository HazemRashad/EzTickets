﻿using Data;
using EzTickets.DTO.Pagination;
using Microsoft.EntityFrameworkCore;
using Models;

namespace EzTickets.Repository
{
    public class EventRepository : IEventRepository
    {

        private readonly DataContext _context;
        public EventRepository(DataContext context)
        {
            _context = context;
        }

        public void Delete(int Id)
        {
            var DeletedEvent = GetById(Id);
            if (DeletedEvent == null)
            {
                throw new Exception("No events found");
            }
            else
            {
                _context.Event.Remove(DeletedEvent);
            }
        }

        public PagedResponse<Event> GetAll(PaginationParams pagination)
        {
            var query = _context.Event
                .Where(e => !e.IsDeleted)
                .OrderBy(e => e.StartDate); 

            var pagedData = query
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .ToList();

            var totalRecords = query.Count();

            return new PagedResponse<Event>(
                pagedData,
                pagination.PageNumber,
                pagination.PageSize,
                totalRecords);
        }

        public PagedResponse<Event> GetAllPublic(PaginationParams pagination)
        {
            var query = _context.Event
                .Where(e => e.Status == EventStatus.Published)
                .OrderBy(e => e.StartDate); 

            var pagedData = query
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .ToList();

            var totalRecords = query.Count();

            return new PagedResponse<Event>(
                pagedData,
                pagination.PageNumber,
                pagination.PageSize,
                totalRecords);
        }

        public Event GetById(int Id)
        {
            var TheEvent = _context.Event
                .FirstOrDefault(e => e.EventID == Id);
            if (TheEvent == null)
            {
                throw new Exception("Event not found");
            }
            return TheEvent;
        }

        public Event GetByIdPublic(int Id)
        {
            var TheEvent = _context.Event
                .FirstOrDefault(e => e.EventID == Id && e.Status == EventStatus.Published);

            if (TheEvent == null)
            {
                throw new Exception("Event not found or not published");
            }
            return TheEvent;
        }

        public List<Event> GetEventsByCategory(EventCategoryType category)
        {
            var EventsByCat = _context.Event
                .Where(e => e.Category == category).ToList();
            if (EventsByCat == null)
            {
                throw new Exception("No Events in the selected category");
            }
            return EventsByCat;
        }

        public List<Event> GetEventsByCity(string city)
        {
            var EventsByCity = _context.Event
                 .Where(e => e.City.ToLower().Contains(city)).ToList();
            if (EventsByCity == null)
            {
                throw new Exception($"No Events in {city}");
            }
            return EventsByCity;
        }

        public List<Event> GetEventsByCountry(string country)
        {
            var EventsByCountry = _context.Event
                 .Where(e => e.Country.ToLower().Contains(country)).ToList();
            if (EventsByCountry == null)
            {
                throw new Exception($"No Events in {country}");
            }
            return EventsByCountry;
        }

        public List<Event> GetEventsByDate(DateTime date)
        {
            var EventByDate = _context.Event
                .Where(e => e.StartDate.Date == date.Date).ToList();
            if (EventByDate == null)
            {
                throw new Exception($"No Events starting on {date}");
            }
            return EventByDate;
        }

        public List<Event> GetEventsByDateRange(DateTime startDate, DateTime endDate)
        {
            var EventsByDateRange = _context.Event
                .Where(e => e.StartDate.Date >= startDate.Date && e.EndDate.Date <= endDate.Date).ToList();
            if (EventsByDateRange == null)
            {
                throw new Exception($"No Events found between {startDate} and {endDate}");
            }
            return EventsByDateRange;
        }

        public List<Event> GetEventsByName(string eventName)
        {
            var EventByName = _context.Event
                .Where(e => e.EventName.ToLower().Contains(eventName)).ToList();
            if (EventByName == null)
            {
                throw new Exception($"No Events match the name {eventName}");
            }
            return EventByName;
        }

        public List<Event> GetEventsByPriceRange(decimal minPrice, decimal maxPrice)
        {
            var EventsByPricing = _context.Event
                .Where(e => e.PricePerTicket >= minPrice && e.PricePerTicket <= maxPrice).ToList();
            if (EventsByPricing == null)
            {
                throw new Exception($"No Events found in this price range");
            }
            return EventsByPricing;
        }

        public List<Event> GetEventsByStatus(EventStatus status)
        {
            var EventsByStatus = _context.Event
                .Where(e => e.Status == status).ToList();
            if (EventsByStatus == null)
            {
                throw new Exception($"No {status} Events found");
            }
            return EventsByStatus;
        }

        public List<Event> GetEventsByVenue(string venueName)
        {
            var EventsByVenue = _context.Event
                .Where(e => e.VenueName.ToLower().Contains(venueName)).ToList();
            if (EventsByVenue == null)
            {
                throw new Exception($"No Events found in {venueName}");
            }
            return EventsByVenue;
        }

        public int GetTotalEventsCount()
        {
            var TotalEventsCount = _context.Event
                .Count();
            return TotalEventsCount;
        }

        public void Insert(Event obj)
        {
            _context.Event.Add(obj);
        }

        public void RestoreEvent(int eventId)
        {
            var eventToRestore = GetById(eventId);
            if (eventToRestore != null && eventToRestore.IsDeleted == true)
            {
                eventToRestore.IsDeleted = false;
                eventToRestore.Status = EventStatus.Draft;
                _context.Event.Update(eventToRestore);
            }
            else
            {
                throw new Exception("Event not or not deleted");

            }
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        public void SoftDeleteEvent(int eventId)
        {
            var eventToDelete = GetById(eventId);
            if (eventToDelete != null)
            {
                eventToDelete.Status = EventStatus.Canceled;
                eventToDelete.IsDeleted = true;
                _context.Event.Update(eventToDelete);
            }
            else
            {
                throw new Exception("Event not found");

            }
        }

        public void Update(Event obj)
        {
            _context.Event.Update(obj);
        }

        public PagedResponse<Event> GetFilteredPublicEvents(
                                                         string? searchQuery = null,
                                                         EventCategoryType? category = null,
                                                         string? city = null,
                                                         string? venue = null,
                                                         decimal? minPrice = null,
                                                         decimal? maxPrice = null,
                                                         DateTime? startDate = null,
                                                         DateTime? endDate = null,
                                                         PaginationParams? pagination = null)
        {
            // Default pagination if not provided
            pagination ??= new PaginationParams();

            var query = _context.Event
                .Where(e => e.Status == EventStatus.Published)
                .AsQueryable();

            // Apply filters
            if (!string.IsNullOrEmpty(searchQuery))
                query = query.Where(e => e.EventName.Contains(searchQuery));

            if (category.HasValue)
                query = query.Where(e => e.Category == category.Value);

            if (!string.IsNullOrEmpty(city))
                query = query.Where(e => e.City.Contains(city));

            if (!string.IsNullOrEmpty(venue))
                query = query.Where(e => e.VenueName.Contains(venue));

            if (minPrice.HasValue || maxPrice.HasValue)
                query = query.Where(e => e.PricePerTicket >= (minPrice ?? 0) &&
                                        e.PricePerTicket <= (maxPrice ?? decimal.MaxValue));

            if (startDate.HasValue && endDate.HasValue)
                query = query.Where(e => e.StartDate >= startDate && e.EndDate <= endDate);
            else if (startDate.HasValue)
                query = query.Where(e => e.StartDate.Date == startDate.Value.Date);

            var totalRecords = query.Count();

            var pagedData = query
                .OrderBy(e => e.StartDate)
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .ToList();

            return new PagedResponse<Event>(
                pagedData,
                pagination.PageNumber,
                pagination.PageSize,
                totalRecords);
        }

        public async Task<bool> PublishEvent(int eventId)
        {
            var @event = await _context.Event
                .Include(e => e.Tickets)
                .FirstOrDefaultAsync(e => e.EventID == eventId);

            if (@event == null || @event.Status == EventStatus.Published)
                return false;

            if (!@event.Tickets.Any())
            {
                var tickets = Enumerable.Range(0, @event.TotalTickets)
                    .Select(i => new Ticket
                    {
                        EventID = eventId,
                        TicketStatus = TicketStatus.Available,
                        SeatNumber = @event.VenueName.Contains("GA") ? null : i + 1,
                        Price = @event.PricePerTicket,
                        CreatedAt = DateTime.UtcNow
                    }).ToList();

                await _context.Ticket.AddRangeAsync(tickets);
            }

            @event.Status = EventStatus.Published;
            @event.AvailableTickets = @event.TotalTickets - @event.Tickets.Count(t => t.TicketStatus == TicketStatus.SoldOut);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<PagedResponse<Event>> GetSalesReportData(PaginationParams pagination)
        {
            var query = _context.Event
                .Include(e => e.Tickets) 
                .AsQueryable();

            var pagedData = await query
                .OrderBy(e => e.StartDate)
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .ToListAsync();

            var totalRecords = await query.CountAsync();

            return new PagedResponse<Event>(
                pagedData,
                pagination.PageNumber,
                pagination.PageSize,
                totalRecords);
        }
    }
}
