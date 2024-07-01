using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using DrahsidLib;
using FFXIVClientStructs.FFXIV.Client.Game.Control;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using FFXIVClientStructs.Interop;
using ImGuiNET;
using System;
using static FFXIVClientStructs.FFXIV.Client.Game.Control.InputManager;

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
        //MovementHook.Initialize();
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
        Globals.Config.Initialize();
    }

    private void InitializeUI() {
        Windows.Initialize();
        PluginInterface.UiBuilder.Draw += DrawUI;
        PluginInterface.UiBuilder.OpenConfigUi += Commands.ToggleConfig;
        PluginInterface.UiBuilder.OpenMainUi += Commands.ToggleConfig;
    }

    private unsafe void DrawUI() {
        Movement.UpdateMoveStatePre();
        Windows.System.Draw();
        Movement.UpdateMoveStatePost();
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
