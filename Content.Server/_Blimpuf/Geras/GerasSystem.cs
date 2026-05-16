using Content.Server.Actions;
using Content.Server.Polymorph.Systems;
using Content.Server.Popups;
using Content.Shared._Blimpuf.Geras;
using Content.Shared._Blimpuf.Geras.Components;
using Content.Shared.DoAfter;
using Content.Shared.Humanoid;
using Content.Shared.MagicMirror;
using Content.Shared.NPC.Systems;
using Content.Shared.Popups;
using Content.Shared.Zombies;
using Robust.Server.GameObjects;
using Robust.Shared.Player;

namespace Content.Server._Blimpuf.Geras;

/// <inheritdoc/>
public sealed partial class GerasSystem : SharedGerasSystem
{
    [Dependency] private readonly ActionsSystem _actionsSystem = default!;
    [Dependency] private readonly PolymorphSystem _polymorphSystem = default!;
    [Dependency] private readonly EntityManager _entityManager = default!;
    [Dependency] private readonly NpcFactionSystem _factionSystem = default!;
    [Dependency] private readonly SharedDoAfterSystem _doAfter = default!;
    [Dependency] private readonly UserInterfaceSystem _uiSystem = default!;
    [Dependency] private PopupSystem _popup = default!;

    /// <inheritdoc/>
    public override void Initialize()
    {
        SubscribeLocalEvent<GerasComponent, MorphIntoGeras>(OnMorphIntoGeras);
        SubscribeLocalEvent<GerasComponent, ChangeHairStyle>(OnChangeHairStyle);
        SubscribeLocalEvent<GerasComponent, MapInitEvent>(OnMapInit);
        SubscribeLocalEvent<GerasComponent, GerasAbilityDoAfterEvent>(OnDoAfter);
        SubscribeLocalEvent<GerasComponent, EntityZombifiedEvent>(OnZombification);
        SubscribeLocalEvent<GerasComponent, BoundUIClosedEvent>(OnGerasUiClosed);
    }

    private void OnMapInit(EntityUid uid, GerasComponent component, MapInitEvent args)
    {
        // try to add geras action
        _actionsSystem.AddAction(uid, ref component.GerasActionEntity, component.GerasAction);
        _actionsSystem.AddAction(uid, ref component.HairActionEntity, component.HairStyleAction);
    }

    private void OnChangeHairStyle(EntityUid uid, GerasComponent component, ChangeHairStyle args)
    {
        if (args.Handled)
            return;

        if (!HasComp<HumanoidAppearanceComponent>(args.Performer) || !TryComp<ActorComponent>(args.Performer, out var actor))
            return;

        if (actor.PlayerSession.AttachedEntity is not { } attachedEntity)
            return;

        if (HasComp<MagicMirrorComponent>(args.Performer))
        {
            _uiSystem.CloseUi(args.Performer, MagicMirrorUiKey.Key);
            RemComp<MagicMirrorComponent>(args.Performer);
        }

        var mirrorComp = EnsureComp<MagicMirrorComponent>(args.Performer);
        mirrorComp.Target = args.Performer;

        _uiSystem.OpenUi(args.Performer, MagicMirrorUiKey.Key, actor.PlayerSession);

        var openEvent = new BoundUIOpenedEvent(MagicMirrorUiKey.Key, args.Performer, attachedEntity);
        RaiseLocalEvent(attachedEntity, openEvent, true);

        args.Handled = true;
    }
    private void OnGerasUiClosed(EntityUid uid, GerasComponent component, BoundUIClosedEvent args)
    {
        if (!args.UiKey.Equals(MagicMirrorUiKey.Key))
            return;

        _uiSystem.CloseUi(uid, MagicMirrorUiKey.Key);

        if (HasComp<MagicMirrorComponent>(uid))
        {
            RemComp<MagicMirrorComponent>(uid);
        }
    }

    private void OnMorphIntoGeras(EntityUid uid, GerasComponent component, MorphIntoGeras args)
    {
        if (TryComp<PendingZombieComponent>(uid, out var pZombieComponent))
        {
            _popup.PopupEntity(Loc.GetString("geras-popup-morph-infected-failed-message-user"), uid, uid, PopupType.LargeCaution);
            return;
        }
        var @event = new GerasAbilityDoAfterEvent();
        // time it takes to activate ability: TimeSpan.FromSeconds(X) X = number of seconds
        var doAfter = new DoAfterArgs(EntityManager, uid, TimeSpan.FromSeconds(6), @event, uid)
        {
            BreakOnDamage = true,
            BreakOnMove = true
        };
        _doAfter.TryStartDoAfter(doAfter);
    }
    private void OnDoAfter(EntityUid uid, GerasComponent component, GerasAbilityDoAfterEvent args)
    {
        // Check if the event was cancelled or interrupted (e.g., moved while casting)
        if (args.Cancelled || args.Handled)
            return;

        var ent = _polymorphSystem.PolymorphEntity(uid, component.GerasPolymorphId);

        if (!ent.HasValue)
            return;

        if (!_entityManager.TryGetComponent<HumanoidAppearanceComponent>(uid, out var appearance))
            return;

        var gerasColorComponent = _entityManager.EnsureComponent<GerasColorComponent>(ent.Value);
        gerasColorComponent.Color = appearance.SkinColor;
        Dirty(ent.Value, gerasColorComponent);

        _popup.PopupEntity(Loc.GetString("geras-popup-morph-message-others", ("entity", ent.Value)), ent.Value, Filter.PvsExcept(ent.Value), true);
        _popup.PopupEntity(Loc.GetString("geras-popup-morph-message-user"), ent.Value, ent.Value);

        args.Handled = true;
    }
    private void OnZombification(EntityUid uid, GerasComponent component, EntityZombifiedEvent args)
    {
        _actionsSystem.RemoveAction(uid, component.GerasActionEntity);
        _actionsSystem.RemoveAction(uid, component.HairActionEntity);
    }
}
