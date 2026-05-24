using Content.Shared._Blimpuf.Antags.Traitor.Components;
using Robust.Shared.Audio;

namespace Content.Server._Blimpuf.Traitor.Components;

[RegisterComponent]
public sealed partial class SyndicateResearchTargetComponent : Component
{
    [DataField] public ResearchTargetEntry? ResearchItem1;

    [DataField] public ResearchTargetEntry? ResearchItem2;

    [DataField] public ResearchTargetEntry? ResearchItem3;

    [DataField] public ResearchTargetEntry? ResearchItem4;

    [DataField] public Boolean Task1Complete;

    [DataField] public Boolean Task2Complete;

    [DataField] public Boolean Task3Complete;

    [DataField] public Boolean Task4Complete;

    [DataField(required: true)] public List<ResearchTargetEntry> ResearchProtoIds = new();

    [DataField] public Boolean ResearchComplete;

    [DataField] public int ActiveResearchNumber;

    [DataField] public SoundSpecifier ResearchSound = new SoundPathSpecifier("/Audio/_Blimpuf/items/traitor-research.ogg");

    [DataField (required: true)] public String ResearchUnlockId;

    [DataField (required: true)] public String ResearchAnnouncementString;

    public EntityUid? ActiveSound;
}

