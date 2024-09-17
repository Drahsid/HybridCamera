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
            "Use turning on frontpedal (Forward smart strafe)",
            ref Globals.Config.useTurnOnFrontpedal,
            "Tries to make the character turn instead of strafe when you are frontpedaling. This slightly changes movement properties."
        );

        WindowDrawHelpers.DrawCheckboxTooltip(
            "Use turning on backpedal (Backward smart strafe)",
            ref Globals.Config.useTurnOnBackpedal,
            "Tries to make the character turn instead of strafe when you are backpedaling. This makes you move overwhelmingly faster when strafing while backpedaling. Only works when your movement mode is legacy."
        );

        WindowDrawHelpers.DrawCheckboxTooltip(
            "Use legacy movement while moving",
            ref Globals.Config.useLegacyWhileMoving,
            "When disabled, uses the movement option selected in the game settings."
        );

        WindowDrawHelpers.DrawCheckboxTooltip(
            "Fullspeed backpedal",
            ref Globals.Config.fullspeedBackpedal,
            "Makes the backpedaling motion full speed, instead of being 60% slower. This also allows you to backpedal at full speed in first person."
        );

        ImGui.Separator();
    }
}
