using Content.Server.Polymorph.Components;
using Content.Server.Popups;
using Content.Server.Zombies;
using Content.Shared.Polymorph;
using Content.Shared.Popups;
using Content.Shared.Zombies;
using Content.Shared.Actions;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Systems;


namespace Content.Server._Blimpuf.Geras;

public sealed partial class GerasBallSystem : EntitySystem
{
    [Dependency] private PopupSystem _popup = default!;
    [Dependency] private readonly SharedActionsSystem _actionsSystem = default!;
    [Dependency] private readonly IEntityManager _entityManager = default!;
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<GerasBallComponent, EntityTerminatingEvent>(OnGerasBallTerminating);
        SubscribeLocalEvent<PendingZombieComponent, ComponentStartup>(OnPendingZombieStartup);
        SubscribeLocalEvent<PendingZombieComponent, ComponentShutdown>(OnPendingZombieShutdown);
        SubscribeLocalEvent<GerasBallComponent, EntityZombifiedEvent>(OnZombification);
        SubscribeLocalEvent<RevertPolymorphActionEvent>(OnManualRevertAction);

    }

    private void OnPendingZombieStartup(EntityUid uid, PendingZombieComponent component, ComponentStartup args)
    {
        if (!TryComp<PolymorphedEntityComponent>(uid, out var polymorphedComp))
            return;
        if (polymorphedComp.Action != null)
        {
            _actionsSystem.SetEnabled(polymorphedComp.Action.Value, false);
            _popup.PopupEntity(Loc.GetString("geras-popup-morph-infected-failed-message-user"), uid, uid, PopupType.LargeCaution);
        }
    }
    private void OnManualRevertAction(RevertPolymorphActionEvent args)
    {
        var morphedEntity = args.Performer;
        if (!HasComp<PendingZombieComponent>(morphedEntity))
            return;
        if (!HasComp<ZombieComponent>(morphedEntity))
            return;
        _popup.PopupEntity(Loc.GetString("geras-popup-morph-infected-failed-message-user"), morphedEntity, morphedEntity, PopupType.LargeCaution);
    }
    private void OnPendingZombieShutdown(EntityUid uid, PendingZombieComponent component, ref ComponentShutdown args)
    {
        if (TryComp<GerasBallComponent>(uid, out var ballComp))
        {
            if (Terminating(uid) || Deleted(uid) || !ballComp.IsZombified)
                return;

            if (!TryComp<PolymorphedEntityComponent>(uid, out var polymorphedComp))
                return;
            if (polymorphedComp.Action != null)
            {
                _popup.PopupEntity(Loc.GetString("geras-popup-morph-infected-cured-message-user"), uid, uid,
                    PopupType.Large);
                _actionsSystem.SetEnabled(polymorphedComp.Action.Value, true);
            }
        }
    }
    private void OnZombification(EntityUid uid, GerasBallComponent component, EntityZombifiedEvent args) => component.IsZombified = true;

    private void OnGerasBallTerminating(EntityUid uid, GerasBallComponent component, ref EntityTerminatingEvent args)
    {
        // 1. Only act if the Geras Ball form became zombified
        if (!component.IsZombified)
            return;

        // 2. Fetch the polymorph tracking data on the same entity to find the original body
        if (!TryComp<PolymorphedEntityComponent>(uid, out var polymorphedComp))
            return;

        if (polymorphedComp.Parent is not { } originalEntity)
            return;

        if (!Exists(originalEntity))
            return;

        // 3. Apply the Zombie component to the reverting original player body
        var zombieSystem = _entityManager.System<ZombieSystem>();
        zombieSystem.ZombifyEntity(originalEntity);
        var mobStateSystem = EntityManager.System<MobStateSystem>();
        // 4. Force the state change to Dead
        mobStateSystem.ChangeMobState(originalEntity, MobState.Dead);
    }
}
