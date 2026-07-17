using System.Threading.Tasks;
using Robust.Shared.Network;

namespace Content.Server._Blimpuf.Discord;

public interface IBlimpufDiscordRoleProvider
{
    Task<DiscordRoleSnapshot?> GetRolesAsync(NetUserId userId);
}

