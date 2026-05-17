using Content.Shared.Actions;
using Content.Shared.DoAfter;
using Robust.Shared.Map;
using Robust.Shared.Serialization;

namespace Content.Shared._Blimpuf.Smile;

/// <summary>
/// A Geras is the small morph of a slime. This system handles exactly that.
/// </summary>
public abstract class SharedReagentSuckerSystem : EntitySystem
{
}

public sealed partial class SuckUpLiquid : InstantActionEvent
{
}
public sealed partial class EmptyLiquid : InstantActionEvent
{
}
public sealed partial class SprayLiquid : InstantActionEvent
{
}
[Serializable, NetSerializable]
public sealed partial class SuckUpLiquidDoAfterEvent : SimpleDoAfterEvent
{
}
[Serializable, NetSerializable]
public sealed partial class EmptyLiquidDoAfterEvent : SimpleDoAfterEvent
{
}
