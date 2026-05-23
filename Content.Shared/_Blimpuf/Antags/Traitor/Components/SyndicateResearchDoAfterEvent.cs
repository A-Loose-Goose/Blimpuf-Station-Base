using Content.Shared.DoAfter;
using Robust.Shared.Serialization;

namespace Content.Shared._Blimpuf.Antags.Traitor.Components;

[Serializable, NetSerializable]
public sealed partial class SyndicateResearchDoAfterEvent : SimpleDoAfterEvent
{
    public String UserName = string.Empty;
    public String ResearchingName = string.Empty;
}
