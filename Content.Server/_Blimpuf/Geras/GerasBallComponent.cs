namespace Content.Server._Blimpuf.Geras;

[RegisterComponent]
public sealed partial class GerasBallComponent : Component
{
    [DataField] public bool IsZombified = false;
}
