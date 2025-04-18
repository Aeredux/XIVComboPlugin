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
        private static uint[] moves = { SCH.Resurrection, SCH.Lustrate, SCH.WhisperingDawn, SCH.SacredSoil, SCH.Ruin, SCH.Ruin2, SCH.Physick, SCH.Recitation, SCH.SummonSeraph, SCH.Aetherpact, SCH.Broil, SCH.Broil2};
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
                case SCH.Ruin2:
                    if (!targetHasDoT())
                        return DoTMove();
                    return SCH.Ruin2;
                case SCH.Resurrection:
                    if (Ready(SCH.Swiftcast))
                        return SCH.Swiftcast;
                    return SCH.Resurrection;
                case SCH.SummonSeraph:
                    if (instance.level >= 80 && (Ready(SCH.SummonSeraph) || instance.schGauge.SeraphTimer > 0))
                        return original(SCH.SummonSeraph);
                    return original(SCH.Aetherpact);
                case SCH.Aetherpact:
                    if (!Ready(SCH.SummonSeraph))
                        return SCH.SummonSeraph;
                    return original(SCH.Aetherpact);
                case SCH.Recitation:
                    if (instance.level >= 74 && Ready(SCH.Recitation))
                        return SCH.Recitation;
                    if (instance.level >= 86 && Ready(SCH.Protraction))
                        return SCH.Protraction;
                    if (Ready(SCH.DeploymentTactics))
                        return SCH.DeploymentTactics;
                    return instance.level >= 74 ? SCH.Recitation : SCH.DeploymentTactics;
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
                case SCH.WhisperingDawn:
                    if (hasStatus(SCH.Buffs.Dissipation))
                        return SCH.LucidDreaming;
                    if (instance.level >= 40 && Ready(SCH.FeyIllumination))
                        return SCH.FeyIllumination;
                    if (instance.level >= 20 && Ready(SCH.WhisperingDawn))
                        return SCH.WhisperingDawn;
                    if (instance.level >= 76 && Ready(SCH.FeyBlessing))
                        return SCH.FeyBlessing;
                    return SCH.LucidDreaming;
                case SCH.SacredSoil:
                    if (Ready(SCH.SacredSoil) && instance.schGauge.Aetherflow > 0)
                        return SCH.SacredSoil;
                    if (instance.level >= 90 && Ready(SCH.Expedient))
                        return SCH.Expedient;
                    return SCH.SacredSoil;
                case SCH.Broil:
                    return SCH.WhisperingDawn;
                case SCH.Broil2:
                    return SCH.Recitation;
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
            return original(SCH.Bio);
        }

        private static uint RuinMove()
        {
            return original(SCH.Ruin);
        }
    }
}
