//st10440432
//Matteo Nusca
using System.ComponentModel.DataAnnotations;

public class Booking // Define the Booking data model.
{
    public int BookingId { get; set; } // Primary key for Booking.

    [Required] // AttendeeName is required.
    public string AttendeeName { get; set; } // Name of the attendee.

    [Required, EmailAddress] // AttendeeEmail is required and must be valid.
    public string AttendeeEmail { get; set; } // Email address of attendee.

    public int EventId { get; set; } // Foreign key for Event.
    public Event? Event { get; set; } // Navigation property to Event.
}

