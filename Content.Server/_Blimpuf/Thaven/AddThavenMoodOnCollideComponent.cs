namespace Content.Server._Blimpuf.Thaven;

/// <summary>
/// Used for projectiles that emag thavens
/// </summary>
[RegisterComponent]
public sealed partial class AddThavenMoodOnCollideComponent : Component
{
    [DataField]
    public bool Used;
}
