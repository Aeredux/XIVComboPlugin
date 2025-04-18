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
    internal unsafe class PLDHelper : BaseHelper
    {
        private static uint[] moves = { PLD.FastBlade, PLD.TotalEclipse, PLD.FightOrFlight, PLD.Sentinel};
        public static new bool applies(uint move)
        {
            return moves.Contains<uint>(move);
        }

        public static new uint move(uint move)
        {
            switch (move)
            {
                case PLD.FastBlade:
                    return SingleTarget();
                case PLD.TotalEclipse:
                    return MultiTarget();
                case PLD.FightOrFlight:
                    if (Ready(PLD.FightOrFlight))
                        return PLD.FightOrFlight;
                    if (instance.level >= 30 && Ready(PLD.SpiritsWithin))
                        return PLD.SpiritsWithin;
                    if (instance.level >= 50 && !Ready(PLD.SpiritsWithin) && Ready(PLD.CircleOfScorn))
                        return PLD.CircleOfScorn;
                    return PLD.FightOrFlight;
                case PLD.Sentinel:
                    if (Ready(DRK.Rampart))
                        return DRK.Rampart;
                    if (instance.level >= 52 && Ready(PLD.Bulwark))
                        return PLD.Bulwark;
                    if (instance.level >= 38 && Ready(PLD.Sentinel))
                        return PLD.Sentinel;
                    return DRK.Rampart;
                default:
                    return 0;
            }
        }

        public static uint SingleTarget()
        {
            if (instance.level >= 54 && hasStatus(PLD.Buffs.GoringBladeReady))
                return PLD.GoringBlade;
            if (instance.level >= 26 && Highlighted(PLD.RageOfHalone))
                return PLD.RageOfHalone;
            if (Highlighted(PLD.RiotBlade))
                return PLD.RiotBlade;
            return PLD.FastBlade;
        }

        public static uint MultiTarget()
        {
            if (instance.level >= 54 && hasStatus(PLD.Buffs.GoringBladeReady))
                return PLD.GoringBlade;
            if (instance.level >= 40 && Highlighted(PLD.Prominence))
                return PLD.Prominence;
            return PLD.TotalEclipse;
        }
    }
}
