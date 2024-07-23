using DotaData.OpenDota.Json;
using DotaData.Persistence.Domain;

namespace DotaData.Mapping;

/// <summary>
/// Maps API objects to the database equivalent.
/// </summary>
internal static class PlayerMapper
{
    public static Player ToDb(this OpenDotaPlayer player)
    {
        if (player.Profile is null) throw new ArgumentNullException(nameof(player), "Profile must not be null");

        return new Player
        {
            AccountId = player.Profile.AccountId ?? throw new ArgumentNullException(nameof(player), "AccountId must not be null"),
            Avatar = player.Profile.Avatar,
            AvatarFull = player.Profile.AvatarFull,
            AvatarMedium = player.Profile.AvatarMedium,
            Cheese = player.Profile.Cheese,
            IsContributor = player.Profile.IsContributor,
            IsSubscriber = player.Profile.IsSubscriber,
            LastLogin = player.Profile.LastLogin,
            LocCountryCode = player.Profile.LocCountryCode,
            FhUnavailable = player.Profile.FhUnavailable,
            Status = player.Profile.Status,
            Name = player.Profile.Name,
            PersonaName = player.Profile.PersonaName,
            Plus = player.Profile.Plus,
            ProfileUrl = player.Profile.ProfileUrl,
            SteamId = player.Profile.SteamId,
            CompetitiveRank = player.CompetitiveRank,
            LeaderboardRank = player.LeaderboardRank,
            RankTier = player.RankTier,
            SoloCompetitiveRank = player.SoloCompetitiveRank
        };
    }
}