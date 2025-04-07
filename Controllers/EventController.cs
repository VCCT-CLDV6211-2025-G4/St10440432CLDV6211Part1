using Microsoft.AspNetCore.Mvc; 
using Microsoft.EntityFrameworkCore; 
using BookingSystemCLVD.Data; 
using BookingSystemCLVD.Models; 

public class EventController : Controller // Define the Event controller.
{
    private readonly ApplicationDbContext _context; // Database context instance.

    public EventController(ApplicationDbContext context) // Constructor for dependency injection.
    {
        _context = context;
    }

    public async Task<IActionResult> Index() => // Show list of events.
        View(await _context.Events.Include(e => e.Venue).ToListAsync()); // Include venue data.

    public async Task<IActionResult> Details(int? id) // Show details of an event.
    {
        if (id == null) return NotFound(); // Handle missing event ID.

        var ev = await _context.Events // Find event with venue.
            .Include(e => e.Venue)
            .FirstOrDefaultAsync(m => m.EventId == id);
        if (ev == null) return NotFound(); // Event not found.

        return View(ev); // Display event details.
    }

    public IActionResult Create() // Show create event form.
    {
        // Populate VenueId dropdown.
        ViewData["VenueId"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.Venues, "VenueId", "VenueName");
        return View(); // Display create form.
    }

    [HttpPost] // Handle create form submission.
    [ValidateAntiForgeryToken] // Prevent request forgery.
    public async Task<IActionResult> Create([Bind("EventId,EventName,Date,Time,VenueId")] Event ev)
    {
        if (ModelState.IsValid) // Check if input is valid.
        {
            _context.Add(ev); // Add new event to database.
            await _context.SaveChangesAsync(); // Save changes to database.
            return RedirectToAction(nameof(Index)); // Go to event list.
        }
        // Repopulate dropdown on error.
        ViewData["VenueId"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.Venues, "VenueId", "VenueName", ev.VenueId);
        return View(ev); // Redisplay form with errors.
    }

    public async Task<IActionResult> Edit(int? id) // Show edit event form.
    {
        if (id == null) return NotFound(); // Handle missing ID.

        var ev = await _context.Events.FindAsync(id); // Find event to edit.
        if (ev == null) return NotFound(); // Event not found.

        // Populate VenueId dropdown.
        ViewData["VenueId"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.Venues, "VenueId", "VenueName", ev.VenueId);
        return View(ev); // Display edit form.
    }

    [HttpPost] // Handle edit form submission.
    [ValidateAntiForgeryToken] // Prevent request forgery.
    public async Task<IActionResult> Edit(int id, [Bind("EventId,EventName,Date,Time,VenueId")] Event ev)
    {
        if (id != ev.EventId) return NotFound(); // ID mismatch.

        if (ModelState.IsValid) // Check if input is valid.
        {
            try
            {
                _context.Update(ev); // Update existing event.
                await _context.SaveChangesAsync(); // Save database changes.
            }
            catch (DbUpdateConcurrencyException) // Handle update conflicts.
            {
                if (!_context.Events.Any(e => e.EventId == id)) // Event not found.
                    return NotFound();
                else throw; // Re-throw other errors.
            }
            return RedirectToAction(nameof(Index)); // Go to event list.
        }
        // Repopulate dropdown on error.
        ViewData["VenueId"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.Venues, "VenueId", "VenueName", ev.VenueId);
        return View(ev); // Redisplay form with errors.
    }

    public async Task<IActionResult> Delete(int? id) // Show delete confirmation.
    {
        if (id == null) return NotFound(); // Handle missing ID.

        var ev = await _context.Events // Find event with venue.
            .Include(e => e.Venue)
            .FirstOrDefaultAsync(m => m.EventId == id);
        if (ev == null) return NotFound(); // Event not found.

        return View(ev); // Display delete view.
    }

    [HttpPost, ActionName("Delete")] // Handle delete confirmation.
    [ValidateAntiForgeryToken] // Prevent request forgery.
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var ev = await _context.Events.FindAsync(id); // Find event to delete.
        _context.Events.Remove(ev); // Remove event from database.
        await _context.SaveChangesAsync(); // Save database changes.
        return RedirectToAction(nameof(Index)); // Go to event list.
    }
}
