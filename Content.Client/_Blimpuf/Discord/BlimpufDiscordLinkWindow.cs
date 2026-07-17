using System.Numerics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;

namespace Content.Client._Blimpuf.Discord;

public sealed class BlimpufDiscordLinkWindow : DefaultWindow
{
    private readonly IUriOpener _uriOpener;
    private string _url = "";

    public BlimpufDiscordLinkWindow()
    {
        _uriOpener = IoCManager.Resolve<IUriOpener>();

        Title = Loc.GetString("blimpuf-discord-link-title");
        MinSize = new Vector2(440, 0);

        var box = new BoxContainer
        {
            Orientation = BoxContainer.LayoutOrientation.Vertical,
            Margin = new Thickness(12),
        };

        var text = new RichTextLabel { HorizontalExpand = true };
        text.SetMessage(Loc.GetString("blimpuf-discord-link-text"));
        box.AddChild(text);

        var linkButton = new Button
        {
            Text = Loc.GetString("blimpuf-discord-link-button"),
            HorizontalAlignment = Control.HAlignment.Center,
            Margin = new Thickness(0, 12, 0, 0),
        };
        linkButton.OnPressed += _ =>
        {
            if (!string.IsNullOrEmpty(_url))
                _uriOpener.OpenUri(_url);
            Close();
        };
        box.AddChild(linkButton);

        Contents.AddChild(box);
    }

    public void SetUrl(string url) => _url = url;
}
