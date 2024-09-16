using Dalamud.Game.ClientState.Keys;
using DrahsidLib;
using FFXIVClientStructs.FFXIV.Client.Game.Control;

namespace HybridCamera;

public static class Movement {
    private static MovementMode CameraMode = Globals.Config.defaultMovementSetting.mode;

    public static unsafe void UpdateMoveStatePre() {
        uint mode = (uint)Globals.Config.defaultMovementSetting.mode;

        if (Service.KeyState == null) {
            return;
        }

        foreach (VirtualKey key in Globals.Config.legacyModeKeyList) {
            if (Service.KeyState[key]) {
                mode = (uint)MovementMode.Legacy;
                break;
            }
        }

        if (Globals.Config.autorunMoveMode.condition && InputManager.IsAutoRunning()) {
            mode = (uint)Globals.Config.autorunMoveMode.mode;
        }

        if (Globals.Config.cameraRotateMoveMode.condition && Globals.PlayerIsRotatingCamera()) {
            mode = (uint)Globals.Config.cameraRotateMoveMode.mode;
        }

        CameraMode = (MovementMode)mode;
        GameConfig.UiControl.Set("MoveMode", mode);

        if (Service.CameraManager->Camera->Mode == (int)CameraControlMode.FirstPerson) {
            GameConfig.UiControl.Set("MoveMode", (int)MovementMode.Standard);
        }
    }

    // for stuff which may need to be run after stuff has changed
    public static void UpdateMoveStatePost() {
        if ((Globals.Config.useTurnOnFrontpedal || Globals.Config.useTurnOnBackpedal || Globals.Config.useTurnOnCameraTurn != TurnOnCameraTurn.None) && KeybindHook.Enabled == false) {
            KeybindHook.EnableHook();
        }
    }
}


