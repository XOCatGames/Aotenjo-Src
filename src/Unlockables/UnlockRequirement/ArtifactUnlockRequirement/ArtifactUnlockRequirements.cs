using System.Collections.Generic;
using Aotenjo;

public class ArtifactUnlockRequirements
{
    public static Dictionary<Artifact, UnlockRequirement> artifactRequirementMap = new()
    {
        //通过北风北解锁
        { Artifacts.CakeExpert, UnlockRequirement.UnlockByLevel(0, 16) },
        { Artifacts.Shredder, UnlockRequirement.UnlockByLevel(0, 16) },
        { Artifacts.WoodenFish, UnlockRequirement.UnlockByLevel(0, 16) },

        //乐器全家桶（除了琵琶）
        { Artifacts.TuningBianzhong, UnlockRequirement.UnlockByLevel(0, 16) },
        { Artifacts.GuqinArtifact, UnlockRequirement.UnlockByLevel(0, 16) },
        { Artifacts.GuzhengArtifact, UnlockRequirement.UnlockByLevel(0, 16) },

        //通过蓝色麻将北风北解锁拼图全家桶
        { Artifacts.Jigsaw2, UnlockRequirement.UnlockByLevel(0, 8, MahjongDeck.BlueDeck) },
        { Artifacts.Jigsaw12, UnlockRequirement.UnlockByLevel(0, 8, MahjongDeck.BlueDeck) },
        { Artifacts.Jigsaw20, UnlockRequirement.UnlockByLevel(0, 8, MahjongDeck.BlueDeck) },
        { Artifacts.PalmFan, UnlockRequirement.UnlockByLevel(0, 16, MahjongDeck.BlueDeck) },
        { Artifacts.VoidStele, UnlockRequirement.UnlockByLevel(0, 16, MahjongDeck.BlueDeck) },
        { Artifacts.FourColorGem, UnlockRequirement.UnlockByLevel(0, 16, MahjongDeck.BlueDeck) },

        { Artifacts.MalachiteVase, UnlockRequirement.UnlockByPlayTileAttribute(TileMaterial.COPPER, 20) },
        {
            Artifacts.IceAxe, UnlockRequirement.UnlockByPlayYaku(FixedYakuType.SanSeSanBuGao)
                .Or(UnlockRequirement.UnlockByPlayYaku(FixedYakuType.SanSeSanJieGao))
        },
        {
            Artifacts.NylonRope, UnlockRequirement.UnlockByPlayYaku(FixedYakuType.SanSeSanBuGao)
                .Or(UnlockRequirement.UnlockByPlayYaku(FixedYakuType.SanSeSanJieGao))
        },
        {
            Artifacts.TravelBag, UnlockRequirement.UnlockByPlayYaku(FixedYakuType.SanSeSanBuGao)
                .Or(UnlockRequirement.UnlockByPlayYaku(FixedYakuType.SanSeSanJieGao))
        },
        { Artifacts.AncientGoldCoin, UnlockRequirement.UnlockByMoneyEarned(500) },

        //通过竹骨麻将北风北解锁三神器
        { Artifacts.TotsukaBladeArtifact, UnlockRequirement.UnlockByLevel(0, 12, MahjongDeck.BambooDeck) },
        { Artifacts.YasakaniMagatama, UnlockRequirement.UnlockByLevel(0, 12, MahjongDeck.BambooDeck) },
        { Artifacts.YataMirror, UnlockRequirement.UnlockByLevel(0, 8, MahjongDeck.BambooDeck) },

        //弃置144张幺九牌后解锁
        { Artifacts.FrogToy, UnlockRequirement.UnlockByCustomStats("discard_yaojiu", 144) },
        { Artifacts.PrincessToy, UnlockRequirement.UnlockByCustomStats("discard_yaojiu", 144) },

        { Artifacts.BlueCrystalBall, UnlockRequirement.UnlockByPlayYaku(FixedYakuType.DuanYao, 72) },
        { Artifacts.PurpleCrystalBall, UnlockRequirement.UnlockByPlayYaku(FixedYakuType.QuanDaiYao, 36) },

        { Artifacts.Censer, UnlockRequirement.UnlockByCustomStats($"encounter_boss_{Bosses.Knowledgeless.name}", 1) },
        {
            Artifacts.VoidTeapot,
            UnlockRequirement.UnlockByCustomStats($"encounter_boss_{Bosses.Knowledgeless.name}", 2)
        },

        { Artifacts.MedusaAmulet, UnlockRequirement.UnlockByPlayYaku(FixedYakuType.ShuangTongZiKe) },

        { Artifacts.DemonStatue, UnlockRequirement.UnlockByCustomStats($"encounter_boss_{Bosses.Heartless.name}", 1) },
        { Artifacts.IronHen, UnlockRequirement.UnlockByCustomStats($"encounter_boss_{Bosses.Greedless.name}", 1) },

        {
            Artifacts.BloodyFilmRoll,
            UnlockRequirement.UnlockByCustomStats($"encounter_boss_{Bosses.Colorless.name}", 2)
        },
        { Artifacts.Misericorde, UnlockRequirement.UnlockByCustomStats($"encounter_boss_{Bosses.Colorless.name}", 1) },

        {
            Artifacts.CollectorsAmulet,
            UnlockRequirement.UnlockByCustomStats($"encounter_boss_{Bosses.Undefeatable.name}", 1)
        },
        { Artifacts.TwinOrizulu, UnlockRequirement.UnlockByCustomStats($"encounter_boss_{Bosses.Timeless.name}", 1) },
        { Artifacts.BambooShoot, UnlockRequirement.UnlockByCustomStats($"encounter_boss_{Bosses.Powerless.name}", 1) },

        { Artifacts.ScarletKaleidoscope, UnlockRequirement.UnlockByLevel(0, 16, MahjongDeck.ScarletDeck) },
        { Artifacts.RustChest, UnlockRequirement.UnlockByCustomStats("discard_abandoned_suit", 288) },
        { Artifacts.RustAxe, UnlockRequirement.UnlockByLevel(0, 8, MahjongDeck.ScarletDeck) },
    };
}