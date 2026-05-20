using Robust.Shared.Audio;

namespace Content.Server._Blimpuf.PlastitaniumDoor
{
    [RegisterComponent]
    public sealed partial class UntamperableComponent : Component
    {
        [DataField("Disabled")] public bool AccessChangeDisabled = false;

        [DataField("denyChangeSound")] public SoundSpecifier DenyChangeSound = new SoundPathSpecifier("/Audio/Machines/airlock_deny.ogg");
    }
}
