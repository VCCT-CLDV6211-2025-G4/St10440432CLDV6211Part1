using BookingSystemCLVD.Data;
using BookingSystemCLVD.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

public class BookingController : Controller
{
    private readonly ApplicationDbContext _context;

    public BookingController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: Booking
    public async Task<IActionResult> Index()
    {
        // Include related Event data when listing bookings
        var bookings = await _context.Bookings
                                     .Include(b => b.Event)
                                     .ToListAsync();
        return View(bookings);
    }

    // GET: Booking/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
            return NotFound();

        // Fetch booking with associated event
        var booking = await _context.Bookings
                                    .Include(b => b.Event)
                                    .FirstOrDefaultAsync(m => m.BookingId == id);
        if (booking == null)
            return NotFound();

        return View(booking);
    }

    // GET: Booking/Create
    public IActionResult Create()
    {
        // Populate Event dropdown list
        ViewData["EventId"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.Events, "EventId", "EventName");
        return View();
    }

    // POST: Booking/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("BookingId,AttendeeName,AttendeeEmail,EventId")] Booking booking)
    {
        if (ModelState.IsValid)
        {
            _context.Add(booking);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Repopulate Event dropdown if model is invalid
        ViewData["EventId"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.Events, "EventId", "EventName", booking.EventId);
        return View(booking);
    }

    // GET: Booking/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
            return NotFound();

        var booking = await _context.Bookings.FindAsync(id);
        if (booking == null)
            return NotFound();

        // Populate Event dropdown with selected value
        ViewData["EventId"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.Events, "EventId", "EventName", booking.EventId);
        return View(booking);
    }

    // POST: Booking/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("BookingId,AttendeeName,AttendeeEmail,EventId")] Booking booking)
    {
        if (id != booking.BookingId)
            return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(booking);
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

        // Repopulate Event dropdown on error
        ViewData["EventId"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.Events, "EventId", "EventName", booking.EventId);
        return View(booking);
    }

    // GET: Booking/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
            return NotFound();

        // Fetch booking and related event
        var booking = await _context.Bookings
                                    .Include(b => b.Event)
                                    .FirstOrDefaultAsync(m => m.BookingId == id);
        if (booking == null)
            return NotFound();

        return View(booking);
    }

    // POST: Booking/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var booking = await _context.Bookings.FindAsync(id);
        _context.Bookings.Remove(booking);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}