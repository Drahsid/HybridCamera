using Dalamud.Data;
using Dalamud.Game.ClientState.Keys;
using Dalamud.Game.ClientState.Objects;
using Dalamud.Game.ClientState;
using Dalamud.Game.Command;
using Dalamud.Game.Gui;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game;
using Dalamud.Game.ClientState.JobGauge;
using FFXIVClientStructs.FFXIV.Client.Game.Control;
using FFXIVClientStructs.FFXIV.Client.Game;

namespace HybridCamera;

public enum MovementMode
{
    Standard = 0,
    Legacy = 1,
    Count
}

internal class Service
{
    [PluginService]
    internal static DalamudPluginInterface Interface { get; private set; } = null!;
    [PluginService]
    internal static ChatGui ChatGui { get; private set; } = null!;
    [PluginService]
    internal static ClientState ClientState { get; private set; } = null!;
    [PluginService]
    internal static CommandManager CommandManager { get; private set; } = null!;
    [PluginService]
    internal static Condition Condition { get; private set; } = null!;
    [PluginService]
    internal static DataManager DataManager { get; private set; } = null!;
    [PluginService]
    internal static Framework Framework { get; private set; } = null!;
    [PluginService]
    internal static GameGui GameGui { get; private set; } = null!;
    [PluginService]
    internal static KeyState KeyState { get; private set; } = null!;
    [PluginService]
    internal static ObjectTable ObjectTable { get; private set; } = null!;
    [PluginService]
    internal static TargetManager TargetManager { get; private set; } = null!;
    [PluginService]
    internal static JobGauges JobGauges { get; private set; } = null!;
    [PluginService]
    public static SigScanner SigScanner { get; private set; } = null!;

    internal static unsafe bool PlayerIsRotatingCamera()
    {
        CameraManager* cm = CameraManager.Instance;
        Camera* cam = null;

        if (cm == null)
        {
            return false;
        }

        cam = cm->Camera;
        if (cam == null)
        {
            return false;
        }

        return ((cam->CameraBase.UnkFlags >> 1) & 1) != 0;
    }
}
