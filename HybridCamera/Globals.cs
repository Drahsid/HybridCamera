using DrahsidLib;

namespace HybridCamera;

internal class Globals {
    public static Configuration Config { get; set; } = null!;

    internal static unsafe bool PlayerIsRotatingCamera() {
        GameCameraManager* cm = GameCameraManager.Instance();
        GameCamera* cam = null;

        if (cm == null) {
            return false;
        }

        cam = cm->Camera;
        if (cam == null) {
            return false;
        }

        return ((cam->CameraBase.UnkFlags >> 1) & 1) != 0;
    }
}
