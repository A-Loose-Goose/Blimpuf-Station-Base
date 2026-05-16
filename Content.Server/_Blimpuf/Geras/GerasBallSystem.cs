using Content.Server.Polymorph.Components;
using Content.Server.Popups;
using Content.Shared.Polymorph;
using Content.Shared.Popups;
using Content.Shared.Zombies;
using Content.Shared.Actions;


namespace Content.Server._Blimpuf.Geras;

public sealed partial class GerasBallSystem : EntitySystem
{
    [Dependency] private PopupSystem _popup = default!;
    [Dependency] private readonly SharedActionsSystem _actionsSystem = default!;
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<PendingZombieComponent, ComponentStartup>(OnPendingZombieStartup);
        SubscribeLocalEvent<PendingZombieComponent, ComponentShutdown>(OnPendingZombieShutdown);
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
        _popup.PopupEntity(Loc.GetString("geras-popup-morph-infected-failed-message-user"), morphedEntity, morphedEntity, PopupType.LargeCaution);
    }
    private void OnPendingZombieShutdown(EntityUid uid, PendingZombieComponent component, ref ComponentShutdown args)
    {
        if (Terminating(uid) || Deleted(uid))
            return;

        if (!TryComp<PolymorphedEntityComponent>(uid, out var polymorphedComp))
            return;
        if (polymorphedComp.Action != null)
        {
            _popup.PopupEntity(Loc.GetString("geras-popup-morph-infected-cured-message-user"), uid, uid, PopupType.Large);
            _actionsSystem.SetEnabled(polymorphedComp.Action.Value, true);
        }
    }
}
