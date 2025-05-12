using BookingSystemCLVD.Data;
using BookingSystemCLVD.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args); // Create web app builder.

// Add services to the container.
builder.Services.AddControllersWithViews(); // Add MVC controllers and views.
builder.Services.AddDbContext<ApplicationDbContext>(options => // Add database context.
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))); // Use SQL Server.

// Register AzureBlobService as a singleton.


var app = builder.Build(); // Build the web application.

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment()) // Non-development environment.
{
    app.UseExceptionHandler("/Home/Error"); // Handle exceptions.
    app.UseHsts(); // Use HTTP Strict Transport Security.
}

app.UseHttpsRedirection(); // Redirect HTTP to HTTPS.
app.UseStaticFiles(); // Serve static files.
app.UseRouting(); // Enable endpoint routing.
app.UseAuthorization(); // Enable authorization.

// Map the default route.
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"); // Default URL pattern.

app.Run(); // Run the web application.
