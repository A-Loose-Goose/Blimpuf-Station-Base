using Content.Shared.Radio;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;

namespace Content.Server._Blimpuf.CartridgeLoader.Cartridges;

[RegisterComponent, Access(typeof(SOSCartridgeSystem))]
[AutoGenerateComponentPause]
public sealed partial class SOSCartridgeComponent : Component
{
    //Path to the id container
    public const string PDAIdContainer = "PDA-id";

    [DataField]
    //Name to use if no id is found
    public LocId DefaultName = "sos-caller-defaultname";

    [ViewVariables(VVAccess.ReadOnly)]
    public string LocalizedDefaultName => Loc.GetString(DefaultName);

    [DataField]
    //Notification message
    public LocId HelpMessage = "sos-message";

    /// <summary>
    /// Message that gets played locally when the SoS button is used
    /// </summary>
    [DataField]
    public LocId NotificationMessage = "sos-notification-message";

    [ViewVariables(VVAccess.ReadOnly)]
    public string LocalizedNotificationMessage => Loc.GetString(NotificationMessage);

    /// <summary>
    /// Message that gets played locally when the SoS button is used and fails to send a message
    /// </summary>
    [DataField]
    public LocId FailureNotificationMessage = "sos-notification-message-failure";

    [ViewVariables(VVAccess.ReadOnly)]
    public string LocalizedFailureNotificationMessage => Loc.GetString(FailureNotificationMessage);

    /// <summary>
    /// Message that gets played locally when the SoS button is used and fails to send a message
    /// </summary>
    [DataField]
    public LocId FunnyNotificationMessage = "sos-notification-message-pizza";

    [ViewVariables(VVAccess.ReadOnly)]
    public string LocalizedFunnyNotificationMessage => Loc.GetString(FunnyNotificationMessage);

    [DataField]
    //Channel to notify
    public ProtoId<RadioChannelPrototype> HelpChannel = "Security";

    [DataField]
    //Timeout between calls
    public TimeSpan Cooldown = TimeSpan.FromSeconds(30);

    [DataField(customTypeSerializer: typeof(TimeOffsetSerializer)), AutoPausedField]
    //Countdown until next call is allowed
    public TimeSpan NextUse = TimeSpan.Zero;

    /// <summary>
    /// The chance the SoS call will randomly fail to send
    /// </summary>
    [DataField]
    public float FailChance = 0.005f;

    /// <summary>
    /// The chance the notification will be replaced with a funny alternative.
    /// </summary>
    [DataField]
    public float FunnyChance = 0.005f;
}
