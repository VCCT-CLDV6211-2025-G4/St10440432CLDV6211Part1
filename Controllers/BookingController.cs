//st10440432
//Matteo Nusca
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookingSystemCLVD.Data;
using BookingSystemCLVD.Models;

public class BookingController : Controller
{
    private readonly ApplicationDbContext _context; // Database context.

    public BookingController(ApplicationDbContext context) // Constructor for dependency.
    {
        _context = context;
    }

    public async Task<IActionResult> Index() => // Show all bookings.
        View(await _context.Bookings.Include(b => b.Event).ToListAsync());

    public async Task<IActionResult> Details(int? id) // Show details of one booking.
    {
        if (id == null) return NotFound(); // Handle missing ID.

        var booking = await _context.Bookings // Find booking with related event.
            .Include(b => b.Event)
            .FirstOrDefaultAsync(m => m.BookingId == id);

        if (booking == null) return NotFound(); // Booking not found.

        return View(booking); // Display booking details.
    }

    public IActionResult Create() // Show create booking form.
    {
        // Populate EventId dropdown.
        ViewData["EventId"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.Events, "EventId", "EventName");
        return View(); // Display create form.
    }

    [HttpPost] // Handle create form submission.
    [ValidateAntiForgeryToken] // Prevent cross-site request forgery.
    public async Task<IActionResult> Create([Bind("BookingId,AttendeeName,AttendeeEmail,EventId")] Booking booking)
    {
        if (ModelState.IsValid) // Check if input is valid.
        {
            _context.Add(booking); // Add new booking to database.
            await _context.SaveChangesAsync(); // Save changes to database.
            return RedirectToAction(nameof(Index)); // Redirect to booking list.
        }

        // Repopulate dropdown on error.
        ViewData["EventId"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.Events, "EventId", "EventName", booking.EventId);
        return View(booking); // Redisplay form with errors.
    }

    public async Task<IActionResult> Edit(int? id) // Show edit booking form.
    {
        if (id == null) return NotFound(); // Handle missing ID.

        var booking = await _context.Bookings.FindAsync(id); // Find booking to edit.
        if (booking == null) return NotFound(); // Booking not found.

        // Populate EventId dropdown.
        ViewData["EventId"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.Events, "EventId", "EventName", booking.EventId);
        return View(booking); // Display edit form.
    }

    [HttpPost] // Handle edit form submission.
    [ValidateAntiForgeryToken] // Prevent cross-site request forgery.
    public async Task<IActionResult> Edit(int id, [Bind("BookingId,AttendeeName,AttendeeEmail,EventId")] Booking booking)
    {
        if (id != booking.BookingId) return NotFound(); // ID mismatch.

        if (ModelState.IsValid) // Check if input is valid.
        {
            try
            {
                _context.Update(booking); // Update existing booking.
                await _context.SaveChangesAsync(); // Save changes to database.
            }
            catch (DbUpdateConcurrencyException) // Handle database update conflicts.
            {
                if (!_context.Bookings.Any(e => e.BookingId == id)) // Booking not found.
                    return NotFound();
                else throw; // Re-throw other exceptions.
            }

            return RedirectToAction(nameof(Index)); // Redirect to booking list.
        }

        // Repopulate dropdown on error.
        ViewData["EventId"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.Events, "EventId", "EventName", booking.EventId);
        return View(booking); // Redisplay form with errors.
    }

    public async Task<IActionResult> Delete(int? id) // Show delete confirmation form.
    {
        if (id == null) return NotFound(); // Handle missing ID.

        var booking = await _context.Bookings // Find booking for deletion.
            .Include(b => b.Event)
            .FirstOrDefaultAsync(m => m.BookingId == id);

        if (booking == null) return NotFound(); // Booking not found.

        return View(booking); // Display delete confirmation.
    }

    [HttpPost, ActionName("Delete")] // Handle delete confirmation submission.
    [ValidateAntiForgeryToken] // Prevent cross-site request forgery.
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var booking = await _context.Bookings.FindAsync(id); // Find booking to delete.
        _context.Bookings.Remove(booking); // Remove booking from database.
        await _context.SaveChangesAsync(); // Save changes to database.
        return RedirectToAction(nameof(Index)); // Redirect to booking list.
    }
}
