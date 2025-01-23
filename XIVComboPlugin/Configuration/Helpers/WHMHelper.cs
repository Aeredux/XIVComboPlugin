using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dalamud.Plugin.Services;
using Lumina.Excel.Sheets;
using XIVComboPlugin.JobActions;

namespace XIVComboTweaks.Configuration.Helpers
{
    internal class WHMHelper : BaseHelper
    {
        private static uint[] moves = { WHM.Cure, WHM.Medica1, WHM.Stone1, WHM.Raise, WHM.Assize, WHM.ThinAir};
        public static new bool applies(uint move)
        {
            return moves.Contains<uint>(move);
        }

        public static new uint move(uint move)
        {
            switch (move)
            {
                case WHM.Cure:
                    if (instance.level >= 35)
                    {
                        if (!hasTarget())
                        {
                            if (!hasStatus(WHM.Buffs.Regen))
                                return WHM.Regen;
                        }
                        else if (!targetStatus(WHM.Buffs.Regen))
                            return WHM.Regen;
                    }
                    if (instance.level >= 52 && instance.whmGauge.Lily >= 3)
                        return WHM.AfflatusSolace;
                    if (instance.level >= 52 && instance.whmGauge.Lily > 0 && instance.mp < 1000)
                        return WHM.AfflatusSolace;
                    if (instance.level >= 30)
                        return WHM.Cure2;
                    return WHM.Cure;
                case WHM.Medica1:
                    if (instance.level >= 50)
                        return WHM.Medica2;
                    return WHM.Medica1;
                case WHM.Stone1:
                    if (instance.level < 46 && !targetStatus(WHM.Debuffs.Aero) || !targetStatus(WHM.Debuffs.Aero2))
                        return instance.level < 46 ? WHM.Aero : WHM.Aero2;
                    if (instance.level >= 64)
                        return WHM.Stone4;
                    if (instance.level >= 54)
                        return WHM.Stone3;
                    if (instance.level >= 18)
                        return WHM.Stone2;
                    return WHM.Stone1;
                case WHM.Raise:
                    if (instance.level >= 18 && Ready(SCH.Swiftcast))
                        return SCH.Swiftcast;
                    return WHM.Raise;
                case WHM.Assize:
                    if (instance.level >= 52 && Ready(WHM.Assize))
                        return WHM.Assize;
                    if (instance.level >= 60 && Ready(WHM.Tetragrammaton))
                        return WHM.Tetragrammaton;
                    if (Ready(WHM.Benediction))
                        return WHM.Benediction;
                    return WHM.Assize;
                case WHM.ThinAir:
                    if (instance.level >= 58 && Charges(WHM.ThinAir) == 2)
                        return WHM.ThinAir;
                    if (Ready(SCH.LucidDreaming))
                        return SCH.LucidDreaming;
                    if (instance.level >= 58)
                        return WHM.ThinAir;
                    return SCH.LucidDreaming;
                default:
                    break;
            }

            return 0;
        }
    }
}
