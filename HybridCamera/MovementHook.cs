using Dalamud.Hooking;
using Dalamud.Game.ClientState.Objects.Types;
using DrahsidLib;
using FFXIVClientStructs.FFXIV.Client.Game.Control;
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
    [FieldOffset(0xF2)] public byte Unk_0xF2;
    [FieldOffset(0xF3)] public byte Unk_0xF3;
    [FieldOffset(0xF4)] public byte Unk_0xF4;
    [FieldOffset(0x80)] public Vector3 Unk_0x80;
    [FieldOffset(0x90)] public float MoveDir; // Relative direction (in radians) that  you are trying to move. Backwards is -PI, Left is HPI, Right is -HPI
    [FieldOffset(0x94)] public byte Unk_0x94;
    [FieldOffset(0xA0)] public Vector3 MoveForward; // direction output by MovementUpdate
    [FieldOffset(0xB0)] public float Unk_0xB0;
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
    [return: MarshalAs(UnmanagedType.U1)]
    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    public unsafe delegate byte MovementUpdate0Delegate(MoveControllerSubMemberForMine* thisx);
    // outputs wishdir_h, wishdir_v, rotatedir, did_backpedal, autorun
    public unsafe delegate void MovementUpdate1Delegate(MoveControllerSubMemberForMine* thisx, float* wishdir_h, float* wishdir_v, float* rotatedir, byte* align_with_camera, byte* autorun, byte dont_rotate_with_camera);
    // outputs direction
    public unsafe delegate void MovementUpdate2Delegate(MoveControllerSubMemberForMine* thisx, float wishdir_h, float wishdir_v, char arg4, byte align_with_camera, Vector3* direction);

    public unsafe delegate byte Client_Game_Control_MoveControl_MoveControllerSubMemberForMine_vf1(MoveControllerSubMemberForMine* thisx);

    public unsafe delegate void TestDelegate(UnkTargetFollowStruct* unk1, IntPtr unk2);

    private static Hook<MovementUpdate0Delegate>? MovementUpdateHook0 { get; set; } = null!;
    private static Hook<MovementUpdate1Delegate>? MovementUpdateHook1 { get; set; } = null!;
    private static Hook<MovementUpdate2Delegate>? MovementUpdateHook2 { get; set; } = null!;
    private static Hook<TestDelegate>? TestHook { get; set; }

    private static Hook<Client_Game_Control_MoveControl_MoveControllerSubMemberForMine_vf1>? MovementUpdateHook3 { get; set; } = null!;

    private static IntPtr MovementUpdateHook0Ptr = IntPtr.Zero;
    private static IntPtr MovementUpdateHook1Ptr = IntPtr.Zero;
    private static IntPtr MovementUpdateHook2Ptr = IntPtr.Zero;
    private static IntPtr MovementUpdateHook3Ptr = IntPtr.Zero;
    private static IntPtr TestHookPtr = IntPtr.Zero;

    public static unsafe void Initialize() {
        MovementUpdateHook0Ptr = Service.SigScanner.ScanText("48 8B C4 4C 89 48 20 53 57 41 56 41 57 48 81 EC 98 00 00 00 44 8B B9 10 01 00 00");
        Service.Logger.Warning($"MovementUpdateHook0Ptr: {MovementUpdateHook0Ptr.ToString("X")}");

        MovementUpdateHook1Ptr = Service.SigScanner.ScanText("48 8B C4 4C 89 48 20 53 57 41 56 41 57 48 81 EC 98 00 00 00");
        Service.Logger.Warning($"MovementUpdateHook1Ptr: {MovementUpdateHook1Ptr.ToString("X")}");

        MovementUpdateHook2Ptr = Service.SigScanner.ScanText("40 55 56 41 54 41 56 41 57 48 8D 6C 24 E0");
        Service.Logger.Warning($"MovementUpdateHook2Ptr: {MovementUpdateHook2Ptr.ToString("X")}");

        MovementUpdateHook3Ptr = Service.SigScanner.ScanText("40 55 53 48 8d 6c 24 c8 48 81 ec 38 01 00 00");
        Service.Logger.Warning($"MovementUpdateHook3Ptr: {MovementUpdateHook3Ptr.ToString("X")}");

        TestHookPtr = Service.SigScanner.ScanText("48 89 5c 24 08 48 89 74 24 10 57 48 83 ec 20 48 8b d9 48 8b fa 0f b6 89 59 05 00 00 be 00 00 00 e0");
        Service.Logger.Warning($"TestHookPtr: {TestHookPtr.ToString("X")}");

        // crash
        /*MovementUpdateHook0 = Service.GameInteropProvider.HookFromAddress<MovementUpdate0Delegate>(MovementUpdateHook0Ptr, MovementUpdate0);
        MovementUpdateHook0.Enable();*/

        MovementUpdateHook1 = Service.GameInteropProvider.HookFromAddress<MovementUpdate1Delegate>(MovementUpdateHook1Ptr, MovementUpdate1);
        MovementUpdateHook1.Enable();

        MovementUpdateHook2 = Service.GameInteropProvider.HookFromAddress<MovementUpdate2Delegate>(MovementUpdateHook2Ptr, MovementUpdate2);
        MovementUpdateHook2.Enable();

        MovementUpdateHook3 = Service.GameInteropProvider.HookFromAddress<Client_Game_Control_MoveControl_MoveControllerSubMemberForMine_vf1>(MovementUpdateHook3Ptr, MovementUpdate3);
        MovementUpdateHook3.Enable();

        TestHook = Service.GameInteropProvider.HookFromAddress<TestDelegate>(TestHookPtr, TestUpdate);
        TestHook.Enable();
    }

    public static void Dispose() {
        //MovementUpdateHook0?.Disable();
        //MovementUpdateHook0?.Dispose();
        MovementUpdateHook1?.Disable();
        MovementUpdateHook1?.Dispose();
        MovementUpdateHook2?.Disable();
        MovementUpdateHook2?.Dispose();
        MovementUpdateHook3?.Disable();
        MovementUpdateHook3?.Dispose();
        TestHook?.Disable();
        TestHook?.Dispose();
    }

    private static unsafe void TestUpdate(UnkTargetFollowStruct* unk1, IntPtr unk2)
    {
        Service.Logger.Info($"UnkTargetFollowStruct: {((IntPtr)unk1).ToString("X")}");
        Service.Logger.Info($"Unk object IDs: {unk1->Unk_0x450.Unk_GameObjectID0.ToString("X")}; {unk1->Unk_0x450.Unk_GameObjectID1.ToString("X")}; {unk1->Unk_GameObjectID1.ToString("X")}");
        Service.Logger.Info($"Follow Type: {unk1->FollowType.ToString("X")}");
    
        foreach (IGameObject obj in Service.ObjectTable)
        {
            if (obj.EntityId == unk1->GameObjectIDToFollow)
            {
                Service.Logger.Info($"{unk1->GameObjectIDToFollow.ToString("X")}: {obj.Name.TextValue}");
                break;
            }
        }

        TestHook.Original(unk1, unk2);
    }

    [return: MarshalAs(UnmanagedType.U1)]
    public static unsafe byte MovementUpdate0(MoveControllerSubMemberForMine* thisx) {
        return MovementUpdateHook0.Original(thisx);
    }

    public static unsafe void MovementUpdate1(MoveControllerSubMemberForMine* thisx, float* wishdir_h, float* wishdir_v, float* rotatedir, byte* align_with_camera, byte* autorun, byte dont_rotate_with_camera) {
        MovementUpdateHook1.Original(thisx, wishdir_h, wishdir_v, rotatedir, align_with_camera, autorun, dont_rotate_with_camera);
    }

    public static unsafe void MovementUpdate2(MoveControllerSubMemberForMine* thisx, float wishdir_h, float wishdir_v, char arg4, byte align_with_camera, Vector3* direction) {
        MovementUpdateHook2.Original(thisx, wishdir_h, wishdir_v, arg4, align_with_camera, direction);
    }

    [return: MarshalAs(UnmanagedType.U1)]
    public static unsafe byte MovementUpdate3(MoveControllerSubMemberForMine* thisx)
    {
        InputManager.MouseButtonHoldState* hold = InputManager.GetMouseButtonHoldState();
        InputManager.MouseButtonHoldState original = *hold;
        if (*hold == (InputManager.MouseButtonHoldState.Left | InputManager.MouseButtonHoldState.Right))
        {
            *hold = InputManager.MouseButtonHoldState.None;
        }

        byte ret = MovementUpdateHook3.Original(thisx);
        *hold = original;
        return ret;
    }
}
