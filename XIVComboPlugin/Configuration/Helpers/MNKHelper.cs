using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Statuses;
using Lumina.Excel.Sheets;
using XIVComboPlugin.JobActions;

namespace XIVComboTweaks.Configuration.Helpers
{
    internal class MNKHelper
    {
        private static MNKHelper instance;
        private MNKGauge g;
        private StatusList s;
        private int level;
        private uint lastMove;

        public static MNKHelper Get(MNKGauge mnkGauge, StatusList statusList, int l, uint lm)
        {
            if (instance == null)
                instance = new MNKHelper();

            instance.g = mnkGauge;
            instance.s = statusList;
            instance.level = l;
            instance.lastMove = lm;
            return instance;
        }

        public uint SingleTarget()
        {
            if (hasStatus(MNK.PerfectBalanceBuff))
            {
                if (g.Nadi == Nadi.SOLAR)
                {
                    return MoveOpo();
                } else
                {
                    return MoveBasedOnPrevious();
                }
            } else
            {
                if (hasStatus(MNK.FormlessFist))
                {
                    return MoveOpo();
                } else
                {
                    return MoveForm();
                }
            }
        }

        public uint MultiTarget()
        {
            uint st = SingleTarget();
            if (st == MNK.Bootshine || st == MNK.LeapingOpo || st == MNK.DragonKick)
                return MNK.ArmOfTheDestroyer;
            if (st == MNK.TwinSnakes || st == MNK.TrueStrike || st == MNK.RisingRaptor)
                return MNK.FourPointFury;
            return MNK.Rockbreaker;
        }
        private uint MoveForm()
        {
            if (hasStatus(MNK.RaptorForm))
            {
                if (g.RaptorFury < 1 && level >= 18)
                    return MNK.TwinSnakes;
                return MNK.TrueStrike;
            }
            else if (hasStatus(MNK.CoeurlForm))
            {
                if (g.CoeurlFury < 1 && level >= 30)
                    return MNK.Demolish;
                return MNK.SnapPunch;
            }
            else
            {
                if (g.OpoOpoFury < 1 && level >= 50)
                    return MNK.DragonKick;
                return MNK.Bootshine;
            }
        }

        private uint MoveOpo()
        {
            if (g.OpoOpoFury < 1 && level >= 50)
                return MNK.DragonKick;
            return MNK.Bootshine;
        }

        private uint MoveBasedOnPrevious()
        {
            if (lastMove == MNK.DragonKick || lastMove == MNK.Bootshine || lastMove == MNK.LeapingOpo)
            {
                if (g.RaptorFury < 1 && level >= 18)
                    return MNK.TwinSnakes;
                return MNK.TrueStrike;
            }
            else if (lastMove == MNK.TwinSnakes || lastMove == MNK.TrueStrike || lastMove == MNK.RisingRaptor)
            {
                if (g.CoeurlFury < 1 && level >= 30)
                    return MNK.Demolish;
                return MNK.SnapPunch;
            }
            else
            {
                if (g.OpoOpoFury < 1 && level >= 50)
                    return MNK.DragonKick;
                return MNK.Bootshine;
            }
        }

        private bool hasStatus(ushort x)
        {
            for (var i = 0; i < s.Length; i++)
                if (s[i].StatusId == x)
                    return true;
            return false;
        }
    }
}
