using Dalamud.Hooking;
using DrahsidLib;
using FFXIVClientStructs.FFXIV.Common.Math;
using System;
using System.Runtime.InteropServices;

namespace HybridCamera;

/*
 * Random reverse engineering on patch 6.xish movement code
 */

enum UnkGameObjectStruct_Unk0x418Flags : byte {
    None,
    Unk_0x1 = (1 << 0),
    NoClip = (1 << 1), // Allows the player to walk through walls
    Unk_0x2 = (1 << 2),
    Unk_0x3 = (1 << 3),
    Unk_0x4 = (1 << 4),
    Unk_0x5 = (1 << 5),
    Unk_0x6 = (1 << 6),
    Unk_0x7 = (1 << 7)
}

// has member functions
[StructLayout(LayoutKind.Explicit, Size = 0x56)]
unsafe struct UnkTargetFollowStruct_Unk0x450
{
    [FieldOffset(0x00)] public IntPtr vtable;
    [FieldOffset(0x10)] public float Unk_0x10;
    [FieldOffset(0x14)] public float Unk_0x14;
    [FieldOffset(0x18)] public float Unk_0x18;
    [FieldOffset(0x20)] public float Unk_0x20;
    [FieldOffset(0x28)] public float Unk_0x28;
    [FieldOffset(0x30)] public Vector3 PlayerPosition;
    [FieldOffset(0x40)] public uint Unk_GameObjectID0;
    [FieldOffset(0x48)] public uint Unk_GameObjectID1;
    [FieldOffset(0x50)] public int Unk_0x50;
    [FieldOffset(0x54)] public short Unk_0x54;
}

// possibly FFXIVClientStructs.FFXIV.Client.Game.Control.InputManager, ctor is ran after CameraManager and TargetSystem
[StructLayout(LayoutKind.Explicit)]
unsafe struct UnkTargetFollowStruct
{
    [FieldOffset(0x10)] public IntPtr Unk_0x10;
    [FieldOffset(0x30)] public ulong Unk_0x30;
    [FieldOffset(0x4C)] public byte Unk_0x4C;
    [FieldOffset(0x4D)] public byte Unk_0x4D;
    [FieldOffset(0x50)] public byte Unk_0x50;
    [FieldOffset(0x150)] public ulong Unk_0x150;
    [FieldOffset(0x180)] public ulong Unk_0x180;
    [FieldOffset(0x188)] public ulong Unk_0x188;
    [FieldOffset(0x1B6)] public byte Unk_0x1B6;
    [FieldOffset(0x1C0)] public byte Unk_0x1C0; // used like a bitfield
    [FieldOffset(0x1EC)] public uint Unk_0x1EC;

    // think some of these floats are arrays of 4
    [FieldOffset(0x2A0)] public float Unk_0x2A0;
    [FieldOffset(0x2B0)] public float Unk_0x2B0;
    [FieldOffset(0x2C0)] public float Unk_0x2C0;
    [FieldOffset(0x2D0)] public float Unk_0x2D0;
    [FieldOffset(0x2E0)] public float Unk_0x2E0;
    [FieldOffset(0x2E4)] public float Unk_0x2E4;
    [FieldOffset(0x2F4)] public float Unk_0x2F4;
    [FieldOffset(0x304)] public float Unk_0x304;
    [FieldOffset(0x314)] public float Unk_0x314;
    [FieldOffset(0x324)] public float Unk_0x324;
    [FieldOffset(0x328)] public float Unk_0x328;
    [FieldOffset(0x338)] public float Unk_0x338;
    [FieldOffset(0x348)] public float Unk_0x348;
    [FieldOffset(0x358)] public float Unk_0x358;
    [FieldOffset(0x368)] public float Unk_0x368;

    [FieldOffset(0x3A0)] public IntPtr Unk_0x3A0;
    [FieldOffset(0x3F0)] public ulong Unk_0x3F0;
    [FieldOffset(0x410)] public uint Unk_0x410;
    [FieldOffset(0x414)] public uint Unk_0x414;
    [FieldOffset(0x418)] public uint Unk_0x418;
    [FieldOffset(0x420)] public uint Unk_0x420;
    [FieldOffset(0x424)] public uint Unk_0x424;
    [FieldOffset(0x428)] public uint Unk_0x428;
    [FieldOffset(0x430)] public uint GameObjectIDToFollow;
    [FieldOffset(0x438)] public uint Unk_0x438;

    // possible union below

