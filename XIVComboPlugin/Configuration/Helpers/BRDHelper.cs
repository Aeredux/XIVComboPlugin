using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lumina.Excel.Sheets;
using XIVComboPlugin.JobActions;

namespace XIVComboTweaks.Configuration.Helpers
{
    internal class BRDHelper : BaseHelper
    {
        private static uint[] moves = { BRD.HeavyShot, BRD.Bloodletter, BRD.QuickNock, BRD.RainOfDeath, BRD.SecondWind, BRD.WanderersMinuet, BRD.RagingStrikes };
        public static new bool applies(uint move)
        {
            return moves.Contains<uint>(move);
        }

        public static new uint move(uint move)
        {
            switch (move)
            {
                // caustic bite/venemous bite > stormbite/windbite > iron jaws > straight shot > heavy shot
                case BRD.HeavyShot:
                    if (hasStatus(BRD.Buffs.Barrage))
                        return instance.level < 70 ? BRD.StraightShot : BRD.RefulgentArrow;
                    if ((instance.level < 64 && !targetStatus(BRD.Debuffs.VenomousBite)) || instance.level >= 64 && !targetStatus(BRD.Debuffs.CausticBite))
                        return instance.level < 64 ? BRD.VenomousBite : BRD.CausticBite;
                    if (instance.level < 64 && !targetStatus(BRD.Debuffs.Windbite) || instance.level >= 64 && !targetStatus(BRD.Debuffs.Stormbite))
                        return instance.level < 64 ? BRD.Windbite : BRD.Stormbite;
                    if (instance.level >= 56)
                        if (instance.level < 64 && (targetStatusTime(BRD.Debuffs.VenomousBite) < 4 || instance.level >= 64 && targetStatusTime(BRD.Debuffs.Windbite) < 4))
                            return BRD.IronJaws;
                        if (instance.level >= 64 && (targetStatusTime(BRD.Debuffs.CausticBite) < 4 || instance.level >= 64 && targetStatusTime(BRD.Debuffs.Stormbite) < 4))
                            return BRD.IronJaws;
                    if (hasStatus(BRD.Buffs.HawksEye) || Highlighted(BRD.StraightShot) || Highlighted(BRD.RefulgentArrow))
                        return instance.level < 70 ? BRD.StraightShot : BRD.RefulgentArrow;
                    return BRD.HeavyShot;
                // 3 charge pitch perfect > 2 charge bloodletter > sidewinder > empyreal arrow > bloodletter
                case BRD.Bloodletter:
                    if (instance.brdGauge.Song == Dalamud.Game.ClientState.JobGauge.Enums.Song.WANDERER && instance.brdGauge.Repertoire == 3)
                        return BRD.PitchPerfect;
                    if (Charges(BRD.Bloodletter) == 2)
                        return BRD.Bloodletter;
                    if (instance.level >= 60 && Ready(BRD.Sidewinder))
                        return BRD.Sidewinder;
                    if (instance.level >= 54 && Ready(BRD.EmpyrealArrow))
                        return BRD.EmpyrealArrow;
                    return BRD.Bloodletter;
                // wide volley > quick nock
                case BRD.QuickNock:
                    if (Highlighted(BRD.WideVolley))
                        return BRD.WideVolley;
                    return BRD.QuickNock;
                // 3 charge pitch perfect > 2 charge RainOfDeath > sidewinder > empyreal arrow > RainOfDeath
                case BRD.RainOfDeath:
                    if (instance.brdGauge.Song == Dalamud.Game.ClientState.JobGauge.Enums.Song.WANDERER && instance.brdGauge.Repertoire == 3)
                        return BRD.PitchPerfect;
                    if (Charges(BRD.RainOfDeath) == 2)
                        return BRD.RainOfDeath;
                    if (instance.level >= 60 && Ready(BRD.Sidewinder))
                        return BRD.Sidewinder;
                    if (instance.level >= 54 && Ready(BRD.EmpyrealArrow))
                        return BRD.EmpyrealArrow;
                    return BRD.RainOfDeath;
                // troubador > nature's minne > second wind
                case BRD.SecondWind:
                    if (instance.level >= 62 && Ready(BRD.Troubadour))
                        return BRD.Troubadour;
                    if (instance.level >= 66 && Ready(BRD.NaturesMinne))
                        return BRD.NaturesMinne;
                    return BRD.SecondWind;
                // Mages ballad > army's paeon > wandering minuet
                case BRD.WanderersMinuet:
                    if (Ready(BRD.MagesBallad))
                        return BRD.MagesBallad;
                    if (instance.level >= 40 && Ready(BRD.ArmysPaeon))
                        return BRD.ArmysPaeon;
                    if (instance.level >= 52)
                        return BRD.WanderersMinuet;
                    return BRD.MagesBallad;
                // battle voice > raging strikes > battle voice
                case BRD.RagingStrikes:
                    if (instance.level >= 50 && Ready(BRD.BattleVoice))
                        return BRD.BattleVoice;
                    if (Ready(BRD.RagingStrikes))
                        return BRD.RagingStrikes;
                    if (instance.level >= 38 && Ready(BRD.Barrage))
                        return BRD.Barrage;
                    return BRD.RagingStrikes;
                default:
                    break;
            }

            return 0;
        }
    }
}
