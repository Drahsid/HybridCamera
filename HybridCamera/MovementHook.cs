using Dalamud.Hooking;
using Dalamud.Logging;
using DrahsidLib;
using System;
using System.Runtime.InteropServices;

namespace HybridCamera;

// this breaks shit dont use it

internal class MovementHook {
    // arg4 does something with turning
    [return: MarshalAs(UnmanagedType.U8)]
    public unsafe delegate Int64 MovementDirectionUpdateDelegate(IntPtr unk_this, float* wishdir_h, float* wishdir_v, byte* arg4, byte* did_backpedal, byte* autorun, byte dont_rotate_with_camera);

    [return: MarshalAs(UnmanagedType.R4)]
    public unsafe delegate float MovementUpdateDelegate(IntPtr unk_this, Int64 arg2, Int64 arg3, byte arg4, float wishdir_h, float wishdir_v, byte autorun, float* arg8);

    private static Hook<MovementDirectionUpdateDelegate>? MovementDirectionUpdateHook { get; set; } = null!;
    private static Hook<MovementUpdateDelegate>? MovementUpdateHook { get; set; } = null!;

    /*public static void Initialize() {
        MovementDirectionUpdateHook ??= Hook<MovementDirectionUpdateDelegate>.FromAddress(Service.SigScanner.ScanText("48 8B C4 4C 89 48 20 53 57 41 56 41 57 48 81 EC 98 00 00 00"), MovementDirectionUpdate);
        MovementDirectionUpdateHook.Enable();
        MovementUpdateHook ??= Hook<MovementUpdateDelegate>.FromAddress(Service.SigScanner.ScanText("40 55 56 41 54 41 56 41 57 48 8D 6C 24 E0"), MovementUpdate);
        MovementUpdateHook.Enable();
    }

    public static void Dispose() {
        MovementDirectionUpdateHook.Disable();
        MovementDirectionUpdateHook.Dispose();
        MovementUpdateHook.Disable();
        MovementUpdateHook.Dispose();
    }

    public static unsafe Int64 MovementDirectionUpdate(IntPtr unk_this, float* wishdir_h, float* wishdir_v, byte* arg4, byte* did_backpedal, byte* autorun, byte dont_rotate_with_camera) {
        byte rotate = (Service.CameraManager->Camera->Mode == (int)CameraControlMode.FirstPerson) ? (byte)1 : (byte)0;
        Int64 ret = MovementDirectionUpdateHook.Original(unk_this, wishdir_h, wishdir_v, arg4, did_backpedal, autorun, rotate);
        Service.Logger.Information($"{unk_this.ToString("X")}: wishdir: {*wishdir_h}, {*wishdir_v}; backpedal? {*did_backpedal}; autorun? {*autorun} arg4? {*arg4}");
        if (Service.CameraManager->Camera->Mode != (int)CameraControlMode.FirstPerson) {
            *did_backpedal = 0;
        }
        return ret;
    }

    public static unsafe float MovementUpdate(IntPtr unk_this, Int64 arg2, Int64 arg3, byte arg4, float wishdir_h, float wishdir_v, byte autorun, float* arg8) {
        float ret = MovementUpdateHook.Original(unk_this, arg2, arg3, arg4, wishdir_h, wishdir_v, autorun, arg8);
        Service.Logger.Information($"wish h {wishdir_h} wish v {wishdir_v}");
        return ret;
    }*/
}
