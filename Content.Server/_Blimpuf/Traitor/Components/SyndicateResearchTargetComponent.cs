using Robust.Shared.Audio;
using Robust.Shared.Prototypes;

namespace Content.Server._Blimpuf.Traitor.Components;

[RegisterComponent]
public sealed partial class SyndicateResearchTargetComponent : Component
{
    [DataField] public String? ResearchItem1;

    [DataField] public String? ResearchItem2;

    [DataField] public String? ResearchItem3;

    [DataField] public String? ResearchItem4;

    [DataField] public Boolean Task1Complete;

    [DataField] public Boolean Task2Complete;

    [DataField] public Boolean Task3Complete;

    [DataField] public Boolean Task4Complete;

    [DataField] public HashSet<EntProtoId> ResearchProtoIds = new();

    [DataField] public Boolean ResearchComplete;

    [DataField] public int ActiveResearchNumber;

    [DataField] public SoundSpecifier ResearchSound = new SoundPathSpecifier("/Audio/_Blimpuf/items/traitor-research.ogg");

    public EntityUid? ActiveSound;
}
