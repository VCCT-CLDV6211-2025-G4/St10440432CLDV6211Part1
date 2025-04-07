using BookingSystemCLVD.Models; 
using System.ComponentModel.DataAnnotations; 

public class Venue // Define the Venue data model.
{
    public int VenueId { get; set; } // Primary key for Venue.

    [Required] // VenueName is required.
    public string VenueName { get; set; } // Name of the venue.

    [Required] // Location is required.
    public string Location { get; set; } // Location of the venue.

    [Range(1, 100000)] // Capacity must be between 1 and 100000.
    public int Capacity { get; set; } // Seating capacity of venue.

    public string? ImageUrl { get; set; } // Optional URL for image.

    public ICollection<Event>? Events { get; set; } // Navigation to Events.
}