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

[StructLayout(LayoutKind.Explicit, Size = 0x20)]
unsafe struct UnkTargetFollowStruct_Unk0x2A0
{
    [FieldOffset(0x00)] public IntPtr vtable; // 0 = ctor, length 1
    [FieldOffset(0x10)] public float Unk_0x10;
}

[StructLayout(LayoutKind.Explicit, Size = 0x18)]
unsafe struct UnkTargetFollowStruct_Unk0x118
{
    [FieldOffset(0x00)] public IntPtr vtable; // 0 = ctor, length 1
}

[StructLayout(LayoutKind.Explicit, Size = 0x48)]
unsafe struct UnkTargetFollowStruct_Unk0xC8
{
    [FieldOffset(0x00)] public IntPtr vtable; // 0 = ctor, length 1
    [FieldOffset(0x08)] public IntPtr vtable2; // 0 = ctor, length 1
}

[StructLayout(LayoutKind.Explicit, Size = 0x60)]
unsafe struct UnkTargetFollowStruct_FollowType4
{
    [FieldOffset(0x00)] public IntPtr vtable; // 0 = ctor, length 3
    [FieldOffset(0x10)] public float Unk_0x10;
    [FieldOffset(0x14)] public float Unk_0x14;
    [FieldOffset(0x18)] public float Unk_0x18;
    [FieldOffset(0x20)] public float Unk_0x20;
    [FieldOffset(0x28)] public float Unk_0x28;
    [FieldOffset(0x30)] public Vector3 PlayerPosition;
    [FieldOffset(0x48)] public uint GameObjectID0;
    [FieldOffset(0x4C)] public uint GameObjectID1;
    [FieldOffset(0x54)] public short FollowingTarget; // nonzero when following target
}

[StructLayout(LayoutKind.Explicit, Size = 0x10)]
unsafe struct UnkTargetFollowStruct_FollowType3
{
    [FieldOffset(0x00)] public IntPtr vtable; // 0 = ctor, length 3
    [FieldOffset(0x08)] public byte Unk_0x8;
    [FieldOffset(0x09)] public byte Unk_0x9;
}

[StructLayout(LayoutKind.Explicit, Size = 0x10)]
unsafe struct UnkTargetFollowStruct_FollowType2
{
    [FieldOffset(0x00)] public IntPtr vtable; // 0 = ctor, length 3
    [FieldOffset(0x08)] public uint GameObjectID;
}

[StructLayout(LayoutKind.Explicit, Size = 0x10)]
unsafe struct UnkTargetFollowStruct_FollowType1
{
    [FieldOffset(0x00)] public IntPtr vtable; // 0 = ctor, length 3
    [FieldOffset(0x08)] public byte Unk_0x8;
}

// Initialized after Client::Game::Control::CameraManager.ctor and Client::Game::Control::TargetSystem.Initialize
[StructLayout(LayoutKind.Explicit, Size = 0x590)]
unsafe struct UnkTargetFollowStruct
{
    [FieldOffset(0x00)] public IntPtr vtable; // 0 = ctor, length 2
    [FieldOffset(0x10)] public IntPtr vtbl_Client__Game__Control__MoveControl__MoveControllerSubMemberForMine;
    [FieldOffset(0x30)] public ulong Unk_0x30;
    [FieldOffset(0x4C)] public byte Unk_0x4C;
    [FieldOffset(0x4D)] public byte Unk_0x4D;
    [FieldOffset(0x50)] public byte Unk_0x50;
    [FieldOffset(0x0C8)] public UnkTargetFollowStruct_Unk0xC8 Unk_0xC8;
    [FieldOffset(0x118)] public UnkTargetFollowStruct_Unk0x118 Unk_0x118;
    [FieldOffset(0x150)] public ulong Unk_0x150; // Client::Graphics::Vfx::VfxDataListenner (sizeof = 0xB0)

    // think some of these floats are arrays of 4
    [FieldOffset(0x2A0)] public UnkTargetFollowStruct_Unk0x2A0 Unk_0x2A0;
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

    // 6.5 -> 7.3 added 0x20 bytes of floats?
    [FieldOffset(0x36C)] public float Unk_0x36C;
    [FieldOffset(0x370)] public float Unk_0x370;
    [FieldOffset(0x374)] public float Unk_0x374;
    [FieldOffset(0x378)] public float Unk_0x378;
    [FieldOffset(0x37C)] public float Unk_0x37C;
    [FieldOffset(0x380)] public float Unk_0x380;
    [FieldOffset(0x384)] public float Unk_0x384;
    [FieldOffset(0x388)] public float Unk_0x388;
    //

