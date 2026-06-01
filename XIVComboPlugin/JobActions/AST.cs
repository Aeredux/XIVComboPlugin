namespace XIVComboPlugin.JobActions
{
    public static class AST
    {
        public const uint
            //DPS
            Malefic = 3596,
            Malefic2 = 3598,
            Malefic3 = 7442,
            Malefic4 = 16555,
            FallMalefic = 25871,
            Gravity = 3615,
            Gravity2 = 25872,
            Oracle = 37029,
            EarthlyStar = 7439,
            DetonateStar = 8324,

            //Cards
            AstralDraw = 37017,
            UmbralDraw = 37018,
            Play1 = 37019,
            Play2 = 37020,
            Play3 = 37021,
            Arrow = 37024,
            Balance = 37023,
            Bole = 37027,
            Ewer = 37028,
            Spear = 37026,
            Spire = 37025,
            MinorArcana = 37022,
            //LordOfCrowns = 7444,
            //LadyOfCrown = 7445,

            //Utility
            Divination = 16552,
            Lightspeed = 3606,

            //DoT
            Combust = 3599,
            Combust2 = 3608,
            Combust3 = 16554,

            //Healing
            Benefic = 3594,
            Benefic2 = 3610,
            AspectedBenefic = 3595,
            Helios = 3600,
            AspectedHelios = 3601,
            HeliosConjuction = 37030,
            Ascend = 3603,
            EssentialDignity = 3614,
            CelestialOpposition = 16553,
            CelestialIntersection = 16556,
            Horoscope = 16557,
            Horoscope2 = 16558,
            Exaltation = 25873,
            Macrocosmos = 25874,
            Synastry = 3612,
            CollectiveUnconscious = 3613;
        public static class Buffs
        {
            internal const ushort
                AspectedBenefic = 835,
                AspectedHelios = 836,
                HeliosConjunction = 3894,
                Horoscope = 1890,
                HoroscopeHelios = 1891,
                NeutralSect = 1892,
                NeutralSectShield = 1921,
                Divination = 1878,
                LordOfCrownsDrawn = 2054,
                LadyOfCrownsDrawn = 2055,
                GiantDominance = 1248,
                ClarifyingDraw = 2713,
                Macrocosmos = 2718,
                //The "Buff" that shows when you're holding onto the card
                BalanceDrawn = 913,
                BoleDrawn = 914,
                ArrowDrawn = 915,
                SpearDrawn = 916,
                EwerDrawn = 917,
                SpireDrawn = 918,
                //The actual buff that buffs players
                BalanceBuff = 3887,
                BoleBuff = 3890,
                ArrowBuff = 3888,
                SpearBuff = 3889,
                EwerBuff = 3891,
                SpireBuff = 3892,
                Lightspeed = 841,
                SelfSynastry = 845,
                TargetSynastry = 846,
                Divining = 3893;
        }

        public static class Debuffs
        {
            internal const ushort
                Combust = 838,
                Combust2 = 843,
                Combust3 = 1881;
        }
    }
}
