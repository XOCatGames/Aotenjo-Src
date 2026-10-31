using System.Collections.Generic;
using Aotenjo;

public class ArtifactUnlockRequirements
{
    public static Dictionary<string, UnlockRequirement> artifactRequirementMap = new()
    {
        //通过北风北解锁
        { Artifacts.CakeExpert.Id, UnlockRequirement.UnlockByLevel(0, 16) },
        { Artifacts.Shredder.Id, UnlockRequirement.UnlockByLevel(0, 16) },
        { Artifacts.WoodenFish.Id, UnlockRequirement.UnlockByLevel(0, 16) },

        //乐器全家桶（除了琵琶）
        { Artifacts.TuningBianzhong.Id, UnlockRequirement.UnlockByLevel(0, 16) },
        { Artifacts.GuqinArtifact.Id, UnlockRequirement.UnlockByLevel(0, 16) },
        { Artifacts.GuzhengArtifact.Id, UnlockRequirement.UnlockByLevel(0, 16) },

        //通过蓝色麻将北风北解锁拼图全家桶
        { Artifacts.Jigsaw2.Id, UnlockRequirement.UnlockByLevel(0, 8, MahjongDeck.BlueDeck) },
        { Artifacts.Jigsaw12.Id, UnlockRequirement.UnlockByLevel(0, 8, MahjongDeck.BlueDeck) },
        { Artifacts.Jigsaw20.Id, UnlockRequirement.UnlockByLevel(0, 8, MahjongDeck.BlueDeck) },
        { Artifacts.PalmFan.Id, UnlockRequirement.UnlockByLevel(0, 16, MahjongDeck.BlueDeck) },
        { Artifacts.VoidStele.Id, UnlockRequirement.UnlockByLevel(0, 16, MahjongDeck.BlueDeck) },
        { Artifacts.FourColorGem.Id, UnlockRequirement.UnlockByLevel(0, 16, MahjongDeck.BlueDeck) },

        { Artifacts.MalachiteVase.Id, UnlockRequirement.UnlockByPlayTileAttribute(TileMaterial.COPPER, 20) },
        {
            Artifacts.IceAxe.Id, UnlockRequirement.UnlockByPlayYaku(FixedYakuType.SanSeSanBuGao)
                .Or(UnlockRequirement.UnlockByPlayYaku(FixedYakuType.SanSeSanJieGao))
        },
        {
            Artifacts.NylonRope.Id, UnlockRequirement.UnlockByPlayYaku(FixedYakuType.SanSeSanBuGao)
                .Or(UnlockRequirement.UnlockByPlayYaku(FixedYakuType.SanSeSanJieGao))
        },
        {
            Artifacts.TravelBag.Id, UnlockRequirement.UnlockByPlayYaku(FixedYakuType.SanSeSanBuGao)
                .Or(UnlockRequirement.UnlockByPlayYaku(FixedYakuType.SanSeSanJieGao))
        },
        { Artifacts.AncientGoldCoin.Id, UnlockRequirement.UnlockByMoneyEarned(500) },

        //通过竹骨麻将北风北解锁三神器
        { Artifacts.TotsukaBladeArtifact.Id, UnlockRequirement.UnlockByLevel(0, 12, MahjongDeck.BambooDeck) },
        { Artifacts.YasakaniMagatama.Id, UnlockRequirement.UnlockByLevel(0, 12, MahjongDeck.BambooDeck) },
        { Artifacts.YataMirror.Id, UnlockRequirement.UnlockByLevel(0, 8, MahjongDeck.BambooDeck) },

        //弃置144张幺九牌后解锁
        { Artifacts.FrogToy.Id, UnlockRequirement.UnlockByCustomStats("discard_yaojiu", 144) },
        { Artifacts.PrincessToy.Id, UnlockRequirement.UnlockByCustomStats("discard_yaojiu", 144) },

        { Artifacts.BlueCrystalBall.Id, UnlockRequirement.UnlockByPlayYaku(FixedYakuType.DuanYao, 72) },
        { Artifacts.PurpleCrystalBall.Id, UnlockRequirement.UnlockByPlayYaku(FixedYakuType.QuanDaiYao, 36) },

        { Artifacts.Censer.Id, UnlockRequirement.UnlockByCustomStats($"encounter_boss_{Bosses.Knowledgeless.name}", 1) },
        {
            Artifacts.VoidTeapot.Id,
            UnlockRequirement.UnlockByCustomStats($"encounter_boss_{Bosses.Knowledgeless.name}", 2)
        },

        { Artifacts.MedusaAmulet.Id, UnlockRequirement.UnlockByPlayYaku(FixedYakuType.ShuangTongZiKe) },

        { Artifacts.DemonStatue.Id, UnlockRequirement.UnlockByCustomStats($"encounter_boss_{Bosses.Heartless.name}", 1) },
        { Artifacts.IronHen.Id, UnlockRequirement.UnlockByCustomStats($"encounter_boss_{Bosses.Greedless.name}", 1) },

        {
            Artifacts.BloodyFilmRoll.Id,
            UnlockRequirement.UnlockByCustomStats($"encounter_boss_{Bosses.Colorless.name}", 2)
        },
        { Artifacts.Misericorde.Id, UnlockRequirement.UnlockByCustomStats($"encounter_boss_{Bosses.Colorless.name}", 1) },

        {
            Artifacts.CollectorsAmulet.Id,
            UnlockRequirement.UnlockByCustomStats($"encounter_boss_{Bosses.Undefeatable.name}", 1)
        },
        { Artifacts.TwinOrizulu.Id, UnlockRequirement.UnlockByCustomStats($"encounter_boss_{Bosses.Timeless.name}", 1) },
        { Artifacts.BambooShoot.Id, UnlockRequirement.UnlockByCustomStats($"encounter_boss_{Bosses.Powerless.name}", 1) },

        { Artifacts.ScarletKaleidoscope.Id, UnlockRequirement.UnlockByLevel(0, 16, MahjongDeck.ScarletDeck) },
        { Artifacts.RustChest.Id, UnlockRequirement.UnlockByCustomStats("discard_abandoned_suit", 288) },
        { Artifacts.RustAxe.Id, UnlockRequirement.UnlockByLevel(0, 8, MahjongDeck.ScarletDeck) },
    };
}