using Robust.Shared.Serialization;

namespace Content.Shared._Blimpuf.Addictions;

[Serializable, NetSerializable]
public sealed class AddictionBuiState : BoundUserInterfaceState
{
    public List<AddictionUiData> Addictions { get; }

    public AddictionBuiState(List<AddictionUiData> addictions)
    {
        Addictions = addictions;
    }
}

[Serializable, NetSerializable]
public sealed class AddictionUiData
{
    public string Name { get; }
    public string Description { get; }
    public List<string> Withdrawals { get; }

    public AddictionUiData(
        string name,
        string description,
        List<string> withdrawals)
    {
        Name = name;
        Description = description;
        Withdrawals = withdrawals;
    }
}
