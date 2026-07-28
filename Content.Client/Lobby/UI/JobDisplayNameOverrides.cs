using Content.Shared.Roles;

namespace Content.Client.Lobby.UI;

public static class JobDisplayNameOverrides
{
    public static string GetLobbyDisplayName(JobPrototype job)
    {
        return job.ID switch
        {
            "NanoTrasenRepresentative" => "CC Representative",
            "BlueShield" => "CCE Officer",
            _ => job.LocalizedName
        };
    }
}
