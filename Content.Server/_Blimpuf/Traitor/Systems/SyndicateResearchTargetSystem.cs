using System.Linq;
using Content.Server._Blimpuf.Traitor.Components;
using Content.Server.Mind;
using Content.Server.Popups;
using Content.Server.SyndicateResearch;
using Content.Server.Traitor.Components;
using Content.Shared._Blimpuf.Antags.Traitor.Components;
using Content.Shared._Starlight.Antags.Traitor;
using Content.Shared.DoAfter;
using Content.Shared.Examine;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Content.Shared.Roles.Components;
using Robust.Server.Audio;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Serialization;

namespace Content.Server._Blimpuf.Traitor.Systems;

public sealed partial class SyndicateResearchTargetSystem : EntitySystem
{
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly IPrototypeManager _prototype = default!;
    [Dependency] private readonly SharedDoAfterSystem _doAfter = default!;
    [Dependency] private readonly AudioSystem _audio = default!;
    [Dependency] private readonly PopupSystem _popup = default!;
    [Dependency] private readonly SyndicateResearchSystem _completedResearch = default!;

    public override void Initialize()
    {
        SubscribeLocalEvent<SyndicateResearchTargetComponent, ExaminedEvent>(OnExamined);
        SubscribeLocalEvent<SyndicateResearchTargetComponent, ComponentStartup>(OnStartup);
        SubscribeLocalEvent<SyndicateResearchTargetComponent, InteractUsingEvent>(OnInteractUsing);
        SubscribeLocalEvent<SyndicateResearchTargetComponent, SyndicateResearchDoAfterEvent>(OnDoAfter);
    }

    private void OnStartup(EntityUid uid, SyndicateResearchTargetComponent component, ComponentStartup args)
    {
        if (component.ResearchProtoIds.Count < 4)
            return;

        var list = component.ResearchProtoIds.ToList();

        _random.Shuffle(list);

        var selected = list.Take(4).ToList();

        component.ResearchItem1 = selected[0];
        component.ResearchItem2 = selected[1];
        component.ResearchItem3 = selected[2];
        component.ResearchItem4 = selected[3];
    }

    private void OnExamined(EntityUid uid, SyndicateResearchTargetComponent component, ExaminedEvent args)
    {
        if (!HasComp<TraitorComponent>(args.Examiner))
            return;

        if (component.ResearchComplete)
        {
            args.PushMarkup(Loc.GetString("traitor-research-complete"));
            return;
        }

        var message = Loc.GetString("traitor-research-incomplete") + "\n";

        if (component.ResearchItem1 != null && !component.Task1Complete)
        {
            var proto1 = _prototype.Index(component.ResearchItem1);

            message += Loc.GetString("traitor-research-task-1-requirements", ("item1", proto1.Name)) + "\n";
        }
        else if (component.ResearchItem1 != null && component.Task1Complete)
        {
            message += Loc.GetString("traitor-research-task-1-complete") + "\n";
        }

        if (component.ResearchItem2 != null && !component.Task2Complete)
        {
            var proto2 = _prototype.Index(component.ResearchItem2);

            message += Loc.GetString("traitor-research-task-2-requirements", ("item2", proto2.Name)) + "\n";
        }
        else if (component.ResearchItem2 != null && component.Task2Complete)
        {
            message += Loc.GetString("traitor-research-task-2-complete") + "\n";
        }

        if (component.ResearchItem3 != null && !component.Task3Complete)
        {
            var proto3 = _prototype.Index(component.ResearchItem3);

            message += Loc.GetString("traitor-research-task-3-requirements", ("item3", proto3.Name)) + "\n";
        }
        else if (component.ResearchItem3 != null && component.Task3Complete)
        {
            message += Loc.GetString("traitor-research-task-3-complete") + "\n";
        }

        if (component.ResearchItem4 != null && !component.Task4Complete)
        {
            var proto4 = _prototype.Index(component.ResearchItem4);

            message += Loc.GetString("traitor-research-task-4-requirements", ("item4", proto4.Name));
        }
        else if (component.ResearchItem4 != null && component.Task4Complete)
        {
            message += Loc.GetString("traitor-research-task-4-complete");
        }
        args.PushMarkup(message);
    }

    private void OnInteractUsing(EntityUid uid, SyndicateResearchTargetComponent component, InteractUsingEvent args)
    {
        if (!TryComp<MetaDataComponent>(args.Used, out var meta))
            return;

        var proto = meta.EntityPrototype?.ID;

        if (proto == component.ResearchItem1 && !component.Task1Complete)
            component.ActiveResearchNumber = 1;
        else if (proto == component.ResearchItem2 && !component.Task2Complete)
            component.ActiveResearchNumber = 2;
        else if (proto == component.ResearchItem3 && !component.Task3Complete)
            component.ActiveResearchNumber = 3;
        else if (proto == component.ResearchItem4 && !component.Task4Complete)
            component.ActiveResearchNumber = 4;
        else
            return;

        component.ActiveSound = _audio.PlayPvs(component.ResearchSound, uid)?.Entity;

        var doAfterEvent = new SyndicateResearchDoAfterEvent
        {
            UserName = Name(args.User),
            ResearchingName = Name(uid),
        };

        var doAfter = new DoAfterArgs(EntityManager, args.User, 30f, doAfterEvent, uid)
        {
            BreakOnMove = true,
            BreakOnHandChange = true,
            BreakOnDropItem = true,
            BreakOnDamage = true,
            NeedHand = true,
            BreakOnWeightlessMove = true,
        };

        var userId = args.User;
        var researchingId = uid;
        var usedId = args.Used;

        if (!TryComp<MetaDataComponent>(userId, out var userMeta))
            return;

        if (!TryComp<MetaDataComponent>(researchingId, out var researchingMeta))
            return;

        if (!TryComp<MetaDataComponent>(usedId, out var usedMeta))
            return;

        _doAfter.TryStartDoAfter(doAfter);
        var popupOthers = Loc.GetString("traitor-research-begin", ("user", userMeta.EntityName), ("researching", researchingMeta.EntityName), ("used", usedMeta.EntityName));
        _popup.PopupEntity(popupOthers, uid, PopupType.LargeCaution);
    }

    private void OnDoAfter(EntityUid uid, SyndicateResearchTargetComponent component, SyndicateResearchDoAfterEvent args)
    {
        if (component.ActiveSound != null) _audio.Stop(component.ActiveSound.Value);

        if (args.Cancelled)
            return;

        if (component.ActiveResearchNumber == 1)
            component.Task1Complete = true;
        else if (component.ActiveResearchNumber == 2)
            component.Task2Complete = true;
        else if (component.ActiveResearchNumber == 3)
            component.Task3Complete = true;
        else if (component.ActiveResearchNumber == 4)
            component.Task4Complete = true;

        var popupOthers = Loc.GetString("traitor-research-finish", ("user", args.UserName), ("researching", args.ResearchingName));
        _popup.PopupEntity(popupOthers, uid, PopupType.LargeCaution);

        if (component.Task1Complete && component.Task2Complete && component.Task3Complete && component.Task4Complete)
        {
            component.ResearchComplete = true;

            if (component.ResearchUnlockId == null)
                return;

            _completedResearch.Research(component.ResearchUnlockId);
        }

    }
}
