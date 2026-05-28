using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared._Blimpuf.Antags.Traitor.Components;

[Serializable, NetSerializable]
[DataDefinition]
public sealed partial class ResearchTargetEntry
{
    [DataField(required: true)] public string DisplayName = string.Empty;

    [DataField(required: true)] public List<EntProtoId> ValidPrototypes = new();
}
