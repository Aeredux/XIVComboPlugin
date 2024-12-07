using System;
using System.Collections.Generic;
using System.IO;
using Dalamud.Configuration;
using Dalamud.Plugin;
using FFXIVClientStructs.Havok.Common.Base.Types;
using Newtonsoft.Json;

namespace XIVComboPlugin
{
    [Serializable]
    public class XIVComboConfiguration : IPluginConfiguration
    {

        public bool[] ComboPresets { get; set; }
        public int Version { get; set; }

        public List<bool> HiddenActions;

        public XIVComboConfiguration() {
            ComboPresets = new bool[Enum.GetNames(typeof(CustomComboPreset)).Length];
        }

    }
}
