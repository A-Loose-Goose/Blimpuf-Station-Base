using Content.Server._Blimpuf.Chemistry.Components;
using Content.Shared.Body.Components;
using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.Inventory.Events;
using Robust.Shared.Timing;

namespace Content.Server._Blimpuf.Chemistry.EntitySystems;

public sealed class InjectReagentWhileWornSystem : EntitySystem
{
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private SharedSolutionContainerSystem _solutionSystem = default!;

    private readonly Dictionary<EntityUid, EntityUid> _wearers = new();
    private readonly Dictionary<EntityUid, TimeSpan> _nextInjection = new();

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<InjectReagentWhileWornComponent, GotEquippedEvent>(OnEquipped);
        SubscribeLocalEvent<InjectReagentWhileWornComponent, GotUnequippedEvent>(OnUnequipped);
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var query = EntityQueryEnumerator<InjectReagentWhileWornComponent>();

        while (query.MoveNext(out var uid, out var component))
        {
            // The item isn't currently being worn.
            if (!_wearers.TryGetValue(uid, out var wearer))
                continue;

            // The timer hasn't expired yet.
            if (!_nextInjection.TryGetValue(uid, out var nextInjection) ||
                _timing.CurTime < nextInjection)
            {
                continue;
            }

            // The wearer has been deleted.
            if (TerminatingOrDeleted(wearer))
            {
                _wearers.Remove(uid);
                _nextInjection.Remove(uid);
                continue;
            }

            InjectReagent(wearer, component);

            // Schedule the next injection.
            _nextInjection[uid] = _timing.CurTime + component.Time;
        }
    }

    private void OnEquipped(EntityUid uid, InjectReagentWhileWornComponent component, ref GotEquippedEvent args)
    {
        // Remember who is wearing this item.
        _wearers[uid] = args.EquipTarget;

        // Don't inject immediately. Wait for the configured interval.
        _nextInjection[uid] = _timing.CurTime + component.Time;
    }

    private void OnUnequipped(EntityUid uid, InjectReagentWhileWornComponent component, ref GotUnequippedEvent args)
    {
        _wearers.Remove(uid);
        _nextInjection.Remove(uid);
    }

    private void InjectReagent(EntityUid wearer, InjectReagentWhileWornComponent component)
    {
        if (!TryComp<BloodstreamComponent>(wearer, out var bloodstream))
            return;

        if (bloodstream.BloodSolution is not { } bloodSolution)
            return;

        _solutionSystem.TryAddReagent(bloodSolution, component.ReagentPrimary, component.QuantityPrimary);

        if (component.ReagentSecondary != null)
            _solutionSystem.TryAddReagent(bloodSolution, component.ReagentSecondary, component.QuantitySecondary);

        if (component.ReagentTertiary != null)
            _solutionSystem.TryAddReagent(bloodSolution, component.ReagentTertiary, component.QuantityTertiary);
    }
}