    [FieldOffset(0x3C0)] public IntPtr Unk_0x3C0;
    [FieldOffset(0x410)] public ulong Unk_0x410;
    [FieldOffset(0x430)] public uint Unk_0x430;
    [FieldOffset(0x434)] public uint Unk_0x434;
    [FieldOffset(0x438)] public uint Unk_0x438;
    [FieldOffset(0x440)] public uint Unk_0x440;
    [FieldOffset(0x444)] public uint Unk_0x444;
    [FieldOffset(0x448)] public uint Unk_0x448;
    [FieldOffset(0x450)] public uint GameObjectIDToFollow;
    [FieldOffset(0x458)] public uint Unk_0x458;

    [FieldOffset(0x460)] public UnkTargetFollowStruct_FollowType3 FollowType3Data;
    [FieldOffset(0x470)] public UnkTargetFollowStruct_FollowType4 FollowType4Data;
    [FieldOffset(0x4D0)] public UnkTargetFollowStruct_FollowType2 FollowType2Data;
    [FieldOffset(0x4E0)] public UnkTargetFollowStruct_FollowType1 FollowType1Data;

    [FieldOffset(0x4F0)] public IntPtr Unk_0x4F0; // some sort of array (indexed by Unk_0x578?) unsure how large

    [FieldOffset(0x568)] public ulong Unk_0x568; // param_1->Unk_0x568 = (lpPerformanceCount->QuadPart * 1000) / lpFrequency->QuadPart;
    [FieldOffset(0x570)] public float Unk_0x570;
    [FieldOffset(0x574)] public int Unk_0x574; // seems to be some sort of counter or timer
    [FieldOffset(0x578)] public byte Unk_0x578; // used as an index (?)
    [FieldOffset(0x579)] public byte FollowType; // 2 faces the player away, 3 runs away, 4 runs towards, 0 is none
                                                 // unknown but known possible values: 1, 5
    [FieldOffset(0x57B)] public byte Unk_0x57B;
    [FieldOffset(0x57C)] public byte Unk_0x57C;
    [FieldOffset(0x57D)] public byte Unk_0x57D;
    [FieldOffset(0x57E)] public byte Unk_0x57E;
    [FieldOffset(0x57F)] public byte Unk_0x57F;
    [FieldOffset(0x580)] public byte Unk_0x580;
}

