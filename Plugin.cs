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
        private DalamudPluginInterface PluginInterface;
        private ChatGui Chat;
        private ClientState ClientState;

        private PluginCommandManager<Plugin> CommandManager;
        private WindowSystem WindowSystem;

        public string Name => "HybridCamera";

        private MovementMode CameraMode = MovementMode.Standard;

        public Plugin(DalamudPluginInterface pluginInterface, CommandManager commandManager, ChatGui chat, ClientState clientState) {
            PluginInterface = pluginInterface;
            Chat = chat;
            ClientState = clientState;

            // Get or create a configuration object
            Globals.Config = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
            Globals.Config.Initialize(PluginInterface);

            // Initialize the UI
            WindowSystem = new WindowSystem(typeof(Plugin).AssemblyQualifiedName);
            WindowSystem.AddWindow(new ConfigWindow());
            PluginInterface.UiBuilder.Draw += DrawUI;
            PluginInterface.UiBuilder.OpenConfigUi += ToggleConfig;

            // Load all of our commands
            CommandManager = new PluginCommandManager<Plugin>(this, commandManager);

            PluginInterface.Create<Service>();
        }

        private unsafe void UpdateMoveStatePre()
        {
            ConfigModule* cm = ConfigModule.Instance();
            MovementMode mode = MovementMode.Standard;

            if (cm != null)
            {
                foreach (VirtualKey key in Globals.Config.legacyModeKeyList)
                {
                    if (Service.KeyState[key])
                    {
                        mode = MovementMode.Legacy;
                        break;
                    }
                }

                if (Globals.Config.autorunMoveMode.condition && InputManager.IsAutoRunning())
                {
                    mode = Globals.Config.autorunMoveMode.mode;
                }

                if (Globals.Config.cameraRotateMoveMode.condition && Service.PlayerIsRotatingCamera())
                {
                    mode = Globals.Config.cameraRotateMoveMode.mode;
                }

                CameraMode = mode;

                cm->SetOption(ConfigOption.MoveMode, (int)CameraMode); // set legacy mode
            }
        }

        // for stuff which may need to be run after stuff has changed
        private void UpdateMoveStatePost()
        {
            if ((Globals.Config.useTurnOnFrontpedal || Globals.Config.useTurnOnBackpedal || Globals.Config.useTurnOnCameraTurn != TurnOnCameraTurn.None) && KeybindHook.Enabled == false)
            {
                KeybindHook.EnableHook();
            }
        }

        private unsafe void DrawUI()
        {
            UpdateMoveStatePre();
            WindowSystem.Draw();
            UpdateMoveStatePost();
        }

        private void ToggleConfig()
        {
            WindowSystem.GetWindow(ConfigWindow.ConfigWindowName).IsOpen = !WindowSystem.GetWindow(ConfigWindow.ConfigWindowName).IsOpen;
        }

        [Command("/phcam")]
        [HelpMessage("Toggle the configuration window.")]
        public void OnPHCam(string command, string args) {
            ToggleConfig();
        }

        #region IDisposable Support
        protected virtual void Dispose(bool disposing)
        {
            if (!disposing) return;

            KeybindHook.DisableHook();

            CommandManager.Dispose();

            PluginInterface.SavePluginConfig(Globals.Config);

            WindowSystem.RemoveAllWindows();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
