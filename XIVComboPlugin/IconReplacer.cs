using System;
using System.Runtime.InteropServices;
using Dalamud.Game;
using Dalamud.Hooking;
using XIVComboPlugin.JobActions;
using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Game.ClientState.JobGauge.Types;
using FFXIVClientStructs.FFXIV.Client.Game;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game.Gauge;
using SerpentCombo = Dalamud.Game.ClientState.JobGauge.Enums.SerpentCombo;
using DreadCombo = Dalamud.Game.ClientState.JobGauge.Enums.DreadCombo;
using XIVComboTweaks.Configuration.Helpers;
using Dalamud.Game.ClientState.Objects;

namespace XIVComboPlugin
{
    public class IconReplacer
    {
        public delegate ulong OnCheckIsIconReplaceableDelegate(uint actionID);

        public delegate ulong OnGetIconDelegate(byte param1, uint param2);

        private readonly IconReplacerAddressResolver Address;
        private readonly Hook<OnCheckIsIconReplaceableDelegate> checkerHook;
        private readonly IClientState clientState;

        private IntPtr comboTimer = IntPtr.Zero;
        private IntPtr lastComboMove = IntPtr.Zero;

        private readonly XIVComboConfiguration Configuration;

        private readonly Hook<OnGetIconDelegate> iconHook;

        private IGameInteropProvider HookProvider;
        private IJobGauges JobGauges;
        private IPluginLog PluginLog;
        private ITargetManager targetManager;

        private unsafe delegate int* getArray(long* address);

        public IconReplacer(ISigScanner scanner, IClientState clientState, IDataManager manager, XIVComboConfiguration configuration, IGameInteropProvider hookProvider, IJobGauges jobGauges, IPluginLog pluginLog, ITargetManager targetManager)
        {
            HookProvider = hookProvider;
            Configuration = configuration;
            this.clientState = clientState;
            this.targetManager = targetManager;
            JobGauges = jobGauges;
            PluginLog = pluginLog;

            Address = new IconReplacerAddressResolver(scanner);

            if (!clientState.IsLoggedIn)
                clientState.Login += SetupComboData;
            else
                SetupComboData();

            PluginLog.Verbose("===== X I V C O M B O =====");
            PluginLog.Verbose("IsIconReplaceable address {IsIconReplaceable}", Address.IsIconReplaceable);
            PluginLog.Verbose("ComboTimer address {ComboTimer}", comboTimer);
            PluginLog.Verbose("LastComboMove address {LastComboMove}", lastComboMove);

            iconHook = HookProvider.HookFromAddress<OnGetIconDelegate>((nint)ActionManager.Addresses.GetAdjustedActionId.Value, GetIconDetour);
            checkerHook = HookProvider.HookFromAddress<OnCheckIsIconReplaceableDelegate>(Address.IsIconReplaceable, CheckIsIconReplaceableDetour);
            HookProvider = hookProvider;
        }

        public unsafe void SetupComboData()
        {
            var actionmanager = (byte*)ActionManager.Instance();
            comboTimer = (IntPtr)(actionmanager + 0x60);
            lastComboMove = comboTimer + 0x4;
        }

        public void Enable()
        {
            iconHook.Enable();
            checkerHook.Enable();
        }

        public void Dispose()
        {
            iconHook.Dispose();
            checkerHook.Dispose();
        }

        private bool hasFlag(CustomComboPreset preset)
        {
            return Configuration.ComboPresets[(int)preset];
        }

        // I hate this function. This is the dumbest function to exist in the game. Just return 1.
        // Determines which abilities are allowed to have their icons updated.

        private ulong CheckIsIconReplaceableDetour(uint actionID)
        {
            return 1;
        }

