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
        public bool useTurnOnBackpedal = true;
        public bool useTurnOnFrontpedal = true;
        public TurnOnCameraTurn useTurnOnCameraTurn = TurnOnCameraTurn.None;
        public List<VirtualKey> legacyModeKeyList;
        #endregion

        public bool showWindow = false;

        private readonly DalamudPluginInterface pluginInterface;

        public Configuration(DalamudPluginInterface pi)
        {
            this.pluginInterface = pi;
        }

        public void Save()
        {
            this.pluginInterface.SavePluginConfig(this);
        }
    }
}
