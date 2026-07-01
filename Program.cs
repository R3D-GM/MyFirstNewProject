using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using MyFirstNewProject.Data;
using MyFirstNewProject.Models;
using MyFirstNewProject.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// ============================================
// ADD IDENTITY SERVICES WITH IN-MEMORY DATABASE
// ============================================

// ✅ USE IN-MEMORY DATABASE (No SQLite needed!)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseInMemoryDatabase("Project1Db"));

// Add Identity with roles
builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    // Password settings
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.Password.RequiredUniqueChars = 1;

    // Lockout settings
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // User settings
    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedEmail = false;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>();

// Add cookie authentication
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromDays(14);
    options.SlidingExpiration = true;
});

// ============================================
// EXISTING SERVICES
// ============================================

// Register API Service
builder.Services.AddHttpClient<ConsigneeService>();
builder.Services.AddScoped<ConsigneeService>();

// Add in-memory caching
builder.Services.AddMemoryCache();

// Add response caching
builder.Services.AddResponseCaching();

// Add response compression
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();
});
builder.Services.Configure<BrotliCompressionProviderOptions>(options =>
{
    options.Level = System.IO.Compression.CompressionLevel.Fastest;
});
builder.Services.Configure<GzipCompressionProviderOptions>(options =>
{
    options.Level = System.IO.Compression.CompressionLevel.Fastest;
});

// ============================================
// ✅ ADD ACTIVITY LOG SERVICE
// ============================================
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ActivityLogService>();

// ============================================
// ✅ ADD NOTIFICATION SERVICE
// ============================================
builder.Services.AddScoped<NotificationService>();

// ============================================
// ✅ ADD VALIDATION SERVICE (Task 6)
// ============================================
builder.Services.AddScoped<ValidationService>();

// ============================================
// ✅ ADD PERFORMANCE SERVICE (Task 7)
// ============================================
builder.Services.AddScoped<PerformanceService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// ============================================
// ✅ GLOBAL EXCEPTION HANDLING (Task 6)
// ============================================
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var exception = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>()?.Error;
        
        if (exception != null)
        {
            var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
            logger.LogError(exception, "Unhandled exception occurred");
            
            context.Response.StatusCode = 500;
            context.Response.ContentType = "text/html";
            
            await context.Response.WriteAsync($@"
                <html>
                <head>
                    <title>Error</title>
                    <style>
                        body {{ font-family: Arial; text-align: center; padding: 50px; background: #0a0e1a; color: #fff; }}
                        h1 {{ color: #ef4444; }}
                        .error-box {{ background: #1a2332; padding: 30px; border-radius: 12px; max-width: 600px; margin: auto; }}
                    </style>
                </head>
                <body>
                    <div class='error-box'>
                        <h1>⚠️ Something went wrong</h1>
                        <p>We're sorry, but an error occurred while processing your request.</p>
                        <p style='color: #94a3b8; font-size: 14px;'>Error: {exception.Message}</p>
                        <a href='/' style='color: #60a5fa; text-decoration: none;'>← Go back to Home</a>
                    </div>
                </body>
                </html>
            ");
        }
    });
});

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseResponseCaching();

app.UseResponseCompression();

// ============================================
// AUTHENTICATION & AUTHORIZATION
// ============================================
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

// ============================================
// SEED DATA
// ============================================
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        await SeedData.InitializeAsync(services);
        Console.WriteLine("✅ Database seeded successfully!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Error seeding database: {ex.Message}");
    }
}

app.Run();