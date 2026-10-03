
using Microsoft.EntityFrameworkCore;
using Sc2ReplayWebApp.Data;
using Sc2ReplayWebApp.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews()
    .AddJsonOptions(opts => opts.JsonSerializerOptions.PropertyNameCaseInsensitive = true);

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlite(
        builder.Configuration.GetConnectionString(
            "DefaultConnection"));
});

builder.Services.AddScoped<ReplayParserService>();

var app = builder.Build();

// Automatically apply any pending migrations to the database when the application starts
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider
    .GetRequiredService<AppDbContext>();

    db.Database.Migrate();
}

app.UseStaticFiles();

app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Replay}/{action=Upload}/{id?}");

app.Run();
