using System;

namespace XIVComboPlugin
{
    //TODO: reorganize the numbers lol lmao
    public enum CustomComboPreset : int
    {
        // DRAGOON
        [CustomComboInfo("Coerthan Torment Combo", "Replace Coerthan Torment with its combo chain", 22)]
        DragoonCoerthanTormentCombo = 0,

        [CustomComboInfo("Chaos Thrust Combo", "Replace Chaos Thrust with its combo chain", 22)]
        DragoonChaosThrustCombo = 1,

        [CustomComboInfo("Full Thrust Combo", "Replace Full Thrust with its combo chain", 22)]
        DragoonFullThrustCombo = 2,

        // DARK KNIGHT
        [CustomComboInfo("Souleater Combo", "Replace Souleater with its combo chain", 32)]
        DarkSouleaterCombo = 3,

        [CustomComboInfo("Stalwart Soul Combo", "Replace Stalwart Soul with its combo chain", 32)]
        DarkStalwartSoulCombo = 4,

        [CustomComboInfo("Blood Gauge Combo", "Replace GCD with Blood Gauge variant", 32)]
        DarkBloodGaugeCombo = 61,

        [CustomComboInfo("Mitigations Replaces", "Replace mitigations", 32)]
        DarkMitgiations = 62,

        [CustomComboInfo("Blood Weapon Replaces", "Replace carve and spit/abyssal drain with blood weapon when it is available", 32)]
        DarkBloodWeapon = 63,

        // PALADIN
        [CustomComboInfo("Royal Authority Combo", "Replace Royal Authority/Rage of Halone with its combo chain", 19)]
        PaladinRoyalAuthorityCombo = 6,

        [CustomComboInfo("Prominence Combo", "Replace Prominence with its combo chain", 19)]
        PaladinProminenceCombo = 7,

        [CustomComboInfo("Requiescat/Imperator Confiteor", "Replace Requiescat/Imperator with Confiteor while under the effect of Requiescat", 19)]
        PaladinRequiescatCombo = 55,

        // WARRIOR
        [CustomComboInfo("Storms Path Combo", "Replace Storms Path with its combo chain", 21)]
        WarriorStormsPathCombo = 8,

        [CustomComboInfo("Storms Eye Combo", "Replace Storms Eye with its combo chain", 21)]
        WarriorStormsEyeCombo = 9,

        [CustomComboInfo("Mythril Tempest Combo", "Replace Mythril Tempest with its combo chain", 21)]
        WarriorMythrilTempestCombo = 10,

        // SAMURAI
        [CustomComboInfo("Yukikaze Combo", "Replace Yukikaze with its combo chain", 34)]
        SamuraiYukikazeCombo = 11,

        [CustomComboInfo("Gekko Combo", "Replace Gekko with its combo chain", 34)]
        SamuraiGekkoCombo = 12,

        [CustomComboInfo("Kasha Combo", "Replace Kasha with its combo chain", 34)]
        SamuraiKashaCombo = 13,

        [CustomComboInfo("Mangetsu Combo", "Replace Mangetsu with its combo chain", 34)]
        SamuraiMangetsuCombo = 14,

        [CustomComboInfo("Oka Combo", "Replace Oka with its combo chain", 34)]
        SamuraiOkaCombo = 15,

        [CustomComboInfo("Iaijutsu into Tsubame", "Replace Iaijutsu with Tsubame after using an Iaijutsu", 34)]
        SamuraiTsubameCombo = 56,

        // NINJA
        [CustomComboInfo("Armor Crush Combo", "Replace Armor Crush with its combo chain or Forked Raiju", 30)]
        NinjaArmorCrushCombo = 17,

        [CustomComboInfo("Aeolian Edge Combo", "Replace Aeolian Edge with its combo chain or Fleeting Raiju", 30)]
        NinjaAeolianEdgeCombo = 18,

        [CustomComboInfo("Hakke Mujinsatsu Combo", "Replace Hakke Mujinsatsu with its combo chain", 30)]
        NinjaHakkeMujinsatsuCombo = 19,

        // GUNBREAKER
        [CustomComboInfo("Solid Barrel Combo", "Replace Solid Barrel with its combo chain", 37)]
        GunbreakerSolidBarrelCombo = 20,

