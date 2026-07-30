using Content.Shared.Chat;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Content.Shared._Starlight.Radio; // Starlight

namespace Content.Shared.Radio.Components;

/// <summary>
///     This component allows an entity to directly translate spoken text into radio messages (effectively an intrinsic
///     radio headset).
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState] // Starlight edit
public sealed partial class IntrinsicRadioTransmitterComponent : Component, ISupportsCustomChannels // Starlight edit
{
    [DataField, AutoNetworkedField] // Starlight-edit
    public HashSet<ProtoId<RadioChannelPrototype>> Channels = new() { SharedChatSystem.CommonChannel };

    /// <summary>
    ///     This is the channel that will be used when using the default/department prefix (<see cref="SharedChatSystem.DefaultChannelPrefix"/>).
    /// </summary>
    [DataField, AutoNetworkedField]
    public string? DefaultChannel; // Blimpuf | Added support for DefaultChannels to Internal Comms

    /// <summary>
    ///     To allow channels that got blacklisted using (<see cref="RadioChannelPrototype.HeadsetTransmittable"/>).
    /// </summary>
    [DataField, AutoNetworkedField]
    public bool AllowBlacklistedComms = false;

    [DataField, AutoNetworkedField] public HashSet<CustomRadioChannelData> CustomChannels { get; set; } = []; //Starlight
}
