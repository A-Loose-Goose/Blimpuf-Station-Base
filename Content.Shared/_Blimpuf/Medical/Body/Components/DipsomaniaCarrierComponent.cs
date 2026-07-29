using Robust.Shared.GameStates;

namespace Content.Shared._Blimpuf.Medical.Body.Components;

/// <summary>
/// Marks the organ currently carrying Dipsomania for a body.
/// Replacing this organ removes the trait's effects.
/// </summary>
[RegisterComponent]
[NetworkedComponent]
public sealed partial class DipsomaniaCarrierComponent : Component;
