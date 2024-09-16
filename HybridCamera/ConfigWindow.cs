using Dalamud.Game.ClientState.Keys;
using DrahsidLib;
using ImGuiNET;
using System.Numerics;

namespace HybridCamera;

public class ConfigWindow : WindowWrapper
{
    public static string ConfigWindowName = "Hybrid Camera Config";
    private static Vector2 MinSize = new Vector2(500, 320);

    public ConfigWindow() : base(ConfigWindowName, MinSize) { }

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

    public override void Draw()
    {
        VirtualKey key;
        float charwidth = ImGui.CalcTextSize("FF").X;
        bool changed = false;

        if (!Globals.Config.shutUpConfigHelp)
        {
            ImGui.TextWrapped("Before using this plugin, I suggest making the following changes to your character config:");
            ImGui.TextWrapped("- Under Character Configuration -> Legacy Type, toggle the option \"Disable camera pivot\" on");
            ImGui.TextWrapped("- Under Character Configuration -> Camera Controls, set the option \"Standard Type Camera Auto-adjustment\" to \"Never\", and subsequently disable \"Enable Y-axis auto-adjustment\"");
            ImGui.TextWrapped("Additionally, you should try using strafe over turn movement, and vice-versa.");
            ImGui.TextWrapped("This means going into your keybinds and changing your primary movement keys from turn to strafe, and decide which feels the best to use while under the influence of this plugin.");
            if (ImGui.Button("Ok!"))
            {
                Globals.Config.shutUpConfigHelp = true;
            }
            ImGui.Separator();
        }

        ImGui.TextDisabled("General Settings");

        ImGui.Text("Use");
        ImGui.SameLine(0);
        ImGui.SetNextItemWidth(charwidth * 8);
        if (ImGui.BeginCombo("Movement Mode", Globals.Config.defaultMovementSetting.mode.ToString()))
        {
            for (int index = 0; index < (int)MovementMode.Count; index++)
            {
                ImGui.SetNextItemWidth(charwidth * 8);
                if (ImGui.Selectable(((MovementMode)index).ToString(), index == (int)Globals.Config.defaultMovementSetting.mode))
                {
                    Globals.Config.defaultMovementSetting.mode = (MovementMode)index;
                }
            }
            ImGui.EndCombo();
        }
        ImGui.SameLine(0);
        ImGui.Text("as default.");

        DrawMoveModeConditionOption("auto-run", ref Globals.Config.autorunMoveMode, "When enabled, forces the selected movement mode while auto-running.");
        DrawMoveModeConditionOption("camera rotation", ref Globals.Config.cameraRotateMoveMode, "When enabled, forces the selected movement mode while rotating the camera. This is probably redundant.");

        //ImGui.Checkbox("Controller Mode", ref Globals.Config.ControllerMode);

        if (WindowDrawHelpers.DrawCheckboxTooltip(
            "Use turning on frontpedal",
            ref Globals.Config.useTurnOnFrontpedal,
            "Tries to make the character turn instead of strafe when you are frontpedaling. This makes you move slightly faster on the horizontal axis (left and right), and slower on the vertical axis (forward).")
            )
        {
            KeybindHook.turnOnFrontpedal = Globals.Config.useTurnOnFrontpedal;
            changed = true;
        }

        if (WindowDrawHelpers.DrawCheckboxTooltip(
            "Use turning on backpedal",
            ref Globals.Config.useTurnOnBackpedal,
            "Tries to make the character turn instead of strafe when you are backpedaling. This makes you move overwhelmingly faster when strafing while backpedaling. Note that this does not work in first person.")
            )
        {
            KeybindHook.turnOnBackpedal = Globals.Config.useTurnOnBackpedal;
            changed = true;
        }

        // Noone should really need to use this, so I'm just gonna hide it behind a config variable
        if (Globals.Config.ShowExperimental)
        {
            ImGui.Separator();
            ImGui.TextDisabled("Experimental");
            ImGui.SetNextItemWidth(charwidth * 9);
            if (ImGui.BeginCombo($"Use turning on camera rotate###turncameraturn", Globals.Config.useTurnOnCameraTurn.ToString()))
            {
                for (int index = 0; index < (int)TurnOnCameraTurn.Count; index++)
                {
                    ImGui.SetNextItemWidth(charwidth * 8);
                    if (ImGui.Selectable(((TurnOnCameraTurn)index).ToString(), index == (int)Globals.Config.useTurnOnCameraTurn))
                    {
                        Globals.Config.useTurnOnCameraTurn = (TurnOnCameraTurn)index;
                        KeybindHook.cameraTurnMode = Globals.Config.useTurnOnCameraTurn;
                        changed = true;
                    }
                }
                ImGui.EndCombo();
            }
            WindowDrawHelpers.DrawTooltip("Tells the game that you are turning instead of strafing when you are turning the camera. You probably want this disabled.");
        }

        if (changed)
        {
            if (Globals.Config.useTurnOnFrontpedal == false && Globals.Config.useTurnOnBackpedal == false && Globals.Config.useTurnOnCameraTurn == TurnOnCameraTurn.None && KeybindHook.Enabled)
            {
                KeybindHook.DisableHook();
            }
            else if (KeybindHook.Enabled == false)
            {
                KeybindHook.EnableHook();
            }
        }

        ImGui.Separator();

        ImGui.TextDisabled("Add and remove the keys for legacy mode below");

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

        ImGui.Separator();
    }
}
