using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Content.Shared._Blimpuf.CCVar;
using Robust.Shared.Configuration;
using Robust.Shared.Network;

namespace Content.Server._Blimpuf.Discord;

public sealed class BlimpufDiscordRoleProvider : IBlimpufDiscordRoleProvider
{
    [Dependency] private readonly IConfigurationManager _cfg = default!;
    [Dependency] private readonly IHttpClientHolder _http = default!;
    [Dependency] private readonly ILogManager _logManager = default!;

    private ISawmill? _sawmill;

    private ISawmill Sawmill => _sawmill ??= _logManager.GetSawmill("blimpuf.discord.roles");

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        NumberHandling = JsonNumberHandling.AllowReadingFromString,
    };

    public async Task<DiscordRoleSnapshot?> GetRolesAsync(NetUserId userId)
    {
        var apiUrl = _cfg.GetCVar(BlimpufCCVars.DiscordRolesApiUrl).Trim().TrimEnd('/');
        if (string.IsNullOrEmpty(apiUrl))
            return null;

        var token = _cfg.GetCVar(BlimpufCCVars.DiscordRolesApiToken).Trim();
        var timeout = Math.Max(1, _cfg.GetCVar(BlimpufCCVars.DiscordRolesApiTimeout));

        using var request = new HttpRequestMessage(
            HttpMethod.Get,
            $"{apiUrl}/api/ss14/users/{userId.UserId:D}/roles");

        if (!string.IsNullOrEmpty(token))
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        try
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(timeout));
            using var response = await _http.Client.SendAsync(request, cts.Token);

            if (response.StatusCode == HttpStatusCode.NotFound)
                return null;

            var content = await response.Content.ReadAsStringAsync(cts.Token);

            if (!response.IsSuccessStatusCode)
            {
                Sawmill.Warning("Discord role service returned HTTP {StatusCode} for {UserId}: {Content}",
                    response.StatusCode,
                    userId,
                    content);
                return null;
            }

            var body = JsonSerializer.Deserialize<RoleSnapshotResponse>(content, JsonOptions);
            if (body == null)
            {
                Sawmill.Warning("Discord role service returned an empty or invalid role response for {UserId}", userId);
                return null;
            }

            var responseUserId = body.UserId == Guid.Empty ? userId.UserId : body.UserId;
            if (responseUserId != userId.UserId)
            {
                Sawmill.Warning("Discord role service returned roles for {ResponseUserId} while querying {UserId}",
                    responseUserId,
                    userId);
                return null;
            }

            if (body.DiscordId == 0)
            {
                Sawmill.Warning("Discord role service returned no Discord ID for {UserId}", userId);
                return null;
            }

            return new DiscordRoleSnapshot(
                userId,
                body.DiscordId,
                body.Roles == null ? [] : new HashSet<ulong>(body.Roles));
        }
        catch (OperationCanceledException)
        {
            Sawmill.Warning("Discord role service timed out for {UserId}", userId);
            return null;
        }
        catch (Exception ex)
        {
            Sawmill.Warning("Discord role service lookup failed for {UserId}: {Error}", userId, ex.Message);
            return null;
        }
    }

    private sealed class RoleSnapshotResponse
    {
        [JsonPropertyName("userId")]
        public Guid UserId { get; set; }

        [JsonPropertyName("discordId")]
        public ulong DiscordId { get; set; }

        [JsonPropertyName("roles")]
        public List<ulong>? Roles { get; set; }
    }
}
