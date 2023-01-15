using Dalamud.Game.ClientState;
using Dalamud.Game.ClientState.Keys;
using Dalamud.Game.Command;
using Dalamud.Game.Gui;
using Dalamud.Interface.Windowing;
using Dalamud.IoC;
using Dalamud.Logging;
using Dalamud.Plugin;
using FFXIVClientStructs.FFXIV.Client.Game.Control;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using HybridCamera.Attributes;
using ImGuiNET;
using System;
using System.Collections.Generic;

[assembly: System.Reflection.AssemblyVersion("1.0.0.*")]

namespace HybridCamera
{
    public class Plugin : IDalamudPlugin
    {
        private readonly DalamudPluginInterface pluginInterface;
        private readonly ChatGui chat;
        private readonly ClientState clientState;

        private readonly PluginCommandManager<Plugin> commandManager;
        private readonly Configuration config;
        private readonly WindowSystem windowSystem;

        private MovementMode CameraMode = MovementMode.Standard;

        public string Name => "HybridCamera";

        public Plugin(DalamudPluginInterface pi, CommandManager commands, ChatGui chat, ClientState clientState) {
            this.pluginInterface = pi;
            this.chat = chat;
            this.clientState = clientState;

            // Get or create a configuration object
            this.config = pi.GetPluginConfig() as Configuration ?? new Configuration();
            this.config.Initialize(pi);

            KeybindHook.turnOnFrontpedal = this.config.useTurnOnFrontpedal;
            KeybindHook.turnOnBackpedal = this.config.useTurnOnBackpedal;
            KeybindHook.cameraTurnMode = this.config.useTurnOnCameraTurn;

            // Initialize the UI
            this.windowSystem = new WindowSystem(typeof(Plugin).AssemblyQualifiedName);

            this.pluginInterface.UiBuilder.Draw += this.OnDraw;

            // Load all of our commands
            this.commandManager = new PluginCommandManager<Plugin>(this, commands);

            pluginInterface.Create<Service>();
        }

        private void DrawMoveModeConditionOption(string descriptor, ref MoveModeCondition cfg)
        {
            float charwidth = ImGui.CalcTextSize("FF").X;

            ImGui.Checkbox($"Force {descriptor} to use mode: ", ref cfg.condition);
            ImGui.SameLine();
            ImGui.SetNextItemWidth(charwidth * 8);
            if (ImGui.BeginCombo($"Movement Mode###{descriptor}", cfg.mode.ToString())) {
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

        public unsafe void OnDraw()
        {
            ConfigModule* cm = ConfigModule.Instance();
            MovementMode mode = MovementMode.Standard;

            if (cm != null)
            {
                foreach (VirtualKey key in config.legacyModeKeyList)
                {
                    if (Service.KeyState[key])
                    {
                        mode = MovementMode.Legacy;
                        break;
                    }
                }

                if (config.autorunMoveMode.condition && InputManager.IsAutoRunning())
                {
                    mode = config.autorunMoveMode.mode;
                }

                if (config.cameraRotateMoveMode.condition && Service.PlayerIsRotatingCamera())
                {
                    mode = config.cameraRotateMoveMode.mode;
                }

                CameraMode = mode;

                cm->SetOption(ConfigOption.MoveMode, (int)CameraMode); // set legacy mode
            }

            if (this.config.showWindow)
            {
                ImGui.Begin("HybridCam"); {
                    VirtualKey key;
                    float charwidth = ImGui.CalcTextSize("FF").X;
                    bool changed = false;
                    ImGui.Text($"Flags are {Convert.ToString(CameraManager.Instance->Camera->CameraBase.UnkFlags, 2)}, Mode is {CameraMode}");
                    ImGui.Text($"{Convert.ToString(CameraManager.Instance->Camera->CameraBase.UnkUInt, 16)}");

                    ImGui.Separator();
                    DrawMoveModeConditionOption("auto-run", ref config.autorunMoveMode);
                    DrawMoveModeConditionOption("camera rotation", ref config.cameraRotateMoveMode);
                    if (ImGui.Checkbox("Use turning on frontpedal", ref config.useTurnOnFrontpedal))
                    {
                        KeybindHook.turnOnFrontpedal = config.useTurnOnFrontpedal;
                        changed = true;
                    }
                    if (ImGui.Checkbox("Use turning on backpedal", ref config.useTurnOnBackpedal))
                    {
                        KeybindHook.turnOnBackpedal = config.useTurnOnBackpedal;
                        changed = true;
                    }

                    ImGui.SetNextItemWidth(charwidth * 9);
                    if (ImGui.BeginCombo($"Use turning on camera rotate (Probably leave this on None!)###turncameraturn", config.useTurnOnCameraTurn.ToString()))
                    {
                        for (int index = 0; index < (int)TurnOnCameraTurn.Count; index++)
                        {
                            ImGui.SetNextItemWidth(charwidth * 8);
                            if (ImGui.Selectable(((TurnOnCameraTurn)index).ToString(), index == (int)config.useTurnOnCameraTurn))
                            {
                                config.useTurnOnCameraTurn = (TurnOnCameraTurn)index;
                                KeybindHook.cameraTurnMode = config.useTurnOnCameraTurn;
                                changed = true;
                            }
                        }
                        ImGui.EndCombo();
                    }

                    if (changed) {
                        if (config.useTurnOnFrontpedal == false && config.useTurnOnBackpedal == false && config.useTurnOnCameraTurn == TurnOnCameraTurn.None && KeybindHook.Enabled)
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

                    for (int index = 0; index < config.legacyModeKeyList.Count; index++)
                    {
                        key = config.legacyModeKeyList[index];
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
                                    config.legacyModeKeyList[index] = validKeys[qndex];
                                }
                            }
                            ImGui.EndCombo();
                        }
                        ImGui.SameLine();
                        ImGui.SetNextItemWidth(charwidth * 4);
                        if (ImGui.Button("-##dropdown_" + index.ToString() + "_delete"))
                        {
                            config.legacyModeKeyList.RemoveAt(index);
                            break;
                        }
                    }
                    ImGui.SameLine();
                    ImGui.SetNextItemWidth(charwidth * 4);
                    if (ImGui.Button("+##dropdown_add_new"))
                    {
                        config.legacyModeKeyList.Add(VirtualKey.SPACE);
                    }

                    ImGui.Separator();
                }
                ImGui.End();
            }

            if ((this.config.useTurnOnFrontpedal || this.config.useTurnOnBackpedal || config.useTurnOnCameraTurn != TurnOnCameraTurn.None) && KeybindHook.Enabled == false)
            {
                KeybindHook.EnableHook();
            }
        }

        [Command("/phcam")]
        [HelpMessage("Toggle the configuration window.")]
        public void OnPHCam(string command, string args) {
            this.config.showWindow = !this.config.showWindow;
        }

        #region IDisposable Support
        protected virtual void Dispose(bool disposing)
        {
            if (!disposing) return;

            KeybindHook.DisableHook();

            this.commandManager.Dispose();

            this.pluginInterface.SavePluginConfig(this.config);

            this.pluginInterface.UiBuilder.Draw -= this.OnDraw;
            this.windowSystem.RemoveAllWindows();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
