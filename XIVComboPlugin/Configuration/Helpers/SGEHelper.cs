using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Statuses;
using FFXIVClientStructs.FFXIV.Client.Game;
using Lumina.Excel.Sheets;
using XIVComboPlugin.JobActions;

namespace XIVComboTweaks.Configuration.Helpers
{
    // swiftcast -> egeiro
    // eukrasian dosis -> toxikon -> dosis
    // zoe -> haima  // actually this doesn't stack
    // phlegma -> dykrasia
    // taurochole -> ixochole -> druochole
    // physis ii -> lucid dreaming
    internal unsafe class SGEHelper
    {
        private static SGEHelper instance;
        private SGEGauge g;
        private StatusList s;
        private int level;
        private uint lastMove;
        private ActionManager* actionManager;

        public static unsafe SGEHelper Get(SGEGauge SGEgauge, StatusList statusList, int l, uint lm)
        {
            if (instance == null)
                instance = new SGEHelper();

            instance.g = SGEgauge;
            instance.s = statusList;
            instance.level = l;
            instance.lastMove = lm;
            instance.actionManager = ActionManager.Instance();
            return instance;
        }

        public bool applies(uint move)
        {
            return move == SGE.Egeiro || move == SGE.Dosis || move == SGE.Dyskrasia || move == SGE.Druochole || move == SGE.LucidDreaming;
        }

        public uint move(uint move)
        {
            switch (move)
            {
                case SGE.Egeiro:
                    if (!Ready(SGE.Swiftcast))
                        return SGE.Egeiro;
                    return SGE.Swiftcast;
                case SGE.Dosis:
                    if (level >= 30 && g.Eukrasia)
                    {
                        if (level >= 72)
                            return SGE.EukrasianDosis2;
                        return SGE.EukrasianDosis;
                    }
                    if (level >= 66 && g.Addersting > 0)
                        return SGE.Toxikon;
                    if (level >= 72)
                        return SGE.Dosis2;
                    return SGE.Dosis;
                case SGE.LucidDreaming:
                    if (level >= 60 && Ready(SGE.Physis2))
                        return SGE.Physis2;
                    if (level >= 20 && level < 60 && Ready(SGE.Physis))
                        return SGE.Physis;
                    return SGE.LucidDreaming;
                //case SGE.Haima:
                //    if (level < 70 || Ready(SGE.Zoe))
                //        return SGE.Zoe;
                //    return SGE.Haima;
                case SGE.Dyskrasia:
                    if (level < 46 || Charges(SGE.Phlegma) > 0)
                        return level >= 72 ? SGE.Phlegma2 : SGE.Phlegma;
                    return SGE.Dyskrasia;
                case SGE.Druochole:
                    if (level >= 62 && Ready(SGE.Taurochole))
                        return SGE.Taurochole;
                    if (level >= 52 && Ready(SGE.Ixochole))
                        return SGE.Ixochole;
                    return SGE.Druochole;
                default:
                    return 0;
            }
        }

        private bool hasStatus(ushort x)
        {
            for (var i = 0; i < s.Length; i++)
                if (s[i].StatusId == x)
                    return true;
            return false;
        }

        private unsafe bool Highlighted(uint action)
        {
            var actionManager = ActionManager.Instance();
            return actionManager->IsActionHighlighted(ActionType.Action, action);
        }

        private unsafe bool Ready(uint action)
        {
            var actionManager = ActionManager.Instance();
            return actionManager->IsActionOffCooldown(ActionType.Action, action);
        }

        private unsafe uint Charges(uint action)
        {
            var actionManager = ActionManager.Instance();
            return actionManager->GetCurrentCharges(action);
        }
    }
}