    // start of some substruct (used for FollowType == 3?)
    [FieldOffset(0x440)] public byte Unk_0x440;
    [FieldOffset(0x448)] public byte Unk_0x448;
    [FieldOffset(0x449)] public byte Unk_0x449;
    // end of substruct

    // start of UnkTargetFollowStruct_Unk0x450 (used for FollowType == 4?)
    [FieldOffset(0x450)] public UnkTargetFollowStruct_Unk0x450 Unk_0x450;
    [FieldOffset(0x4A0)] public int Unk_0x4A0; // intersects UnkTargetFollowStruct_Unk0x450->0x50
    [FieldOffset(0x4A4)] public byte Unk_0x4A4; // intersects UnkTargetFollowStruct_Unk0x450->0x54
    [FieldOffset(0x4A5)] public byte FollowingTarget; // nonzero when following target (intersects UnkTargetFollowStruct_Unk0x450->0x54)
    // end of substruct

    [FieldOffset(0x4B0)] public ulong Unk_0x4B0; // start of some substruct (dunno where this one ends) (used for FollowType == 2?)
    [FieldOffset(0x4B8)] public uint Unk_GameObjectID1;
    [FieldOffset(0x4C0)] public byte Unk_0x4C0; // start of some substruct (dunno where this one ends) (used for FollowType == 1?)
    [FieldOffset(0x4C8)] public byte Unk_0x4C8;

    // possible union probably ends around here

    [FieldOffset(0x4D0)] public IntPtr Unk_0x4D0; // some sort of array (indexed by Unk_0x558?) unsure how large

    [FieldOffset(0x548)] public ulong Unk_0x548; // param_1->Unk_0x548 = (lpPerformanceCount->QuadPart * 1000) / lpFrequency->QuadPart;
    [FieldOffset(0x550)] public float Unk_0x550;
    [FieldOffset(0x554)] public int Unk_0x554; // seems to be some sort of counter or timer
    [FieldOffset(0x558)] public byte Unk_0x558; // used as an index (?)
    [FieldOffset(0x559)] public byte FollowType; // 2 faces the player away, 3 runs away, 4 runs towards, 0 is none
                                                 // unknown but known possible values: 1, 5
    [FieldOffset(0x55B)] public byte Unk_0x55B;
    [FieldOffset(0x55C)] public byte Unk_0x55C;
    [FieldOffset(0x55D)] public byte Unk_0x55D;
    [FieldOffset(0x55E)] public byte Unk_0x55E;
    [FieldOffset(0x55F)] public byte Unk_0x55F;
    [FieldOffset(0x560)] public byte Unk_0x560;
}

[StructLayout(LayoutKind.Explicit)]
unsafe struct UnkGameObjectStruct {
    [FieldOffset(0xD0)] public int Unk_0xD0;
    [FieldOffset(0x101)] public byte Unk_0x101;
    [FieldOffset(0x1C0)] public Vector3 DesiredPosition;
    [FieldOffset(0x1D0)] public float NewRotation;
    [FieldOffset(0x1FC)] public byte Unk_0x1FC;
    [FieldOffset(0x1FF)] public byte Unk_0x1FF;
    [FieldOffset(0x200)] public byte Unk_0x200;
    [FieldOffset(0x2C6)] public byte Unk_0x2C6;
    [FieldOffset(0x3D0)] public CSGameObject* Actor; // Points to local player
    [FieldOffset(0x3E0)] public byte Unk_0x3E0;
    [FieldOffset(0x3EC)] public float Unk_0x3EC; // This, 0x3F0, 0x418, and 0x419 seem to determine the direction (and where) you turn when turning around or facing left/right
    [FieldOffset(0x3F0)] public float Unk_0x3F0;
    [FieldOffset(0x418)] public byte Unk_0x418; // flags?
    [FieldOffset(0x419)] public byte Unk_0x419; // flags?
}

