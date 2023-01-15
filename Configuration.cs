using Dalamud.Configuration;
using Dalamud.Game.ClientState.Keys;
using Dalamud.Plugin;
using System.Collections.Generic;

namespace HybridCamera
{
    public enum TurnOnCameraTurn
    {
        None = 0,
        Turning,
        WithoutTurning,
        Count
    }

    public class MoveModeCondition
    {
        public MovementMode mode = MovementMode.Standard;
        public bool condition = false;
    }

    public class Configuration : IPluginConfiguration
    {
        int IPluginConfiguration.Version { get; set; }

        #region Saved configuration values
        public MoveModeCondition autorunMoveMode;
        public MoveModeCondition cameraRotateMoveMode;
        public bool useTurnOnBackpedal;
        public bool useTurnOnFrontpedal;
        public TurnOnCameraTurn useTurnOnCameraTurn;
        public List<VirtualKey> legacyModeKeyList;
        #endregion

        private DalamudPluginInterface pluginInterface;

        public Configuration() {
            autorunMoveMode = new MoveModeCondition();
            cameraRotateMoveMode = new MoveModeCondition();
            useTurnOnBackpedal = true;
            useTurnOnFrontpedal = true;
            useTurnOnCameraTurn = TurnOnCameraTurn.None;
        }

        public void Initialize(DalamudPluginInterface pi)
        {
            pluginInterface = pi;
            if (legacyModeKeyList== null)
            {
                legacyModeKeyList = new List<VirtualKey>
                {
                    VirtualKey.W,
                    VirtualKey.A,
                    VirtualKey.S,
                    VirtualKey.D
                };
            }
            KeybindHook.turnOnFrontpedal = useTurnOnFrontpedal;
            KeybindHook.turnOnBackpedal = useTurnOnBackpedal;
            KeybindHook.cameraTurnMode = useTurnOnCameraTurn;
        }

        public void Save()
        {
            pluginInterface.SavePluginConfig(this);
        }
    }
}
