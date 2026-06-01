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
    internal unsafe class GNBHelper : BaseHelper
    {
        private static uint[] moves = { GNB.KeenEdge, GNB.DemonSlice, GNB.NoMercy, GNB.HeartOfCorundum};
        public static new bool applies(uint move)
        {
            return moves.Contains<uint>(move);
        }

        public static new uint move(uint move)
        {
            switch (move)
            {
                case GNB.KeenEdge:
                    return SingleTarget();
                case GNB.DemonSlice:
                    return MultiTarget();
                case GNB.HeartOfCorundum:
                    if (instance.level >= 68 && Ready(GNB.HeartOfStone))
                        return original(GNB.HeartOfStone);
                    if (Ready(GNB.Camouflage))
                        return GNB.Camouflage;
                    if (Ready(GNB.Rampart))
                        return GNB.Rampart;
                    if (instance.level >= 38 && Ready(GNB.Nebula))
                        return original(GNB.Nebula);
                    return instance.level >= 68 ? original(GNB.HeartOfStone) : GNB.Camouflage;
                case GNB.NoMercy:
                    return Instant();
                default:
                    return 0;
            }
        }

        public static uint Instant()
        {
            if (Ready(GNB.NoMercy))
                return GNB.NoMercy;
            if (instance.level >= 70 && Highlighted(original(GNB.Continuation)))
                return original(GNB.Continuation);
            if (instance.level >= 30 && Ready(GNB.DangerZone))
                return GNB.DangerZone;
            if (instance.level >= 62 && Ready(GNB.BowShock))
                return GNB.BowShock;
            return GNB.NoMercy;
        }

        public static uint SingleTarget()
        {
            if (instance.level >= 54 && Highlighted(GNB.SonicBreak))
                return GNB.SonicBreak;
            if (instance.level >= 60)
            {
                if (Highlighted(original(GNB.GnashingFang)) && Ready(original(GNB.GnashingFang)))
                {
                    return original(GNB.GnashingFang);
                }
            }
            if (instance.gnbGauge.Ammo > 0)
                return GNB.BurstStrike;
            if (instance.level >= 26 && Highlighted(GNB.SolidBarrel))
                return GNB.SolidBarrel;
            if (instance.level >= 4 && Highlighted(GNB.BrutalShell))
                return GNB.BrutalShell;
            return GNB.KeenEdge;
        }

        public static uint MultiTarget()
        {
            //if (instance.level >= 54 && Highlighted(GNB.SonicBreak))
            //    return GNB.SonicBreak;
            //if (instance.level >= 60)
            //{
            //    if (Highlighted(GNB.GnashingFang) && Ready(GNB.GnashingFang))
            //    {
            //        return GNB.GnashingFang;
            //    }
            //    if (Highlighted(GNB.SavageClaw))
            //    {
            //        return GNB.SavageClaw;
            //    }
            //    if (Highlighted(GNB.WickedTalon))
            //    {
            //        return GNB.WickedTalon;
            //    }
            //}
            if (instance.level >= 40 && Highlighted(GNB.DemonSlaughter))
                return GNB.DemonSlaughter;
            return GNB.DemonSlice;
        }
    }
}
