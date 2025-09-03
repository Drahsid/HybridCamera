using Dalamud.Hooking;
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

    private static IntPtr CheckStrafeKeybindPtr = IntPtr.Zero;

    [return: MarshalAs(UnmanagedType.U1)]
    private delegate bool CheckStrafeKeybindDelegate(IntPtr ptr, KeybindID keybind);

    private static Hook<CheckStrafeKeybindDelegate> Hook { get; set; } = null!;

    public static void EnableHook() {
        if (Enabled) return;

        if (CheckStrafeKeybindPtr == IntPtr.Zero)
        {
            CheckStrafeKeybindPtr = Service.SigScanner.ScanText("e8 ?? ?? ?? ?? 84 c0 74 04 41 c6 06 01 ba 44 01 00 00");
            Service.Logger.Warning($"CheckStrafeKeybindPtr: {CheckStrafeKeybindPtr.ToString("X")}");
        }

        if (Hook == null)
        {
            Hook = Service.GameInteropProvider.HookFromAddress<CheckStrafeKeybindDelegate>(CheckStrafeKeybindPtr, CheckStrafeKeybind);
        }

        Hook.Enable();
        Enabled = true;
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
            if (Globals.Config.useTurnOnFrontpedal) {
                if (Hook.Original(ptr, KeybindID.MoveForward)) {
                    return false;
                }
            }
            if (Globals.Config.useTurnOnBackpedal) {
                if (Hook.Original(ptr, KeybindID.MoveBack)) {
                    return false;
                }
            }
        }

        return Hook.Original(ptr, keybind);
    }

    public static void UpdateKeybindHook()
    {
        if (Enabled == false && Globals.Config.Enabled == true
            && (Globals.Config.useTurnOnFrontpedal || Globals.Config.useTurnOnBackpedal))
        {
            EnableHook();
        }
        else if (Enabled == true
            && Globals.Config.useTurnOnFrontpedal == false && Globals.Config.useTurnOnBackpedal == false) {
            DisableHook();
        }
    }

    public static void Dispose() {
        DisableHook();
        Hook?.Dispose();
    }
}

