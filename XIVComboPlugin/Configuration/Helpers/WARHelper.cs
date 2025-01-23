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
            return move == WAR.HeavySwing || move == WAR.Overpower || move == WAR.Infuriate || move == WAR.RawIntuition;
        }

        public static new uint move(uint move)
        {
            switch (move)
            {
                case WAR.HeavySwing:
                    if (instance.level >= 35 && instance.warGauge.BeastGauge >= 50)
                    {
                        return instance.level >= 54 ? WAR.FellCleave : WAR.InnerBeast;
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
                    if (instance.level >= 35 && instance.warGauge.BeastGauge >= 50)
                    {
                        return instance.level >= 60 ? WAR.Decimate : instance.level >= 45 ? WAR.SteelCyclone : WAR.InnerBeast;
                    }
                    if (instance.lastMove == WAR.Overpower && instance.level >= 40)
                        return WAR.MythrilTempest;
                    return WAR.Overpower;
                case WAR.Infuriate:
                    if (instance.level >= 60 && Charges(WAR.Infuriate) == 2)
                        return WAR.Infuriate;
                    if (Ready(WAR.Berserk))
                        return WAR.Berserk;
                    return WAR.Infuriate;
                case WAR.RawIntuition:
                    if (instance.level >= 56 && Ready(WAR.RawIntuition))
                        return WAR.RawIntuition;
                    if (Ready(WAR.Rampart))
                        return WAR.Rampart;
                    if (instance.level >= 30 && Ready(WAR.ThrillOfBattle))
                        return WAR.ThrillOfBattle;
                    if (instance.level >= 30 && Ready(WAR.Vengeance))
                        return WAR.Vengeance;
                    return WAR.RawIntuition;
                default:
                    return 0;
            }
        }
    }
}
