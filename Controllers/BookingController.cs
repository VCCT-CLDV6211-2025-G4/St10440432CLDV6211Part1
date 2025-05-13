using BookingSystemCLVD.Data;
using BookingSystemCLVD.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

public class BookingController : Controller
{
    private readonly ApplicationDbContext _context;

    public BookingController(ApplicationDbContext context)
    {
        _context = context; // database context injection
    }

    // Show list of bookings, with optional search
    public async Task<IActionResult> Index(string searchTerm)
    {
        var bookings = _context.Bookings
                               .Include(b => b.Event)              // include related event
                               .ThenInclude(e => e.Venue)         // include related venue
                               .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            // filter by booking ID or event name
            bookings = bookings.Where(b =>
                b.BookingId.ToString().Contains(searchTerm) ||
                b.Event.EventName.Contains(searchTerm));
        }

        return View(await bookings.ToListAsync());
    }

    // Show details for one booking
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var booking = await _context.Bookings
                                    .Include(b => b.Event)
                                    .ThenInclude(e => e.Venue)
                                    .FirstOrDefaultAsync(m => m.BookingId == id);
        if (booking == null) return NotFound();

        return View(booking);
    }

    // Display form to create a new booking
    public IActionResult Create()
    {
        ViewData["EventId"] = new SelectList(_context.Events, "EventId", "EventName");
        return View();
    }

    // Handle submission of new booking form
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("BookingId,AttendeeName,AttendeeEmail,EventId")] Booking booking)
    {
        // make sure selected event exists
        var selectedEvent = await _context.Events
                                          .Include(e => e.Venue)
                                          .FirstOrDefaultAsync(e => e.EventId == booking.EventId);

        if (selectedEvent == null)
        {
            ModelState.AddModelError("", "Invalid event selection.");
        }
        else
        {
            // check for duplicate booking (same venue, date, time)
            bool isDuplicate = await _context.Bookings
                .Include(b => b.Event)
                .AnyAsync(b =>
                    b.Event.VenueId == selectedEvent.VenueId &&
                    b.Event.Date == selectedEvent.Date &&
                    b.Event.Time == selectedEvent.Time &&
                    b.AttendeeEmail == booking.AttendeeEmail);

            if (isDuplicate)
            {
                ModelState.AddModelError("", "You have already booked this event or a similar one at the same venue, date, and time.");
            }
        }

        if (ModelState.IsValid)
        {
            _context.Add(booking);           // save new booking
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // if there was a problem, redisplay form
        ViewData["EventId"] = new SelectList(_context.Events, "EventId", "EventName", booking.EventId);
        return View(booking);
    }

    // Display form to edit an existing booking
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var booking = await _context.Bookings.FindAsync(id);
        if (booking == null) return NotFound();

        ViewData["EventId"] = new SelectList(_context.Events, "EventId", "EventName", booking.EventId);
        return View(booking);
    }

    // Handle submission of edit form
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("BookingId,AttendeeName,AttendeeEmail,EventId")] Booking booking)
    {
        if (id != booking.BookingId) return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(booking);     // apply changes
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Bookings.Any(e => e.BookingId == id))
                    return NotFound();
                else
                    throw;
            }
            return RedirectToAction(nameof(Index));
        }

        // if validation fails, show form again
        ViewData["EventId"] = new SelectList(_context.Events, "EventId", "EventName", booking.EventId);
        return View(booking);
    }

    // Show confirmation page to delete a booking
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var booking = await _context.Bookings
                                    .Include(b => b.Event)
                                    .ThenInclude(e => e.Venue)
                                    .FirstOrDefaultAsync(m => m.BookingId == id);
        if (booking == null) return NotFound();

        return View(booking);
    }

    // Actually delete the booking
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var booking = await _context.Bookings.FindAsync(id);
        if (booking != null)
        {
            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}
