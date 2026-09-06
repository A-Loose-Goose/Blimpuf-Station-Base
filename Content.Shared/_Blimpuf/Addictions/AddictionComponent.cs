using Content.Shared._Blimpuf.Addictions;
using Robust.Shared.Prototypes;

namespace Content.Server._Blimpuf.Addictions;

[RegisterComponent]
public sealed partial class AddictionComponent : Component
{
    /// <summary>
    /// The addictions currently affecting this entity.
    /// </summary>
    [DataField]
    public List<ProtoId<AddictionPrototype>> Addictions = new();

    /// <summary>
    /// Runtime state for each active addiction.
    /// </summary>
    [DataField]
    public Dictionary<ProtoId<AddictionPrototype>, AddictionState> States = new();
}

/// <summary>
/// Runtime state belonging to an active addiction.
/// </summary>
[DataDefinition]
public sealed partial class AddictionState
{
    /// <summary>
    /// The time at which this addiction will next cause a withdrawal.
    /// </summary>
    [DataField]
    public TimeSpan NextWithdrawal;

    /// <summary>
    /// Stores all active withdrawals on the entity.
    /// </summary>
    [DataField]
    public List<ProtoId<AddictionPrototype>> ActiveWithdrawals = new();
}
