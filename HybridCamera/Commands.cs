using DrahsidLib;
using Dalamud.Game.Command;

namespace HybridCamera;

internal static class Commands
{
    public static void Initialize()
    {
        Service.CommandManager.AddHandler("/phcam", new CommandInfo(OnPHCam)
        {
            ShowInHelp = true,
            HelpMessage = "Toggle the configuration window."
        });
    }

    public static void Dispose()
    {
        Service.CommandManager.RemoveHandler("/phcam");
    }

    public static void ToggleConfig()
    {
        Windows.Config.IsOpen = !Windows.Config.IsOpen;
    }

    public static void OnPHCam(string command, string _args)
    {
        var args = _args.Split(' ');
        args = args.Length > 0 ? args : new string[] { string.Empty };

        if (args.Length > 0 && !string.IsNullOrEmpty(args[0]))
        {
            switch (args[0].ToLower())
            {
                case "on":
                    Globals.Config.Enabled = true;
                    break;
                case "off":
                    Globals.Config.Enabled = false;
                    break;
                case "config":
                    Windows.Config.IsOpen = true;
                    break;
            }
        }
        else
        {
            ToggleConfig();
        }
    }
    

}