[StructLayout(LayoutKind.Explicit)]
unsafe struct MoveControllerSubMemberForMine {
    [FieldOffset(0x10)] public Vector3 Direction; // direction?
    [FieldOffset(0x20)] public UnkGameObjectStruct* ActorStruct;
    [FieldOffset(0x28)] public uint Unk_0x28;
    [FieldOffset(0x3C)] public byte Moved;
    [FieldOffset(0x3D)] public byte Rotated; // 1 when the character has rotated
    [FieldOffset(0x3E)] public byte MovementLock; // Pretty much forced auto run when nonzero. Maybe used for scene transitions?
    [FieldOffset(0x3F)] public byte Unk_0x3F;
    [FieldOffset(0x40)] public byte Unk_0x40;
    [FieldOffset(0x80)] public Vector3 ZoningPosition; // this gets set to your positon when you are in a scene/zone transition
    [FieldOffset(0x90)] public float MoveDir; // Relative direction (in radians) that  you are trying to move. Backwards is -PI, Left is HPI, Right is -HPI
    [FieldOffset(0x94)] public byte Unk_0x94;
    [FieldOffset(0xA0)] public Vector3 MoveForward; // direction output by MovementUpdate
    [FieldOffset(0xB0)] public float Unk_0xB0;
    [FieldOffset(0xF2)] public byte Unk_0xF2;
    [FieldOffset(0xF3)] public byte Unk_0xF3;
    [FieldOffset(0xF4)] public byte Unk_0xF4;
    [FieldOffset(0x104)] public byte Unk_0x104; // If you were moving last frame, this value is 0, you moved th is frame, and you moved on only one axis, this can get set to 3
    [FieldOffset(0x110)] public Int32 WishdirChanged; // 1 when your movement direction has changed (0 when autorunning, for example). This is set to 2 if dont_rotate_with_camera is 0, and this is not 1
    [FieldOffset(0x114)] public float Wishdir_Horizontal; // Relative direction on the horizontal axis
    [FieldOffset(0x118)] public float Wishdir_Vertical; // Relative direction on the vertical (forward) axis
    [FieldOffset(0x120)] public byte Unk_0x120;
    [FieldOffset(0x121)] public byte Rotated1; // 1 when the character has rotated, with the exception of standard-mode turn rotation
    [FieldOffset(0x122)] public byte Unk_0x122;
    [FieldOffset(0x123)] public byte Unk_0x123;
}

internal class MovementHook {
    public unsafe delegate byte Client_Game_Control_MoveControl_MoveControllerSubMemberForMine_vf1(MoveControllerSubMemberForMine* thisx);
    [return: MarshalAs(UnmanagedType.U1)]
    // outputs wishdir_h, wishdir_v, rotatedir, align_with_camera, autorun
    public unsafe delegate void MovementDirectionUpdateDelegate(MoveControllerSubMemberForMine* thisx, float* wishdir_h, float* wishdir_v, float* rotatedir, byte* align_with_camera, byte* autorun, byte dont_rotate_with_camera);
    // outputs direction
    public unsafe delegate void MovementUpdateDelegate(MoveControllerSubMemberForMine* thisx, float wishdir_h, float wishdir_v, char arg4, byte align_with_camera, Vector3* direction);

    public unsafe delegate void MovementUpdate2Delegate(MoveControllerSubMemberForMine* thisx, Vector3* direction, byte arg3, UInt64 arg4, byte arg5, byte arg6, UInt64 arg7, UInt64 arg8);

    private static Hook<Client_Game_Control_MoveControl_MoveControllerSubMemberForMine_vf1>? MoveControllerSubMemberForMine_vf1Hook { get; set; } = null!;
    private static Hook<MovementDirectionUpdateDelegate>? MovementDirectionUpdateHook { get; set; } = null!;
    private static Hook<MovementUpdateDelegate>? MovementUpdateHook { get; set; } = null!;
    private static Hook<MovementUpdate2Delegate>? MovementUpdate2Hook { get; set; } = null!;

    private static IntPtr MoveControllerSubMemberForMine_vfPtr = IntPtr.Zero;
    private static IntPtr MovementDirectionUpdatePtr = IntPtr.Zero;
    private static IntPtr MovementUpdatePtr = IntPtr.Zero;
    private static IntPtr MovementUpdate2Ptr = IntPtr.Zero;

