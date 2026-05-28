namespace Content.Server._Blimpuf.Polymorph;

[RegisterComponent]
public sealed partial class PolymorphOnUseComponent : Component
{
    [DataField(required: true)]
    public string ProtoId;
}