        [CustomComboInfo("Gnashing Fang Continuation", "Put Continuation moves on Gnashing Fang when appropriate", 37)]
        GunbreakerGnashingFangCont = 52,

        [CustomComboInfo("Burst Strike Continuation", "Put Continuation moves on Burst Strike when appropriate", 37)]
        GunbreakerBurstStrikeCont = 45,

        [CustomComboInfo("Demon Slaughter Combo", "Replace Demon Slaughter with its combo chain", 37)]
        GunbreakerDemonSlaughterCombo = 22,
        
        [CustomComboInfo("Fated Circle Continuation", "Put Continuation moves on Fated Circle when appropriate", 37)]
        GunbreakerFatedCircleCont = 54,

        // MACHINIST
        [CustomComboInfo("(Heated) Shot Combo", "Replace either form of Clean Shot with its combo chain", 31)]
        MachinistMainCombo = 23,

        [CustomComboInfo("Spread Shot Heat", "Replace Spread Shot or Scattergun with Auto Crossbow when overheated", 31)]
        MachinistSpreadShotFeature = 24,

        [CustomComboInfo("Heat Blast when overheated", "Replace Hypercharge with Heat Blast when overheated", 31)]
        MachinistOverheatFeature = 47,

        // BLACK MAGE
        [CustomComboInfo("Enochian Stance Switcher", "Change Fire 4, Blizzard 4, Flare, and Freeze to the appropriate element depending on stance", 25)]
        BlackEnochianFeature = 25,

        [CustomComboInfo("Flare Star on Ice", "Change Blizzard 4 and Freeze to Flare Star when in Astral Fire", 25)]
        BlackFlareIceCombo = 41,

        // ASTROLOGIAN
        [CustomComboInfo("Astral/Umbral Draw on Minor Arcana", "Draw replaces Minor Arcana when no Arcana is available", 33)]
        AstrologianCardsOnDrawFeature = 27,

        // SUMMONER
        [CustomComboInfo("ED Fester/Necrotize", "Change Fester/Necrotize into Energy Drain when out of Aetherflow stacks", 27)]
        SummonerEDFesterCombo = 39,

        [CustomComboInfo("ES Painflare", "Change Painflare into Energy Syphon when out of Aetherflow stacks", 27)]
        SummonerESPainflareCombo = 40,
        
        [CustomComboInfo("Solar Bahamut Lux", "Change Summon Solar Bahamut into Lux Solaris after summoning", 27)]
        SummonerSolarBahamutLuxSolaris = 28,
        
        // SCHOLAR
        [CustomComboInfo("ED Aetherflow", "Change Energy Drain into Aetherflow when you have no more Aetherflow stacks", 28)]
        ScholarEnergyDrainFeature = 37,

        // DANCER
        [CustomComboInfo("AoE GCD procs", "DNC AoE procs turn into their normal abilities when not procced", 38)]
        DancerAoeGcdFeature = 32,

        [CustomComboInfo("Fan Dance Combos", "Change Fan Dance and Fan Dance 2 into Fan Dance 3 while flourishing", 38)]
        DancerFanDanceCombo = 33,

        [CustomComboInfo("Fan Dance IV", "Change Flourish into Fan Dance IV while flourishing", 38)]
        DancerFanDance4Combo = 60,

        [CustomComboInfo("Standard Last Dance", "Change Standard Step into Last Dance when ready", 38)]
        DancerLastDanceCombo = 21,

        // WHITE MAGE
        [CustomComboInfo("Solace into Misery", "Replaces Afflatus Solace with Afflatus Misery when Misery is ready to be used", 24)]
        WhiteMageSolaceMiseryFeature = 35,

        [CustomComboInfo("Rapture into Misery", "Replaces Afflatus Rapture with Afflatus Misery when Misery is ready to be used", 24)]
        WhiteMageRaptureMiseryFeature = 36,

        // BARD
        [CustomComboInfo("Heavy Shot into Straight Shot", "Replaces Heavy Shot/Burst Shot with Straight Shot/Refulgent Arrow when procced", 23)]
        BardStraightShotUpgradeFeature = 42,

