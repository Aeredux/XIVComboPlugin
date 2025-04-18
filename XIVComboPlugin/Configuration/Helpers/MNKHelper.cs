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
    internal unsafe class MNKHelper : BaseHelper
    {
        private static uint[] moves = { MNK.Bootshine, MNK.ArmOfTheDestroyer, MNK.RiddleOfFire, MNK.RiddleOfEarth, MNK.Mantra, MNK.InspiredMeditaion, MNK.MasterfulBlitz, MNK.PerfectBalance};
        public static new bool applies(uint move)
        {
            return moves.Contains<uint>(move);
        }

        public static new uint move(uint move)
        {
            switch (move)
            {
                case MNK.Bootshine:
                    return SingleTarget();
                case MNK.ArmOfTheDestroyer:
                    return MultiTarget();
                case MNK.RiddleOfFire:
                    if (instance.level >= 70 && Ready(MNK.Brotherhood))
                        return MNK.Brotherhood; 
                    return original(MNK.RiddleOfFire);
                case MNK.RiddleOfEarth:
                    if (instance.level < 64)
                        return BRD.SecondWind;
                    if (Ready(MNK.RiddleOfEarth))
                        return MNK.RiddleOfEarth;
                    if (Highlighted(MNK.EarthsReply))
                        return MNK.EarthsReply;
                    if (Ready(BRD.SecondWind))
                        return BRD.SecondWind;
                    return MNK.RiddleOfEarth;
                case MNK.PerfectBalance:
                    if (!instance.mnkGauge.BeastChakra.Contains(BeastChakra.None))
                        return original(MNK.MasterfulBlitz);
                    return MNK.PerfectBalance;
                case MNK.Mantra:
                    if (instance.level >= 42 && Ready(MNK.Mantra))
                        return MNK.Mantra;
                    if (Ready(MNK.Bloodbath))
                        return MNK.Bloodbath;
                    return instance.level >= 42 ? MNK.Mantra : MNK.Bloodbath;
                case MNK.InspiredMeditaion:
                    return instance.level >= 40 ? original(MNK.InspiredMeditaion) : original(MNK.Meditation);
                case MNK.MasterfulBlitz:
                    return MNK.PerfectBalance;
                default:
                    return 0;
            }
        }

        public static uint SingleTarget()// add in opo move after formless fist, leave option to early masterful blitz on E. same for multi
        {
            if (instance.level >= 52 && (hasStatus(MNK.Buffs.FormlessFist) || hasStatus(MNK.Buffs.OpoOpoForm)))
                return MoveOpo();
            if (instance.level >= 100 && hasStatus(MNK.Buffs.RaptorForm) && hasStatus(MNK.Buffs.FiresRumination))
                return MNK.FiresReply;
            if (instance.level >= 96  && hasStatus(MNK.Buffs.WindsRumination))
                return MNK.WindsReply;
            if (!instance.mnkGauge.BeastChakra.Contains(BeastChakra.None))
                return original(MNK.MasterfulBlitz);
            if (hasStatus(MNK.Buffs.PerfectBalance))
            {
                if (instance.mnkGauge.Nadi == Nadi.Solar)
                {
                    return MoveOpo();
                } else
                {
                    return FillUniqueChakra();//haseffect solar for even minute phantom rush?
                }
            } else
            {
                return MoveForm();
            }
        }

        public static uint MultiTarget()
        {
            uint st = SingleTarget();
            if (st == MNK.FiresReply) return MNK.FiresReply;
            if (st == MNK.WindsReply) return MNK.WindsReply;
            if (!instance.mnkGauge.BeastChakra.Contains(BeastChakra.None))
                return original(MNK.MasterfulBlitz);
            if (st == MNK.Bootshine || st == MNK.LeapingOpo || st == MNK.DragonKick)
                return instance.level >= 82 ? MNK.ShadowOfTheDestroyer : instance.level >= 26? MNK.ArmOfTheDestroyer : st;
            if (st == MNK.TwinSnakes || st == MNK.TrueStrike || st == MNK.RisingRaptor)
                return instance.level >= 45 ? MNK.FourPointFury : st;
            return instance.level >= 30 ? MNK.Rockbreaker : st;
        }
        private static uint MoveForm()
        {
            if (hasStatus(MNK.Buffs.RaptorForm))
            {
                if (instance.mnkGauge.RaptorFury < 1 && instance.level >= 18)
                    return original(MNK.TwinSnakes);
                return original(MNK.TrueStrike);
            }
            else if (hasStatus(MNK.Buffs.CoeurlForm))
            {
                if (instance.mnkGauge.CoeurlFury < 1 && instance.level >= 30)
                    return original(MNK.Demolish);
                return original(MNK.SnapPunch);
            }
            else
            {
                if (instance.mnkGauge.OpoOpoFury < 1 && instance.level >= 50)
                    return original(MNK.DragonKick);
                return original(MNK.Bootshine);
            }
        }

        private static uint MoveOpo()
        {
            if (instance.mnkGauge.OpoOpoFury < 1 && instance.level >= 50)
                return original(MNK.DragonKick);
            return original(MNK.Bootshine);
        }
        private static uint MoveRaptor()
        {
            if (instance.mnkGauge.RaptorFury < 1 && instance.level >= 18)
                return original(MNK.TwinSnakes);
            return original(MNK.TrueStrike);
        }

        private static uint MoveCoeurl()
        {
            if (instance.mnkGauge.RaptorFury < 1 && instance.level >= 30)
                return original(MNK.Demolish);
            return original(MNK.SnapPunch);
        }

        private static uint FillUniqueChakra()
        {
            if (!instance.mnkGauge.BeastChakra.Contains(BeastChakra.OpoOpo))
                return MoveOpo();
            if (!instance.mnkGauge.BeastChakra.Contains(BeastChakra.Raptor))
                return MoveRaptor();
            if (!instance.mnkGauge.BeastChakra.Contains(BeastChakra.Coeurl))
                return MoveCoeurl();
            return MoveOpo();
        }
    }
}
