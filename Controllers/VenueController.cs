//st10440432
//Matteo Nusca
using BookingSystemCLVD.Data;
using BookingSystemCLVD.Models; 
using Microsoft.AspNetCore.Mvc; 
using Microsoft.EntityFrameworkCore; 

public class VenueController : Controller // Define the Venue controller.
{
    private readonly ApplicationDbContext _context; // Database context instance.

    public VenueController(ApplicationDbContext context) // Constructor for dependency injection.
    {
        _context = context;
    }

    // GET: Venues
    public async Task<IActionResult> Index() // Show a list of venues.
    {
        return View(await _context.Venues.ToListAsync()); // Get all venues.
    }

    // GET: Venues/Details/5
    public async Task<IActionResult> Details(int? id) // Show details for a venue.
    {
        if (id == null) return NotFound(); // Handle missing ID.

        var venue = await _context.Venues // Find venue by ID.
            .FirstOrDefaultAsync(m => m.VenueId == id);
        if (venue == null) return NotFound(); // Venue not found.

        return View(venue); // Display venue details.
    }

    // GET: Venues/Create
    public IActionResult Create() // Show create venue form.
    {
        return View(); // Display the create form.
    }

    // POST: Venues/Create
    [HttpPost] // Handle create form submission.
    [ValidateAntiForgeryToken] // Prevent request forgery.
    public async Task<IActionResult> Create([Bind("VenueId,VenueName,Location,Capacity,ImageUrl")] Venue venue)
    {
        if (ModelState.IsValid) // Check if input is valid.
        {
            _context.Add(venue); // Add new venue to database.
            await _context.SaveChangesAsync(); // Save changes to database.
            return RedirectToAction(nameof(Index)); // Go to venue list.
        }
        return View(venue); // Redisplay form with errors.
    }

    // GET: Venues/Edit/5
    public async Task<IActionResult> Edit(int? id) // Show edit venue form.
    {
        if (id == null) return NotFound(); // Handle missing ID.

        var venue = await _context.Venues.FindAsync(id); // Find venue to edit.
        if (venue == null) return NotFound(); // Venue not found.
        return View(venue); // Display the edit form.
    }

    // POST: Venues/Edit/5
    [HttpPost] // Handle edit form submission.
    [ValidateAntiForgeryToken] // Prevent request forgery.
    public async Task<IActionResult> Edit(int id, [Bind("VenueId,VenueName,Location,Capacity,ImageUrl")] Venue venue)
    {
        if (id != venue.VenueId) return NotFound(); // ID mismatch.

        if (ModelState.IsValid) // Check if input is valid.
        {
            try
            {
                _context.Update(venue); // Update existing venue.
                await _context.SaveChangesAsync(); // Save changes to database.
            }
            catch (DbUpdateConcurrencyException) // Handle update conflicts.
            {
                if (!_context.Venues.Any(e => e.VenueId == id)) // Venue not found.
                    return NotFound();
                else throw; // Re-throw other errors.
            }
            return RedirectToAction(nameof(Index)); // Go to venue list.
        }
        return View(venue); // Redisplay form with errors.
    }

    // GET: Venues/Delete/5
    public async Task<IActionResult> Delete(int? id) // Show delete confirmation.
    {
        if (id == null) return NotFound(); // Handle missing ID.

        var venue = await _context.Venues // Find venue to delete.
            .FirstOrDefaultAsync(m => m.VenueId == id);
        if (venue == null) return NotFound(); // Venue not found.

        return View(venue); // Display delete view.
    }

    // POST: Venues/Delete/5
    [HttpPost, ActionName("Delete")] // Handle delete confirmation.
    [ValidateAntiForgeryToken] // Prevent request forgery.
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var venue = await _context.Venues.FindAsync(id); // Find venue to delete.
        _context.Venues.Remove(venue); // Remove venue from database.
        await _context.SaveChangesAsync(); // Save database changes.
        return RedirectToAction(nameof(Index)); // Go to venue list.
    }
}