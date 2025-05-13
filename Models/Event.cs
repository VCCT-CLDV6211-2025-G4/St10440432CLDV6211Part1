//st10440432
//Matteo Nusca

using BookingSystemCLVD.Models;
using System.ComponentModel.DataAnnotations;

public class Event
{
    public int EventId { get; set; } // Primary key

    [Required]
    public string EventName { get; set; } // Name of the event

    [DataType(DataType.Date)]
    public DateTime Date { get; set; } // Date the event takes place

    [DataType(DataType.Time)]
    public DateTime Time { get; set; } // Time the event starts

    public int VenueId { get; set; } // Foreign key to the venue
    public Venue? Venue { get; set; } // Navigation property to the venue

    public ICollection<Booking>? Bookings { get; set; } // Bookings for this event
}
