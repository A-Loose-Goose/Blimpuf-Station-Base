namespace Content.Server._Blimpuf.Discord;

public interface IBlimpufDiscordLinkService
{
    string GetAuthUrl(string state);
}