        /// <summary>
        ///     Replace an ability with another ability
        ///     actionID is the original ability to be "used"
        ///     Return either actionID (itself) or a new Action table ID as the
        ///     ability to take its place.
        ///     I tend to make the "combo chain" button be the last move in the combo
        ///     For example, Souleater combo on DRK happens by dragging Souleater
        ///     onto your bar and mashing it.
        /// </summary>
        private unsafe ulong GetIconDetour(byte self, uint actionID)
        {
            if (clientState.LocalPlayer == null) return iconHook.Original(self, actionID);
            // Last resort. For some reason GetIcon fires after leaving the lobby but before ClientState.Login
            if (lastComboMove == IntPtr.Zero)
            {
                SetupComboData();
                return iconHook.Original(self, actionID);
            }
            if (comboTimer == IntPtr.Zero)
            {
                SetupComboData();
                return iconHook.Original(self, actionID);
            }

            uint lastMove = (uint)Marshal.ReadInt32(lastComboMove);
            var comboTime = Marshal.PtrToStructure<float>(comboTimer);
            var level = clientState.LocalPlayer.Level;
            var actionManager = ActionManager.Instance();
            BaseHelper.Get(clientState.LocalPlayer, level, lastMove, JobGauges, targetManager);

            // DRAGOON

            // Replace Coerthan Torment with Coerthan Torment combo chain
            if (hasFlag(CustomComboPreset.DragoonCoerthanTormentCombo))
                if (actionID == DRG.CTorment)
                {
                    if ((lastMove == DRG.DoomSpike || lastMove == DRG.DraconianFury) && level >= 62)
                        return DRG.SonicThrust;
                    if (lastMove == DRG.SonicThrust && level >= 72)
                        return DRG.CTorment;
                    return iconHook.Original(self, DRG.DoomSpike);
                }

            // Replace Chaos Thrust with the Chaos Thrust combo chain
            if (hasFlag(CustomComboPreset.DragoonChaosThrustCombo))
                if (actionID == DRG.ChaosThrust || actionID == DRG.ChaoticSpring)
                {
                    if ((lastMove == DRG.TrueThrust || lastMove == DRG.RaidenThrust) && level >= 18)
                        return iconHook.Original(self, DRG.Disembowel);
                    if ((lastMove == DRG.Disembowel || lastMove == DRG.SpiralBlow) && level >= 50)
                        return iconHook.Original(self, DRG.ChaosThrust);
                    if ((lastMove == DRG.ChaosThrust || lastMove == DRG.ChaoticSpring) && level >= 58)
                        return DRG.WheelingThrust;
                    if (lastMove == DRG.WheelingThrust && level >= 64)
                        return DRG.Drakesbane;
                    return iconHook.Original(self, DRG.TrueThrust);
                }

            // Replace Full Thrust with the Full Thrust combo chain
            if (hasFlag(CustomComboPreset.DragoonFullThrustCombo))
                if (actionID == DRG.FullThrust || actionID == DRG.HeavensThrust)
                {
                    if ((lastMove == DRG.TrueThrust || lastMove == DRG.RaidenThrust) && level >= 4)
                        return iconHook.Original(self, DRG.VorpalThrust);
                    if ((lastMove == DRG.VorpalThrust || lastMove == DRG.LanceBarrage) && level >= 26)
                        return iconHook.Original(self, DRG.FullThrust);
                    if ((lastMove == DRG.FullThrust || lastMove == DRG.HeavensThrust) && level >= 56)
                        return DRG.FangAndClaw;
                    if (lastMove == DRG.FangAndClaw && level >= 64)
                        return DRG.Drakesbane;
                    return iconHook.Original(self, DRG.TrueThrust);
                }

            // DARK KNIGHT

            // Replace Souleater with Bloodspiller if gauge, same for Stalwart Soul/Quietus
            if (hasFlag(CustomComboPreset.DarkBloodGaugeCombo))
            {
                if (actionID == DRK.EdgeofDarkness && level < 40)
                    return DRK.FloodofDarkness;
                var gauge = JobGauges.Get<DRKGauge>();
                if (gauge.Blood >= 50 || SearchBuffArray(DRK.Buffs.Delirium))
                {
                    if (actionID == DRK.Souleater)
                        return DRK.Bloodspiller;
                    if (actionID == DRK.StalwartSoul || actionID == DRK.Souleater && level < 64)
                        return DRK.Quietus;
                }
                
            }

            // Replace Shadow wall with: Dark Mind > Rampart > Dark Missionary
            if (hasFlag(CustomComboPreset.DarkMitgiations))
            {
                if (actionID == DRK.ShadowWall)
                {
                    if (level >= 45 && actionManager->IsActionOffCooldown(ActionType.Action, DRK.DarkMind))
                    {
                        return DRK.DarkMind;
                    }
                    if (level >= 8 && actionManager->IsActionOffCooldown(ActionType.Action, DRK.Rampart) || level < 38)
                    {
                        return DRK.Rampart;
                    }
                    if (level >= 66 && actionManager->IsActionOffCooldown(ActionType.Action, DRK.DarkMissionary))
                    {
                        return DRK.DarkMissionary;
                    }
                }
            }

            // Replace carve and spit/abyssal drain with blood weapon/delirium when it is available
            if (hasFlag(CustomComboPreset.DarkBloodWeapon))
            {
                if (actionID == DRK.CarveAndSpit || actionID == DRK.AbyssalDrain)
                {
                    if (actionManager->IsActionOffCooldown(ActionType.Action, DRK.BloodWeapon))
                    {
                        if (level >= 68)
                            return DRK.Delirium;
                        return DRK.BloodWeapon;
                    }
                    if (actionID == DRK.CarveAndSpit && level < 60 && level >= 56)
                        return DRK.AbyssalDrain;
                }
            }

            // Replace Souleater with Souleater combo chain
            if (hasFlag(CustomComboPreset.DarkSouleaterCombo))
                if (actionID == DRK.Souleater)
                {
                    if (lastMove == DRK.HardSlash && level >= 2)
                        return DRK.SyphonStrike;
                    if (lastMove == DRK.SyphonStrike && level >= 26)
                        return DRK.Souleater;
                    return DRK.HardSlash;
                }

            // Replace Stalwart Soul with Stalwart Soul combo chain
            if (hasFlag(CustomComboPreset.DarkStalwartSoulCombo))
                if (actionID == DRK.StalwartSoul)
                {
                    if (lastMove == DRK.Unleash && level >= 40)
                        return DRK.StalwartSoul;
                    return DRK.Unleash;
                }

            // PALADIN

            // Replace Royal Authority with Royal Authority combo
            if (hasFlag(CustomComboPreset.PaladinRoyalAuthorityCombo))
                if (actionID == PLD.RoyalAuthority || actionID == PLD.RageOfHalone)
                {
                    if (lastMove == PLD.FastBlade && level >= 4)
                        return PLD.RiotBlade;
                    if (lastMove == PLD.RiotBlade && level >= 26)
                        return iconHook.Original(self, PLD.RageOfHalone);
                    return PLD.FastBlade;
                }

            // Replace Prominence with Prominence combo
            if (hasFlag(CustomComboPreset.PaladinProminenceCombo))
                if (actionID == PLD.Prominence)
                {
                    if (lastMove == PLD.TotalEclipse && level >= 40)
                        return PLD.Prominence;
                    return PLD.TotalEclipse;
                }

            // Replace Requiescat/Imperator with Confiteor when under the effect of Requiescat
            if (hasFlag(CustomComboPreset.PaladinRequiescatCombo))
                if (actionID == PLD.Requiescat || actionID == PLD.Imperator)
                {
                    if (SearchBuffArray(PLD.BuffRequiescat) && level >= 80)
                        return iconHook.Original(self, PLD.Confiteor);
                    return iconHook.Original(self, actionID);
                }

            // WARRIOR
            if (hasFlag(CustomComboPreset.WarriorStormsPathCombo))
            {
                if (WARHelper.applies(actionID))
                    return WARHelper.move(actionID);
            }

            //// Replace Storm's Path with Storm's Path combo
            //if (hasFlag(CustomComboPreset.WarriorStormsPathCombo))
            //    if (actionID == WAR.StormsPath)
            //    {
            //        if (lastMove == WAR.HeavySwing && level >= 4)
            //            return WAR.Maim;
            //        if (lastMove == WAR.Maim && level >= 26)
            //            return WAR.StormsPath;
            //        return WAR.HeavySwing;
            //    }

            //// Replace Storm's Eye with Storm's Eye combo
            //if (hasFlag(CustomComboPreset.WarriorStormsEyeCombo))
            //    if (actionID == WAR.StormsEye)
            //    {
            //        if (lastMove == WAR.HeavySwing && level >= 4)
            //            return WAR.Maim;
            //        if (lastMove == WAR.Maim && level >= 50)
            //            return WAR.StormsEye;
            //        return WAR.HeavySwing;
            //    }

            //// Replace Mythril Tempest with Mythril Tempest combo
            //if (hasFlag(CustomComboPreset.WarriorMythrilTempestCombo))
            //    if (actionID == WAR.MythrilTempest)
            //    {
            //        if (lastMove == WAR.Overpower && level >= 40)
            //            return WAR.MythrilTempest;
            //        return WAR.Overpower;
            //    }

            // SAMURAI
            if (hasFlag(CustomComboPreset.SamuraiTsubameCombo))
                if (actionID == SAM.Iaijutsu)
                {
                    if (SearchBuffArray(SAM.BuffTsubameReady) ||
                        SearchBuffArray(SAM.BuffTsubame1) ||
                        SearchBuffArray(SAM.BuffTsubame2) ||
                        SearchBuffArray(SAM.BuffTsubame3))
                        return iconHook.Original(self, SAM.Tsubame);
                    return iconHook.Original(self, actionID);
                }

            // Replace Yukikaze with Yukikaze combo
            if (hasFlag(CustomComboPreset.SamuraiYukikazeCombo))
                if (actionID == SAM.Yukikaze)
                {
                    if (SearchBuffArray(SAM.BuffMeikyoShisui))
                        return SAM.Yukikaze;
                    if ((lastMove == SAM.Hakaze || lastMove == SAM.Gyofu) && level >= 50)
                        return SAM.Yukikaze;
                    return iconHook.Original(self, SAM.Hakaze);
                }

            // Replace Gekko with Gekko combo
            if (hasFlag(CustomComboPreset.SamuraiGekkoCombo))
                if (actionID == SAM.Gekko)
                {
                    if (SearchBuffArray(SAM.BuffMeikyoShisui))
                        return SAM.Gekko;
                    if ((lastMove == SAM.Hakaze || lastMove == SAM.Gyofu) && level >= 4)
                        return SAM.Jinpu;
                    if (lastMove == SAM.Jinpu && level >= 30)
                        return SAM.Gekko;
                    return iconHook.Original(self, SAM.Hakaze);
                }

            // Replace Kasha with Kasha combo
            if (hasFlag(CustomComboPreset.SamuraiKashaCombo))
                if (actionID == SAM.Kasha)
                {
                    if (SearchBuffArray(SAM.BuffMeikyoShisui))
                        return SAM.Kasha;
                    if ((lastMove == SAM.Hakaze || lastMove == SAM.Gyofu) && level >= 18)
                        return SAM.Shifu;
                    if (lastMove == SAM.Shifu && level >= 40)
                        return SAM.Kasha;
                    return iconHook.Original(self, SAM.Hakaze);
                }

            // Replace Mangetsu with Mangetsu combo
            if (hasFlag(CustomComboPreset.SamuraiMangetsuCombo))
                if (actionID == SAM.Mangetsu)
                {
                    if (SearchBuffArray(SAM.BuffMeikyoShisui))
                        return SAM.Mangetsu;
                    if ((lastMove == SAM.Fuga || lastMove == SAM.Fuko) && level >= 35)
                        return SAM.Mangetsu;
                    return iconHook.Original(self, SAM.Fuga);
                }

            // Replace Oka with Oka combo
            if (hasFlag(CustomComboPreset.SamuraiOkaCombo))
                if (actionID == SAM.Oka)
                {
                    if (SearchBuffArray(SAM.BuffMeikyoShisui))
                        return SAM.Oka;
                    if ((lastMove == SAM.Fuga || lastMove == SAM.Fuko) && level >= 45)
                        return SAM.Oka;
                    return iconHook.Original(self, SAM.Fuga);
                }

            // NINJA

            // Replace Armor Crush with Armor Crush combo
            if (hasFlag(CustomComboPreset.NinjaArmorCrushCombo))
                if (actionID == NIN.ArmorCrush)
                {
                    if (SearchBuffArray(NIN.BuffRaijuReady))
                        return NIN.ForkedRaiju;
                    if (lastMove == NIN.SpinningEdge && level >= 4)
                        return NIN.GustSlash;
                    if (lastMove == NIN.GustSlash && level >= 54)
                        return NIN.ArmorCrush;
                    return NIN.SpinningEdge;
                }

            // Replace Aeolian Edge with Aeolian Edge combo
            if (hasFlag(CustomComboPreset.NinjaAeolianEdgeCombo))
                if (actionID == NIN.AeolianEdge)
                {
                    if (SearchBuffArray(NIN.BuffRaijuReady))
                        return NIN.FleetingRaiju;
                    if (lastMove == NIN.SpinningEdge && level >= 4)
                        return NIN.GustSlash;
                    if (lastMove == NIN.GustSlash && level >= 26)
                        return NIN.AeolianEdge;
                    return NIN.SpinningEdge;
                }

            // Replace Hakke Mujinsatsu with Hakke Mujinsatsu combo
            if (hasFlag(CustomComboPreset.NinjaHakkeMujinsatsuCombo))
                if (actionID == NIN.HakkeM)
                {
                    if (lastMove == NIN.DeathBlossom && level >= 52)
                        return NIN.HakkeM;
                    return NIN.DeathBlossom;
                }

            // GUNBREAKER

            // Replace Solid Barrel with Solid Barrel combo
            if (hasFlag(CustomComboPreset.GunbreakerSolidBarrelCombo))
                if (actionID == GNB.SolidBarrel)
                {
                    if (lastMove == GNB.KeenEdge && level >= 4)
                        return GNB.BrutalShell;
                    if (lastMove == GNB.BrutalShell && level >= 26)
                        return GNB.SolidBarrel;
                    return GNB.KeenEdge;
                }

            // Replace Wicked Talon with Gnashing Fang combo
            if (hasFlag(CustomComboPreset.GunbreakerGnashingFangCont))
                if (actionID == GNB.GnashingFang)
                {
                    if (level >= GNB.LevelContinuation)
                    {
                        if (SearchBuffArray(GNB.BuffReadyToRip))
                            return GNB.JugularRip;
                        if (SearchBuffArray(GNB.BuffReadyToTear))
                            return GNB.AbdomenTear;
                        if (SearchBuffArray(GNB.BuffReadyToGouge))
                            return GNB.EyeGouge;
                    }
                    return iconHook.Original(self, GNB.GnashingFang);
                }

            // Replace Burst Strike with Continuation
            if (hasFlag(CustomComboPreset.GunbreakerBurstStrikeCont))
                if (actionID == GNB.BurstStrike)
                {
                    if (level >= GNB.LevelEnhancedContinuation)
                        if (SearchBuffArray(GNB.BuffReadyToBlast))
                            return GNB.Hypervelocity;
                    return GNB.BurstStrike;
                }

            // Replace Demon Slaughter with Demon Slaughter combo
            if (hasFlag(CustomComboPreset.GunbreakerDemonSlaughterCombo))
                if (actionID == GNB.DemonSlaughter)
                {
                    if (lastMove == GNB.DemonSlice && level >= 40)
                        return GNB.DemonSlaughter;
                    return GNB.DemonSlice;
                }

            // Replace Fated Brand with Continuation
            if (hasFlag(CustomComboPreset.GunbreakerFatedCircleCont))
                if (actionID == GNB.FatedCircle)
                {
                    if (level >= GNB.LevelEnhancedContinuation2)
                        if (SearchBuffArray(GNB.BuffReadyToRaze))
                            return GNB.FatedBrand;
                    return GNB.FatedCircle;
                }

            // MACHINIST

            // Replace Clean Shot with Heated Clean Shot combo
            // Or with Heat Blast when overheated.
            if (hasFlag(CustomComboPreset.MachinistMainCombo))
                if (actionID == MCH.CleanShot || actionID == MCH.HeatedCleanShot)
                {
                    if (lastMove == MCH.SplitShot && level >= 2)
                        return iconHook.Original(self, MCH.SlugShot);
                    if (lastMove == MCH.SlugShot && level >= 26)
                        return iconHook.Original(self, MCH.CleanShot);
                    return iconHook.Original(self, MCH.SplitShot);
                }


            // Replace Hypercharge with Heat Blast when overheated
            if (hasFlag(CustomComboPreset.MachinistOverheatFeature))
                if (actionID == MCH.Hypercharge)
                {
                    var gauge = JobGauges.Get<MCHGauge>();
                    if (gauge.IsOverheated)
                        if (level >= 35) return iconHook.Original(self, MCH.HeatBlast);
                    return MCH.Hypercharge;
                }

            // Replace Spread Shot with Auto Crossbow when overheated.
            if (hasFlag(CustomComboPreset.MachinistSpreadShotFeature))
                if (actionID == MCH.SpreadShot || actionID == MCH.Scattergun)
                {
                    if (JobGauges.Get<MCHGauge>().IsOverheated && level >= 52)
                        return MCH.AutoCrossbow;
                    return iconHook.Original(self, MCH.SpreadShot);
                }

            // BLACK MAGE

            if (hasFlag(CustomComboPreset.BlackFlareIceCombo))
            {
                if (actionID == BLM.Blizzard4 || actionID == BLM.Freeze)
                {
                    var gauge = JobGauges.Get<BLMGauge>();
                    if (gauge.InAstralFire && level >= 100)
                        return BLM.FlareStar;
                    return iconHook.Original(self, actionID);
                }
            }

            // B4 and F4 change to each other depending on stance, as do Flare and Freeze.
            if (hasFlag(CustomComboPreset.BlackEnochianFeature))
            {
                if (actionID == BLM.Fire4 || actionID == BLM.Blizzard4)
                {
                    var gauge = JobGauges.Get<BLMGauge>();
                    if (gauge.InUmbralIce && level >= 58)
                        return BLM.Blizzard4;
                    if (gauge.AstralSoulStacks == 6)
                        return BLM.FlareStar;
                    if (level >= 60)
                        return BLM.Fire4;
                }

                if (actionID == BLM.Flare || actionID == BLM.Freeze)
                {
                    var gauge = JobGauges.Get<BLMGauge>();
                    if (gauge.AstralSoulStacks == 6)
                        return BLM.FlareStar;
                    if (gauge.InAstralFire && level >= 50)
                        return BLM.Flare;
                    return BLM.Freeze;
                }
            }


            // ASTROLOGIAN
            // Change Play 1/2/3 to Astral/Umbral Draw if that Play action doesn't have a card ready to be played.
            if (hasFlag(CustomComboPreset.AstrologianCardsOnDrawFeature))
            {
                if (actionID == AST.MinorArcana)
                {
                    var x = iconHook.Original(self, actionID);
                    if (x != AST.MinorArcana && level >= 70) 
                        return x;
                    return iconHook.Original(self, AST.AstralDraw);
                }
            }

            // SUMMONER
           //SummonerOneKeyCombo
            if (hasFlag(CustomComboPreset.SummonerOneKeyCombo) && actionID == SMN.Ruin3)
            {
                SMNGauge smnGauge = JobGauges.Get<SMNGauge>();
                if (actionManager->IsActionOffCooldown(ActionType.Action, SMN.SummonBahamut))
                    return SMN.SummonBahamut;
                else if (actionManager->IsActionHighlighted(ActionType.Action, SMN.Deathflare) && actionManager->IsActionOffCooldown(ActionType.Action, SMN.Deathflare))
                    return SMN.Deathflare;
                else if (lastMove == SMN.Deathflare)
                    return SMN.EnkindleBahamut;
                // at this point all summons are ready (bahamut always ready)
                // attunement time doesn't work for bahamut
                // isattuned doesn't seem to work
                // ready is down only after eikon is summoned
                else if (!actionManager->IsActionHighlighted(ActionType.Action, SMN.SummonIfrit) && smnGauge.IsIfritReady)
                    return SMN.AstralImpulse;
                else if (actionManager->IsActionHighlighted(ActionType.Action, SMN.SummonIfrit))
                    return SMN.SummonIfrit;
                else if (smnGauge.AttunmentTimerRemaining > 0 && smnGauge.IsTitanReady)
                    return SMN.RubyRite;
                else if (smnGauge.IsTitanReady)
                    return SMN.SummonTitan;
                else if (smnGauge.AttunmentTimerRemaining > 0 && smnGauge.IsGarudaReady)
                    return SMN.TopazRite;
                else if (smnGauge.IsGarudaReady)
                    return SMN.SummonGaruda;
                else if (smnGauge.AttunmentTimerRemaining > 0)
                    return SMN.EmeraldRite;
                else if (SearchBuffArray(SMN.Buffs.FurtherRuin))
                    return SMN.Ruin4;
                else
                    return SMN.Ruin3;
            }

            if (hasFlag(CustomComboPreset.SummonerOneKeyCombo) && actionID == SMN.Outburst)
            {
                SMNGauge smnGauge = JobGauges.Get<SMNGauge>();
                if (actionManager->IsActionOffCooldown(ActionType.Action, SMN.SummonBahamut))
                    return SMN.SummonBahamut;
                else if (actionManager->IsActionHighlighted(ActionType.Action, SMN.Deathflare) && actionManager->IsActionOffCooldown(ActionType.Action, SMN.Deathflare))
                    return SMN.Deathflare;
                else if (lastMove == SMN.Deathflare)
                    return SMN.EnkindleBahamut;
                // at this point all summons are ready (bahamut always ready)
                // attunement time doesn't work for bahamut
                // isattuned doesn't seem to work
                // ready is down only after eikon is summoned
                else if (!actionManager->IsActionHighlighted(ActionType.Action, SMN.SummonIfrit) && smnGauge.IsIfritReady)
                    return SMN.AstralFlare;
                else if (actionManager->IsActionHighlighted(ActionType.Action, SMN.SummonIfrit))
                    return SMN.SummonIfrit;
                else if (smnGauge.AttunmentTimerRemaining > 0 && smnGauge.IsTitanReady)
                    return SMN.RubyDisaster;
                else if (smnGauge.IsTitanReady)
                    return SMN.SummonTitan;
                else if (smnGauge.AttunmentTimerRemaining > 0 && smnGauge.IsGarudaReady)
                    return SMN.TopazDisaster;
                else if (smnGauge.IsGarudaReady)
                    return SMN.SummonGaruda;
                else if (smnGauge.AttunmentTimerRemaining > 0)
                    return SMN.EmeraldDisaster;
                else if (SearchBuffArray(SMN.Buffs.FurtherRuin))
                    return SMN.Ruin4;
                else
                    return SMN.Tridisaster;
            }

            if (hasFlag(CustomComboPreset.SummonerOneKeyCombo) && actionID == SMN.Resurrection)
            {
                if (actionManager->IsActionOffCooldown(ActionType.Action, SMN.Swiftcast))
                    return SMN.Swiftcast;
                else return SMN.Resurrection;
            }

                // Change Fester/Necrotize into Energy Drain
                if (hasFlag(CustomComboPreset.SummonerEDFesterCombo))
                if (actionID == SMN.Fester || actionID == SMN.Necrotize)
                {
                    SMNGauge smnGauge = JobGauges.Get<SMNGauge>();
                    if (!smnGauge.HasAetherflowStacks)
                        return SMN.EnergyDrain;
                    return iconHook.Original(self, actionID);
                }
            //Change Painflare into Energy Syphon
            if (hasFlag(CustomComboPreset.SummonerESPainflareCombo))
                if (actionID == SMN.Painflare)
                {
                    SMNGauge smnGauge = JobGauges.Get<SMNGauge>();
                    if (!smnGauge.HasAetherflowStacks)
                        return SMN.EnergySiphon;
                    return SMN.Painflare;
                }
            
            //Change Summon Solar Bahamut into Lux Solaris
            if(hasFlag(CustomComboPreset.SummonerSolarBahamutLuxSolaris))
                if (actionID == SMN.Aethercharge)
                {
                    if (SearchBuffArray(SMN.Buffs.RefulgentLux))
                        return SMN.LuxSolaris;
                    return iconHook.Original(self, actionID);
                }



            // SCHOLAR
            // Change Energy Drain into Aetherflow when you have no more Aetherflow stacks.
            if (hasFlag(CustomComboPreset.ScholarEnergyDrainFeature))
            {
                if (SCHHelper.applies(actionID))
                    return SCHHelper.move(actionID);
            }

            // Sage
            // various combos
            if (hasFlag(CustomComboPreset.SageCombos))
            {
                SGEHelper sgeHelper = SGEHelper.Get(JobGauges.Get<SGEGauge>(), clientState.LocalPlayer.StatusList, level, lastMove);
                if (sgeHelper.applies(actionID))
                {
                    return sgeHelper.move(actionID);
                }
            }

            // DANCER
            /*
             * one button combo
             * single:
             * dance > yellow/green > 2 combo
             * 
             * 
             * multi:
             * dance > enhanced > basic, order doesn't matter for enhanced
             */
            // AoE GCDs are split into two buttons, because priority matters
            // differently in different single-target moments. Thanks yoship.
            // Replaces each GCD with its procced version.
            if (hasFlag(CustomComboPreset.DancerOneKeyCombo))
            {
                if (actionID == DNC.Cascade || actionID == DNC.Emboite)
                {
                    if (level >= 70 && Ready(DNC.TechnicalStep) && !SearchBuffArray(DNC.Buffs.StandardStep))
                        return DNC.TechnicalStep;
                    if (SearchBuffArray(DNC.Buffs.TechnicalStep))
                    {
                        if (JobGauges.Get<DNCGauge>().CompletedSteps == 4)
                            return DNC.TechnicalFinish4;
                        return JobGauges.Get<DNCGauge>().NextStep;
                    }
                    if (Ready(DNC.StandardStep))
                        return DNC.StandardStep;
                    if (SearchBuffArray(DNC.Buffs.StandardStep))
                    {
                        if (JobGauges.Get<DNCGauge>().CompletedSteps == 2)
                            return DNC.StandardFinish2;
                        return JobGauges.Get<DNCGauge>().NextStep;
                    }
                    if (Highlighted(DNC.ReverseCascade))
                        return DNC.ReverseCascade;
                    if (Highlighted(DNC.Fountainfall))
                        return DNC.Fountainfall;
                    if (Highlighted(DNC.Fountain))
                        return DNC.Fountain;
                    return DNC.Cascade;
                }
            }
            //multi
            if (hasFlag(CustomComboPreset.DancerOneKeyCombo))
            {
                if (actionID == DNC.Windmill)
                {
                    if (level >= 70 && Ready(DNC.TechnicalStep))
                        return DNC.TechnicalStep;
                    if (SearchBuffArray(DNC.Buffs.TechnicalStep))
                    {
                        if (JobGauges.Get<DNCGauge>().CompletedSteps == 4)
                            return DNC.TechnicalFinish4;
                        return JobGauges.Get<DNCGauge>().NextStep;
                    }
                    if (Ready(DNC.StandardStep))
                        return DNC.StandardStep;
                    if (SearchBuffArray(DNC.Buffs.StandardStep))
                    {
                        if (JobGauges.Get<DNCGauge>().CompletedSteps == 2)
                            return DNC.StandardFinish2;
                        return JobGauges.Get<DNCGauge>().NextStep;
                    }
                    if (Highlighted(DNC.RisingWindmill))
                        return DNC.RisingWindmill;
                    if (Highlighted(DNC.Bloodshower))
                        return DNC.Bloodshower;
                    if (Highlighted(DNC.Bladeshower))
                        return DNC.Bladeshower;
                    return DNC.Windmill;
                }
            }
            //if (hasFlag(CustomComboPreset.DancerAoeGcdFeature))
            //{
            //    if (actionID == DNC.Bloodshower)
            //    {
            //        if (SearchBuffArray(DNC.Buffs.FlourishingFlow) || SearchBuffArray(DNC.Buffs.SilkenFlow))
            //            return DNC.Bloodshower;
            //        return DNC.Bladeshower;
            //    }

            //    if (actionID == DNC.RisingWindmill)
            //    {
            //        if (SearchBuffArray(DNC.Buffs.FlourishingSymmetry) || SearchBuffArray(DNC.Buffs.SilkenSymmetry))
            //            return DNC.RisingWindmill;
            //        return DNC.Windmill;
            //    }
            //}

            // Fan Dance changes into Fan Dance 3 while flourishing.
            if (hasFlag(CustomComboPreset.DancerFanDanceCombo))
            {
                if (actionID == DNC.FanDance1)
                {
                    if (SearchBuffArray(DNC.Buffs.ThreeFoldFanDance))
                        return DNC.FanDance3;
                    return DNC.FanDance1;
                }

                // Fan Dance 2 changes into Fan Dance 3 while flourishing.
                if (actionID == DNC.FanDance2)
                {
                    if (SearchBuffArray(DNC.Buffs.ThreeFoldFanDance))
                        return DNC.FanDance3;
                    return DNC.FanDance2;
                }
            }

            if (hasFlag(CustomComboPreset.DancerFanDance4Combo))
            {
                if (actionID == DNC.Flourish)
                {
                    if (SearchBuffArray(DNC.Buffs.FourFoldFanDance))
                        return DNC.FanDance4;
                    return DNC.Flourish;
                }
            }

            if (hasFlag(CustomComboPreset.DancerLastDanceCombo))
            {
                if (actionID == DNC.StandardStep)
                {
                    if (SearchBuffArray(DNC.Buffs.LastDanceReady))
                        return DNC.LastDance;
                    return iconHook.Original(self, actionID);
                }
            }

            // WHM
            if (WHMHelper.applies(actionID))
                return WHMHelper.move(actionID);
            //// Replace Solace with Misery when full blood lily
            //if (hasFlag(CustomComboPreset.WhiteMageSolaceMiseryFeature))
            //    if (actionID == WHM.Solace)
            //    {
            //        if (JobGauges.Get<WHMGauge>().BloodLily == 3)
            //            return WHM.Misery;
            //        return WHM.Solace;
            //    }

            //// Replace Solace with Misery when full blood lily
            //if (hasFlag(CustomComboPreset.WhiteMageRaptureMiseryFeature))
            //    if (actionID == WHM.Rapture)
            //    {
            //        if (JobGauges.Get<WHMGauge>().BloodLily == 3)
            //            return WHM.Misery;
            //        return WHM.Rapture;
            //    }

            // BARD
            if (BRDHelper.applies(actionID))
                return BRDHelper.move(actionID);

            //// Replace HS/BS with SS/RA when procced.
            //if (hasFlag(CustomComboPreset.BardStraightShotUpgradeFeature))
            //    if (actionID == BRD.HeavyShot || actionID == BRD.BurstShot)
            //    {
            //        if (SearchBuffArray(BRD.Buffs.HawksEye) || SearchBuffArray(BRD.Buffs.Barrage))
            //            return iconHook.Original(self, BRD.StraightShot);
            //        return iconHook.Original(self, BRD.HeavyShot);
            //    }

            //if (hasFlag(CustomComboPreset.BardAoEUpgradeFeature))
            //    if (actionID == BRD.QuickNock || actionID == BRD.Ladonsbite)
            //    {
            //        if (SearchBuffArray(BRD.Buffs.HawksEye) || SearchBuffArray(BRD.Buffs.Barrage))
            //            return iconHook.Original(self, BRD.WideVolley);
            //        return iconHook.Original(self, BRD.QuickNock);
            //    }

            // MONK
            //if (hasFlag(CustomComboPreset.MonkFuryCombo))
            //{
            //    if (actionID == MNK.Bootshine || actionID == MNK.LeapingOpo)
            //    {
            //        if (JobGauges.Get<MNKGauge>().OpoOpoFury < 1 && level >= 50)
            //            return MNK.DragonKick;
            //        return iconHook.Original(self, actionID);
            //    }

            //    if (actionID == MNK.TrueStrike || actionID == MNK.RisingRaptor)
            //    {
            //        if (JobGauges.Get<MNKGauge>().RaptorFury < 1 && level >= 18)
            //            return MNK.TwinSnakes;
            //        return iconHook.Original(self, actionID);
            //    }

            //    if (actionID == MNK.SnapPunch || actionID == MNK.PouncingCoeurl)
            //    {
            //        if (JobGauges.Get<MNKGauge>().CoeurlFury < 1 && level >= 30)
            //            return MNK.Demolish;
            //        return iconHook.Original(self, actionID);
            //    }
            //}

            // combines all 3 forms into one action
            /*
             * if perfect balance:
             *      if solar nadi opened:
             *          use q or q+ (determined by stacks)
             *      else solar nadi not opened:
             *          use next in 3 combo (determined by previous move, or could use beast chakra)
             * if no perfect balance: 
             *      if formless fist:
             *          use q or q+ (determined by stacks)
             *      if not formless fist:
             *          use next in 3 combo (determined by form)
             */
            //perfect balance makes it so that you can't get form
            //so when u exit you have no form, unless you get formless fist from masterful blitz
            MNKHelper mnkHelper = MNKHelper.Get(JobGauges.Get<MNKGauge>(), clientState.LocalPlayer.StatusList, level, lastMove);
            if (hasFlag(CustomComboPreset.MonkFuryCombo2))

            {
                if (actionID == MNK.Demolish)
                    return mnkHelper.SingleTarget();
            }

            // same as above but for Aoe
            if (hasFlag(CustomComboPreset.MonkFuryCombo3))
            {
                if (actionID == MNK.Rockbreaker)
                    return mnkHelper.MultiTarget();
            }

            if (hasFlag(CustomComboPreset.MonkPerfectBlitz))
            {
                if (actionID == MNK.MasterfulBlitz)
                {
                    if (JobGauges.Get<MNKGauge>().BlitzTimeRemaining <= 0 || level < 60)
                        return MNK.PerfectBalance;
                    return iconHook.Original(self, actionID);
                }
            }

            // RED MAGE

            // Replace Veraero/thunder 2 with Impact when Dualcast is active
            if (hasFlag(CustomComboPreset.RedMageAoECombo))
            {
                if (actionID == RDM.Veraero2)
                {
                    if (SearchBuffArray(RDM.BuffSwiftcast) || SearchBuffArray(RDM.BuffDualcast) || 
                        SearchBuffArray(RDM.BuffAcceleration) || SearchBuffArray(RDM.BuffChainspell))
                        return iconHook.Original(self, RDM.Scatter);
                    return iconHook.Original(self, actionID);
                }

                if (actionID == RDM.Verthunder2)
                {
                    if (SearchBuffArray(RDM.BuffSwiftcast) || SearchBuffArray(RDM.BuffDualcast) ||
                        SearchBuffArray(RDM.BuffAcceleration) || SearchBuffArray(RDM.BuffChainspell))
                        return iconHook.Original(self, RDM.Scatter);
                    return iconHook.Original(self, actionID);
                }
            }

            // Replace Redoublement with Redoublement combo, Enchanted if possible.
            if (hasFlag(CustomComboPreset.RedMageMeleeCombo))
                if (actionID == RDM.Redoublement)
                {
                    if ((lastMove == RDM.Riposte) && level >= 35)
                        return iconHook.Original(self, RDM.Zwerchhau);

                    if (lastMove == RDM.Zwerchhau && level >= 50)
                        return iconHook.Original(self, RDM.Redoublement);

                    return iconHook.Original(self, RDM.Riposte);
                }

            if (hasFlag(CustomComboPreset.RedMageVerprocCombo))
            {
                if (actionID == RDM.Verstone)
                {
                    if (SearchBuffArray(RDM.BuffGrandImpactReady)) return iconHook.Original(self, RDM.Jolt);
                    if (level >= 80 && (lastMove == RDM.Verflare || lastMove == RDM.Verholy)) return RDM.Scorch;
                    if (level >= 90 && lastMove == RDM.Scorch) return RDM.Resolution;
                    if (SearchBuffArray(RDM.BuffVerstoneReady)) return RDM.Verstone;
                    return iconHook.Original(self, RDM.Jolt);
                }
                if (actionID == RDM.Verfire)
                {
                    if (SearchBuffArray(RDM.BuffGrandImpactReady)) return iconHook.Original(self, RDM.Jolt);
                    if (level >= 80 && (lastMove == RDM.Verflare || lastMove == RDM.Verholy)) return RDM.Scorch;
                    if (level >= 90 && lastMove == RDM.Scorch) return RDM.Resolution;
                    if (SearchBuffArray(RDM.BuffVerfireReady)) return RDM.Verfire;
                    return iconHook.Original(self, RDM.Jolt);
                }
            }

            // REAPER 
            if (hasFlag(CustomComboPreset.ReaperSliceCombo))
            {
                if (actionID == RPR.Slice)
                {
                    if (lastMove == RPR.Slice && level >= RPR.Levels.WaxingSlice)
                        return RPR.WaxingSlice;

                    if (lastMove == RPR.WaxingSlice && level >= RPR.Levels.InfernalSlice)
                        return RPR.InfernalSlice;

                    return RPR.Slice;
                }
            }

            if (hasFlag(CustomComboPreset.ReaperScytheCombo))
            {
                if (actionID == RPR.SpinningScythe)
                {
                    if (lastMove == RPR.SpinningScythe && level >= RPR.Levels.NightmareScythe)
                        return RPR.NightmareScythe;

                    return RPR.SpinningScythe;
                }
            }

            if (hasFlag(CustomComboPreset.ReaperRegressFeature))
            {
                if (actionID == RPR.Egress || actionID == RPR.Ingress)
                {
                    if (SearchBuffArray(RPR.Buffs.Threshold)) return RPR.Regress;
                    return actionID;
                }
            }

            if (hasFlag(CustomComboPreset.ReaperEnshroudCombo))
            {
                if (actionID == RPR.Enshroud)
                {
                    if (SearchBuffArray(RPR.Buffs.Enshrouded)) return RPR.Communio;
                    if (SearchBuffArray(RPR.Buffs.PerfectioParata)) return RPR.Perfectio;
                    return actionID;
                }
            }

            if (hasFlag(CustomComboPreset.ReaperArcaneFeature))
            {
                if (actionID == RPR.ArcaneCircle)
                {
                    if (SearchBuffArray(RPR.Buffs.ImSac1) ||
                        SearchBuffArray(RPR.Buffs.ImSac2))
                        return RPR.PlentifulHarvest;
                    return actionID;
                }
            }
            
            // PICTOMANCER
            if (hasFlag(CustomComboPreset.PictoSubtractivePallet))
            {
                if (actionID == PCT.Fire1)
                {
                    if (SearchBuffArray(PCT.SubPallet))
                        return iconHook.Original(self, PCT.Bliz1);
                    return iconHook.Original(self, PCT.Fire1);
                }

                if (actionID == PCT.Fire2)
                {
                    if (SearchBuffArray(PCT.SubPallet))
                        return iconHook.Original(self, PCT.Bliz2);
                    return iconHook.Original(self, PCT.Fire2);
                }
            }

            if (hasFlag(CustomComboPreset.PictoHolyWhiteCombo))
            {
                if (actionID == PCT.HolyWhite)
                {
                    if (SearchBuffArray(PCT.Monochrome))
                        return PCT.CometBlack;
                    return PCT.HolyWhite;
                }
            }

            bool pictoMuseEnabled = hasFlag(CustomComboPreset.PictoMotifMuseFeature);
            bool pictoFollowUpEnabled = hasFlag(CustomComboPreset.PictoMuseCombo);

            if (actionID == PCT.CreatureMotif)
            {
                var PCTGauge = JobGauges.Get<PCTGauge>();
                if (pictoMuseEnabled && PCTGauge.CreatureMotifDrawn)
                    return iconHook.Original(self, PCT.LivingMuse);
                return iconHook.Original(self, actionID);
            }

            if (actionID == PCT.WeaponMotif)
            {
                var PCTGauge = JobGauges.Get<PCTGauge>();
                if (pictoMuseEnabled && PCTGauge.WeaponMotifDrawn)
                    return iconHook.Original(self, PCT.SteelMuse);
                if (pictoFollowUpEnabled && SearchBuffArray(PCT.HammerReady))
                    return iconHook.Original(self, PCT.HammerStamp);
                return iconHook.Original(self, actionID);
            }

            if (actionID == PCT.LandscapeMotif)
            {
                var PCTGauge = JobGauges.Get<PCTGauge>();
                if (pictoMuseEnabled && PCTGauge.LandscapeMotifDrawn)
                    return PCT.StarryMuse;
                if (pictoFollowUpEnabled && SearchBuffArray(PCT.StarStruck))
                    return PCT.StarPrism;
                return PCT.StarryMotif;
            }

            //VIPER
            
            if (hasFlag(CustomComboPreset.ViperDeathLashCombo))
            {
                if (actionID == VPR.SteelFangs || actionID == VPR.DreadFangs)
                    if (JobGauges.Get<VPRGauge>().SerpentCombo == SerpentCombo.DEATHRATTLE)
                        return VPR.DeathRattle;

                if (actionID == VPR.DreadMaw || actionID == VPR.SteelMaw)
                    if (JobGauges.Get<VPRGauge>().SerpentCombo == SerpentCombo.LASTLASH)
                        return VPR.LastLash;
            }

            if (hasFlag(CustomComboPreset.ViperLegacyCombo))
            {
                switch (actionID)
                {
                    case VPR.SteelFangs:
                    case VPR.SteelMaw:
                        if (JobGauges.Get<VPRGauge>().SerpentCombo == SerpentCombo.FIRSTLEGACY)
                            return VPR.FirstLegacy;
                        break;

                    case VPR.DreadFangs:
                    case VPR.DreadMaw:
                        if (JobGauges.Get<VPRGauge>().SerpentCombo == SerpentCombo.SECONDLEGACY)
                            return VPR.SecondLegacy;
                        break;

                    case VPR.HuntersCoil:
                    case VPR.HuntersDen:
                        if (JobGauges.Get<VPRGauge>().SerpentCombo == SerpentCombo.THIRDLEGACY)
                            return VPR.ThirdLegacy;
                        break;

                    case VPR.SwiftskinsCoil:
                    case VPR.SwiftskinsDen:
                        if (JobGauges.Get<VPRGauge>().SerpentCombo == SerpentCombo.FOURTHLEGACY)
                            return VPR.FourthLegacy;
                        break;
                }
            }

            if (hasFlag(CustomComboPreset.ViperTwinsCombo))
            {
                if (actionID == VPR.UncoiledFury)
                {
                    if ((int)JobGauges.Get<VPRGauge>().SerpentCombo == 9)
                    {
                        if (SearchBuffArray(VPR.Buffs.PoisedForTwinfang))
                            return iconHook.Original(self, VPR.Twinfang);
                        if (SearchBuffArray(VPR.Buffs.PoisedForTwinblood))
                            return iconHook.Original(self, VPR.Twinblood);
                        return iconHook.Original(self, actionID);
                    }
                }

                switch (actionID)
                {
                    case VPR.SwiftskinsCoil:
                        if ((int)JobGauges.Get<VPRGauge>().SerpentCombo == 7)
                            return iconHook.Original(self, VPR.Twinblood);
                        break;

                    case VPR.HuntersCoil:
                        if ((int)JobGauges.Get<VPRGauge>().SerpentCombo == 7)
                            return iconHook.Original(self, VPR.Twinfang);
                        break;

                    case VPR.SwiftskinsDen:
                        if ((int)JobGauges.Get<VPRGauge>().SerpentCombo == 8)
                            return iconHook.Original(self, VPR.Twinblood);
                        break;

                    case VPR.HuntersDen:
                        if ((int)JobGauges.Get<VPRGauge>().SerpentCombo == 8)
                            return iconHook.Original(self, VPR.Twinfang);
                        break;
                }
            }

            if (hasFlag(CustomComboPreset.ViperViceCombo))
            {
                switch (actionID)
                {
                    case VPR.SwiftskinsCoil:
                    case VPR.HuntersCoil:
                        {
                            var gauge = JobGauges.Get<VPRGauge>();
                            if (gauge.SerpentCombo != SerpentCombo.NONE || gauge.DreadCombo != 0)
                                return iconHook.Original(self, actionID);
                            return VPR.Vicewinder;
                        }

                    case VPR.SwiftskinsDen:
                    case VPR.HuntersDen:
                        {
                            var gauge = JobGauges.Get<VPRGauge>();
                            if (gauge.SerpentCombo != SerpentCombo.NONE || gauge.DreadCombo != 0)
                                return iconHook.Original(self, actionID);
                            return VPR.Vicepit;
                        }
                }
            }

            return iconHook.Original(self, actionID);
        }

        private bool SearchBuffArray(ushort needle)
        {
            if (needle == 0) return false;
            var buffs = clientState.LocalPlayer.StatusList;
            for (var i = 0; i < buffs.Length; i++)
                if (buffs[i].StatusId == needle)
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
    }
}
