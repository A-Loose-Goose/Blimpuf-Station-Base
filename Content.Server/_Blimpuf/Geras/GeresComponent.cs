using Content.Shared.Polymorph;
using Robust.Shared.Prototypes;

namespace Content.Server._Blimpuf.Geras;

[RegisterComponent]
public sealed partial class GerasComponent : Component
{
    [DataField] public ProtoId<PolymorphPrototype> GerasPolymorphId = "SlimeMorphGeras";
    
    [DataField] public EntProtoId GerasAction = "ActionMorphGeras";

    [DataField] public EntityUid? GerasActionEntity;
}