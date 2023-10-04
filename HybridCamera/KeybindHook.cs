using Dalamud.Hooking;
using Dalamud.Logging;
using Dalamud.Plugin.Services;
using DrahsidLib;
using System;
using System.Runtime.InteropServices;

// credit to https://github.com/Caraxi/SimpleTweaksPlugin/blob/main/Tweaks/SmartStrafe.cs

namespace HybridCamera;

internal static class KeybindHook {
    public enum KeybindID : int {
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

    private static IntPtr CheckStrafeKeybindPtr = IntPtr.Zero;

    [return: MarshalAs(UnmanagedType.U1)]
    private delegate bool CheckStrafeKeybindDelegate(IntPtr ptr, KeybindID keybind);

    private static Hook<CheckStrafeKeybindDelegate> Hook { get; set; } = null!;

    public static void EnableHook() {
        if (Enabled) return;
        CheckStrafeKeybindPtr = Service.SigScanner.ScanText("E8 ?? ?? ?? ?? 84 C0 74 04 41 C6 07 01 BA 44 01 00 00");
        Service.Logger.Warning($"CheckStrafeKeybindPtr: {CheckStrafeKeybindPtr.ToString("X")}");
        Hook = Service.GameInteropProvider.HookFromAddress<CheckStrafeKeybindDelegate>(CheckStrafeKeybindPtr, CheckStrafeKeybind);

        Hook.Enable();
        Enabled = true;

        Service.Logger.Information(CheckStrafeKeybindPtr.ToString("X"));
    }

    public static void DisableHook() {
        if (Enabled == false) {
            return;
        }
        Hook.Disable();
        Enabled = false;
    }

    // assuming the config option is on
    private static unsafe bool CheckStrafeKeybind(IntPtr ptr, KeybindID keybind) {
        if (keybind == KeybindID.StrafeLeft || keybind == KeybindID.StrafeRight) {
            bool rotatingCam = Globals.PlayerIsRotatingCamera();

            if (turnOnFrontpedal) {
                if (Hook.Original(ptr, KeybindID.MoveForward)) {
                    return false;
                }
            }
            if (turnOnBackpedal) {
                if (Hook.Original(ptr, KeybindID.MoveBack)) {
                    return false;
                }
            }

            if (rotatingCam) {
                if (cameraTurnMode == TurnOnCameraTurn.Turning) {
                    return false;
                }
            }
            else if (cameraTurnMode == TurnOnCameraTurn.WithoutTurning) {
                return false;
            }
        }

        return Hook.Original(ptr, keybind);
    }
}

