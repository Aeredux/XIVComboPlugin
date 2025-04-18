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
        private static uint[] moves = { RDM.Jolt, RDM.Riposte, RDM.Scatter, RDM.Fleche};
        public static new bool applies(uint move)
        {
            return moves.Contains<uint>(move);
        }

        public static new uint move(uint move)
        {
            switch (move)
            {
                case RDM.Jolt:
                    return SingleTargetSpell();
                case RDM.Riposte:
                    return SingleTarget();
                case RDM.Scatter:
                    return MultiTarget();
                case RDM.Fleche:
                    return Instant();
                default:
                    return 0;
            }
        }

        public static uint SingleTargetSpell()
        {
            if (instance.level >= 30 && hasStatus(RDM.Buffs.VerstoneReady))
                return RDM.Verstone;
            if (instance.level >= 26 && hasStatus(RDM.Buffs.VerfireReady))
                return RDM.Verfire;
            if (hasStatus(RDM.Buffs.Acceleration) || hasStatus(RDM.Buffs.Dualcast))
            {
                return LessWhite() ? RDM.Veraero : RDM.Verthunder;
            }
            return RDM.Jolt;
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
            if (hasStatus(RDM.Buffs.Dualcast) || hasStatus(RDM.Buffs.Acceleration))
                return RDM.Scatter;
            if (instance.level >= 22 && LessWhite())
                return RDM.Veraero2;
            if (instance.level >= 18 && !LessWhite())
                return RDM.Verthunder2;
            return RDM.Jolt;
        }

        public static uint Instant()
        {
            if (instance.level >= 45 && Ready(RDM.Fleche))
                return original(RDM.Fleche);
            if (Charges(RDM.Engagement) > 0 || instance.level < 45)
                return original(RDM.Engagement);
            return RDM.Fleche;
        }

        private static bool LessWhite()
        {
            return instance.rdmGauge.WhiteMana < instance.rdmGauge.BlackMana;
        }
    }
}
