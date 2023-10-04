using DrahsidLib;
using Dalamud.Game.Command;

namespace HybridCamera;

internal static class Commands {
    public static void Initialize() {
        Service.CommandManager.AddHandler("/phcam", new CommandInfo(OnPHCam)
        {
            ShowInHelp = true,
            HelpMessage  = "Toggle the configuration window."
        });
    }

    public static void ToggleConfig() {
        Windows.Config.IsOpen = !Windows.Config.IsOpen;
    }

    public static void OnPHCam(string command, string args) {
        Windows.Config.IsOpen = !Windows.Config.IsOpen;
    }
}
