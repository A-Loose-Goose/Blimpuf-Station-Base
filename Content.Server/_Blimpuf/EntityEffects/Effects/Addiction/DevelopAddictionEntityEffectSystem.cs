using Content.Server._Blimpuf.Addictions;
using Content.Shared._Blimpuf.Addictions;
using Content.Shared.EntityEffects;
using Content.Shared.Localizations;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Server._Blimpuf.EntityEffects.Effects.Addiction;

/// <summary>
/// Has a chance to give the entity a specific addiction.
/// </summary>
public sealed partial class DevelopAddictionEntityEffectSystem : EntityEffectSystem<AddictionComponent, Addiction>
{
    [Dependency] private readonly AddictionSystem _addiction = default!;
    [Dependency] private readonly IRobustRandom _random = default!;

    protected override void Effect(Entity<AddictionComponent> entity, ref EntityEffectEvent<Addiction> args)
    {
        if (!_random.Prob(args.Effect.Chance))
            return;

        _addiction.AddAddiction(entity, args.Effect.AddictionId);
    }
}

public sealed partial class Addiction : EntityEffectBase<Addiction>
{
    /// <summary>
    /// The addiction that will be added.
    /// </summary>
    [DataField(required: true)]
    public ProtoId<AddictionPrototype> AddictionId;

    /// <summary>
    /// The chance for this effect to add the addiction.
    /// </summary>
    [DataField]
    public float Chance = 1f;

    public override string EntityEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys, ILocalizationManager loc)
    {
        var addiction = prototype.Index(AddictionId);

        return loc.GetString("entity-effect-guidebook-develop-addiction", ("chance", Chance * 100), ("addiction", addiction.Name));
    }
}

