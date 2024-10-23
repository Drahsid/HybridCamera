using Dalamud.Configuration;
using Dalamud.Game.ClientState.Keys;
using DrahsidLib;
using System;
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
    [Obsolete] public MoveModeCondition? autorunMoveMode = null;
    [Obsolete] public MoveModeCondition? cameraRotateMoveMode = null;
    [Obsolete] public List<VirtualKey>? legacyModeKeyList = null;
    [Obsolete] public TurnOnCameraTurn? useTurnOnCameraTurn = null;
    [Obsolete] public bool ControllerMode = false;

    public bool useTurnOnBackpedal;
    public bool useTurnOnFrontpedal;
    public bool useLegacyWhileMoving;
    public bool useLegacyTurning;
    public bool disableLRMouseMove;
    //public bool fullspeedBackpedal;
    public bool shutUpConfigHelp;
    public bool ShowExperimental;
    public bool HideTooltips;
    #endregion

    public Configuration() {
        useTurnOnBackpedal = true;
        useTurnOnFrontpedal = true;
        useLegacyWhileMoving = true;
        useLegacyTurning = true;
        disableLRMouseMove = false;
        //fullspeedBackpedal = false;
        shutUpConfigHelp = false;
        ShowExperimental = false;
        HideTooltips = false;
    }

    public void Save() {
        Service.Interface.SavePluginConfig(this);
    }
}
