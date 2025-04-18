using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Statuses;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using Lumina.Excel.Sheets;
using XIVComboPlugin.JobActions;

namespace XIVComboTweaks.Configuration.Helpers
{
    internal unsafe class SMNHelper : BaseHelper
    {
        private static uint[] moves = { SMN.Ruin, SMN.Outburst, SMN.Fester, SMN.Painflare, SCH.Resurrection, SMN.Ruin2, SMN.Tridisaster};
        public static new bool applies(uint move)
        {
            return moves.Contains<uint>(move);
        }

        public static new uint move(uint move)
        {
            switch (move)
            {
                case SMN.Ruin:
                    return SingleTarget();
                case SMN.Outburst:
                    return MultiTarget();
                case SMN.Fester:
                    return SingleInstant();
                case SMN.Painflare:
                    return MultiInstant();
                case SCH.Resurrection:
                    if (Ready(SCH.Swiftcast))
                        return SCH.Swiftcast;
                    return SCH.Resurrection;
                case SMN.Ruin2:
                    return original(SMN.Ruin);
                case SMN.Tridisaster:
                    return instance.level >= 26 ? original(SMN.Outburst) : original(SMN.Ruin);
                default:
                    return 0;
            }
        }

        /*
         * summon bahamut/phoenix/solar bahamut - dreadwyrm trance
         * summon ifrit - all gems ready
         * ruby rite - ifrit out
         * ifrit moves
         * summon titan
         * titan rite
         * titan moves?
         * summaon garuda
         * garuda rite
         * garuda move
         * ruin 4
         * ruin 3
         * 
         */
        public static uint SingleTarget()
        {
            int level = instance.level;
            // Bahamut
            if (instance.level >= 58 && Ready(SMN.DreadwyrmTrance))
                return original(SMN.DreadwyrmTrance);
            if (bahamutOut())
                return original(SMN.Ruin);
            // Ifrit
            if (Highlighted(original(SMN.SummonIfrit)))
                return original(SMN.SummonIfrit);
            if (instance.smnGauge.AttunementTimerRemaining > 0 && instance.smnGauge.IsIfritAttuned)
                return original(SMN.Gemshine);
            if (level >= 92)
            {
                if (original(SMN.AstralFlow) == SMN.CrimsonCyclone)
                    return original(SMN.CrimsonCyclone);
                if (original(SMN.AstralFlow) == SMN.CrimsonStrike)
                    return original(SMN.CrimsonStrike);
            }
            // Titan
            if ((Highlighted(original(SMN.SummonTitan))))
                return original(SMN.SummonTitan);
            if (instance.smnGauge.AttunementTimerRemaining > 0 && instance.smnGauge.IsTitanAttuned)
                return original(SMN.Gemshine);
            // Garuda
            if ((Highlighted(original(SMN.SummonGaruda))))
                return original(SMN.SummonGaruda);
            if (level >= 92 && original(SMN.AstralFlow) == SMN.Slipstream)
                return SMN.Slipstream;
            if (instance.smnGauge.AttunementTimerRemaining > 0 && instance.smnGauge.IsGarudaAttuned)
                return original(SMN.Gemshine);
            // Other
            if (level > 64 && hasStatus(SMN.Buffs.FurtherRuin))
                return SMN.Ruin4;
            else
                return original(SMN.Ruin);
        }

