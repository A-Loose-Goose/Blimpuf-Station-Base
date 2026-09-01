using Robust.Shared.Audio;
using Robust.Shared.GameStates;

namespace Content.Shared._Blimpuf.Item.ItemToggle.Components;

/// <summary>
/// Handles the changes to the gun component when toggled
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class ItemToggleGunComponent : Component
{
    /// <summary>
    ///     The noise this item makes when fired with it on.
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite), DataField, AutoNetworkedField]
    public SoundSpecifier? ActivatedSoundFire;

    /// <summary>
    ///     The noise this item makes when fired with it off.
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite), DataField, AutoNetworkedField]
    public SoundSpecifier? DeactivatedSoundFire;

    /// <summary>
    ///     Fire rate of this item when activated.
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite), DataField, AutoNetworkedField]
    public float? ActivatedFirerate = null;

    /// <summary>
    ///     Fire rate of this item when deactivated.
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite), DataField, AutoNetworkedField]
    public float? DeactivatedFirerate = null;

    /// <summary>
    ///     Max Angle Recoil of this item when activated.
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite), DataField, AutoNetworkedField]
    public Angle? ActivatedMaxAngle = null;

    /// <summary>
    ///     Max Angle Recoil of this item when deactivated.
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite), DataField, AutoNetworkedField]
    public Angle? DeactivatedMaxAngle = null;

    /// <summary>
    ///     Min Angle Recoil of this item when activated.
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite), DataField, AutoNetworkedField]
    public Angle? ActivatedMinAngle = null;

    /// <summary>
    ///     Min Angle Recoil of this item when deactivated.
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite), DataField, AutoNetworkedField]
    public Angle? DeactivatedMinAngle = null;

    /// <summary>
    ///     Angle increase of Recoil of this item when activated.
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite), DataField, AutoNetworkedField]
    public Angle? ActivatedAngleIncrease = null;

    /// <summary>
    ///     Angle increase of Recoil of this item when deactivated.
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite), DataField, AutoNetworkedField]
    public Angle? DeactivatedAngleIncrease = null;

    /// <summary>
    ///     Angle Decay of Recoil of this item when activated.
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite), DataField, AutoNetworkedField]
    public Angle? ActivatedAngleDecay = null;

    /// <summary>
    ///     Angle Decay of Recoil of this item when deactivated.
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite), DataField, AutoNetworkedField]
    public Angle? DeactivatedAngleDecay = null;
}
