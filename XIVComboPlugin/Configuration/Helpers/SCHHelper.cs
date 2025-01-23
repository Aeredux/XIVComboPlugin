using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lumina.Excel.Sheets;
using XIVComboPlugin.JobActions;

namespace XIVComboTweaks.Configuration.Helpers
{
    internal class SCHHelper : BaseHelper
    {
        private static uint[] moves = { SCH.Resurrection, SCH.Lustrate, SCH.LucidDreaming, SCH.SacredSoil, SCH.Ruin, SCH.Broil, SCH.Physick};
        public static new bool applies(uint move)
        {
            return moves.Contains<uint>(move);
        }

        public static new uint move(uint move)
        {
            switch (move)
            {
                case SCH.Physick:
                    if (instance.level >= 30)
                        return SCH.Adloquium;
                    return SCH.Physick;
                case SCH.Ruin:
                    if (!targetHasDoT())
                        return DoTMove();
                    return RuinMove();
                case SCH.Broil:
                    if (!targetHasDoT())
                        return DoTMove();
                    return BroilMove();
                case SCH.Resurrection:
                    if (Ready(SCH.Swiftcast))
                        return SCH.Swiftcast;
                    return SCH.Resurrection;
                case SCH.Lustrate:
                    if (instance.schGauge.Aetherflow > 0)
                    {
                        if (instance.level >= 62 && Ready(SCH.Excogitation))
                            return SCH.Excogitation;
                        if (instance.level >= 52 && Ready(SCH.Indomitability))
                            return SCH.Indomitability;
                        return SCH.Lustrate;
                    }
                    if (instance.level >= 60 && !Ready(SCH.Aetherflow) && Ready(SCH.Dissipation))
                    {
                        return SCH.Dissipation;
                    }
                    return SCH.Aetherflow;
                case SCH.LucidDreaming:
                    if (hasStatus(SCH.Buffs.Dissipation))
                        return SCH.LucidDreaming;
                    if (instance.level >= 20 && Ready(SCH.WhisperingDawn))
                        return SCH.WhisperingDawn;
                    if (instance.level >= 76 && Ready(SCH.FeyBlessing))
                        return SCH.FeyBlessing;
                    return SCH.LucidDreaming;
                case SCH.SacredSoil:
                    if (instance.level < 50 || instance.schGauge.Aetherflow == 0 && Ready(SCH.FeyIllumination))
                        return SCH.FeyIllumination;
                    return SCH.SacredSoil;
                default:
                    return 0;
            }
        }

        public static bool targetHasDoT()
        {
            if (instance.level >= 72)
                return targetStatus(SCH.Debuffs.Biolysis);
            if (instance.level >= 26)
                return targetStatus(SCH.Debuffs.Bio2);
            return targetStatus(SCH.Debuffs.Bio1);
        }

        private static uint DoTMove()
        {
            if (instance.level >= 72)
                return SCH.Biolysis;
            if (instance.level >= 26)
                return SCH.Bio2;
            return SCH.Bio;
        }

        private static uint RuinMove()
        {
            if (instance.level >= 38)
                return SCH.Ruin2;
            return SCH.Ruin;
        }

        private static uint BroilMove()
        {
            if (instance.level >= 72)
                return SCH.Broil3;
            if (instance.level >= 64)
                return SCH.Broil2;
            return SCH.Ruin;
        }
    }
}
