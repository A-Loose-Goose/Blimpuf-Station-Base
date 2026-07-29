using Content.Shared.Body.Components;
using Content.Shared.EntityConditions;
using Content.Shared.EntityConditions.Conditions.Body;

namespace Content.Server.EntityConditions.Conditions;

/// <summary>
/// Returns true if this entity has any of the listed metabolizer types.
/// </summary>
/// <inheritdoc cref="EntityConditionSystem{T, TCondition}"/>
public sealed partial class MetabolizerTypeEntityConditionSystem : EntityConditionSystem<MetabolizerComponent, MetabolizerTypeCondition>
{
    protected override void Condition(Entity<MetabolizerComponent> entity, ref EntityConditionEvent<MetabolizerTypeCondition> args)
    {
        args.Result = entity.Comp.MetabolizerTypes?.Overlaps(args.Condition.Type) == true;
    }
}
