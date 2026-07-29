using Content.Shared.Body.Components;
using Content.Shared.Body.Organ;
using Content.Shared._Blimpuf.Medical.Body.Components;
using Content.Shared._Starlight.Medical.Body.Prototypes;
using Content.Shared.EntityConditions;
using Content.Shared.EntityConditions.Conditions.Body;
using Content.Shared.Body.Systems;
using Robust.Shared.Prototypes;

namespace Content.Server.EntityConditions.Conditions;

/// <summary>
/// Returns true if this entity has any of the listed metabolizer types.
/// </summary>
/// <inheritdoc cref="EntityConditionSystem{T, TCondition}"/>
public sealed partial class MetabolizerTypeEntityConditionSystem : EntityConditionSystem<MetabolizerComponent, MetabolizerTypeCondition>
{
    [Dependency] private SharedBodySystem _body = default!;
    
    private static readonly ProtoId<MetabolizerTypePrototype> LegacyDwarfMetabolizerType = "Dwarf";

    protected override void Condition(Entity<MetabolizerComponent> entity, ref EntityConditionEvent<MetabolizerTypeCondition> args)
    {
        if (entity.Comp.MetabolizerTypes?.Overlaps(args.Condition.Type) == true)
        {
            args.Result = true;
            return;
        }

        if (!args.Condition.Type.Contains(LegacyDwarfMetabolizerType)
            || !TryComp<OrganComponent>(entity, out var organ)
            || organ.Body is not { } body)
            return;

        args.Result = _body.TryGetOrgansWithComponent<DipsomaniaCarrierComponent>((body, null), out _);
    }
}
