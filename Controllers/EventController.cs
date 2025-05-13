//st10440432
//Matteo Nusca

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BookingSystemCLVD.Data;
using BookingSystemCLVD.Models;

public class EventController : Controller
{
    private readonly ApplicationDbContext _context;

    public EventController(ApplicationDbContext context)
    {
        _context = context;
    }

    // Show all events
    public async Task<IActionResult> Index()
    {
        return View(await _context.Events.Include(e => e.Venue).ToListAsync());
    }

    // Show details for one event
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var ev = await _context.Events
            .Include(e => e.Venue)
            .FirstOrDefaultAsync(m => m.EventId == id);

        if (ev == null) return NotFound();

        return View(ev);
    }

    // Show create form
    public IActionResult Create()
    {
        ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "VenueName");
        return View();
    }

    // Handle form submission for creating event
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("EventId,EventName,Date,Time,VenueId")] Event ev)
    {
        if (ModelState.IsValid)
        {
            // Check for double booking
            var existingEvent = await _context.Events.FirstOrDefaultAsync(e =>
                e.VenueId == ev.VenueId &&
                e.Date.Date == ev.Date.Date &&
                e.Time.TimeOfDay == ev.Time.TimeOfDay);

            if (existingEvent != null)
            {
                ModelState.AddModelError("", "This venue is already booked for the selected date and time.");
            }
            else
            {
                _context.Add(ev);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
        }

        ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "VenueName", ev.VenueId);
        return View(ev);
    }

    // Show edit form
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var ev = await _context.Events.FindAsync(id);
        if (ev == null) return NotFound();

        ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "VenueName", ev.VenueId);
        return View(ev);
    }

    // Handle form submission for editing
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("EventId,EventName,Date,Time,VenueId")] Event ev)
    {
        if (id != ev.EventId) return NotFound();

        if (ModelState.IsValid)
        {
            // Check for conflicts with other events
            var conflict = await _context.Events.FirstOrDefaultAsync(e =>
                e.EventId != ev.EventId &&
                e.VenueId == ev.VenueId &&
                e.Date.Date == ev.Date.Date &&
                e.Time.TimeOfDay == ev.Time.TimeOfDay);

            if (conflict != null)
            {
                ModelState.AddModelError("", "This venue is already booked for the selected date and time."); // error message
            }
            else
            {
                try
                {
                    _context.Update(ev);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Events.Any(e => e.EventId == id))
                        return NotFound();
                    else
                        throw;
                }
            }
        }

        ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "VenueName", ev.VenueId);
        return View(ev);
    }

    // Show delete confirmation
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var ev = await _context.Events
            .Include(e => e.Venue)
            .FirstOrDefaultAsync(m => m.EventId == id);

        if (ev == null) return NotFound();

        return View(ev);
    }

    // Delete the event (if no bookings exist)
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var hasBookings = await _context.Bookings.AnyAsync(b => b.EventId == id);

        if (hasBookings)
        {
            ModelState.AddModelError("", "Cannot delete this event because it has existing bookings.");
            var ev = await _context.Events.Include(e => e.Venue).FirstOrDefaultAsync(e => e.EventId == id);
            return View("Delete", ev);
        }

        var evToDelete = await _context.Events.FindAsync(id);
        _context.Events.Remove(evToDelete);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}