[StructLayout(LayoutKind.Explicit)]
unsafe struct UnkGameObjectStruct {
    [FieldOffset(0xD0)] public int Unk_0xD0;
    [FieldOffset(0x101)] public byte Unk_0x101;
    [FieldOffset(0x1C0)] public Vector3 DesiredPosition;
    [FieldOffset(0x1D0)] public float NewRotation;
    [FieldOffset(0x1D4)] public float Unk_0x1D4;
    [FieldOffset(0x1E4)] public float Unk_0x1E4;
    [FieldOffset(0x1F4)] public int Unk_0x1F4; // movement mode? determines speed selected from MoveControllerSubMemberForMine->MoveSpeedMaximums
    [FieldOffset(0x1FC)] public byte Unk_0x1FC;
    [FieldOffset(0x1FE)] public byte Unk_0x1FE;
    [FieldOffset(0x1FF)] public byte Unk_0x1FF;
    [FieldOffset(0x200)] public byte Unk_0x200;
    [FieldOffset(0x2C6)] public byte Unk_0x2C6;
    [FieldOffset(0x3D0)] public CSGameObject* Actor; // Points to local player
    [FieldOffset(0x348)] public byte Unk_0x348;
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
    [FieldOffset(0x28)] public float Unk_0x28;
    [FieldOffset(0x38)] public float Unk_0x38;
    [FieldOffset(0x3C)] public byte Moved;
    [FieldOffset(0x3D)] public byte Rotated; // 1 when the character has rotated
    [FieldOffset(0x3E)] public byte MovementLock; // Pretty much forced auto run when nonzero. Maybe used for scene transitions?
    [FieldOffset(0x3F)] public byte Unk_0x3F; // 1 when mouse-running (lmb + rmb)
    [FieldOffset(0x40)] public byte Unk_0x40;
    [FieldOffset(0x44)] public float MoveSpeed;
    [FieldOffset(0x50)] public float* MoveSpeedMaximums;
    [FieldOffset(0x80)] public Vector3 ZoningPosition; // this gets set to your positon when you are in a scene/zone transition
    [FieldOffset(0x90)] public float MoveDir; // Relative direction (in radians) that  you are trying to move. Backwards is -PI, Left is HPI, Right is -HPI
    [FieldOffset(0x94)] public byte Unk_0x94;
    [FieldOffset(0xA0)] public Vector3 MoveForward; // direction output by MovementUpdate
    [FieldOffset(0xB0)] public float Unk_0xB0;
    [FieldOffset(0xB4)] public byte Unk_0xB4; // 
    [FieldOffset(0xF2)] public byte Unk_0xF2;
    [FieldOffset(0xF3)] public byte Unk_0xF3;
    [FieldOffset(0xF4)] public byte Unk_0xF4;
    [FieldOffset(0xF5)] public byte Unk_0xF5;
    [FieldOffset(0xF6)] public byte Unk_0xF6;
    [FieldOffset(0x104)] public byte Unk_0x104; // If you were moving last frame, this value is 0, you moved th is frame, and you moved on only one axis, this can get set to 3
    [FieldOffset(0x110)] public Int32 WishdirChanged; // 1 when your movement direction has changed (0 when autorunning, for example). This is set to 2 if dont_rotate_with_camera is 0, and this is not 1
    [FieldOffset(0x114)] public float Wishdir_Horizontal; // Relative direction on the horizontal axis
    [FieldOffset(0x118)] public float Wishdir_Vertical; // Relative direction on the vertical (forward) axis
    [FieldOffset(0x120)] public byte Unk_0x120;
    [FieldOffset(0x121)] public byte Rotated1; // 1 when the character has rotated, with the exception of standard-mode turn rotation
    [FieldOffset(0x122)] public byte Unk_0x122;
    [FieldOffset(0x123)] public byte Unk_0x123;
    [FieldOffset(0x125)] public byte Unk_0x125; // 1 when walking
    [FieldOffset(0x12A)] public byte Unk_0x12A;
}

internal class MovementHook
{
    public unsafe delegate byte Client_Game_Control_MoveControl_MoveControllerSubMemberForMine_vf1(MoveControllerSubMemberForMine* thisx);
    [return: MarshalAs(UnmanagedType.U1)]
    // outputs wishdir_h, wishdir_v, rotatedir, align_with_camera, autorun
    public unsafe delegate void MovementDirectionUpdateDelegate(MoveControllerSubMemberForMine* thisx, float* wishdir_h, float* wishdir_v, float* rotatedir, byte* align_with_camera, byte* autorun, byte dont_rotate_with_camera);
    // outputs direction
    public unsafe delegate void MovementUpdateDelegate(MoveControllerSubMemberForMine* thisx, float wishdir_h, float wishdir_v, char arg4, byte align_with_camera, Vector3* direction);

    public unsafe delegate void MovementUpdate2Delegate(MoveControllerSubMemberForMine* thisx, Vector3* direction, byte arg3, UInt64 align_with_camera, byte arg5, byte arg6, Vector3* arg7, UInt64 arg8);

    public unsafe delegate void MovementSpeedUpdateDelegate(MoveControllerSubMemberForMine* thisx);
    public unsafe delegate void UnkTargetFollowStructUpdateDelegate(UnkTargetFollowStruct* thisx, IntPtr arg2);

    private static Hook<Client_Game_Control_MoveControl_MoveControllerSubMemberForMine_vf1>? MoveControllerSubMemberForMine_vf1Hook { get; set; } = null!;
    private static Hook<MovementDirectionUpdateDelegate>? MovementDirectionUpdateHook { get; set; } = null!;
    private static Hook<MovementUpdateDelegate>? MovementUpdateHook { get; set; } = null!;
    private static Hook<MovementUpdate2Delegate>? MovementUpdate2Hook { get; set; } = null!;
    private static Hook<MovementSpeedUpdateDelegate>? MovementSpeedUpdateHook { get; set; } = null!;
    private static Hook<UnkTargetFollowStructUpdateDelegate>? UnkTargetFollowStructHook { get; set; } = null!;

