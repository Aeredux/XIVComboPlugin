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
    internal unsafe class RPRHelper : BaseHelper
    {
        private static uint[] moves = { RPR.Slice, RPR.SpinningScythe, RPR.GrimSwathe, RPR.HellsEgress, RPR.HellsIngress};
        public static new bool applies(uint move)
        {
            return moves.Contains<uint>(move);
        }

        public static new uint move(uint move)
        {
            switch (move)
            {
                case RPR.Slice:
                    return SingleTarget();
                case RPR.SpinningScythe:
                    return MultiTarget();
                case RPR.GrimSwathe:
                    if (instance.level < 55)
                        return RPR.BloodStalk;
                    return RPR.GrimSwathe;
                case RPR.HellsIngress:
                case RPR.HellsEgress:
                    if (instance.level >= 74 && Highlighted(RPR.Regress))
                        return RPR.Regress;
                    return move;
                default:
                    return 0;
            }
        }

        public static uint SingleTarget()
        {
            if (hasStatus(RPR.Buffs.SoulReaver))
            {
                if (hasStatus(RPR.Buffs.EnhancedGallows))
                    return RPR.Gallows;
                return RPR.Gibbet;
            }
            if (instance.level >= 60 && Ready(RPR.SoulSlice) && instance.rprGauge.Soul <= 50)
                return RPR.SoulSlice;
            if (targetStatusTime(RPR.Debuffs.DeathsDesign) < 10)
                return RPR.ShadowOfDeath;
            if (instance.level >= 30 && Highlighted(RPR.InfernalSlice))
                return RPR.InfernalSlice;
            if (Highlighted(RPR.WaxingSlice) && Highlighted(RPR.WaxingSlice))
                return RPR.WaxingSlice;
            return RPR.Slice;
        }

        public static uint MultiTarget()
        {
            if (hasStatus(RPR.Buffs.SoulReaver))
            {
                return RPR.Guillotine;
            }
            if (instance.level >= 60 && Ready(RPR.SoulScythe) && instance.rprGauge.Soul <= 50)
                return instance.level >= 65 ? RPR.SoulScythe : RPR.SoulSlice;
            if (instance.level >= 35 && targetStatusTime(RPR.Debuffs.DeathsDesign) < 10)
                return RPR.WhorlOfDeath;
            if (instance.level >= 45 && Highlighted(RPR.NightmareScythe))
                return RPR.NightmareScythe;
            if (instance.level >= 25)
                return RPR.SpinningScythe;
            return RPR.Slice;
        }
    }
}
