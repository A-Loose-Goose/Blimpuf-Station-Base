using Robust.Shared.GameStates;

namespace Content.Shared._Blimpuf.SecureVent.Components;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class SecuredVentComponent : Component
{
    [DataField, AutoNetworkedField] public bool IsSecuredVent = true;
}