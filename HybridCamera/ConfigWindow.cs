using Dalamud.Bindings.ImGui;
using Dalamud.Game.ClientState.Keys;
using DrahsidLib;
using System.Numerics;
using System.Threading.Channels;

namespace HybridCamera;

public class ConfigWindow : WindowWrapper {
    public static string ConfigWindowName = "Hybrid Camera Config";
    private static Vector2 MinSize = new Vector2(500, 320);

    public ConfigWindow() : base(ConfigWindowName, MinSize) {}

    private void DrawMoveModeConditionOption(string descriptor, ref MoveModeCondition cfg, string tooltip)
    {
        float charwidth = ImGui.CalcTextSize("FF").X;

        ImGui.Checkbox($"Force {descriptor} to use mode: ", ref cfg.condition);
        WindowDrawHelpers.DrawTooltip(tooltip);
        ImGui.SameLine();
        ImGui.SetNextItemWidth(charwidth * 8);
        if (ImGui.BeginCombo($"Movement Mode###{descriptor}combo", cfg.mode.ToString()))
        {
            for (int index = 0; index < (int)MovementMode.Count; index++)
            {
                ImGui.SetNextItemWidth(charwidth * 8);
                if (ImGui.Selectable(((MovementMode)index).ToString(), index == (int)cfg.mode))
                {
                    cfg.mode = (MovementMode)index;
                }
            }
            ImGui.EndCombo();
        }
        WindowDrawHelpers.DrawTooltip(tooltip);
    }

    private void DrawOldConfig()
    {
        VirtualKey key;
        float charwidth = ImGui.CalcTextSize("FF").X;

        DrawMoveModeConditionOption("auto-run", ref Globals.Config.autorunMoveMode, "When enabled, forces the selected movement mode while auto-running.");
        DrawMoveModeConditionOption("camera rotation", ref Globals.Config.cameraRotateMoveMode, "When enabled, forces the selected movement mode while rotating the camera. This is probably redundant.");

        for (int index = 0; index < Globals.Config.legacyModeKeyList.Count; index++)
        {
            key = Globals.Config.legacyModeKeyList[index];
            ImGui.SetNextItemWidth(charwidth * 4);
            if (ImGui.BeginCombo("key##" + index.ToString() + "_" + key.ToString(), key.ToString()))
            {
                VirtualKey[] validKeys = (VirtualKey[])Service.KeyState.GetValidVirtualKeys();

                for (int qndex = 0; qndex < validKeys.Length; qndex++)
                {
                    bool selected = key == validKeys[qndex];
                    ImGui.SetNextItemWidth(charwidth * 12);
                    if (ImGui.Selectable(validKeys[qndex].ToString() + "##dropdown_" + index.ToString() + "_key_" + validKeys[qndex].ToString(), selected))
                    {
                        Globals.Config.legacyModeKeyList[index] = validKeys[qndex];
                    }
                }
                ImGui.EndCombo();
            }
            ImGui.SameLine();
            ImGui.SetNextItemWidth(charwidth * 4);
            if (ImGui.Button("-##dropdown_" + index.ToString() + "_delete"))
            {
                Globals.Config.legacyModeKeyList.RemoveAt(index);
                break;
            }
        }
        ImGui.SameLine();
        ImGui.SetNextItemWidth(charwidth * 4);
        if (ImGui.Button("+##dropdown_add_new"))
        {
            Globals.Config.legacyModeKeyList.Add(VirtualKey.SPACE);
        }
    }

    private void DrawConfig()
    {
        WindowDrawHelpers.DrawCheckboxTooltip(
            "Use legacy movement while moving",
            ref Globals.Config.useLegacyWhileMoving,
            "When disabled, uses the movement option selected in the game settings."
        );

        WindowDrawHelpers.DrawCheckboxTooltip(
            "Use legacy movement when using turn inputs",
            ref Globals.Config.useLegacyTurning,
            "When enabled, 'Turn Left/Right' movement will always behave as if you were using legacy movement. Requires 'Use legacy movement while moving' to be enabled."
        );
    }

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
            "Enabled",
            ref Globals.Config.Enabled,
            "When enabled, Hybrid Camera runs."
        );

        WindowDrawHelpers.DrawCheckboxTooltip(
            "Keybind Mode",
            ref Globals.Config.KeybindMode,
            "Enables old input detection method (2023 Hybrid Camera) which uses keyboard input rather than interpreting movement data from the game. You probably want to keep this disabled."
        );

        ImGui.Separator();

        ImGui.TextDisabled(Globals.Config.KeybindMode ? "Keybind Mode" : "Modern Mode");
        if (Globals.Config.KeybindMode)
        {
            DrawOldConfig();
        }
        else
        {
            DrawConfig();
        }

        ImGui.Separator();


        ImGui.TextDisabled("Smart Strafe");
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

        ImGui.Separator();

        ImGui.TextDisabled("QoL");
        WindowDrawHelpers.DrawCheckboxTooltip(
            "Disable LMB + RMB Mouse Auto Run",
            ref Globals.Config.disableLRMouseMove,
            "When enabled, you will not run forwards while holding left mouse + right mouse."
        );

        ImGui.Separator();
    }
}
