using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Game.ClientState.JobGauge.Types;
using Lumina.Excel.Sheets;
using XIVComboPlugin.JobActions;

namespace XIVComboTweaks.Configuration.Helpers
{
    internal class BRDHelper : BaseHelper
    {
        private static uint[] moves = { BRD.HeavyShot, BRD.Bloodletter, BRD.QuickNock, BRD.RainOfDeath, BRD.Troubadour, BRD.WanderersMinuet, BRD.RagingStrikes, BRD.MagesBallad, BRD.ArmysPaeon};
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
                    return gcdSingle();
                // 3 charge pitch perfect > 2 charge bloodletter > sidewinder > empyreal arrow > bloodletter
                case BRD.Bloodletter:
                    return instantSingle();
                // wide volley > quick nock
                case BRD.QuickNock:
                    return gcdMulti();
                // 3 charge pitch perfect > 2 charge RainOfDeath > sidewinder > empyreal arrow > RainOfDeath
                case BRD.RainOfDeath:
                    return instantMulti();
                // troubador > nature's minne > second wind
                case BRD.Troubadour:
                    if (instance.level >= 62 && Ready(BRD.Troubadour))
                        return BRD.Troubadour;
                    if (instance.level >= 66 && Ready(BRD.NaturesMinne))
                        return BRD.NaturesMinne;
                    return BRD.SecondWind;
                // Mages ballad > army's paeon > wandering minuet
                case BRD.WanderersMinuet:
                    if (instance.level >= 52 && Ready(BRD.WanderersMinuet))
                        return BRD.WanderersMinuet;
                    if (instance.level <= 30 || Ready(BRD.MagesBallad))
                        return BRD.MagesBallad;
                    if (instance.level >= 40 && Ready(BRD.ArmysPaeon))
                        return BRD.ArmysPaeon;
                    return BRD.WanderersMinuet;
                case BRD.MagesBallad:
                    if (instance.brdGauge.Song == Song.Army)
                        return BRD.MagesBallad;
                    if (instance.brdGauge.Song == Song.Wanderer)
                        return BRD.ArmysPaeon;
                    return BRD.WanderersMinuet;
                case BRD.ArmysPaeon:
                    if (instance.brdGauge.Song == Song.Wanderer)
                        return BRD.WanderersMinuet;
                    if (instance.brdGauge.Song == Song.Mage)
                        return BRD.MagesBallad;
                    return BRD.ArmysPaeon;
                // battle voice > raging strikes > battle voice
                case BRD.RagingStrikes:
                    if (instance.level >= 50 && Ready(BRD.BattleVoice))
                        return BRD.BattleVoice;
                    if (Ready(BRD.RagingStrikes))
                        return BRD.RagingStrikes;
                    if (instance.level >= 90 && Ready(BRD.RadiantFinale) && !instance.brdGauge.Coda.Contains(Dalamud.Game.ClientState.JobGauge.Enums.Song.None))
                        return BRD.RadiantFinale;
                    if (instance.level >= 38 && Ready(BRD.Barrage))
                        return BRD.Barrage;
                    return BRD.RagingStrikes;
                default:
                    break;
            }
            return 0;
        }

        public static uint gcdSingle()
        {
            if (instance.level >= 86 && instance.brdGauge.SoulVoice >= 80 || instance.level >= 80 && instance.brdGauge.SoulVoice == 100)
                return BRD.ApexArrow;
            if (instance.level >= 86 && hasStatus(BRD.Buffs.BlastArrowReady))
                return BRD.BlastArrow;
            if (instance.level >= 96 && hasStatus(BRD.Buffs.ResonantArrowReady))
                return BRD.ResonantArrow;
            if (instance.level >= 100 && hasStatus(BRD.Buffs.RadiantEncoreReady))
                return BRD.RadiantEncore;
            if (hasStatus(BRD.Buffs.Barrage))
                return instance.level < 70 ? BRD.StraightShot : BRD.RefulgentArrow;
            if ((instance.level < 64 && !targetStatus(BRD.Debuffs.VenomousBite)) || instance.level >= 64 && !targetStatus(BRD.Debuffs.CausticBite))
                return instance.level < 64 ? BRD.VenomousBite : BRD.CausticBite;
            if (instance.level >= 30 && (instance.level < 64 && !targetStatus(BRD.Debuffs.Windbite) || instance.level >= 64 && !targetStatus(BRD.Debuffs.Stormbite)))
                return instance.level < 64 ? BRD.Windbite : BRD.Stormbite;
            if (instance.level >= 56)
                if (instance.level < 64 && (targetStatusTime(BRD.Debuffs.VenomousBite) < 4 || instance.level >= 64 && targetStatusTime(BRD.Debuffs.Windbite) < 4))
                    return BRD.IronJaws;
            if (instance.level >= 64 && (targetStatusTime(BRD.Debuffs.CausticBite) < 4 || instance.level >= 64 && targetStatusTime(BRD.Debuffs.Stormbite) < 4))
                return BRD.IronJaws;
            if (hasStatus(BRD.Buffs.HawksEye) || Highlighted(BRD.StraightShot) || Highlighted(BRD.RefulgentArrow))
                return original(BRD.StraightShot);
            return original(BRD.HeavyShot);
        }

        public static uint gcdMulti()
        {
            uint single = gcdSingle();
            if (single == BRD.ApexArrow || single == BRD.BlastArrow || single == BRD.ResonantArrow || single == BRD.RadiantEncore)
                return single;
            if (single == BRD.StraightShot || single == BRD.RefulgentArrow)
                return original(BRD.WideVolley);
            return original(BRD.QuickNock);
        }

        public static uint instantSingle()
        {
            if (instance.brdGauge.Song == Dalamud.Game.ClientState.JobGauge.Enums.Song.Wanderer && instance.brdGauge.Repertoire == 3)
                return BRD.PitchPerfect;
            if (instance.level < 84 && Charges(BRD.Bloodletter) == 2 || instance.level >= 84 && Charges(BRD.Bloodletter) == 3 ||
                Charges(BRD.Bloodletter) > 0 && instance.level >= 54 && instance.level < 84 && Ready(BRD.EmpyrealArrow) && instance.brdGauge.Song == Dalamud.Game.ClientState.JobGauge.Enums.Song.Mage ||
                Charges(BRD.Bloodletter) > 1 && instance.level >= 84 && Ready(BRD.EmpyrealArrow) && instance.brdGauge.Song == Dalamud.Game.ClientState.JobGauge.Enums.Song.Mage)
                return original(BRD.Bloodletter);
            if (instance.level >= 54 && Ready(BRD.EmpyrealArrow))
                return BRD.EmpyrealArrow;
            if (instance.level >= 60 && Ready(BRD.Sidewinder))
                return BRD.Sidewinder;
            if (instance.level >= 52 && instance.brdGauge.Song == Dalamud.Game.ClientState.JobGauge.Enums.Song.Wanderer && Charges(BRD.Bloodletter) == 0 && instance.brdGauge.SongTimer < 5000)
                return BRD.PitchPerfect;
            return original(BRD.Bloodletter);
        }

        public static uint instantMulti()
        {
            uint single = instantSingle();
            if (single == BRD.Bloodletter || single == BRD.HeartbreakShot)
                return instance.level >= 45 ? BRD.RainOfDeath : single;
            return single;
        }
    }
}
