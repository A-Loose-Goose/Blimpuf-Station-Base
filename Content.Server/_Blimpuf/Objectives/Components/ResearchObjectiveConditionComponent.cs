using Robust.Shared.GameObjects;

namespace Content.Server._Blimpuf.Objectives.Components;

[RegisterComponent]
public sealed partial class ResearchObjectiveConditionComponent : Component
{
    [DataField] public int ResearchRequired;
}
