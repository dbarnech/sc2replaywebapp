using Microsoft.EntityFrameworkCore;
using Sc2ReplayWebApp.Data;
using Sc2ReplayWebApp.Models;
using System.Security.Claims;

namespace Sc2ReplayWebApp.Services;

public interface IBattleNetUserService
{
    Task<BattleNetUser> GetOrCreateUser(ClaimsPrincipal principal, string region = "us", CancellationToken cancellationToken = default);
}

public class BattleNetUserService : IBattleNetUserService
{
    private readonly AppDbContext _db;
    private readonly IBlizzardApiClient _blizzardApiClient;

    public BattleNetUserService(
        AppDbContext db,
        IBlizzardApiClient blizzardApiClient)
    {
        _db = db;
        _blizzardApiClient = blizzardApiClient;
    }

    public async Task<BattleNetUser> GetOrCreateUser(
        ClaimsPrincipal principal,
        string region = "us",
        CancellationToken cancellationToken = default)
    {
        var battleNetId =
            principal.FindFirst(
                ClaimTypes.NameIdentifier)?.Value;

        var battleTag =
            principal.Identity?.Name;

        if (string.IsNullOrWhiteSpace(battleNetId))
        {
            throw new InvalidOperationException(
                "Battle.net ID claim is missing.");
        }

        var user =
            await _db.BattleNetUsers
                .FirstOrDefaultAsync(
                    u => u.BattleNetId == battleNetId,
                    cancellationToken);

        // Get SC2 player from Blizzard API to ensure the user exists
        var sc2Player =
            await _blizzardApiClient.GetSc2AccountPlayerAsync(
                battleNetId,
                region,
                cancellationToken);

        if (user == null)
        {
            user = new BattleNetUser
            {
                BattleNetId = battleNetId,
                BattleTag = battleTag ?? "",
                FirstLogin = DateTime.UtcNow,
                LastLogin = DateTime.UtcNow,
                Region = region ?? "",
                Sc2PlayerName = sc2Player?.Name ?? "",
                Sc2ProfileId = sc2Player?.ProfileId ?? "",
                Sc2RegionId = sc2Player?.RegionId ?? 0,
                Sc2RealmId = sc2Player?.RealmId ?? 0
            };
            _db.BattleNetUsers.Add(user);
        }
        else
        {
            user.BattleTag = battleTag ?? user.BattleTag;
            user.LastLogin = DateTime.UtcNow;
            user.Region = region ?? user.Region;
            user.Sc2PlayerName = sc2Player?.Name ?? user.Sc2PlayerName;
            user.Sc2ProfileId = sc2Player?.ProfileId ?? user.Sc2ProfileId;
            user.Sc2RegionId = sc2Player?.RegionId ?? user.Sc2RegionId;
            user.Sc2RealmId = sc2Player?.RealmId ?? user.Sc2RealmId;
        }

        await _db.SaveChangesAsync(cancellationToken);

        return user;
    }
}