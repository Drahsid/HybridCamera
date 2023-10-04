using Dalamud.Game.ClientState;
using Dalamud.Game.ClientState.Keys;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game.Control;
using System;

[assembly: System.Reflection.AssemblyVersion("1.2.0")]

namespace HybridCamera;

public class Plugin : IDalamudPlugin
{
    private DalamudPluginInterface PluginInterface;
    private IChatGui Chat { get; init; }
    private IClientState ClientState { get; init; }

    private ICommandManager CommandManager {  get; init; }

    private WindowSystem WindowSystem;

    public string Name => "HybridCamera";

    private MovementMode CameraMode = MovementMode.Standard;
    private ConfigWindow ConfigWnd;

    public Plugin(DalamudPluginInterface pluginInterface, ICommandManager commandManager, IChatGui chat, IClientState clientState) {
        PluginInterface = pluginInterface;
        Chat = chat;
        ClientState = clientState;
        CommandManager = commandManager;

        InitializeConfig();
        InitializeUI();
    }

    private void InitializeConfig()
    {
        Globals.Config = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
        Globals.Config.Initialize(PluginInterface);
    }

    private void InitializeUI()
    {
        WindowSystem = new WindowSystem(typeof(Plugin).AssemblyQualifiedName);
        ConfigWnd = new ConfigWindow();
        WindowSystem.AddWindow(ConfigWnd);
        PluginInterface.UiBuilder.Draw += DrawUI;
        PluginInterface.UiBuilder.OpenConfigUi += ToggleConfig;
    }

    private unsafe void UpdateMoveStatePre()
    {
        uint mode = (uint)MovementMode.Standard;

        if (Service.KeyState == null) {
            return;
        }

        foreach (VirtualKey key in Globals.Config.legacyModeKeyList)
        {
            if (Service.KeyState[key])
            {
                mode = (uint)MovementMode.Legacy;
                break;
            }
        }

        if (Globals.Config.autorunMoveMode.condition && InputManager.IsAutoRunning())
        {
            mode = (uint)Globals.Config.autorunMoveMode.mode;
        }

        if (Globals.Config.cameraRotateMoveMode.condition && Service.PlayerIsRotatingCamera())
        {
            mode = (uint)Globals.Config.cameraRotateMoveMode.mode;
        }

        CameraMode = (MovementMode)mode;
        GameConfig.UiControl.Set("MoveMode", mode);

        if (Globals.CameraManager->WorldCamera->Mode == (int)CameraControlMode.FirstPerson)
        {
            GameConfig.UiControl.Set("MoveMode", (int)MovementMode.Standard);
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
        ConfigWnd.IsOpen = !ConfigWnd.IsOpen;
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
