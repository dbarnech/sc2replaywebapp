using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Json;

namespace Sc2ReplayWebApp.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public const string BattleNetAuthenticationScheme = "BattleNet";
        public const string BattleNetOAuthConfigSection = "BattleNet";

        public static AuthenticationBuilder AddBattleNetAuthentication(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            return services
                .Configure<OAuthOptions>(configuration.GetRequiredSection(BattleNetOAuthConfigSection))
                .AddAuthentication(options =>
                {
                    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;

                    options.DefaultChallengeScheme = BattleNetAuthenticationScheme;
                })
                .AddCookie()
                .AddOAuth(BattleNetAuthenticationScheme, options =>
                {
                    var battleNetOptions = configuration.GetRequiredSection(BattleNetOAuthConfigSection);
                    var oauthOptions = battleNetOptions.Get<OAuthOptions>() ?? throw new InvalidOperationException("Failed to bind BattleNet OAuth options.");
                    oauthOptions.Validate();

                    options.ClientId = oauthOptions.ClientId;
                    options.ClientSecret = oauthOptions.ClientSecret;
                    options.CallbackPath = oauthOptions.CallbackPath;
                    options.AuthorizationEndpoint = oauthOptions.AuthorizationEndpoint;
                    options.TokenEndpoint = oauthOptions.TokenEndpoint;
                    ArgumentException.ThrowIfNullOrEmpty(oauthOptions.UserInformationEndpoint);
                    options.UserInformationEndpoint = oauthOptions.UserInformationEndpoint;
                    options.SaveTokens = true;
                    options.CorrelationCookie.Expiration = TimeSpan.FromMinutes(15);

                    options.Scope.Add("openid");
                    options.Scope.Add("sc2.profile");

                    options.ClaimActions.MapJsonKey(
                        ClaimTypes.NameIdentifier,
                        "sub");

                    options.ClaimActions.MapJsonKey(
                        ClaimTypes.Name,
                        "battletag");

                    options.ClaimActions.MapJsonKey(
                        "battle_net_id",
                        "id");

                    options.Events.OnCreatingTicket =
                        async context =>
                        {
                            using var request =
                                new HttpRequestMessage(
                                    HttpMethod.Get,
                                    context.Options.UserInformationEndpoint);

                            request.Headers.Authorization =
                                new System.Net.Http.Headers.AuthenticationHeaderValue(
                                    "Bearer",
                                    context.AccessToken);

                            var response =
                                await context.Backchannel.SendAsync(
                                    request);

                            var json =
                                await response.Content
                                    .ReadAsStringAsync();

                            using var document =
                                JsonDocument.Parse(json);

                            context.RunClaimActions(
                                document.RootElement);
                        };
                });
        }
    }
}
