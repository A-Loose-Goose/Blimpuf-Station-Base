using Content.Server.Actions;
using Content.Server.Polymorph.Systems;
using Content.Server.Popups;
using Content.Shared._Blimpuf.Geras;
using Content.Shared._Blimpuf.Geras.Components;
using Content.Shared.DoAfter;
using Content.Shared.Humanoid;
using Robust.Shared.Player;

namespace Content.Server._Blimpuf.Geras;

/// <inheritdoc/>
public sealed class GerasSystem : SharedGerasSystem
{
    [Dependency] private readonly ActionsSystem _actionsSystem = default!;
    [Dependency] private readonly PolymorphSystem _polymorphSystem = default!;
    [Dependency] private readonly SharedDoAfterSystem _doAfter = default!;
    [Dependency] private PopupSystem _popup = default!;

    /// <inheritdoc/>
    public override void Initialize()
    {
        SubscribeLocalEvent<GerasComponent, MorphIntoGeras>(OnMorphIntoGeras);
        SubscribeLocalEvent<GerasComponent, MapInitEvent>(OnMapInit);
        SubscribeLocalEvent<GerasComponent, GerasAbilityDoAfterEvent>(OnDoAfter);
    }

    private void OnMapInit(EntityUid uid, GerasComponent component, MapInitEvent args) =>
        // try to add geras action
        _actionsSystem.AddAction(uid, ref component.GerasActionEntity, component.GerasAction);

    private void OnMorphIntoGeras(EntityUid uid, GerasComponent component, MorphIntoGeras args)
    {
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

        if (!EntityManager.TryGetComponent<HumanoidAppearanceComponent>(uid, out var appearance))
            return;

        var gerasColorComponent = EntityManager.EnsureComponent<GerasColorComponent>(ent.Value);
        gerasColorComponent.Color = appearance.SkinColor;
        Dirty(ent.Value, gerasColorComponent);

        _popup.PopupEntity(Loc.GetString("geras-popup-morph-message-others", ("entity", ent.Value)), ent.Value, Filter.PvsExcept(ent.Value), true);
        _popup.PopupEntity(Loc.GetString("geras-popup-morph-message-user"), ent.Value, ent.Value);

        args.Handled = true;
    }
}
