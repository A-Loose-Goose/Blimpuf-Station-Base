namespace Content.Server._Blimpuf.Chemistry.Components;

[RegisterComponent]
public sealed partial class InjectReagentWhileWornComponent : Component
{
    /// <summary>
    /// Reagent that will be injected into the user
    /// </summary>
    [DataField]
    public string ReagentPrimary = "Omnizine";

    /// <summary>
    /// Optional Second Reagent to inject
    /// </summary>
    [DataField]
    public string? ReagentSecondary;

    /// <summary>
    /// Optional Third Reagent to inject
    /// </summary>
    [DataField]
    public string? ReagentTertiary;

    /// <summary>
    /// The amount of the Reagent to be injected
    /// </summary>
    [DataField]
    public float QuantityPrimary = 5;

    /// <summary>
    /// Amount to inject optionally if there's a second reagent defined
    /// </summary>
    [DataField]
    public float QuantitySecondary = 5;

    /// <summary>
    /// Amount to inject optionally if there's a third reagent defined
    /// </summary>
    [DataField]
    public float QuantityTertiary = 5;

    /// <summary>
    /// The time that has to pass before injection
    /// </summary>
    [DataField]
    public TimeSpan Time = TimeSpan.FromSeconds(5);
}
