using Dalamud.Game.ClientState;
using Dalamud.Game.ClientState.Keys;
using Dalamud.Game.Command;
using Dalamud.Game.Gui;
using Dalamud.Interface.Windowing;
using Dalamud.IoC;
using Dalamud.Logging;
using Dalamud.Plugin;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using HybridCamera.Attributes;
using ImGuiNET;
using System;
using System.Collections.Generic;

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

        public string Name => "HybridCamera";

        [PluginService] public static KeyState KeyState { get; private set; }

        public Plugin(DalamudPluginInterface pi, CommandManager commands, ChatGui chat, ClientState clientState) {
            this.pluginInterface = pi;
            this.chat = chat;
            this.clientState = clientState;

            // Get or create a configuration object
            this.config = (Configuration)this.pluginInterface.GetPluginConfig();
            if (this.config == null)
            {
                this.config = this.pluginInterface.Create<Configuration>();
                this.config.legacyModeKeyList = new List<VirtualKey>();
                this.config.legacyModeKeyList.Add(VirtualKey.W);
                this.config.legacyModeKeyList.Add(VirtualKey.A);
                this.config.legacyModeKeyList.Add(VirtualKey.S);
                this.config.legacyModeKeyList.Add(VirtualKey.D);
            }

            // Initialize the UI
            this.windowSystem = new WindowSystem(typeof(Plugin).AssemblyQualifiedName);

            this.pluginInterface.UiBuilder.Draw += this.OnDraw;

            // Load all of our commands
            this.commandManager = new PluginCommandManager<Plugin>(this, commands);
        }

        public unsafe void OnDraw()
        {
            ConfigModule* cm = ConfigModule.Instance();

            if (cm != null)
            {
                bool eval = false;
                foreach (VirtualKey key in config.legacyModeKeyList)
                {
                    if (KeyState[key])
                    {
                        eval = true;
                        break;
                    }
                }

                if (eval)
                {
                    cm->SetOption(ConfigOption.MoveMode, 1); // set legacy mode
                }
                else
                {
                    cm->SetOption(ConfigOption.MoveMode, 0); // set standard mode
                }
            }

            if (this.config.showWindow)
            {
                ImGui.Begin("HybridCam"); {
                    VirtualKey key;
                    float charwidth = ImGui.CalcTextSize("FF").X;

                    ImGui.TextDisabled("Add and remove the keys for legacy mode below");

                    for (int index = 0; index < config.legacyModeKeyList.Count; index++)
                    {
                        key = config.legacyModeKeyList[index];
                        ImGui.SetNextItemWidth(charwidth * 4);
                        if (ImGui.BeginCombo("key##" + index.ToString() + "_" + key.ToString(), key.ToString()))
                        {
                            VirtualKey[] validKeys = KeyState.GetValidVirtualKeys();

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
