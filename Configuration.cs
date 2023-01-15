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

        public bool showWindow = false;

        private DalamudPluginInterface pluginInterface;

        public Configuration() {
            this.legacyModeKeyList = new List<VirtualKey>
            {
                VirtualKey.W,
                VirtualKey.A,
                VirtualKey.S,
                VirtualKey.D
            };
            this.autorunMoveMode = new MoveModeCondition();
            this.cameraRotateMoveMode = new MoveModeCondition();
            this.useTurnOnBackpedal = true;
            this.useTurnOnFrontpedal = true;
            this.useTurnOnCameraTurn = TurnOnCameraTurn.None;
            this.showWindow = false;
        }

        public void Initialize(DalamudPluginInterface pi)
        {
            this.pluginInterface = pi;
            KeybindHook.turnOnFrontpedal = useTurnOnFrontpedal;
            KeybindHook.turnOnBackpedal = useTurnOnBackpedal;
            KeybindHook.cameraTurnMode = useTurnOnCameraTurn;
        }

        public void Save()
        {
            this.pluginInterface.SavePluginConfig(this);
        }
    }
}
