using Content.Server._Blimpuf.Discord;
using Content.Server.EUI;
using Robust.Shared.Enums;
using Robust.Shared.Player;

namespace Content.Server._NullLink.PlayerData;

public sealed partial class NullLinkPlayerManager
{
    [Dependency] private EuiManager _euiManager = default!;

    private readonly HashSet<ICommonSession> _discordPromptOpen = [];

    private void OpenDiscordPrompt(ICommonSession session, string url)
    {
        if (session.Status == SessionStatus.Disconnected || !_discordPromptOpen.Add(session))
            return;

        var eui = new BlimpufDiscordLinkEui(OnDiscordPromptClosed, url);
        _euiManager.OpenEui(eui, session);
        eui.StateDirty();
    }

    internal void OnDiscordPromptClosed(ICommonSession session)
        => _discordPromptOpen.Remove(session);
}