    private static IntPtr MoveControllerSubMemberForMine_vfPtr = IntPtr.Zero;
    private static IntPtr MovementDirectionUpdatePtr = IntPtr.Zero;
    private static IntPtr MovementUpdatePtr = IntPtr.Zero;
    private static IntPtr MovementUpdate2Ptr = IntPtr.Zero;
    private static IntPtr MovementSpeedUpdatePtr = IntPtr.Zero;
    private static IntPtr UnkTargetFollowStructUpdatePtr = IntPtr.Zero;


    private static IntPtr _MoveControllerSubMemberForMine = IntPtr.Zero;

    public static unsafe void Initialize()
    {
        //MoveControllerSubMemberForMine_vfPtr = Service.SigScanner.ScanText("40 55 53 48 ?? ?? ?? ?? 48 81 ec ?? ?? 00 00 48 83 79 ?? 00");
        //Service.Logger.Warning($"MoveControllerSubMemberForMine_vfPtr: {MoveControllerSubMemberForMine_vfPtr.ToString("X")}");

        MovementDirectionUpdatePtr = Service.SigScanner.ScanText("48 8b c4 4c 89 48 ?? 53 55 57 41 54 48 81 ec ?? 00 00 00");
        Service.Logger.Warning($"MovementDirectionUpdatePtr: {MovementDirectionUpdatePtr.ToString("X")}");

        //MovementUpdatePtr = Service.SigScanner.ScanText("48 8b c4 48 89 70 ?? 48 89 78 ?? 55 41 56 41 57");
        //Service.Logger.Warning($"MovementUpdatePtr: {MovementUpdatePtr.ToString("X")}");

        //MovementUpdate2Ptr = Service.SigScanner.ScanText("48 89 5c 24 ?? 48 89 6c 24 ?? 48 89 74 24 ?? 48 89 7c 24 ?? 41 56 48 83 ec ?? f3 0f 10 4a");
        //Service.Logger.Warning($"MovementUpdate2Ptr: {MovementUpdate2Ptr.ToString("X")}");

        //MovementSpeedUpdatePtr = Service.SigScanner.ScanText("40 53 48 83 ec ?? 80 79 ?? 00 48 8b d9 0f ?? ?? ?? ?? ?? 48 89 ?? ?? ?? 48");
        //Service.Logger.Warning($"MovementSpeedUpdatePtr: {MovementSpeedUpdatePtr.ToString("X")}");

        //MoveControllerSubMemberForMine_vf1Hook = Service.GameInteropProvider.HookFromAddress<Client_Game_Control_MoveControl_MoveControllerSubMemberForMine_vf1>(MoveControllerSubMemberForMine_vfPtr, MoveControllerSubMemberForMine_vf1);
        //MoveControllerSubMemberForMine_vf1Hook.Enable();

        MovementDirectionUpdateHook = Service.GameInteropProvider.HookFromAddress<MovementDirectionUpdateDelegate>(MovementDirectionUpdatePtr, MovementDirectionUpdate);
        MovementDirectionUpdateHook.Enable();

        //MovementUpdateHook = Service.GameInteropProvider.HookFromAddress<MovementUpdateDelegate>(MovementUpdatePtr, MovementUpdate);
        //MovementUpdateHook.Enable();

        //MovementUpdate2Hook = Service.GameInteropProvider.HookFromAddress<MovementUpdate2Delegate>(MovementUpdate2Ptr, MovementUpdate2);
        //MovementUpdate2Hook.Enable();

        //MovementSpeedUpdateHook = Service.GameInteropProvider.HookFromAddress<MovementSpeedUpdateDelegate>(MovementSpeedUpdatePtr, MovementSpeedUpdate);
        //MovementSpeedUpdateHook.Enable();

        //UnkTargetFollowStructUpdatePtr = Service.SigScanner.ScanText("48 89 5c 24 ?? 48 89 74 24 ?? 57 48 83 ec ?? 48 8b d9 48 8b fa 0f b6 89 ?? ?? 00 00 be 00 00 00 e0");
        //UnkTargetFollowStructHook = Service.GameInteropProvider.HookFromAddress<UnkTargetFollowStructUpdateDelegate>(UnkTargetFollowStructUpdatePtr, UnkTargetFollowStruct_Update);
        //UnkTargetFollowStructHook.Enable();
    }