    public static unsafe void Initialize() {
        //MoveControllerSubMemberForMine_vfPtr = Service.SigScanner.ScanText("40 55 53 48 ?? ?? ?? ?? 48 81 ec ?? ?? 00 00 48 83 79 ?? 00");
        //Service.Logger.Warning($"MoveControllerSubMemberForMine_vfPtr: {MoveControllerSubMemberForMine_vfPtr.ToString("X")}");

        MovementDirectionUpdatePtr = Service.SigScanner.ScanText("48 8b c4 4c 89 48 ?? 53 55 57 41 54 48 81 ec ?? 00 00 00");
        Service.Logger.Warning($"MovementDirectionUpdatePtr: {MovementDirectionUpdatePtr.ToString("X")}");

        //MovementUpdatePtr = Service.SigScanner.ScanText("48 8b c4 48 89 70 ?? 48 89 78 ?? 55 41 56 41 57");
        //Service.Logger.Warning($"MovementUpdatePtr: {MovementUpdatePtr.ToString("X")}");

        //MovementUpdate2Ptr = Service.SigScanner.ScanText("48 89 5c 24 ?? 48 89 6c 24 ?? 48 89 74 24 ?? 48 89 7c 24 ?? 41 56 48 83 ec ?? f3 0f 10 4a");
        //Service.Logger.Warning($"MovementUpdate2Ptr: {MovementUpdate2Ptr.ToString("X")}");

        //MoveControllerSubMemberForMine_vf1Hook = Service.GameInteropProvider.HookFromAddress<Client_Game_Control_MoveControl_MoveControllerSubMemberForMine_vf1>(MoveControllerSubMemberForMine_vfPtr, MoveControllerSubMemberForMine_vf1);
        //MoveControllerSubMemberForMine_vf1Hook.Enable();

        MovementDirectionUpdateHook = Service.GameInteropProvider.HookFromAddress<MovementDirectionUpdateDelegate>(MovementDirectionUpdatePtr, MovementDirectionUpdate);
        MovementDirectionUpdateHook.Enable();

        //MovementUpdateHook = Service.GameInteropProvider.HookFromAddress<MovementUpdateDelegate>(MovementUpdatePtr, MovementUpdate);
        //MovementUpdateHook.Enable();

        //MovementUpdate2Hook = Service.GameInteropProvider.HookFromAddress<MovementUpdate2Delegate>(MovementUpdate2Ptr, MovementUpdate2);
        //MovementUpdate2Hook.Enable();
    }

    public static void Dispose() {
        //MoveControllerSubMemberForMine_vf1Hook?.Disable();
        //MoveControllerSubMemberForMine_vf1Hook?.Dispose();
        MovementDirectionUpdateHook?.Disable();
        MovementDirectionUpdateHook?.Dispose();
        //MovementUpdateHook?.Disable();
        //MovementUpdateHook?.Dispose();
        //MovementUpdate2Hook?.Disable();
        //MovementUpdate2Hook?.Dispose();
    }

    [return: MarshalAs(UnmanagedType.U1)]
    public static unsafe byte MoveControllerSubMemberForMine_vf1(MoveControllerSubMemberForMine* thisx) {
        return MoveControllerSubMemberForMine_vf1Hook.Original(thisx);
    }

    // outputs wishdir_h, wishdir_v, rotatedir, align_with_camera, autorun
    public static unsafe void MovementDirectionUpdate(MoveControllerSubMemberForMine* thisx, float* wishdir_h, float* wishdir_v, float* rotatedir, byte* align_with_camera, byte* autorun, byte dont_rotate_with_camera) {
        MovementDirectionUpdateHook.Original(thisx, wishdir_h, wishdir_v, rotatedir, align_with_camera, autorun, dont_rotate_with_camera);

        float h = *wishdir_h;
        float v = *wishdir_v;
        if (h != 0 || v != 0)
        {
            GameConfig.UiControl.Set("MoveMode", (int)MovementMode.Legacy);
            // fix very specific circumstance where front/backpedaling would partially have standard behavior on first frame of movement
            if (h == 0)
            {
                if (Globals.Config.useTurnOnFrontpedal && v > 0)
                {
                    *align_with_camera = 0;
                }

                if (Globals.Config.useTurnOnBackpedal && v < 0)
                {
                    *align_with_camera = 0;
                }
            }
        }
        else
        {
            GameConfig.UiControl.Set("MoveMode", (int)MovementMode.Standard);
        }

        // rerun the function to get corrected values
        *wishdir_h = 0;
        *wishdir_v = 0;
        *rotatedir = 0;
        *autorun = 0;
        MovementDirectionUpdateHook.Original(thisx, wishdir_h, wishdir_v, rotatedir, align_with_camera, autorun, dont_rotate_with_camera);
    }

    public static unsafe void MovementUpdate(MoveControllerSubMemberForMine* thisx, float wishdir_h, float wishdir_v, char arg4, byte align_with_camera, Vector3* direction) {
        MovementUpdateHook.Original(thisx, wishdir_h, wishdir_v, arg4, align_with_camera, direction);
    }

    public static unsafe void MovementUpdate2(MoveControllerSubMemberForMine* thisx, Vector3* direction, byte arg3, UInt64 arg4, byte arg5, byte arg6, UInt64 arg7, UInt64 arg8)
    {
        MovementUpdate2Hook.Original(thisx, direction, arg3, arg4, arg5, arg6, arg7, arg8); // thisx->Moved indirectly gets updated in here if direction is laterally non-zero
    }
}
