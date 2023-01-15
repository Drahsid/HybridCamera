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

            // Load all of our commands
            CommandManager = new PluginCommandManager<Plugin>(this, commandManager);

            PluginInterface.Create<Service>();
        }  

        [Command("/phcam")]
        [HelpMessage("Toggle the configuration window.")]
        public void OnPHCam(string command, string args) {
            Globals.Config.showWindow = !Globals.Config.showWindow;
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
