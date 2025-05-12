//st10440432
//Matteo Nusca

using BookingSystemCLVD.Data;
using BookingSystemCLVD.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using BookingSystemCLVD.Services;

public class VenueController : Controller
{
    private readonly ApplicationDbContext _context; // Database context.

    public VenueController(ApplicationDbContext context) // Constructor for dependency injection.
    {
        _context = context;
    }

    // GET: Venues - Display list of all venues.
    public async Task<IActionResult> Index()
    {
        return View(await _context.Venues.ToListAsync()); // Retrieve all venues from database.
    }

    // GET: Venues/Details/5 - Display venue details by ID.
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
            return NotFound(); // Return 404 if no ID provided.

        var venue = await _context.Venues
            .FirstOrDefaultAsync(m => m.VenueId == id); // Find venue by ID.

        if (venue == null)
            return NotFound(); // Return 404 if venue not found.

        return View(venue); // Show venue details.
    }

    // GET: Venues/Create - Display create venue form.
    public IActionResult Create()
    {
        return View(); // Show empty venue form.
    }

    // POST: Venues/Create - Handle new venue submission.
    [HttpPost]
    [ValidateAntiForgeryToken] // Prevent cross-site request forgery.
    public async Task<IActionResult> Create(Venue venue, IFormFile ImageFile, [FromServices] AzureBlobService blobService)
    {
        if (ModelState.IsValid) // Validate input.
        {
            if (ImageFile != null) // Check if image was uploaded.
                venue.ImageUrl = await blobService.UploadFileAsync(ImageFile); // Upload image to Azure Blob.

            _context.Add(venue); // Add venue to database.
            await _context.SaveChangesAsync(); // Save changes.
            return RedirectToAction(nameof(Index)); // Go back to venue list.
        }

        return View(venue); // Show form again if model state invalid.
    }

    // GET: Venues/Edit/5 - Display edit form for selected venue.
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
            return NotFound(); // Return 404 if ID not provided.

        var venue = await _context.Venues.FindAsync(id); // Find venue.
        if (venue == null)
            return NotFound(); // Return 404 if venue not found.

        return View(venue); // Show edit form.
    }

    // POST: Venues/Edit/5 - Handle venue update.
    [HttpPost]
    [ValidateAntiForgeryToken] // Prevent cross-site request forgery.
    public async Task<IActionResult> Edit(int id, [Bind("VenueId,VenueName,Location,Capacity,ImageUrl")] Venue venue)
    {
        if (id != venue.VenueId)
            return NotFound(); // ID mismatch - return 404.

        if (ModelState.IsValid) // Validate input.
        {
            try
            {
                _context.Update(venue); // Update venue in database.
                await _context.SaveChangesAsync(); // Save changes.
            }
            catch (DbUpdateConcurrencyException) // Handle concurrency error.
            {
                if (!_context.Venues.Any(e => e.VenueId == id)) // Check if venue exists.
                    return NotFound(); // Return 404 if not found.
                else
                    throw; // Re-throw error if it's something else.
            }

            return RedirectToAction(nameof(Index)); // Go back to venue list.
        }

        return View(venue); // Show form again if model invalid.
    }

    // GET: Venues/Delete/5 - Confirm deletion of venue.
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
            return NotFound(); // Return 404 if no ID.

        var venue = await _context.Venues
            .FirstOrDefaultAsync(m => m.VenueId == id); // Find venue by ID.

        if (venue == null)
            return NotFound(); // Return 404 if not found.

        return View(venue); // Show delete confirmation.
    }

    // POST: Venues/Delete/5 - Handle confirmed deletion.
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken] // Prevent cross-site request forgery.
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var venue = await _context.Venues.FindAsync(id); // Find venue by ID.
        _context.Venues.Remove(venue); // Remove from database.
        await _context.SaveChangesAsync(); // Save changes.
        return RedirectToAction(nameof(Index)); // Go back to venue list.
    }
}
