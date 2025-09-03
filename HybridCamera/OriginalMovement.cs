using Dalamud.Game.ClientState.Keys;
using DrahsidLib;
using FFXIVClientStructs.FFXIV.Client.Game.Control;

namespace HybridCamera;

public static class OriginalMovement {
    private static MovementMode CameraMode = MovementMode.Standard;

    internal static unsafe bool PlayerIsRotatingCamera()
    {
        GameCameraManager* cm = GameCameraManager.Instance();
        GameCamera* cam = null;

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

    public static unsafe void UpdateMoveState() {
        uint mode = (uint)MovementMode.Standard;

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

        if (Globals.Config.cameraRotateMoveMode.condition && PlayerIsRotatingCamera()) {
            mode = (uint)Globals.Config.cameraRotateMoveMode.mode;
        }

        CameraMode = (MovementMode)mode;
        GameConfig.UiControl.Set("MoveMode", mode);

        if (Service.CameraManager->Camera->Mode == (int)CameraControlMode.FirstPerson) {
            GameConfig.UiControl.Set("MoveMode", (int)MovementMode.Standard);
        }
    }
}