        //maybe just translate ST result -> MT. But then, would have to account for upgrades on actions
        public static uint MultiTarget()
        {
            int level = instance.level;
            // Bahamut
            if (instance.level >= 58 && Ready(SMN.DreadwyrmTrance))
                return original(SMN.DreadwyrmTrance);
            if (bahamutOut())
                return instance.level >= 26 ? original(SMN.Outburst) : original(SMN.Ruin);
            // Ifrit
            if (Highlighted(original(SMN.SummonIfrit)))
                return original(SMN.SummonIfrit);
            if (instance.smnGauge.AttunementTimerRemaining > 0 && instance.smnGauge.IsIfritAttuned)
                return instance.level >= 26 ? original(SMN.PreciousBrilliance) : original(SMN.Gemshine);
            if (level >= 92)
            {
                if (original(SMN.AstralFlow) == SMN.CrimsonCyclone)
                    return original(SMN.CrimsonCyclone);
                if (original(SMN.AstralFlow) == SMN.CrimsonStrike)
                    return original(SMN.CrimsonStrike);
            }
            // Titan
            if ((Highlighted(original(SMN.SummonTitan))))
                return original(SMN.SummonTitan);
            if (instance.smnGauge.AttunementTimerRemaining > 0 && instance.smnGauge.IsTitanAttuned)
                return instance.level >= 26 ? original(SMN.PreciousBrilliance) : original(SMN.Gemshine);
            // Garuda
            if ((Highlighted(original(SMN.SummonGaruda))))
                return original(SMN.SummonGaruda);
            if (level >= 92 && original(SMN.AstralFlow) == SMN.Slipstream)
                return SMN.Slipstream;
            if (instance.smnGauge.AttunementTimerRemaining > 0 && instance.smnGauge.IsGarudaAttuned)
                return instance.level >= 26 ? original(SMN.PreciousBrilliance) : original(SMN.Gemshine);
            // Other
            if (level > 64 && hasStatus(SMN.Buffs.FurtherRuin))
                return SMN.Ruin4;
            else
                return instance.level >= 26 ? original(SMN.Outburst) : original(SMN.Ruin);
        }

        public static uint SingleInstant()
        {
            if (instance.level >= 92 && original(SMN.AstralFlow) == SMN.MountainBuster)
                return SMN.MountainBuster;
            if (Ready(SMN.EnergyDrain) && !instance.smnGauge.HasAetherflowStacks)
                return SMN.EnergyDrain;
            else if (instance.level >= 70 && bahamutOut() && Ready(original(SMN.EnkindleBahamut)))
                return original(SMN.EnkindleBahamut);
            else if (instance.level >= 60 && bahamutOut() && Ready(original(SMN.AstralFlow)))
                return original(SMN.AstralFlow);
            if (instance.level >= 100 && hasStatus(SMN.Buffs.RefulgentLux))
                return SMN.LuxSolaris;
            if (Ready(SCH.LucidDreaming))
                return SCH.LucidDreaming;
            else if (!instance.smnGauge.HasAetherflowStacks)
                return SMN.EnergyDrain;
            return original(SMN.Fester);
        }

        public static uint MultiInstant()
        {
            if (instance.level >= 92 && original(SMN.AstralFlow) == SMN.MountainBuster)
                return SMN.MountainBuster;
            if (Ready(SMN.EnergyDrain) && !instance.smnGauge.HasAetherflowStacks)
                return instance.level >= 52 ? SMN.EnergySiphon : SMN.EnergyDrain;
            else if (instance.level >= 70 && bahamutOut() && Ready(original(SMN.EnkindleBahamut)))
                return original(SMN.EnkindleBahamut);
            else if (instance.level >= 60 && bahamutOut() && Ready(original(SMN.AstralFlow)))
                return original(SMN.AstralFlow);
            if (instance.level >= 100 && hasStatus(SMN.Buffs.RefulgentLux))
                return SMN.LuxSolaris;
            else if (!instance.smnGauge.HasAetherflowStacks)
                return instance.level >= 52 ? SMN.EnergySiphon : SMN.EnergyDrain;
            if (Ready(SCH.LucidDreaming))
                return SCH.LucidDreaming;
            return instance.level >= 40 ? SMN.Painflare : SMN.Fester;
        }

        public static bool bahamutOut()
        {
            return original(SMN.AstralFlow) == SMN.Deathflare || original(SMN.AstralFlow) == SMN.Rekindle || original(SMN.AstralFlow) == SMN.Sunflare;
        }
    }
}
