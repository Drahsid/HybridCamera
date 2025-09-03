using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using DrahsidLib;
using Dalamud.Bindings.ImGui;
using System;

namespace HybridCamera;

public class Plugin : IDalamudPlugin {
    private IDalamudPluginInterface PluginInterface;
    private IChatGui Chat { get; init; }
    private IClientState ClientState { get; init; }
    private ICommandManager CommandManager { get; init; }

    public string Name => "HybridCamera";

    public Plugin(IDalamudPluginInterface pluginInterface, ICommandManager commandManager, IChatGui chat, IClientState clientState) {
        PluginInterface = pluginInterface;
        Chat = chat;
        ClientState = clientState;
        CommandManager = commandManager;

        DrahsidLib.DrahsidLib.Initialize(pluginInterface, DrawTooltip);

        InitializeCommands();
        InitializeConfig();
        InitializeUI();
        MovementHook.Initialize();
    }

    public static void DrawTooltip(string text) {
        if (ImGui.IsItemHovered() && Globals.Config.HideTooltips == false) {
            ImGui.SetTooltip(text);
        }
    }

    private void InitializeCommands() {
        Commands.Initialize();
    }

    private void InitializeConfig() {
        Globals.Config = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
        Globals.Config.PostInit();
    }

    private void InitializeUI() {
        Windows.Initialize();
        PluginInterface.UiBuilder.Draw += DrawUI;
        PluginInterface.UiBuilder.OpenConfigUi += Commands.ToggleConfig;
        PluginInterface.UiBuilder.OpenMainUi += Commands.ToggleConfig;
    }

    private unsafe void DrawUI()
    {
        Windows.System.Draw();
        
        if (Globals.Config.Enabled == false)
        {
            KeybindHook.Enabled = false;
        }
        KeybindHook.UpdateKeybindHook();
    }


    #region IDisposable Support
    protected virtual void Dispose(bool disposing) {
        if (!disposing) {
            return;
        }

        KeybindHook.Dispose();
        MovementHook.Dispose();

        PluginInterface.SavePluginConfig(Globals.Config);

        PluginInterface.UiBuilder.Draw -= DrawUI;
        Windows.Dispose();
        PluginInterface.UiBuilder.OpenConfigUi -= Commands.ToggleConfig;
        PluginInterface.UiBuilder.OpenMainUi -= Commands.ToggleConfig;

        Commands.Dispose();
    }

    public void Dispose() {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    #endregion
}
