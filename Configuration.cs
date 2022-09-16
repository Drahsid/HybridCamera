using Dalamud.Configuration;
using Dalamud.Game.ClientState.Keys;
using Dalamud.Plugin;
using System.Collections.Generic;

namespace HybridCamera
{
    public class Configuration : IPluginConfiguration
    {
        int IPluginConfiguration.Version { get; set; }

        #region Saved configuration values
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
