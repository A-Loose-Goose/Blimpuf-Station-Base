using Content.Shared.Actions.Events;
using Content.Shared.Interaction.Events;
using Content.Shared.Polymorph;

namespace Content.Server._Blimpuf.Polymorph;

public sealed class TeleportationRelaySystem : EntitySystem
{
    public override void Initialize()
    {
        SubscribeLocalEvent<PolymorphOnUseComponent, UseInHandEvent>(OnUse);
    }

    private void OnUse(
        EntityUid uid,
        PolymorphOnUseComponent component,
        UseInHandEvent args)
    {
        if (args.Handled)
            return;

        var ev = new PolymorphActionEvent(component.ProtoId);

        RaiseLocalEvent(args.User, ev);

        args.Handled = true;
    }
}
