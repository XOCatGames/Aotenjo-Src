using System.Collections.Generic;

namespace Aotenjo
{
    public class ArtifactRecipes
    {
        public static ArtifactRecipe TricolorOrizuluRecipe = ArtifactRecipe.Create("tricolor_orizulu",
            new() { Artifacts.RedOrizuru, Artifacts.BlueOrizuru, Artifacts.GreenOrizuru }, Artifacts.TricolorOrizulu);

        public static ArtifactRecipe YinYangNunchakuRecipe = ArtifactRecipe.Create("yin_yang_nunchaku",
            new() { Artifacts.BlackOrizuru, Artifacts.WhiteOrizuru, Artifacts.Nunchaku }, Artifacts.YinYangNunchaku);

        public static ArtifactRecipe WildcardWindvaneRecipe = ArtifactRecipe.Create("wildcard_wind_vane",
            new() { Artifacts.WindVane, Artifacts.MagnetiteSample }, Artifacts.WildcardWindVane);

        public static ArtifactRecipe ClimbingKitRecipe = ArtifactRecipe.Create("climbing_kit",
            new() { Artifacts.TravelBag, Artifacts.IceAxe }, Artifacts.ClimbingKit);

        public static ArtifactRecipe BlackHoleRecipe = ArtifactRecipe.Create("black_hole",
            new() { Artifacts.BottomlessBag, Artifacts.ZhouPiBook }, Artifacts.BlackHole);

        public static ArtifactRecipe HalfLucky47Recipe = ArtifactRecipe.Create("half_lucky_47",
            new() { Artifacts.LuckySeven, Artifacts.UnluckyFour }, Artifacts.HalfLucky47);

        public static ArtifactRecipe FallenCrystalBallRecipe = ArtifactRecipe.Create("fallen_crystal_ball",
            new() { Artifacts.CorruptedCrystalBall, Artifacts.MysteriousFleshBall }, Artifacts.FallenCrystalBall);

        public static ArtifactRecipe MalachiteDaggerRecipe = ArtifactRecipe.Create("malachite_dagger",
            new() { Artifacts.MalachiteVase, Artifacts.CopperDagger }, Artifacts.MalachiteDagger);

        public static ArtifactRecipe GoldenLoudspeakerRecipe = ArtifactRecipe.Create("golden_loudspeaker",
            new() { Artifacts.SilverLoudspeaker, Artifacts.GoldenOrizuru }, Artifacts.GoldenLoudspeaker);

        public static ArtifactRecipe PorcelainHalberdRecipe = ArtifactRecipe.Create("porcelain_halberd",
            new() { Artifacts.PorcelainSpear, Artifacts.PorcelainSword }, Artifacts.PorcelainHalberd);

        public static ArtifactRecipe XiaoqianOrizuluRecipe = ArtifactRecipe.Create("xiaoqian_orizulu",
            new() { Artifacts.SymbolSmall, Artifacts.WoodenFish, Artifacts.BaseOrizuru }, Artifacts.XiaoqianOrizulu);

        public static ArtifactRecipe DaqianOrizuluRecipe = ArtifactRecipe.Create("daqian_orizulu",
            new() { Artifacts.SymbolBig, Artifacts.WoodenFish, Artifacts.BaseOrizuru }, Artifacts.DaqianOrizulu);

        public static ArtifactRecipe TrinityForceRecipe = ArtifactRecipe.Create("trinity_force",
            new() { Artifacts.SymbolBig, Artifacts.SymbolMid, Artifacts.SymbolSmall }, Artifacts.TrinityForce);

        public static ArtifactRecipe CopperLionRecipe = ArtifactRecipe.Create("copper_lion",
            new() { Artifacts.CopperStatue, Artifacts.StoneLion }, Artifacts.CopperLion);

        public static ArtifactRecipe BigEncyclopediaRecipe = ArtifactRecipe.Create("big_encycliopedia",
            new() { Artifacts.Encyclopedia, Artifacts.SymbolBig }, Artifacts.BigEncyclopedia);

        public static ArtifactRecipe CheatingWeightRecipe = ArtifactRecipe.Create("cheating_weight",
            new() { Artifacts.Weight, Artifacts.SilverIngot }, Artifacts.CheatingWeight);

        public static ArtifactRecipe FiveSealingFuluRecipe = ArtifactRecipe.Create("ghost_of_five_fulu",
            new() { Artifacts.GhostFulu, Artifacts.PeachWoodSword }, Artifacts.FiveSealingFulu);
        
        public static ArtifactRecipe FirePouchRecipe = ArtifactRecipe.Create("fire_pouch",
            new() { Artifacts.RedBag, Artifacts.RedOrizuru }, Artifacts.FirePatternPouch);

        public static List<ArtifactRecipe> recipes = new()
        {
            TricolorOrizuluRecipe,
            YinYangNunchakuRecipe,
            WildcardWindvaneRecipe,
            ClimbingKitRecipe,
            BlackHoleRecipe,
            HalfLucky47Recipe,
            FallenCrystalBallRecipe,
            MalachiteDaggerRecipe,
            GoldenLoudspeakerRecipe,
            PorcelainHalberdRecipe,
            XiaoqianOrizuluRecipe,
            DaqianOrizuluRecipe,
            TrinityForceRecipe,
            CopperLionRecipe,
            BigEncyclopediaRecipe,
            FiveSealingFuluRecipe,
            FirePouchRecipe
        };
    }
}