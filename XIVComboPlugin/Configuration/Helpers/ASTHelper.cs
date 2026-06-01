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
    internal unsafe class ASTHelper : BaseHelper
    {
        private static uint[] moves = { AST.Malefic2, AST.CelestialOpposition, AST.Ascend, AST.AstralDraw, AST.EssentialDignity, AST.Combust2, AST.Benefic, AST.Helios};
        public static new bool applies(uint move)
        {
            return moves.Contains<uint>(move);
        }

        public static new uint move(uint move)
        {
            switch (move)
            {
                case AST.Malefic2:
                    if (hasDoT())
                        return original(AST.Malefic);
                    return original(AST.Combust);
                case AST.CelestialOpposition:
                    if (instance.level >= 60 && Ready(AST.CelestialOpposition))
                        return AST.CelestialOpposition;
                    if (Ready(SCH.LucidDreaming))
                        return SCH.LucidDreaming;
                    if (instance.level >= 76 && Ready(AST.Horoscope) && !hasStatus(AST.Buffs.HoroscopeHelios) && !hasStatus(AST.Buffs.Horoscope))
                        return AST.Horoscope;
                    if (instance.level >= 76 && hasStatus(AST.Buffs.Horoscope))
                        return AST.AspectedHelios;
                    if (instance.level >= 76 && hasStatus(AST.Buffs.HoroscopeHelios))
                        return AST.Horoscope2;
                    return instance.level >= 60 ? AST.CelestialOpposition : SCH.LucidDreaming;
                case AST.Ascend:
                    if (Ready(SCH.Swiftcast))
                        return SCH.Swiftcast;
                    return AST.Ascend;
                case AST.AstralDraw:
                    return cardDraw();
                case AST.EssentialDignity:
                    return instantSingleHeals();
                case AST.Combust2:
                    return original(AST.AstralDraw);
                case AST.Benefic:
                    if (instance.level < 26)
                        return AST.Benefic;
                    if (instance.level >= 34 && ((targetIsPlayer() && !targetStatus(AST.Buffs.AspectedBenefic)) || (!targetIsPlayer() && !hasStatus(AST.Buffs.AspectedBenefic))))
                        return AST.AspectedBenefic;
                    return AST.Benefic2;
                case AST.Helios:
                    if (instance.level >= 40 && !hasStatus(AST.Buffs.AspectedHelios))
                        return AST.AspectedHelios;
                    return AST.Helios;
                default:
                    return 0;
            }
        }

        public static bool hasDoT()
        {
            return (instance.level < 46 && targetStatus(AST.Debuffs.Combust)) || 
                instance.level < 72 && targetStatus(AST.Debuffs.Combust2) 
                || instance.level >= 72 && targetStatus(AST.Debuffs.Combust3);
        }

        public static uint instantSingleHeals()
        {
            if (instance.level >= 30) {
                if (instance.astGauge.DrawnCards.Contains(CardType.Arrow))
                    return AST.Arrow;
                if (instance.astGauge.DrawnCards.Contains(CardType.Spire))
                    return AST.Spire;
                if (instance.astGauge.DrawnCards.Contains(CardType.Bole))
                    return AST.Bole;
                if (instance.astGauge.DrawnCards.Contains(CardType.Ewer))
                    return AST.Ewer;
            }
            if (instance.level >= 74 && Ready(AST.CelestialIntersection))
                return AST.CelestialIntersection;

            return AST.EssentialDignity;
        }

        public static uint cardDraw()
        {
            bool canPlay1 = hasDamageBoostCard();
            if (canPlay1)
            {
                if (Ready(AST.Lightspeed) && !hasStatus(AST.Buffs.Lightspeed))
                    return AST.Lightspeed;
                if (instance.level >= 50 && Ready(AST.Divination))
                    return AST.Divination;
                return original(AST.Play1);
            }
            if (instance.level >= 70 && !instance.astGauge.DrawnCrownCard.Equals(CardType.None))
                return original(AST.MinorArcana);
            if (Ready(AST.Lightspeed) && !hasStatus(AST.Buffs.Lightspeed) && Ready(original(AST.AstralDraw)))
                return original(AST.Lightspeed);
            return original(AST.AstralDraw);
        }

        public static bool hasDamageBoostCard()
        {
            return instance.level >= 30 && (original(AST.Play1) != AST.Play1);
        }
    }
}
