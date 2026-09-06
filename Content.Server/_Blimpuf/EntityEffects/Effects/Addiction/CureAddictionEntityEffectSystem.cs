using System.Linq;
using Content.Server._Blimpuf.Addictions;
using Content.Shared._Blimpuf.Addictions;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.EntityEffects;
using Robust.Shared.Prototypes;

namespace Content.Server._Blimpuf.EntityEffects.Effects.Addiction;

/// <summary>
/// Removes addictions that list the specified reagent as a curing reagent.
/// </summary>
public sealed partial class CureAddictionEntityEffectSystem : EntityEffectSystem<AddictionComponent, CureAddiction>
{
    [Dependency] private readonly IPrototypeManager _prototype = default!;
    [Dependency] private readonly AddictionSystem _addiction = default!;

    protected override void Effect(Entity<AddictionComponent> entity, ref EntityEffectEvent<CureAddiction> args)
    {
        var reagent = args.Effect.Reagent;

        // Make a copy because RemoveAddiction modifies the addiction list.
        var addictions = entity.Comp.Addictions.ToList();

        foreach (var addictionId in addictions)
        {
            if (!_prototype.TryIndex(addictionId, out var addiction))
                continue;

            if (!addiction.CuringReagents.Contains(reagent))
                continue;

            _addiction.RemoveAddiction(entity, addictionId);
        }
    }
}

public sealed partial class CureAddiction : EntityEffectBase<CureAddiction>
{
    /// <summary>
    /// The reagent that is curing the addiction.
    /// </summary>
    [DataField(required: true)]
    public ProtoId<ReagentPrototype> Reagent;
}
