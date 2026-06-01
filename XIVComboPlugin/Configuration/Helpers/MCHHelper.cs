using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Statuses;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using Lumina.Excel.Sheets;
using XIVComboPlugin.JobActions;

namespace XIVComboTweaks.Configuration.Helpers
{
    internal unsafe class MCHHelper : BaseHelper
    {
        private static uint[] moves = { MCH.HeatedSplitShot, MCH.Scattergun, MCH.GaussRound, MCH.Tactician, MCH.RookAutoturret};
        public static new bool applies(uint move)
        {
            return moves.Contains<uint>(move);
        }

        public static new uint move(uint move)
        {
            switch (move)
            {
                case MCH.HeatedSplitShot:
                    return SingleTarget();
                case MCH.Scattergun:
                    return MultiTarget();
                case MCH.GaussRound:
                    return SingleInstant();
                case MCH.Tactician:
                    if (instance.level >= 62 && !Ready(MCH.Tactician))
                        return MCH.Dismantle;
                    return MCH.Tactician;
                case MCH.RookAutoturret:
                    if (instance.mchGauge.IsRobotActive)
                        return original(MCH.RookOverdrive);
                    return original(MCH.RookAutoturret);
                default:
                    return 0;
            }
        }

        public static uint SingleTarget()
        {
            int level = instance.level;
            if (level >= 35 && hasStatus(MCH.Buffs.Overheated))
                return (original(MCH.Heatblast));
            if (Ready(original(MCH.HotShot)))
                return original(MCH.HotShot);
            if (instance.level >= 58 && Ready(MCH.Drill))
                if (!Ready(MCH.Reassemble))
                    return MCH.Drill;
            if (instance.level >= 26 && Highlighted(original(MCH.CleanShot)))
                return original(MCH.CleanShot);
            if (Highlighted(original(MCH.SlugShot)))
                return original(MCH.SlugShot);
            return original(MCH.SplitShot);
        }

        public static uint MultiTarget()
        {
            if (instance.level >= 52 && hasStatus(MCH.Buffs.Overheated))
                return MCH.AutoCrossbow;
            if (instance.level >= 72 && Ready(MCH.BioBlaster))
                if (!Ready(MCH.Reassemble))
                    return MCH.BioBlaster;
            return original(MCH.SpreadShot);
        }

        public static uint SingleInstant()
        {
            if (instance.level >= 45 && Ready(MCH.Wildfire) && statusTime(MCH.Buffs.Overheated) >= 8)
                return MCH.Wildfire;
            if (instance.level >= 66 && Ready(MCH.BarrelStabilizer))
                return original(MCH.BarrelStabilizer);
            if (instance.mchGauge.Heat >= 50 && !hasStatus(MCH.Buffs.Overheated) && Ready(MCH.Hypercharge))
                return MCH.Hypercharge;
            if (/*(instance.level >= 58 && Ready(MCH.Drill) || instance.level < 58) && */Ready(MCH.Reassemble))
                return MCH.Reassemble;
            //int maxCharges = instance.level >= 74 ? 3 : 2;
            if (instance.level >= 50 && Charges(original(MCH.Ricochet))  >= Charges(original(MCH.GaussRound)))
                return original(MCH.Ricochet);
            return original(MCH.GaussRound);
        }

        public static uint MultiInstant()
        {
            if (instance.level >= 66 && Ready(MCH.BarrelStabilizer))
                return original(MCH.BarrelStabilizer);
            if (instance.mchGauge.Heat >= 50)
                return MCH.Hypercharge;
            if ((instance.level >= 58 && Ready(MCH.Drill) || instance.level < 58) && Ready(MCH.Reassemble))
                return MCH.Reassemble;
            //int maxCharges = instance.level >= 74 ? 3 : 2;
            if (instance.level >= 50 && Charges(original(MCH.Ricochet)) >= Charges(original(MCH.GaussRound)))
                return original(MCH.Ricochet);
            return original(MCH.GaussRound);
        }
    }
}
