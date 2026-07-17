using Content.Server.Connection.Whitelist;

namespace Content.Server._Blimpuf.Discord;

public sealed partial class BlimpufDiscordRolesCondition : WhitelistCondition
{
    [DataField]
    public List<ulong> Roles = [];
}
