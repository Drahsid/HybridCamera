using Dalamud.Game.ClientState;
using Dalamud.Game.ClientState.Keys;
using Dalamud.Game.Command;
using Dalamud.Game.Gui;
using Dalamud.Interface.Windowing;
using Dalamud.Logging;
using Dalamud.Plugin;
using FFXIVClientStructs.FFXIV.Client.Game.Control;
using HybridCamera.Attributes;
using System;

[assembly: System.Reflection.AssemblyVersion("1.1.0")]

namespace HybridCamera;

public class Plugin : IDalamudPlugin
{
    private DalamudPluginInterface PluginInterface;
    private ChatGui Chat;
    private ClientState ClientState;

    private PluginCommandManager<Plugin> CommandManager;
    private WindowSystem WindowSystem;

    public string Name => "HybridCamera";

    private MovementMode CameraMode = MovementMode.Standard;
    private ConfigWindow ConfigWnd;

    internal const string CameraManagerSig = "4C 8D 35 ?? ?? ?? ?? 85 D2";

    public Plugin(DalamudPluginInterface pluginInterface, CommandManager commandManager, ChatGui chat, ClientState clientState) {
        PluginInterface = pluginInterface;
        Chat = chat;
        ClientState = clientState;

        // Get or create a configuration object
        Globals.Config = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
        Globals.Config.Initialize(PluginInterface);

        // Initialize the UI
        WindowSystem = new WindowSystem(typeof(Plugin).AssemblyQualifiedName);
        ConfigWnd = new ConfigWindow();
        WindowSystem.AddWindow(ConfigWnd);
        PluginInterface.UiBuilder.Draw += DrawUI;
        PluginInterface.UiBuilder.OpenConfigUi += ToggleConfig;

        // Load all of our commands
        CommandManager = new PluginCommandManager<Plugin>(this, commandManager);

        PluginInterface.Create<Service>();
        Initialize();
    }

    private unsafe void Initialize() {
        Globals.CameraManager = (GameCameraManager*)Service.SigScanner.GetStaticAddressFromSig(CameraManagerSig); // g_ControlSystem_CameraManager
        PluginLog.Warning(((IntPtr)Globals.CameraManager).ToString("X"));
    }

    private unsafe void UpdateMoveStatePre()
    {
        uint mode = (uint)MovementMode.Standard;

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
