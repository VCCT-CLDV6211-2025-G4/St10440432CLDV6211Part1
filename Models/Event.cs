using BookingSystemCLVD.Models;
using System.ComponentModel.DataAnnotations;

//st10440432
//Matteo Nusca
public class Event // Define the Event data model.
{
    public int EventId { get; set; } // Primary key for Event.

    [Required] // EventName is required.
    public string EventName { get; set; } // Name of the event.

    [DataType(DataType.Date)] // Specifies Date data type.
    public DateTime Date { get; set; } // Date of the event.

    [DataType(DataType.Time)] // Specifies Time data type.
    public DateTime Time { get; set; } // Time of the event.

    public int VenueId { get; set; } // Foreign key for Venue.
    public Venue? Venue { get; set; } // Navigation to the Venue.

    public ICollection<Booking>? Bookings { get; set; } // Navigation to Bookings.
}