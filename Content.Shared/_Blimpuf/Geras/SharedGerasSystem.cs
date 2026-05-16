using Content.Shared.Actions;

namespace Content.Shared._Blimpuf.Geras;

/// <summary>
/// A Geras is the small morph of a slime. This system handles exactly that.
/// </summary>
public abstract class SharedGerasSystem : EntitySystem
{

}

public sealed partial class MorphIntoGeras : InstantActionEvent
{
}

public sealed partial class ChangeHairStyle : InstantActionEvent
{
}
