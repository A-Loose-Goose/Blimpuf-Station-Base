using Content.Shared.Chemistry.Reagent;
using Robust.Shared.Prototypes;

namespace Content.Shared._Blimpuf.Addictions;

/// <summary>
/// Defines an addiction and how it behaves.
/// </summary>
[Prototype]
public sealed partial class AddictionPrototype : IPrototype
{
    [IdDataField]
    public string ID { get; private set; } = default!;

    /// <summary>
    /// The name displayed for this addiction.
    /// </summary>
    [DataField(required: true)]
    public LocId Name;

    /// <summary>
    /// The description displayed for this addiction.
    /// </summary>
    [DataField(required: true)]
    public LocId Description;

    /// <summary>
    /// Reagents that satisfy this addiction.
    /// Taking one of these will reset the withdrawal timer.
    /// </summary>
    [DataField]
    public List<ProtoId<ReagentPrototype>> SatiatingReagents = new();

    /// <summary>
    /// How long the user can go without satisfying the addiction
    /// before withdrawal occurs.
    /// </summary>
    [DataField]
    public TimeSpan SatiationTime = TimeSpan.FromMinutes(10);

    /// <summary>
    /// Possible withdrawal effects that can be randomly selected
    /// when the addiction's timer expires.
    /// </summary>
    [DataField]
    public List<ProtoId<AddictionPrototype>> Withdrawals = new();

    /// <summary>
    /// Reagents that completely cure this addiction.
    /// </summary>
    [DataField]
    public List<ProtoId<ReagentPrototype>> CuringReagents = new();
}
