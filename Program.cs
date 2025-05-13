//st10440432
//Matteo Nusca
using BookingSystemCLVD.Data;
using BookingSystemCLVD.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args); // Create web app builder.

// Add services to the container.
builder.Services.AddControllersWithViews(); // Add MVC controllers and views.
builder.Services.AddDbContext<ApplicationDbContext>(options => // Add database context.
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))); // Use SQL Server.

// Register AzureBlobService
builder.Services.AddSingleton<AzureBlobService>();

var app = builder.Build(); // Build the web application.

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment()) // In production
{
    app.UseExceptionHandler("/Home/Error"); // Handle exceptions
}

app.UseHttpsRedirection(); 
app.UseStaticFiles(); 

app.UseRouting(); 
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run(); // Run the web application.
