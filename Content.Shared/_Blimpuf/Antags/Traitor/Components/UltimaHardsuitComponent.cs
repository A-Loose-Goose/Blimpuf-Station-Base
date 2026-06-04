using Content.Shared.Actions;
using Content.Shared.Ninja.Systems;
using Robust.Shared.Audio;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared._Blimpuf.Antags.Traitor.Components;

/// <summary>
/// Adds the 3 explosion actions onto the wearer of the Ultima Hardsuit
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
[Access(typeof(SharedNinjaSuitSystem))]
public sealed partial class UltimaHardsuitComponent : Component
{
    /// <summary>
    /// The action id for creating an EMP burst
    /// </summary>
    [DataField] public EntProtoId EmpAction = "ActionUltimaHardsuitEmp";

    [DataField, AutoNetworkedField] public EntityUid? EmpActionEntity;

    /// <summary>
    /// The action id for creating a Flashbang burst
    /// </summary>
    [DataField] public EntProtoId FlashbangAction = "ActionUltimaHardsuitFlashbang";

    [DataField, AutoNetworkedField] public EntityUid? FlashbangActionEntity;

    /// <summary>
    /// The action id for creating a blast explosion burst
    /// </summary>
    [DataField] public EntProtoId BlastAction = "ActionUltimaHardsuitBlast";

    [DataField, AutoNetworkedField] public EntityUid? BlastActionEntity;
}

public sealed partial class UltimaHardsuitEmpEvent : InstantActionEvent;

public sealed partial class UltimaHardsuitFlashbangEvent : InstantActionEvent;

public sealed partial class UltimaHardsuitBlastEvent : InstantActionEvent;
