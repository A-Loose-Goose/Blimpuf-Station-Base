using Content.Shared.Roles;
using Robust.Shared.Prototypes;

namespace Content.Shared._Blimpuf.Roles;

/// <summary>
/// Discord roles that bypass playtime requirements for selected jobs or all jobs.
/// </summary>
[Prototype("discordJobTimeOverride")]
public sealed partial class DiscordJobTimeOverridePrototype : IPrototype
{
    [IdDataField]
    public string ID { get; private set; } = default!;

    [DataField(required: true)]
    public ulong[] Roles = [];

    [DataField]
    public bool AllJobs;

    [DataField]
    public HashSet<ProtoId<JobPrototype>> Jobs = [];
}
