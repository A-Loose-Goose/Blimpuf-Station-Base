using Content.Shared.Eui;
using Robust.Shared.Serialization;

namespace Content.Shared._Blimpuf.Discord;

[NetSerializable, Serializable]
public sealed class BlimpufDiscordLinkEuiState : EuiStateBase
{
    public string Url { get; set; } = "";
}
