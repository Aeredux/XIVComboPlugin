using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FFXIVClientStructs.FFXIV.Client.Game;
using Lumina.Excel.Sheets;
using XIVComboPlugin.JobActions;

namespace XIVComboTweaks.Configuration.Helpers
{
    internal class DRKHelper : BaseHelper
    {
        private static uint[] moves = { DRK.DarkMind, DRK.HardSlash, DRK.EdgeofDarkness, DRK.Unleash, DRK.FloodofDarkness };
        public static new bool applies(uint move)
        {
            return moves.Contains<uint>(move);
        }

        public static new uint move(uint move)
        {
            switch (move)
            {
                case DRK.DarkMind:
                    if (instance.level >= 45 && Ready(DRK.DarkMind))
                    {
                        return DRK.DarkMind;
                    }
                    if (instance.level >=  82 && Charges(DRK.Oblation) == 2)
                    {
                        return DRK.Oblation;
                    }
                    if (Ready(DRK.Rampart))
                    {
                        return DRK.Rampart;
                    }
                    if (instance.level >= 66 && Ready(DRK.DarkMissionary))
                    {
                        return DRK.DarkMissionary;
                    }
                    if (instance.level >= 38 && Ready(DRK.ShadowWall))
                    {
                        return instance.level >= 92 ? DRK.ShadowedVigil : DRK.ShadowWall;
                    }
                    return instance.level >= 45 ? DRK.DarkMind : DRK.Rampart;
                case DRK.HardSlash:
                    return SingleCombo();
                case DRK.Unleash:
                    return MultiCombo();
                case DRK.EdgeofDarkness:
                    return SingleInstant();
                case DRK.FloodofDarkness:
                    return MultiInstant();
                default:
                    break;
            }
            return 0;
        }

        public static uint SingleInstant()
        {
            int level = instance.level;
            if (level >= 80 && Ready(DRK.LivingShadow))
                return DRK.LivingShadow;
            if (instance.mp >= 10000 || instance.dRKGauge.DarksideTimeRemaining == 0)
                return SingleEdge();
            if (level >= 90 && Charges(DRK.Shadowbringer) == 2)
                return DRK.Shadowbringer;
            if (instance.level >= 35 && Ready(DRK.BloodWeapon))
                return level >= 68 ? DRK.Delirium : DRK.BloodWeapon;
            if (level >= 56 && Ready(DRK.AbyssalDrain))
                return level >= 60 ? DRK.CarveAndSpit : DRK.AbyssalDrain;
            if (instance.mp >= 7000)
                return SingleEdge();
            if (level >= 90 && Charges(DRK.Shadowbringer) > 0)
                return DRK.Shadowbringer;
            return SingleEdge();
        }

        public static uint MultiInstant()
        {
            int level = instance.level;
            if (level >= 80 && Ready(DRK.LivingShadow))
                return DRK.LivingShadow;
            if (instance.mp >= 10000 || instance.dRKGauge.DarksideTimeRemaining == 0)
                return MultiEdge();
            if (level >= 90 && Charges(DRK.Shadowbringer) == 2)
                return DRK.Shadowbringer;
            if (instance.level >= 35 && Ready(DRK.BloodWeapon))
                return level >= 68 ? DRK.Delirium : DRK.BloodWeapon;
            if (level >= 56 && Ready(DRK.AbyssalDrain))
                return DRK.AbyssalDrain;
            if (instance.mp >= 7000)
                return MultiEdge();
            if (level >= 90 && Charges(DRK.Shadowbringer) > 0)
                return DRK.Shadowbringer;
            return MultiEdge();
        }

        public static uint SingleEdge()
        {
            if (instance.level >= 74)
                return DRK.EdgeofShadow;
            if (instance.level >= 40)
                return DRK.EdgeofDarkness;
            return DRK.FloodofDarkness;
        }

        public static uint MultiEdge()
        {
            if (instance.level >= 74)
                return DRK.FloodofShadow;
            return DRK.FloodofDarkness;
        }

        public static uint SingleCombo()
        {
            if (instance.level >= 100 && hasStatus(DRK.Buffs.Scorn))
                return DRK.Disesteem;
            if (instance.dRKGauge.Blood >= 50 || instance.level < 96 && hasStatus(DRK.Buffs.Delirium) || instance.level >= 96 && hasStatus(DRK.Buffs.EnhancedDelirium))
                return (uint)(instance.level >= 96 ? instance.iconHook.Original(instance.self, DRK.Bloodspiller) : DRK.Bloodspiller);
            if (instance.lastMove == DRK.HardSlash && instance.level >= 2)
                return DRK.SyphonStrike;
            if (instance.lastMove == DRK.SyphonStrike && instance.level >= 26)
                return DRK.Souleater;
            return DRK.HardSlash;
        }

        public static uint MultiCombo()
        {
            if (instance.level >= 100 && hasStatus(DRK.Buffs.Scorn))
                return DRK.Disesteem;
            if (instance.dRKGauge.Blood >= 50 || instance.level < 96 && hasStatus(DRK.Buffs.Delirium) || instance.level >= 96 && hasStatus(DRK.Buffs.EnhancedDelirium))
                return (uint)(instance.level >= 96 ? instance.iconHook.Original(instance.self, DRK.Quietus) : instance.level >= 64? DRK.Quietus : DRK.Bloodspiller);
            if (instance.lastMove == DRK.Unleash && instance.level >= 40)
                return DRK.StalwartSoul;
            return DRK.Unleash;
        }
    }
}
