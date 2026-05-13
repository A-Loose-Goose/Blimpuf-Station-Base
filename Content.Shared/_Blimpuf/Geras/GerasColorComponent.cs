using Robust.Shared.GameStates;

namespace Content.Shared._Blimpuf.Geras;
    
[RegisterComponent, NetworkedComponent]
[AutoGenerateComponentState(true)]
public sealed partial class GerasColorComponent : Component
{
    [DataField("color"), AutoNetworkedField]
    public Color Color;
}