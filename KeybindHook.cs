using Dalamud.Hooking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static HybridCamera.KeybindHook;

// credit to https://github.com/Caraxi/SimpleTweaksPlugin/blob/main/Tweaks/SmartStrafe.cs

namespace HybridCamera
{
    internal static class KeybindHook
    {
        public enum KeybindID : int
        {
            MoveForward = 321,
            MoveBack = 322,
            TurnLeft = 323,
            TurnRight = 324,
            StrafeLeft = 325,
            StrafeRight = 326
        }

        public static bool Enabled = false;
        public static bool turnOnFrontpedal = false;
        public static bool turnOnBackpedal = false;
        public static TurnOnCameraTurn cameraTurnMode = TurnOnCameraTurn.None;

        internal const string CheckStrafeKeybindSig = "E8 ?? ?? ?? ?? 84 C0 74 04 41 C6 07 01 BA 44 01 00 00";

        [return: MarshalAs(UnmanagedType.U1)]
        private delegate bool CheckStrafeKeybindDelegate(IntPtr ptr, KeybindID keybind);

        private static Hook<CheckStrafeKeybindDelegate> Hook;

        public static void EnableHook()
        {
            if (Enabled) return;
            Hook ??= new Hook<CheckStrafeKeybindDelegate>(Service.SigScanner.ScanText(CheckStrafeKeybindSig), CheckStrafeKeybind);
            Hook.Enable();
            Enabled = true;
        }

        public static void DisableHook()
        {
            if (Enabled == false) return;
            Hook.Disable();
            Enabled = false;
        }

        // assuming the config option is on
        private static bool CheckStrafeKeybind(IntPtr ptr, KeybindID keybind)
        {
            if (keybind == KeybindID.StrafeLeft || keybind == KeybindID.StrafeRight) {
                bool rotatingCam = Service.PlayerIsRotatingCamera();
                if (turnOnFrontpedal)
                {
                    if (Hook.Original(ptr, KeybindID.MoveForward))
                    {
                        return false;
                    }
                }
                if (turnOnBackpedal) {
                    if (Hook.Original(ptr, KeybindID.MoveBack))
                    {
                        return false;
                    }
                }

                if (rotatingCam)
                {
                    if (cameraTurnMode == TurnOnCameraTurn.Turning)
                    {
                        return false;
                    }
                }
                else if (cameraTurnMode == TurnOnCameraTurn.WithoutTurning)
                {
                    return false;
                }
            }
            return Hook.Original(ptr, keybind);
        }
    }
}
