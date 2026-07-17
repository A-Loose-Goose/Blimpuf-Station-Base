using Content.Client.Eui;
using Content.Shared._Blimpuf.Discord;
using Content.Shared.Eui;
using JetBrains.Annotations;

namespace Content.Client._Blimpuf.Discord;

[UsedImplicitly]
public sealed class BlimpufDiscordLinkEui : BaseEui
{
    private readonly BlimpufDiscordLinkWindow _window = new();

    public override void Opened()
    {
        base.Opened();
        _window.OpenCentered();
    }

    public override void Closed()
    {
        base.Closed();
        _window.Close();
    }

    public override void HandleState(EuiStateBase state)
    {
        base.HandleState(state);
        if (state is BlimpufDiscordLinkEuiState linkState)
            _window.SetUrl(linkState.Url);
    }
}
