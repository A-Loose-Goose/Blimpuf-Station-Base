using Robust.Shared.Configuration;

namespace Content.Shared._Blimpuf.CCVar;

[CVarDefs]
public sealed partial class BlimpufCCVars
{
    public static readonly CVarDef<string> DiscordRolesApiUrl =
        CVarDef.Create("blimpuf.discord_roles.api_url", "", CVar.SERVERONLY | CVar.CONFIDENTIAL);

    public static readonly CVarDef<string> DiscordRolesApiToken =
        CVarDef.Create("blimpuf.discord_roles.api_token", "", CVar.SERVERONLY | CVar.CONFIDENTIAL);

    public static readonly CVarDef<int> DiscordRolesApiTimeout =
        CVarDef.Create("blimpuf.discord_roles.api_timeout", 5, CVar.SERVERONLY);

    public static readonly CVarDef<string> DiscordWhitelistRoles =
        CVarDef.Create("blimpuf.discord_roles.whitelist_roles", string.Empty, CVar.SERVERONLY);
}
