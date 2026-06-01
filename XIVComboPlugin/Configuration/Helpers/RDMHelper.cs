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
    internal unsafe class RDMHelper : BaseHelper
    {
        private static uint[] moves = { RDM.Jolt2, RDM.Riposte, RDM.Impact, RDM.Fleche, RDM.Verraise, RDM.Acceleration};
        public static new bool applies(uint move)
        {
            return moves.Contains<uint>(move);
        }

        public static new uint move(uint move)
        {
            switch (move)
            {
                case RDM.Jolt2:
                    return SingleTargetSpell();
                case RDM.Riposte:
                    return SingleTarget();
                case RDM.Impact:
                    return MultiTarget();
                case RDM.Fleche:
                    return Instant();
                case RDM.Verraise:
                    if (hasFastCast())
                        return RDM.Verraise;
                    if (Ready(SCH.Swiftcast))
                        return SCH.Swiftcast;
                    return RDM.Verraise;
                case RDM.Acceleration:
                    return Buffs();
                default:
                    return 0;
            }
        }

        public static uint SingleTargetSpell()
        {
            if (Highlighted(RDM.Scorch))
                return RDM.Scorch;
            if (instance.level >= 68 && instance.rdmGauge.ManaStacks >= 3)
                return instance.level >= 70 ? LessWhite() ? original(RDM.Veraero) : original(RDM.Verthunder) : original(RDM.Verthunder);
            if (hasFastCast())
            {
                return LessWhite() ? original(RDM.Veraero) : original(RDM.Verthunder);
            }
            if (instance.level >= 30 && hasStatus(RDM.Buffs.VerstoneReady))
                return RDM.Verstone;
            if (instance.level >= 26 && hasStatus(RDM.Buffs.VerfireReady))
                return RDM.Verfire;
            return original(RDM.Jolt);
        }

        public static uint SingleTarget()
        {
            if (instance.level >= 50 && Highlighted(RDM.Redoublement))
                return original(RDM.Redoublement);
            if (instance.level >= 50 && Highlighted(RDM.Zwerchhau))
                return original(RDM.Zwerchhau);
            return original(RDM.Riposte);
        }

        public static uint MultiTarget()
        {
            if (Highlighted(RDM.Scorch))
                return RDM.Scorch;
            if (instance.level >= 68 && instance.rdmGauge.ManaStacks >= 3)
                return instance.level >= 70 ? LessWhite() ? original(RDM.Veraero) : original(RDM.Verthunder) : original(RDM.Verthunder);
            if (hasFastCast())
                return original(RDM.Scatter);
            if (instance.level >= 22 && LessWhite())
                return original(RDM.Veraero2);
            if (instance.level >= 18 && !LessWhite())
                return original(RDM.Verthunder2);
            return RDM.Jolt;
        }

        public static uint Instant()
        {
            if (instance.level >= 45 && Ready(RDM.Fleche))
                return original(RDM.Fleche);
            if (instance.level >= 56 && Ready(RDM.ContreSixte))
                return original(RDM.ContreSixte);
            if (Charges(RDM.Engagement) > 0 || instance.level < 45)
                return original(RDM.Engagement);
            return RDM.Fleche;
        }

        private static bool LessWhite()
        {
            return instance.rdmGauge.WhiteMana < instance.rdmGauge.BlackMana;
        }

        private static bool hasFastCast()
        {
            return hasStatus(RDM.Buffs.Dualcast) || hasStatus(RDM.Buffs.Acceleration) || hasStatus(RDM.Buffs.SwiftCast);
        }

        private static uint Buffs()
        {
            if (instance.level >= 58 && Ready(RDM.Embolden))
                return original(RDM.Embolden);
            if (instance.level >= 60 && Ready(RDM.Manafication))
                return RDM.Manafication;
            return RDM.Acceleration;
        }
    }
}
