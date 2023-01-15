using Dalamud.Game.ClientState.Keys;
using Dalamud.Interface.Windowing;
using FFXIVClientStructs.FFXIV.Client.Game.Control;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace HybridCamera
{
    public class ConfigWindow : Window, IDisposable
    {
        public static string ConfigWindowName = "Hybrid Camera Config";

        public ConfigWindow() : base(ConfigWindowName) {}

        private void DrawMoveModeConditionOption(string descriptor, ref MoveModeCondition cfg)
        {
            float charwidth = ImGui.CalcTextSize("FF").X;

            ImGui.Checkbox($"Force {descriptor} to use mode: ", ref cfg.condition);
            ImGui.SameLine();
            ImGui.SetNextItemWidth(charwidth * 8);
            if (ImGui.BeginCombo($"Movement Mode###{descriptor}", cfg.mode.ToString()))
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
        }

        public override void Draw()
        {
            VirtualKey key;
            float charwidth = ImGui.CalcTextSize("FF").X;
            bool changed = false;
            //ImGui.Text($"Flags are {Convert.ToString(CameraManager.Instance->Camera->CameraBase.UnkFlags, 2)}, Mode is {CameraMode}");
            //ImGui.Text($"{Convert.ToString(CameraManager.Instance->Camera->CameraBase.UnkUInt, 16)}");

            ImGui.Separator();
            DrawMoveModeConditionOption("auto-run", ref Globals.Config.autorunMoveMode);
            DrawMoveModeConditionOption("camera rotation", ref Globals.Config.cameraRotateMoveMode);
            if (ImGui.Checkbox("Use turning on frontpedal", ref Globals.Config.useTurnOnFrontpedal))
            {
                KeybindHook.turnOnFrontpedal = Globals.Config.useTurnOnFrontpedal;
                changed = true;
            }
            if (ImGui.Checkbox("Use turning on backpedal", ref Globals.Config.useTurnOnBackpedal))
            {
                KeybindHook.turnOnBackpedal = Globals.Config.useTurnOnBackpedal;
                changed = true;
            }

            ImGui.SetNextItemWidth(charwidth * 9);
            if (ImGui.BeginCombo($"Use turning on camera rotate (Probably leave this on None!)###turncameraturn", Globals.Config.useTurnOnCameraTurn.ToString()))
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
                    VirtualKey[] validKeys = Service.KeyState.GetValidVirtualKeys();

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

        public void Dispose() { }
    }
}
