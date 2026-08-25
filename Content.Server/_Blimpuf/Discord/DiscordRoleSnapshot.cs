using System.Collections.Generic;
using Robust.Shared.Network;

namespace Content.Server._Blimpuf.Discord;

public sealed record DiscordRoleSnapshot(
    NetUserId UserId,
    ulong DiscordId,
    IReadOnlySet<ulong> Roles);
