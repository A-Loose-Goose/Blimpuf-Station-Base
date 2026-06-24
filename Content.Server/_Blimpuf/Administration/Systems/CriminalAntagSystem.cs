using Content.Shared._Starlight.Railroading;
using Content.Shared._Starlight.Railroading.Events;
using Robust.Shared.Prototypes;

namespace Content.Server._Blimpuf.Administration.Systems;

public sealed partial class CriminalAntagSystem : EntitySystem
{
    private static readonly EntProtoId CriminalCard = "RRCardCriminal";

    public void MakeCriminal(EntityUid target)
    {
        if (!TryComp<RailroadableComponent>(target, out var railroadable))
            return;

        if (railroadable.ActiveCard is { } oldCard && !Deleted(oldCard.Owner))
            QueueDel(oldCard.Owner);

        var card = Spawn(CriminalCard, Transform(target).Coordinates);
        var cardComp = Comp<RailroadCardComponent>(card);
        var ruleOwner = EnsureComp<RuleOwnerComponent>(card);
        var performer = EnsureComp<RailroadCardPerformerComponent>(card);

        cardComp.Subject = target;
        performer.Performer = (target, railroadable);
        railroadable.ActiveCard = (card, cardComp, ruleOwner);

        var ev = new RailroadingCardChosenEvent((target, railroadable));
        RaiseLocalEvent(card, ref ev);
    }
}
