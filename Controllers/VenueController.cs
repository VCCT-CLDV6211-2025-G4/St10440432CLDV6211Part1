//st10440432
//Matteo Nusca

using BookingSystemCLVD.Data;
using BookingSystemCLVD.Models;
using BookingSystemCLVD.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

public class VenueController : Controller
{
    private readonly ApplicationDbContext _context;

    public VenueController(ApplicationDbContext context)
    {
        _context = context; // get database access
    }

    // Show list of all venues
    public async Task<IActionResult> Index()
    {
        return View(await _context.Venues.ToListAsync());
    }

    // Show details of one venue
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var venue = await _context.Venues
            .FirstOrDefaultAsync(m => m.VenueId == id);

        if (venue == null) return NotFound();

        return View(venue);
    }

    // Show the create venue form
    public IActionResult Create()
    {
        return View();
    }

    // Handle form submission to create a venue
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Venue venue, IFormFile ImageFile, [FromServices] AzureBlobService blobService)
    {
        if (ModelState.IsValid)
        {
            // Upload image to Azure if selected
            if (ImageFile != null && ImageFile.Length > 0)
            {
                venue.ImageUrl = await blobService.UploadFileAsync(ImageFile);
            }

            _context.Add(venue); // save venue to database
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        return View(venue); // show form again if error
    }

    // Show the edit form for a venue
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var venue = await _context.Venues.FindAsync(id);
        if (venue == null) return NotFound();

        return View(venue);
    }

    // Handle edit form submission
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Venue venue, IFormFile ImageFile, [FromServices] AzureBlobService blobService)
    {
        if (id != venue.VenueId) return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                // Upload new image if one is selected
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    venue.ImageUrl = await blobService.UploadFileAsync(ImageFile);
                }

                _context.Update(venue); // update venue
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Venues.AnyAsync(e => e.VenueId == id))
                    return NotFound();
                else
                    throw;
            }
        }

        return View(venue); // show form again if error
    }

    // Show confirmation page to delete a venue
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var venue = await _context.Venues
            .FirstOrDefaultAsync(m => m.VenueId == id);

        if (venue == null) return NotFound();

        return View(venue);
    }

    // Handle actual deletion of venue
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        // Don't allow delete if venue is linked to events
        var hasLinkedEvents = await _context.Events.AnyAsync(e => e.VenueId == id);
        if (hasLinkedEvents)
        {
            ModelState.AddModelError("", "This venue cannot be deleted because it is associated with one or more events.");
            var venue = await _context.Venues.FindAsync(id);
            return View("Delete", venue);
        }

        // Proceed to delete
        var venueToDelete = await _context.Venues.FindAsync(id);
        if (venueToDelete != null)
        {
            _context.Venues.Remove(venueToDelete);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }
}