        [CustomComboInfo("Quick Nock into Shadowbite", "Replaces Quick Nock/Ladonsbite with Wide Volley/Shadowbite when procced", 23)]
        BardAoEUpgradeFeature = 59,

        // MONK
        [CustomComboInfo("Monk Fury Combo", "Replaces Bootshine, True Strike, and Snap Punch when no Fury charges are available", 20)]
        MonkFuryCombo = 43,

        [CustomComboInfo("Monk Fury Combo 2", "Replaces Bootshine, True Strike, and Snap Punch when no Fury charges are available", 20)]
        MonkFuryCombo2 = 64,

        [CustomComboInfo("Monk Fury Combo 3", "Comboe aoe", 20)]
        MonkFuryCombo3 = 65,

        [CustomComboInfo("Perfect Balance on Masterful Blitz", "Replaces Masterful Blitz with Perfect Balance when no Blitz moves are available", 20)]
        MonkPerfectBlitz = 44,

        // RED MAGE
        [CustomComboInfo("Red Mage AoE Combo", "Replaces Veraero/thunder 2 with Impact when Dualcast or Swiftcast are active", 35)]
        RedMageAoECombo = 48,

        [CustomComboInfo("Redoublement combo", "Replaces Redoublement with its combo chain, following enchantment rules", 35)]
        RedMageMeleeCombo = 49,

        [CustomComboInfo("Verproc into Jolt", "Replaces Verstone/Verfire with Jolt/Scorch when no proc is available", 35)]
        RedMageVerprocCombo = 53,

        // REAPER
        [CustomComboInfo("Slice Combo", "Replace Slice with its combo chain", 39)]
        ReaperSliceCombo = 16,

        [CustomComboInfo("Scythe Combo", "Replace Spinning Scythe with its combo chain", 39)]
        ReaperScytheCombo = 57,

        [CustomComboInfo("Double Regress", "Regress always replaces both Hell's Egress and Hell's Ingress", 39)]
        ReaperRegressFeature = 58,

        [CustomComboInfo("Enshroud Combo", "Replace Enshroud with Communio while you are Enshrouded", 39)]
        ReaperEnshroudCombo = 26,

        [CustomComboInfo("Arcane Circle Combo", "Replace Arcane Circle with Plentiful Harvest while you have Immortal Sacrifice", 39)]
        ReaperArcaneFeature = 30,

        //PICTOMANCER
        [CustomComboInfo("Additive to Subtractive Combo","Replace Additive combo with Subtractive combo when Subtractive Pallet is active",42)]
        PictoSubtractivePallet = 31,

        [CustomComboInfo("Motifs and Muses", "Replace Motifs with their relevant Muses", 42)]
        PictoMotifMuseFeature = 34,

        [CustomComboInfo("Landscape and Steel follow-ups", "Additionally replace Landscape Motif with Star Prism and Weapon Motif with Hammer Stamp when appropriate", 42)]
        PictoMuseCombo = 38,

        [CustomComboInfo("Holy White to Comet Black", "Replace Holy in White with Comet in Black when Monochrome Tones is active", 42)]
        PictoHolyWhiteCombo = 5,

        //Viper
        [CustomComboInfo("Vicewinder/Vicepit on Hunter & Swiftskin", "Replace Hunter & Swiftskin with respective starters when appropriate", 41)]
        ViperViceCombo = 29,

        [CustomComboInfo("Death Rattle/Last Lash Finisher", "Replace moves with Death Rattle or Last Lash when available", 41)]
        ViperDeathLashCombo = 46,

        [CustomComboInfo("Twinfang/Twinblood", "Replace actions with their respective Twinfang/Twinblood moves when available", 41)]
        ViperTwinsCombo = 50,
        
        [CustomComboInfo("Generational Legacy", "Legacy moves replace Generation moves when usable", 41)]
        ViperLegacyCombo = 51,
    }

    public class CustomComboInfoAttribute : Attribute
    {
        internal CustomComboInfoAttribute(string fancyName, string description, byte classJob)
        {
            FancyName = fancyName;
            Description = description;
            ClassJob = classJob;
        }

        public string FancyName { get; }
        public string Description { get; }
        public byte ClassJob { get; }

    }
}
