using Dalamud.Configuration;
using Dalamud.Game.ClientState.Keys;
using DrahsidLib;
using System.Collections.Generic;

namespace HybridCamera;

public enum TurnOnCameraTurn {
    None = 0,
    Turning,
    WithoutTurning,
    Count
}

public class MoveModeCondition {
    public MovementMode mode = MovementMode.Standard;
    public bool condition = false;
}

public class Configuration : IPluginConfiguration {
    int IPluginConfiguration.Version { get; set; }

    #region Saved configuration values
    public MoveModeCondition autorunMoveMode;
    public MoveModeCondition cameraRotateMoveMode;
    public bool useTurnOnBackpedal;
    public bool useTurnOnFrontpedal;
    public TurnOnCameraTurn useTurnOnCameraTurn;
    public List<VirtualKey> legacyModeKeyList;
    public bool shutUpConfigHelp;
    public bool ShowExperimental;
    public bool HideTooltips;
    #endregion

    public Configuration() {
        autorunMoveMode = new MoveModeCondition();
        cameraRotateMoveMode = new MoveModeCondition();
        useTurnOnBackpedal = true;
        useTurnOnFrontpedal = true;
        useTurnOnCameraTurn = TurnOnCameraTurn.None;
        shutUpConfigHelp = false;
        ShowExperimental = false;
        HideTooltips = false;
        legacyModeKeyList = new List<VirtualKey>();
    }

    public void Initialize() {
        KeybindHook.turnOnFrontpedal = useTurnOnFrontpedal;
        KeybindHook.turnOnBackpedal = useTurnOnBackpedal;
        KeybindHook.cameraTurnMode = useTurnOnCameraTurn;
        if (legacyModeKeyList == null)
        {
            legacyModeKeyList = new List<VirtualKey>
            {
                VirtualKey.W,
                VirtualKey.A,
                VirtualKey.S,
                VirtualKey.D
            };
        }
    }

    public void Save() {
        Service.Interface.SavePluginConfig(this);
    }
}
