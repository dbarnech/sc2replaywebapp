using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Sc2ReplayWebApp.Services
{
    public interface IBlizzardApiClient
    {
        Task<string> GetAccessTokenAsync(CancellationToken cancellationToken = default);
        Task<BlizzardApiClient.Sc2ProfileMetadataResponse?> GetSc2AccountPlayerAsync(string battleNetId, string region = "us", CancellationToken cancellationToken = default);
        Task<BlizzardApiClient.Sc2ProfileMetadataResponse?> GetSc2ProfileAsync(string profileId, int regionId, int realmId, string locale = "en_US", string region = "us", CancellationToken cancellationToken = default);
    }

    public class BlizzardApiClient : IBlizzardApiClient
    {
        private readonly OAuthOptions _oauthOptions;

        private readonly HttpClient _httpClient;

        public BlizzardApiClient(HttpClient httpClient, IOptions<OAuthOptions> oauthOptions)
        {
            _httpClient = httpClient;
            _oauthOptions = oauthOptions.Value;
        }

        public async Task<string> GetAccessTokenAsync(CancellationToken cancellationToken = default)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "https://oauth.battle.net/token");
            var authHeader = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{_oauthOptions.ClientId}:{_oauthOptions.ClientSecret}"));
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authHeader);
            request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "grant_type", "client_credentials" }
            });
            var response = await _httpClient.SendAsync(request, cancellationToken);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(content, JsonSerializerOptions.Web);
            return tokenResponse?.AccessToken ??
                throw new InvalidOperationException("Failed to retrieve access token from Blizzard API.");
        }

        //TODO: Account API -> GET Player endpoint returns a collection of profiles, not a single one
        // simplification here and return first one.
        public async Task<Sc2ProfileMetadataResponse?> GetSc2AccountPlayerAsync(string battleNetId, string region = "us", CancellationToken cancellationToken = default)
        {
            //TODO: optimize this method to cache the access token and refresh it when it expires, instead of requesting a new token for every API call.
            var accessToken = await GetAccessTokenAsync(cancellationToken);

            var request = new HttpRequestMessage(HttpMethod.Get, ApiUriBuilder.GetSc2AccountPlayerUri(region, battleNetId));
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.SendAsync(request, cancellationToken);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var players = JsonSerializer
                .Deserialize<Sc2ProfileMetadataResponse[]>(content, JsonSerializerOptions.Web) ??
                throw new InvalidOperationException("Failed to deserialize StarCraft II player profile.");

            return players.Length > 0 ?
                players[0] :
                null;
        }

        public async Task<Sc2ProfileMetadataResponse?> GetSc2ProfileAsync(string profileId, int regionId, int realmId, string locale = "en_US", string region = "us", CancellationToken cancellationToken = default)
        {
            //TODO: optimize this method to cache the access token and refresh it when it expires, instead of requesting a new token for every API call.
            var accessToken = await GetAccessTokenAsync(cancellationToken);

            var request = new HttpRequestMessage(HttpMethod.Get, ApiUriBuilder.GetSc2ProfileMetadataUri(region, regionId, realmId, profileId, locale));
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.SendAsync(request, cancellationToken);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            return JsonSerializer
                .Deserialize<Sc2ProfileMetadataResponse>(content, JsonSerializerOptions.Web) ??
                throw new InvalidOperationException("Failed to deserialize StarCraft II player profile.");
        }

        public static class ApiUriBuilder
        {
            public const string BaseUriUs = "https://us.api.blizzard.com";
            public const string BaseUriEurope = "https://eu.api.blizzard.com";
            public const string BaseUriKorea = "https://kr.api.blizzard.com";
            public const string BaseUriTaiwan = "https://tw.api.blizzard.com";
            public const string BaseUriChina = "https://gateway.blizzard.com.cn";

            /// <param name="region">The region of the data to retrieve (us, eu, kr, tw, cn).</param>
            public static string GetBaseUri(string region)
            {
                return region.ToLower() switch
                {
                    "us" => BaseUriUs,
                    "eu" => BaseUriEurope,
                    "kr" => BaseUriKorea,
                    "tw" => BaseUriTaiwan,
                    "cn" => BaseUriChina,
                    _ => throw new ArgumentException($"Invalid region: {region}. Valid regions are: us, eu, kr, tw, cn.")
                };
            }

            /// <param name="region">The region of the data to retrieve (us, eu, kr, tw, cn).</param>
            public static string GetSc2AccountPlayerUri(string region, string battleNetId)
            {
                return $"{GetBaseUri(region)}/sc2/player/{battleNetId}";
            }

            /// <param name="region">The region of the data to retrieve (us, eu, kr, tw, cn).</param>
            /// <param name="regionId">The region for the profile (1=US, 2=EU, 3=KO and TW, 5=CN).</param>
            /// <param name="realmId">The region of the profile (1 or 2).</param>
            /// <param name="profileId">The profile ID.</param>
            public static string GetSc2ProfileMetadataUri(string region, int regionId, int realmId, string profileId, string locale = "en_US")
            {
                return $"{GetBaseUri(region)}/sc2/metadata/profile/{regionId}/{realmId}/{profileId}?locale={locale}";
            }

            /// <param name="region">The region of the data to retrieve (us, eu, kr, tw, cn).</param>
            /// <param name="regionId">The region for the profile (1=US, 2=EU, 3=KO and TW, 5=CN).</param>
            /// <param name="realmId">The region of the profile (1 or 2).</param>
            /// <param name="profileId">The profile ID.</param>
            public static string GetSc2ProfileProfilesUri(string region, int regionId, string realmId, string profileId, string locale = "en_US")
            {
                return $"{GetBaseUri(region)}/sc2/profile/{regionId}/{realmId}/{profileId}?locale={locale}";
            }
        }

        public class Sc2ProfileMetadataResponse
        {
            /// <summary>
            /// The name of the player
            /// </summary>
            public string Name { get; set; } = null!;
            /// <summary>
            /// The region of the player (1 or 2).
            /// </summary>
            public int RealmId { get; set; }
            /// <summary>
            /// The region or the player. (1=US, 2=EU, 3=KO and TW, 5=CN).
            /// </summary>
            public int RegionId { get; set; }
            /// <summary>
            /// The unique identifier of the player's profile. This can be used to retrieve the player's profile information from the Blizzard API.
            /// </summary>
            /// <remarks>Example: 94277</remarks>
            public string ProfileId { get; set; } = null!;
            /// <summary>
            /// The LEGACY URL of the player's profile page on the official StarCraft II website. This can be used to link to the player's profile from your application.
            /// </summary>
            /// <remarks>Example https://starcraft2.blizzard.com/profile/1/2/94277</remarks>
            public string ProfileUrl { get; set; } = null!;
            /// <summary>
            /// The URL of the player's avatar image. This is a direct link to the image file, which can be used to display the player's avatar in your application.
            /// </summary>
            /// <remarks>Example https://static.starcraft2.com/starport/bda9a860-ca36-11ec-b5ea-4bed4e205979/portraits/15-16.jpg</remarks>
            public string AvatarUrl { get; set; } = null!;
        }

        //TODO: Should use Microsoft.AspNetCore.Authentication.OAuth.OAuthTokenResponse instead
        private class TokenResponse
        {
            [JsonPropertyName("access_token")]
            public string AccessToken { get; set; } = null!;
            [JsonPropertyName("token_type")]
            public string TokenType { get; set; } = null!;
            [JsonPropertyName("expires_in")]
            public int ExpiresIn { get; set; }
            [JsonPropertyName("scope")]
            public string Scope { get; set; } = null!;
        }
    }
}
