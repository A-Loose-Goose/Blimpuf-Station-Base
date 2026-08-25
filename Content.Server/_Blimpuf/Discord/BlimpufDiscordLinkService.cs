using System.Security.Cryptography;
using System.Text;
using Content.Shared._Starlight.CCVar;
using Robust.Shared.Configuration;

namespace Content.Server._Blimpuf.Discord;

public sealed class BlimpufDiscordLinkService : IBlimpufDiscordLinkService
{
    private const string Scope = "identify+guilds+guilds.members.read";

    [Dependency] private IConfigurationManager _cfg = default!;
    [Dependency] private ILogManager _logManager = default!;

    private ISawmill? _sawmill;
    private bool _loggedInvalidCallback;

    private ISawmill Sawmill => _sawmill ??= _logManager.GetSawmill("blimpuf.discord.link");

    public string GetAuthUrl(string state)
    {
        var clientId = _cfg.GetCVar(StarlightCCVars.DiscordKey).Trim();
        var redirectUri = _cfg.GetCVar(StarlightCCVars.DiscordCallback).Trim();
        var secret = _cfg.GetCVar(StarlightCCVars.Secret);

        if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(redirectUri) || string.IsNullOrEmpty(secret))
            return "";

        if (!Uri.TryCreate(redirectUri, UriKind.Absolute, out var uri)
            || uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps)
        {
            if (!_loggedInvalidCallback)
            {
                Sawmill.Error($"discord.callback is not a valid absolute http(s) url: '{redirectUri}'. Discord linking will fail.");
                _loggedInvalidCallback = true;
            }

            return "";
        }

        var secretKeyBytes = Encoding.UTF8.GetBytes(secret);
        using var hmac = new HMACSHA256(secretKeyBytes);

        var dataBytes = Encoding.UTF8.GetBytes(state);
        var hashBytes = hmac.ComputeHash(dataBytes);
        var signedState = $"{state}.{BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant()}";
        var encodedState = Uri.EscapeDataString(signedState);

        return $"https://discord.com/api/oauth2/authorize?client_id={clientId}&redirect_uri={Uri.EscapeDataString(redirectUri)}&response_type=code&scope={Scope}&state={encodedState}";
    }
}
