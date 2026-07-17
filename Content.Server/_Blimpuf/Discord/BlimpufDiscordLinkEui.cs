using Content.Server.EUI;
using Content.Shared._Blimpuf.Discord;
using Robust.Shared.Player;

namespace Content.Server._Blimpuf.Discord;

public sealed class BlimpufDiscordLinkEui(Action<ICommonSession> onClosed, string url) : BaseEui
{
    public override BlimpufDiscordLinkEuiState GetNewState() => new() { Url = url };

    public override void Closed()
    {
        base.Closed();
        onClosed(Player);
    }
}