    public static void Dispose()
    {
        //MoveControllerSubMemberForMine_vf1Hook?.Disable();
        //MoveControllerSubMemberForMine_vf1Hook?.Dispose();
        MovementDirectionUpdateHook?.Disable();
        MovementDirectionUpdateHook?.Dispose();
        //MovementUpdateHook?.Disable();
        //MovementUpdateHook?.Dispose();
        //MovementUpdate2Hook?.Disable();
        //MovementUpdate2Hook?.Dispose();
        //MovementSpeedUpdateHook?.Disable();
        //MovementSpeedUpdateHook?.Dispose();

        //UnkTargetFollowStructHook.Disable();
        //UnkTargetFollowStructHook.Dispose();
    }

    [return: MarshalAs(UnmanagedType.U1)]
    public static unsafe byte MoveControllerSubMemberForMine_vf1(MoveControllerSubMemberForMine* thisx)
    {
        var ret = MoveControllerSubMemberForMine_vf1Hook.Original(thisx);
        return ret;
    }

    // outputs wishdir_h, wishdir_v, rotatedir, align_with_camera, autorun
    public static unsafe void MovementDirectionUpdate(MoveControllerSubMemberForMine* thisx, float* wishdir_h, float* wishdir_v, float* rotatedir, byte* align_with_camera, byte* autorun, byte dont_rotate_with_camera)
    {
        MovementDirectionUpdateHook.Original(thisx, wishdir_h, wishdir_v, rotatedir, align_with_camera, autorun, dont_rotate_with_camera);

        if (Globals.Config.disableLRMouseMove && thisx->Unk_0x3F != 0 && thisx->WishdirChanged == 2 && *wishdir_v != 0)
        {
            thisx->Unk_0x3F = 0;
            thisx->WishdirChanged = 0;
            *wishdir_v = 0;
        }

        _MoveControllerSubMemberForMine = (IntPtr)thisx;

        if (!Globals.Config.useLegacyWhileMoving && !Globals.Config.useLegacyTurning)
        {
            return;
        }

        float h = *wishdir_h;
        float v = *wishdir_v;
        float r = *rotatedir;

        MovementMode newMode = MovementMode.Standard;
        if (Globals.Config.useLegacyWhileMoving)
        {
            if (Globals.Config.useLegacyTurning && r != 0)
            {
                newMode = MovementMode.Legacy;
            }

            if (Globals.Config.useLegacyWhileMoving && (h != 0 || v != 0))
            {
                newMode = MovementMode.Legacy;

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

            switch (newMode)
            {
                default:
                case MovementMode.Standard:
                    GameConfig.UiControl.Set("MoveMode", (int)MovementMode.Standard);
                    break;
                case MovementMode.Legacy:
                    GameConfig.UiControl.Set("MoveMode", (int)MovementMode.Legacy);
                    *wishdir_h = 0;
                    *wishdir_v = 0;
                    *rotatedir = 0;
                    *autorun = 0;
                    MovementDirectionUpdateHook.Original(thisx, wishdir_h, wishdir_v, rotatedir, align_with_camera, autorun, dont_rotate_with_camera);
                    break;
            }
        }
    }

    public static unsafe void MovementUpdate(MoveControllerSubMemberForMine* thisx, float wishdir_h, float wishdir_v, char arg4, byte align_with_camera, Vector3* direction)
    {
        MovementUpdateHook.Original(thisx, wishdir_h, wishdir_v, arg4, align_with_camera, direction);
    }

    public static unsafe void MovementUpdate2(MoveControllerSubMemberForMine* thisx, Vector3* direction, byte arg3, UInt64 align_with_camera, byte arg5, byte arg6, Vector3* arg7, UInt64 arg8)
    {
        MovementUpdate2Hook.Original(thisx, direction, arg3, align_with_camera, arg5, arg6, arg7, arg8); // thisx->Moved indirectly gets updated in here if direction is laterally non-zero
    }

    public static unsafe void MovementSpeedUpdate(MoveControllerSubMemberForMine* thisx)
    {
        MovementSpeedUpdateHook?.Original(thisx);
    }

    public static unsafe void UnkTargetFollowStruct_Update(UnkTargetFollowStruct* thisx, IntPtr arg2)
    {
        UnkTargetFollowStructHook?.Original(thisx, arg2);
    }
}
