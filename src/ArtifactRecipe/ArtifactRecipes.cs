using System.Collections.Generic;

namespace Aotenjo
{
    public class ArtifactRecipes
    {
        public static ArtifactRecipe TricolorOrizuluRecipe = ArtifactRecipe.Create("tricolor_orizulu",
            new() { Artifacts.RedOrizuru.Id, Artifacts.BlueOrizuru.Id, Artifacts.GreenOrizuru.Id }, Artifacts.TricolorOrizulu.Id);

        public static ArtifactRecipe YinYangNunchakuRecipe = ArtifactRecipe.Create("yin_yang_nunchaku",
            new() { Artifacts.BlackOrizuru.Id, Artifacts.WhiteOrizuru.Id, Artifacts.Nunchaku.Id }, Artifacts.YinYangNunchaku.Id);

        public static ArtifactRecipe WildcardWindvaneRecipe = ArtifactRecipe.Create("wildcard_wind_vane",
            new() { Artifacts.WindVane.Id, Artifacts.MagnetiteSample.Id }, Artifacts.WildcardWindVane.Id);

        public static ArtifactRecipe ClimbingKitRecipe = ArtifactRecipe.Create("climbing_kit",
            new() { Artifacts.TravelBag.Id, Artifacts.IceAxe.Id }, Artifacts.ClimbingKit.Id);

        public static ArtifactRecipe BlackHoleRecipe = ArtifactRecipe.Create("black_hole",
            new() { Artifacts.BottomlessBag.Id, Artifacts.ZhouPiBook.Id }, Artifacts.BlackHole.Id);

        public static ArtifactRecipe HalfLucky47Recipe = ArtifactRecipe.Create("half_lucky_47",
            new() { Artifacts.LuckySeven.Id, Artifacts.UnluckyFour.Id }, Artifacts.HalfLucky47.Id);

        public static ArtifactRecipe FallenCrystalBallRecipe = ArtifactRecipe.Create("fallen_crystal_ball",
            new() { Artifacts.CorruptedCrystalBall.Id, Artifacts.MysteriousFleshBall.Id }, Artifacts.FallenCrystalBall.Id);

        public static ArtifactRecipe MalachiteDaggerRecipe = ArtifactRecipe.Create("malachite_dagger",
            new() { Artifacts.MalachiteVase.Id, Artifacts.CopperDagger.Id }, Artifacts.MalachiteDagger.Id);

        public static ArtifactRecipe GoldenLoudspeakerRecipe = ArtifactRecipe.Create("golden_loudspeaker",
            new() { Artifacts.SilverLoudspeaker.Id, Artifacts.GoldenOrizuru.Id }, Artifacts.GoldenLoudspeaker.Id);

        public static ArtifactRecipe PorcelainHalberdRecipe = ArtifactRecipe.Create("porcelain_halberd",
            new() { Artifacts.PorcelainSpear.Id, Artifacts.PorcelainSword.Id }, Artifacts.PorcelainHalberd.Id);

        public static ArtifactRecipe XiaoqianOrizuluRecipe = ArtifactRecipe.Create("xiaoqian_orizulu",
            new() { Artifacts.SymbolSmall.Id, Artifacts.WoodenFish.Id, Artifacts.BaseOrizuru.Id }, Artifacts.XiaoqianOrizulu.Id);

        public static ArtifactRecipe DaqianOrizuluRecipe = ArtifactRecipe.Create("daqian_orizulu",
            new() { Artifacts.SymbolBig.Id, Artifacts.WoodenFish.Id, Artifacts.BaseOrizuru.Id }, Artifacts.DaqianOrizulu.Id);

        public static ArtifactRecipe TrinityForceRecipe = ArtifactRecipe.Create("trinity_force",
            new() { Artifacts.SymbolBig.Id, Artifacts.SymbolMid.Id, Artifacts.SymbolSmall.Id }, Artifacts.TrinityForce.Id);

        public static ArtifactRecipe CopperLionRecipe = ArtifactRecipe.Create("copper_lion",
            new() { Artifacts.CopperStatue.Id, Artifacts.StoneLion.Id }, Artifacts.CopperLion.Id);

        public static ArtifactRecipe BigEncyclopediaRecipe = ArtifactRecipe.Create("big_encycliopedia",
            new() { Artifacts.Encyclopedia.Id, Artifacts.SymbolBig.Id }, Artifacts.BigEncyclopedia.Id);

        public static ArtifactRecipe CheatingWeightRecipe = ArtifactRecipe.Create("cheating_weight",
            new() { Artifacts.Weight.Id, Artifacts.SilverIngot.Id }, Artifacts.CheatingWeight.Id);

        public static ArtifactRecipe FiveSealingFuluRecipe = ArtifactRecipe.Create("ghost_of_five_fulu",
            new() { Artifacts.GhostFulu.Id, Artifacts.PeachWoodSword.Id }, Artifacts.FiveSealingFulu.Id);
        
        public static ArtifactRecipe FirePouchRecipe = ArtifactRecipe.Create("fire_pouch",
            new() { Artifacts.RedBag.Id, Artifacts.RedOrizuru.Id }, Artifacts.FirePatternPouch.Id);

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