using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Statuses;
using FFXIVClientStructs.FFXIV.Client.Game;
using Lumina.Excel.Sheets;
using XIVComboPlugin.JobActions;

namespace XIVComboTweaks.Configuration.Helpers
{
    // swiftcast -> egeiro
    // eukrasian dosis -> toxikon -> dosis
    // zoe -> haima  // actually this doesn't stack
    // phlegma -> dykrasia
    // taurochole -> ixochole -> druochole
    // physis ii -> lucid dreaming
    internal unsafe class WARHelper : BaseHelper
    {
        public static new bool applies(uint move)
        {
            return move == WAR.HeavySwing || move == WAR.Overpower || move == WAR.Upheaval || move == WAR.Orogeny || move == WAR.BloodWhetting;
        }

        public static new uint move(uint move)
        {
            switch (move)
            {
                case WAR.HeavySwing:
                    if (instance.level >= 35 && instance.warGauge.BeastGauge >= 50 || instance.level >= 70 && hasStatus(WAR.Buffs.InnerReleaseStacks))
                    {
                        return original(WAR.InnerBeast);
                    }
                    if (instance.lastMove == WAR.HeavySwing && instance.level >= 4)
                        return WAR.Maim;
                    if (instance.lastMove == WAR.Maim && instance.level >= 26)
                    {
                        if (instance.level >= 50 && statusTime(WAR.Buffs.SurgingTempest) < 30)
                            return WAR.StormsEye;
                        return WAR.StormsPath;
                    }
                    return WAR.HeavySwing;
                case WAR.Overpower:
                    if (instance.level >= 35 && instance.warGauge.BeastGauge >= 50 || instance.level >= 70 && hasStatus(WAR.Buffs.InnerReleaseStacks))
                    {
                        return instance.level >= 45 ? original(WAR.SteelCyclone) : WAR.InnerBeast;
                    }
                    if (instance.lastMove == WAR.Overpower && instance.level >= 40)
                        return WAR.MythrilTempest;
                    return WAR.Overpower;
                case WAR.Upheaval:
                    if (instance.level < 70 && Ready(WAR.Berserk) || instance.level >= 70 && Ready(WAR.InnerRelease))
                        return instance.level >= 70 ? WAR.InnerRelease : WAR.Berserk;
                    if (instance.level >= 64 && Ready(WAR.Upheaval))
                        return WAR.Upheaval;
                    return WAR.Infuriate;
                case WAR.Orogeny:
                    if (instance.level < 70 && Ready(WAR.Berserk) || instance.level >= 70 && Ready(WAR.InnerRelease))
                        return instance.level >= 70 ? WAR.InnerRelease : WAR.Berserk;
                    if (instance.level >= 82 && Ready(WAR.Orogeny))
                        return WAR.Orogeny;
                    if (instance.level >= 64 && Ready(WAR.Upheaval))
                        return WAR.Upheaval;
                    return WAR.Infuriate;
                case WAR.BloodWhetting:
                    if (instance.level >= 56 && Ready(original(WAR.RawIntuition)))
                        return original(WAR.RawIntuition);
                    if (Ready(WAR.Rampart))
                        return WAR.Rampart;
                    if (instance.level >= 30 && Ready(WAR.ThrillOfBattle))
                        return WAR.ThrillOfBattle;
                    if (instance.level >= 30 && Ready(WAR.Vengeance))
                        return original(WAR.Vengeance);
                    return original(WAR.RawIntuition);
                default:
                    return 0;
            }
        }
    }
}
