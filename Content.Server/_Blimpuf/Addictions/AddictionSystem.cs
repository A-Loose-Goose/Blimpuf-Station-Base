using System.Linq;
using Content.Shared._Blimpuf.Addictions;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Timing;

namespace Content.Server._Blimpuf.Addictions;

public sealed class AddictionSystem : EntitySystem
{
    [Dependency] private readonly IPrototypeManager _prototype = default!;
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly IGameTiming _timing = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<AddictionComponent, MapInitEvent>(OnMapInit);
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var query = EntityQueryEnumerator<AddictionComponent>();

        while (query.MoveNext(out var uid, out var component))
        {
            foreach (var addictionId in component.Addictions)
            {
                if (!component.States.TryGetValue(addictionId, out var state))
                    continue;

                if (state.NextWithdrawal > _timing.CurTime)
                    continue;

                TriggerWithdrawal(uid, addictionId, state);
            }
        }
    }

    /// <summary>
    /// Initializes the state of all addictions on an entity.
    /// </summary>
    private void OnMapInit(EntityUid uid, AddictionComponent component, MapInitEvent args)
    {
        foreach (var addictionId in component.Addictions)
        {
            if (!_prototype.TryIndex(addictionId, out var addiction))
                continue;

            InitializeAddiction(component, addictionId, addiction);
        }
    }

    /// <summary>
    /// Adds an addiction or withdrawal to an entity.
    /// </summary>
    public void AddAddiction(EntityUid uid, ProtoId<AddictionPrototype> addictionId)
    {
        if (!TryComp<AddictionComponent>(uid, out var component))
            component = AddComp<AddictionComponent>(uid);

        if (component.Addictions.Contains(addictionId))
            return;

        if (!_prototype.HasIndex(addictionId))
            return;

        component.Addictions.Add(addictionId);

        if (_prototype.TryIndex(addictionId, out var addiction) &&
            addiction.SatiationTime > TimeSpan.Zero)
        {
            InitializeAddiction(component, addictionId, addiction);
        }
    }

    /// <summary>
    /// Removes an addiction or withdrawal from an entity.
    /// </summary>
    public void RemoveAddiction(EntityUid uid, ProtoId<AddictionPrototype> addictionId)
    {
        if (!TryComp<AddictionComponent>(uid, out var component))
            return;

        component.Addictions.Remove(addictionId);
        component.States.Remove(addictionId);
    }

    /// <summary>
    /// Checks whether an entity currently has an addiction or withdrawal.
    /// </summary>
    public bool HasAddiction(EntityUid uid, ProtoId<AddictionPrototype> addictionId)
    {
        return TryComp<AddictionComponent>(uid, out var component) &&
               component.Addictions.Contains(addictionId);
    }

    /// <summary>
    /// Resets the withdrawal timer for an addiction.
    /// </summary>
    public void SatisfyAddiction(EntityUid uid, ProtoId<AddictionPrototype> addictionId)
    {
        if (!TryComp<AddictionComponent>(uid, out var component))
            return;

        if (!_prototype.TryIndex(addictionId, out var addiction))
            return;

        if (!component.Addictions.Contains(addictionId))
            return;

        InitializeAddiction(component, addictionId, addiction);
    }

    /// <summary>
    /// Initializes or resets the timer for an addiction.
    /// </summary>
    private void InitializeAddiction(
        AddictionComponent component,
        ProtoId<AddictionPrototype> addictionId,
        AddictionPrototype addiction)
    {
        component.States[addictionId] = new AddictionState
        {
            NextWithdrawal = _timing.CurTime + addiction.SatiationTime
        };
    }

    /// <summary>
    /// Triggers a random withdrawal for an addiction.
    /// </summary>
    private void TriggerWithdrawal(
        EntityUid uid,
        ProtoId<AddictionPrototype> addictionId,
        AddictionState state)
    {
        if (!_prototype.TryIndex(addictionId, out var addiction))
            return;

        if (addiction.Withdrawals.Count == 0)
        {
            state.NextWithdrawal = TimeSpan.MaxValue;
            return;
        }

        var availableWithdrawals = addiction.Withdrawals
            .Where(withdrawal => !state.ActiveWithdrawals.Contains(withdrawal))
            .ToList();

        if (availableWithdrawals.Count == 0)
        {
            state.NextWithdrawal = TimeSpan.MaxValue;
            return;
        }

        var withdrawalId = _random.Pick(availableWithdrawals);

        state.ActiveWithdrawals.Add(withdrawalId);

        // Add the withdrawal as an active addiction so it can be checked
        // and removed through the normal addiction system.
        AddAddiction(uid, withdrawalId);

        // Start the timer for the next possible withdrawal.
        state.NextWithdrawal = _timing.CurTime + addiction.SatiationTime;
    }
}

