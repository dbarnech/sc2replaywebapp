using Microsoft.EntityFrameworkCore;
using Sc2ReplayWebApp.Data;
using Sc2ReplayWebApp.Extensions;
using Sc2ReplayWebApp.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews()
    .AddJsonOptions(opts => opts.JsonSerializerOptions.PropertyNameCaseInsensitive = true);

builder.Services
    .AddDbContext<AppDbContext>(options =>
    {
        options.UseSqlite(
            builder.Configuration.GetConnectionString("DefaultConnection"));
    });

builder.Services
    .AddBattleNetAuthentication(builder.Configuration);

builder.Services
    .AddHttpClient()
    .AddScoped<IReplayParserService, ReplayParserService>()
    .AddScoped<IBattleNetUserService, BattleNetUserService>()
    .AddSingleton<IBlizzardApiClient, BlizzardApiClient>();

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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Replay}/{action=List}/{id?}");

app.Run();
