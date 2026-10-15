using System.Linq;
using Content.Server.Silicons.Laws;
using Content.Shared._Blimpuf.Silicons.Laws;
using Content.Shared._Blimpuf.Silicons.Laws.Components;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.IdentityManagement;
using Content.Shared.Popups;
using Content.Shared.Silicons.Laws.Components;

namespace Content.Server._Blimpuf.Silicons.Laws;

public sealed class SiliconLawConfiguratorServerSystem : EntitySystem
{
    [Dependency] private SiliconLawSystem _siliconLaw = default!;
    [Dependency] private SharedPopupSystem _popup = default!;
    [Dependency] private ItemSlotsSystem _itemSlots = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<SiliconLawConfiguratorComponent, LawConfiguratorDoAfterEvent>(OnDoAfter);
    }

    private void OnDoAfter(Entity<SiliconLawConfiguratorComponent> ent, ref LawConfiguratorDoAfterEvent args)
    {
        if (args.Cancelled || args.Handled || args.Args.Target is not { } target)
            return;

        var user = args.Args.User;

        if (_itemSlots.GetItemOrNull(ent, "circuit_holder") is not { } board || !TryComp<SiliconLawProviderComponent>(board, out var provider))
            return;

        var lawset = provider.Lawset ?? _siliconLaw.GetLawset(provider.Laws);
        var laws = lawset.Laws.Select(l => l.ShallowClone()).ToList();

        _siliconLaw.SetLaws(laws, target, provider.LawUploadSound);

        _popup.PopupEntity(Loc.GetString("laws-configurator-success", ("user", Identity.Entity(args.User, EntityManager)), ("target", Identity.Entity(target, EntityManager))), target, PopupType.Medium);
        args.Handled = true;
    }
}
