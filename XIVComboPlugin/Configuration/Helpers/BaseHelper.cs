using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Objects;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Game.ClientState.Statuses;
using Dalamud.Hooking;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Gauge;
using FFXIVClientStructs.FFXIV.Client.Graphics.Render;
using Lumina.Excel.Sheets;
using XIVComboPlugin.JobActions;
using static XIVComboPlugin.IconReplacer;

namespace XIVComboTweaks.Configuration.Helpers
{
    // swiftcast -> egeiro
    // eukrasian dosis -> toxikon -> dosis
    // zoe -> haima  // actually this doesn't stack
    // phlegma -> dykrasia
    // taurochole -> ixochole -> druochole
    // physis ii -> lucid dreaming
    public unsafe class BaseHelper
    {
        public static BaseHelper instance;
        public StatusList s;
        public uint mp;
        public int level;
        public uint lastMove;
        private ActionManager* actionManager;
        private ITargetManager targetManager;
        private IPlayerCharacter localPlayer;
        public Hook<OnGetIconDelegate> iconHook;
        public byte self;

        public BRDGauge brdGauge;
        public WARGauge warGauge;
        public WHMGauge whmGauge;
        public SCHGauge schGauge;
        public DRKGauge dRKGauge;
        public MNKGauge mnkGauge;
        public RPRGauge rprGauge;
        public SMNGauge smnGauge;
        public GNBGauge gnbGauge;
        public RDMGauge rdmGauge;

        public static unsafe BaseHelper Get(IPlayerCharacter localPlayer, int l, uint lm, IJobGauges jobGauges, ITargetManager targetManager, Hook<OnGetIconDelegate> iconHook, byte self)
        {
            if (instance == null)
                instance = new BaseHelper();

            instance.setValues(localPlayer, l, lm, jobGauges, targetManager, iconHook, self);
            return instance;
        }

        public BaseHelper()
        {
        }

        private unsafe void setValues(IPlayerCharacter localPlayer, int l, uint lm, IJobGauges jobGauges, ITargetManager targetManager, Hook<OnGetIconDelegate> iconHook, byte self)
        {
            this.s = localPlayer.StatusList;
            this.mp = localPlayer.CurrentMp;
            this.localPlayer = localPlayer;
            this.level = l;
            this.lastMove = lm;
            this.actionManager = ActionManager.Instance();
            this.targetManager = targetManager;
            this.iconHook = iconHook;
            this.self = self;
            this.brdGauge = jobGauges.Get<BRDGauge>();
            this.warGauge = jobGauges.Get<WARGauge>();
            this.whmGauge = jobGauges.Get<WHMGauge>();
            this.schGauge = jobGauges.Get<SCHGauge>();
            this.dRKGauge = jobGauges.Get<DRKGauge>();
            this.mnkGauge = jobGauges.Get<MNKGauge>();
            this.rprGauge = jobGauges.Get<RPRGauge>();
            this.smnGauge = jobGauges.Get<SMNGauge>();
            this.gnbGauge = jobGauges.Get<GNBGauge>();
            this.rdmGauge = jobGauges.Get<RDMGauge>();
        }

        public static bool applies(uint move)
        {
            return false;
        }

        public uint move(uint move)
        {
            return 0;
        }

        public static bool hasStatus(ushort x)
        {
            for (var i = 0; i < instance.s.Length; i++)
                if (instance.s[i].StatusId == x)
                    return true;
            return false;
        }

        public static float statusTime(ushort x)
        {
            for (var i = 0; i < instance.s.Length; i++)
                if (instance.s[i].StatusId == x)
                    return instance.s[i].RemainingTime;
            return 0;
        }

        public static unsafe bool Highlighted(uint action)
        {
            return instance.actionManager->IsActionHighlighted(ActionType.Action, action);
        }

        public static unsafe bool Ready(uint action)
        {
            return instance.actionManager->IsActionOffCooldown(ActionType.Action, action);
        }

        public static unsafe uint Charges(uint action)
        {
            return instance.actionManager->GetCurrentCharges(action);
        }

        public static bool targetStatus(ushort x)
        {
            if (instance.targetManager.Target is not IBattleChara chara)
                return false;
            for (var i = 0; i < chara.StatusList.Length; i++)
                if (chara.StatusList[i].StatusId == x && chara.StatusList[i].SourceId == instance.localPlayer.EntityId)
                    return true;
            return false;
        }

        public static float targetStatusTime(ushort x)
        {
            if (instance.targetManager.Target is not IBattleChara chara)
                return 0;
            for (var i = 0; i < chara.StatusList.Length; i++)
                if (chara.StatusList[i].StatusId == x && chara.StatusList[i].SourceId == instance.localPlayer.EntityId)
                    return chara.StatusList[i].RemainingTime;
            return 0;
        }

        public static bool hasTarget()
        {
            return instance.targetManager.Target is IBattleChara;
        }

        public static uint original(uint move)
        {
            return (uint)instance.iconHook.Original(instance.self, move);
        }
    }
}
