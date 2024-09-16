using DrahsidLib;
using ImGuiNET;
using System.Numerics;

namespace HybridCamera;

public class ConfigWindow : WindowWrapper {
    public static string ConfigWindowName = "Hybrid Camera Config";
    private static Vector2 MinSize = new Vector2(500, 320);

    public ConfigWindow() : base(ConfigWindowName, MinSize) {}

    public override void Draw() {
        if (!Globals.Config.shutUpConfigHelp) {
            ImGui.TextWrapped("Before using this plugin, I suggest making the following changes to your character config:");
            ImGui.TextWrapped("- Under Character Configuration -> Legacy Type, toggle the option \"Disable camera pivot\" on");
            ImGui.TextWrapped("- Under Character Configuration -> Camera Controls, set the option \"Standard Type Camera Auto-adjustment\" to \"Never\", and subsequently disable \"Enable Y-axis auto-adjustment\"");
            ImGui.TextWrapped("Additionally, you should try using strafe over turn movement, and vice-versa.");
            ImGui.TextWrapped("This means going into your keybinds and changing your primary movement keys from turn to strafe, and decide which feels the best to use while under the influence of this plugin.");
            if (ImGui.Button("Ok!")) {
                Globals.Config.shutUpConfigHelp = true;
            }
            ImGui.Separator();
        }

        ImGui.TextDisabled("General Settings");

        WindowDrawHelpers.DrawCheckboxTooltip(
            "Use turning on frontpedal",
            ref Globals.Config.useTurnOnFrontpedal,
            "Tries to make the character turn instead of strafe when you are frontpedaling. This makes you move slightly faster on the horizontal axis (left and right), and slower on the vertical axis (forward)."
        );

        WindowDrawHelpers.DrawCheckboxTooltip(
            "Use turning on backpedal",
            ref Globals.Config.useTurnOnBackpedal,
            "Tries to make the character turn instead of strafe when you are backpedaling. This makes you move overwhelmingly faster when strafing while backpedaling. Note that this does not work in first person."
        );

        ImGui.Separator();
    }
}
