using Microsoft.EntityFrameworkCore;
using Sc2ReplayWebApp.Models;

namespace Sc2ReplayWebApp.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(
        DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<ReplaySummary> Replays => Set<ReplaySummary>();

    public DbSet<PlayerSummary> Players => Set<PlayerSummary>();
}