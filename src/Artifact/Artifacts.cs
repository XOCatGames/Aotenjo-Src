using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    public static class Artifacts
    {

        #region 通用遗物
        
        //形似“外界”文字的小巧玩意儿，对部分灵韵有共鸣
        public static readonly Artifact SymbolBig = Artifact.CreateOnBlockEffectArtifact("symbol_big", Rarity.COMMON,
            (_, _, block, lst) =>
            {
                if (block.All(a => a.IsNumbered() && a.GetOrder() >= 7)) lst.Add(ScoreEffect.AddFan(6, SymbolBig));
            }).SetHighlightRequirement((t, _) => t.IsNumbered() && t.GetOrder() >= 7);

        //形似“外界”文字的小巧玩意儿，对纯灵有共鸣
        public static readonly Artifact SymbolMid = Artifact.CreateOnBlockEffectArtifact("symbol_mid", Rarity.COMMON,
            (_, _, block, lst) =>
            {
                if (block.All(a => a.IsNumbered() && a.GetOrder() >= 4 && a.GetOrder() <= 6)) lst.Add(ScoreEffect.AddFan(6, SymbolMid));
            }).SetHighlightRequirement((a, _) => a.IsNumbered() && a.GetOrder() >= 4 && a.GetOrder() <= 6);

        //形似“外界”文字的小巧玩意儿，对部分灵韵有共鸣
        public static readonly Artifact SymbolSmall = Artifact.CreateOnBlockEffectArtifact("symbol_small", Rarity.COMMON,
            (_, _, block, lst) =>
            {
                if (block.All(a => a.IsNumbered() && a.GetOrder() <= 3)) lst.Add(ScoreEffect.AddFan(6, SymbolSmall));
            }).SetHighlightRequirement((a, _) => a.IsNumbered() && a.GetOrder() <= 3);

        //铜质的路灯, 似乎适合三指抓握
        public static readonly Artifact BronzeStreetlamp = Artifact.CreateOnTileEffectArtifact("bronze_street_lamp", Rarity.COMMON,
            (_, _, tile, lst) =>
            {
                if (tile.IsNumbered() && tile.GetOrder() % 3 == 0) lst.Add(ScoreEffect.AddFan(1, BronzeStreetlamp));
            }).SetHighlightRequirement((tile, _) => tile.IsNumbered() && tile.GetOrder() % 3 == 0);

        //五魁无所遁形
        public static readonly Artifact PeachWoodSword = Artifact.CreateOnBlockEffectArtifact("peach_wood_sword", Rarity.COMMON,
            (_, _, block, lst) =>
            {
                if (block.Any(a => a.IsNumbered() && a.GetOrder() == 5)) lst.Add(ScoreEffect.AddFan(4, PeachWoodSword));
            }).SetHighlightRequirement((tile, _) => tile.IsNumbered() && tile.GetOrder() == 5);
        
        //内有珍珠，西米露和仙草
        public static readonly Artifact TeaCup = Artifact.CreateOnTileEffectArtifact("tea_cup", Rarity.RARE,
            (_, _, tile, lst) =>
            {
                if (tile.CompactWithCategory(Tile.Category.Jian)) lst.Add(ScoreEffect.AddFu(30, TeaCup));
            }).SetHighlightRequirement((tile, _) => tile.CompactWithCategory(Tile.Category.Jian));

        //空空如也的竹门笔筒
        public static readonly Artifact BambooPenContainer = Artifact.CreateOnTileEffectArtifact("bamboo_pen_container", Rarity.COMMON,
            (_, _, tile, lst) =>
            {
                if (tile.CompactWithCategory(Tile.Category.Suo)) lst.Add(ScoreEffect.AddFu(10, BambooPenContainer));
            }).SetHighlightRequirement((tile, _) => tile.CompactWithCategory(Tile.Category.Suo));

        //特制的钱币，仅能在周派城镇寻得
        public static readonly Artifact CopperCoin = Artifact.CreateOnTileEffectArtifact("ancient_copper_coin", Rarity.COMMON,
            (_, _, tile, lst) =>
            {
                if (tile.CompactWithCategory(Tile.Category.Bing)) lst.Add(ScoreEffect.AddFu(8, CopperCoin));
            }).SetHighlightRequirement((tile, _) => tile.CompactWithCategory(Tile.Category.Bing));

        //奇怪的卡片，似有金钱于内
        public static readonly Artifact BankCard = Artifact.CreateOnTileEffectArtifact("bank_card", Rarity.COMMON,
            (_, _, tile, lst) =>
            {
                if (tile.CompactWithCategory(Tile.Category.Wan)) lst.Add(ScoreEffect.AddFu(12, BankCard));
            }).SetHighlightRequirement((tile, _) => tile.CompactWithCategory(Tile.Category.Wan));

        //普普通通的纸鹤，用尺子折制
        public static readonly Artifact BaseOrizuru = Artifact.CreateOnSelfEffectArtifact("orizulu", Rarity.COMMON,
            (_, _, lst) => lst.Add(ScoreEffect.AddFan(3, BaseOrizuru)));

        //纯白的纸鹤，似有一丝纯灵
        public static readonly Artifact WhiteOrizuru = Artifact.CreateOnSelfEffectArtifact("white_orizulu", Rarity.COMMON,
            (player, perm, lst) => {
                if (perm.JiangFulfillAll(t => !t.IsYaoJiu(player)))
                    lst.Add(ScoreEffect.AddFan(4, WhiteOrizuru));
            });

        //纯黑的纸鹤，略有一丝不详
        public static readonly Artifact BlackOrizuru = Artifact.CreateOnSelfEffectArtifact("black_orizulu", Rarity.COMMON,
            (player, perm, lst) => {
                if (perm.JiangFulfillAll(t => t.IsYaoJiu(player)))
                    lst.Add(ScoreEffect.AddFan(6, BlackOrizuru));
            });

        //纯红的纸鹤，颇为喜庆
        public static readonly Artifact RedOrizuru = Artifact.CreateOnSelfEffectArtifact("red_orizulu", Rarity.COMMON,
            (_, perm, lst) => {
                if (perm.JiangFulfillAll(t => t.GetCategory() == Tile.Category.Wan))
                    lst.Add(ScoreEffect.AddFan(5, RedOrizuru));
            });

        //蓝色的纸鹤，这种染料不多见
        public static readonly Artifact BlueOrizuru = Artifact.CreateOnSelfEffectArtifact("blue_orizulu", Rarity.COMMON,
            (_, perm, lst) => {
                if (perm.JiangFulfillAll(t => t.GetCategory() == Tile.Category.Bing))
                    lst.Add(new EarnMoneyEffect(1, BlueOrizuru));
            });

        //与草地颜色相仿的纸鹤，折痕崭新
        public static readonly Artifact GreenOrizuru = Artifact.CreateOnSelfEffectArtifact("green_orizulu", Rarity.COMMON,
            (_, perm, lst) => {
                if (perm.JiangFulfillAll(t => t.GetCategory() == Tile.Category.Suo))
                    lst.Add(ScoreEffect.AddFu(60, GreenOrizuru));
            });

        //散发杂灵的纸鹤，需要特殊的载体释放力量
        public static readonly Artifact GlitchedOrizuru = new GlitchedOrizulu()
            .SetHighlightRequirement((a, p) => a.IsHonor(p));

        //啊，迷人的金色纸鹤
        public static readonly Artifact GoldenOrizuru = new GoldenOrizuluArtifact();

        //包罗万象，三种颜色在其中交织
        public static readonly Artifact Kaleidoscope = Artifact.CreateOnSelfEffectArtifact("kaleidoscope", Rarity.EPIC,
            (_, perm, lst) =>
            {
                if (perm.ToTiles().Select(t => t.GetCategory()).Distinct().Count() == 3) lst.Add(ScoreEffect.MulFan(3, Kaleidoscope));
            });

        //万法宗制式千足银
        public static readonly Artifact SilverIngot = Artifact.CreateOnBlockEffectArtifact("silver_ingot", Rarity.RARE,
            (_, perm, _, lst) =>
            {
                if (perm.ToTiles().Where(t => t.IsNumbered()).Select(t => t.GetCategory()).Distinct().Count() == 1)
                    lst.Add(ScoreEffect.AddFan(6, SilverIngot));
            }).SetHighlightRequirement((tile, player) => {
                Permutation perm = player.GetAccumulatedPermutation();
                if (tile.IsHonor(player) || perm == null) return true;
                return perm.blocks.All(b => b.OfCategory(tile.GetCategory()));
            });

        //万法宗制式万足金
        public static readonly Artifact GoldIngot = Artifact.CreateOnSelfEffectArtifact("gold_ingot", Rarity.EPIC,
            (_, perm, lst) =>
            {
                if (perm.ToTiles().All(t => !t.IsNumbered())) return;
                if (perm.ToTiles().Select(t => t.GetCategory()).Distinct().Count() == 1) lst.Add(ScoreEffect.MulFan(2.5, GoldIngot));
            }).SetHighlightRequirement((tile, player) => {
                Permutation perm = player.GetAccumulatedPermutation();
                if (perm == null) return true;
                return tile.IsNumbered() && perm.blocks.All(b => b.OfCategory(tile.GetCategory()));
            });

        //怎么也推不倒
        public static readonly Artifact RolyPolyToy = Artifact.CreateOnTileEffectArtifact("roly_poly_toy", Rarity.COMMON,
            (_, _, tile, lst) =>
            {
                if (tile.IsRotationalSymmetric()) lst.Add(ScoreEffect.AddFu(10, RolyPolyToy));
            }).SetHighlightRequirement((a, _) => a.IsRotationalSymmetric());

        //老头和小孩专用
        public static readonly Artifact WoodenCrutch = Artifact.CreateOnBlockEffectArtifact("wooden_crutch", Rarity.COMMON,
            (player, _, block, lst) =>
            {
                if (block.IsAAA() && (block.All(a => a.IsYaoJiu(player))))
                {
                    lst.Add(ScoreEffect.AddFan(8, WoodenCrutch));
                }
            }).SetHighlightRequirement((a, player) => a.IsYaoJiu(player));

        //斧头帮的制式斧子，据说从不外传
        public static readonly Artifact FireAxe = new FireAxeArtifact();

        //精美的工艺品，眼神灵动
        public static readonly Artifact OrigamiBear = new OrigamiBearArtifact();

        //八颗葫芦一颗不少
        public static readonly Artifact Tanghulu = new TanghuluArtifact();

        //来自桃花源，一触碰就会变化颜色的花朵
        public static readonly Artifact TricolorFlower = new TricolorFlowerArtifact();

        //是怎样的异兽才会长出这样的羽毛？
        public static readonly Artifact BloodFeather = Artifact.CreateOnTileEffectArtifact("blood_feather", Rarity.RARE,
            (player, _, tile, lst) =>
            {
                if (tile.ContainsRed(player)) lst.Add(ScoreEffect.AddFu(12, BloodFeather));
            }).SetHighlightRequirement((a, player) => a.ContainsRed(player));

        //周派最后的遗产
        public static readonly Artifact Golden3Dots = new Golden3sArtifact()
            .SetHighlightRequirement((a, _) => a.CompactWithCategory(Tile.Category.Bing));

        //一直指向同一个位置，那是哪里？
        public static readonly Artifact Compass = Artifact.CreateOnBlockEffectArtifact("compass", Rarity.COMMON,
            (_, _, block, lst) =>
            {
                if (block.All(a => a.CompactWithCategory(Tile.Category.Feng))) lst.Add(ScoreEffect.AddFan(12, Compass));
            })
            .SetHighlightRequirement((a, _) => a.CompactWithCategory(Tile.Category.Feng));

        //可爱的陶瓷小猪
        public static readonly Artifact PiggyBank = new PiggyBankArtifact()
            .SetHighlightRequirement((a, _) => a.CompactWithCategory(Tile.Category.Bing));

        public static readonly Artifact Encyclopedia = new EncyclopediaArtifact();

        public static readonly Artifact Mower = new MowerArtifact();

        public static readonly Artifact TrashCan = new TrashCanArtifact();

        public static readonly Artifact GoldenScissor = new GoldenScissorArtifact();

        public static readonly Artifact LibraryCard = new LibraryCardArtifact();

        public static readonly Artifact SpringBoot = new SpringBootArtifact();

        public static readonly Artifact BicolorOrizulu = Artifact.CreateOnSelfEffectArtifact("bicolor_orizulu", Rarity.RARE,
            (player, perm, lst) =>
            {
                if (player.GetCurrentSelectedBlocks().Any(b =>
                        b.All(t => t.IsNumbered()) && perm.jiang.All(t => t.IsNumbered())
                            && b.GetCategory() != perm.jiang.GetCategory()))

                    lst.Add(ScoreEffect.MulFan(2, BicolorOrizulu));
            });

        public static readonly Artifact CopperMirror = new CopperMirrorArtifact();
        public static readonly Artifact CrystalMirror = new CrystalMirrorArtifact();
        public static readonly Artifact TwinOrizulu = new TwinOrizuluArtifact();
        public static readonly Artifact Brush = new BrushArtifact();

        public static readonly Artifact AllSeeingEye = Artifact.CreateOnSelfEffectArtifact("all_seeing_eye", Rarity.EPIC,
            (_, perm, lst) =>
            {
                if (perm.ToTiles().Select(t => t.GetCategory()).Distinct().Count() == 5) lst.Add(ScoreEffect.MulFan(5, AllSeeingEye));
            });

        public static readonly Artifact FrogToy = new FrogToyArtifact();
        public static readonly Artifact PrincessToy = new PrincessToyArtifact();

        public static readonly Artifact Leaf = Artifact.CreateOnSelfEffectArtifact("leaf", Rarity.COMMON,
            (_, perm, lst) =>
            {
                if (perm.blocks.All(b => !b.IsAAA())) lst.Add(ScoreEffect.AddFu(60, Leaf));
            });

        public static readonly Artifact BronzePocketWatch = new BronzePocketWatchArtifact();

        public static readonly Artifact CopperRing = Artifact.CreateOnSelfEffectArtifact("copper_ring", Rarity.COMMON,
            (_, perm, lst) =>
            {
                int count = perm.ToTiles().Where(t => t.IsNumbered()).Select(t => t.GetOrder()).Distinct().Count();
                lst.Add(ScoreEffect.AddFu(count * 8, CopperRing));
            }).SetHighlightRequirement((t, _) => t.IsNumbered());

        public static readonly Artifact SilverRing = Artifact.CreateOnSelfEffectArtifact("silver_ring", Rarity.RARE,
            (player, perm, lst) =>
            {
            if (perm.blocks.Any(a => a.IsABC() && perm.blocks.Any(b => a != b && b.IsABC() && player.GetCombinator().ASuccB(b.tiles[0], a.tiles[2], false))))
                    lst.Add(ScoreEffect.AddFan(12, SilverRing));
            });

        public static readonly Artifact LeadBlock = Artifact.CreateOnBlockEffectArtifact("lead_block", Rarity.RARE,
            (_, _, _, lst) =>
            {
                lst.Add(ScoreEffect.AddFu(20, LeadBlock));
            });

        public static readonly Artifact D10Dice = Artifact.CreateOnSelfEffectArtifact("d10_dice", Rarity.COMMON,
            (player, _, lst) =>
            {
                int num = player.GenerateRandomInt(10) + 1;
                if (player.GetArtifacts().Contains(LeadBlock))
                {
                    lst.Add(new TextEffect("effect_leaded_name", D10Dice));
                    num = 10;
                }
                lst.Add(ScoreEffect.AddFan(num, D10Dice));
            });

        public static readonly Artifact CopperIngot = Artifact.CreateOnSelfEffectArtifact("copper_ingot", Rarity.COMMON,
            (_, perm, lst) =>
            {
                List<Tile> tiles = perm.ToTiles();
                for(int i = 0; i < 3; i++)
                {
                    if(tiles.All(t => t.GetCategory() != (Tile.Category)i))
                    {
                        lst.Add(ScoreEffect.AddFan(4, CopperIngot));
                        return;
                    }
                }
            }).SetHighlightRequirement((tile, player) => {
                Permutation perm = player.GetAccumulatedPermutation();
                if (tile.IsHonor(player) || perm == null) return true;
                if (perm.GetPermType() == PermutationType.THIRTEEN_ORPHANS) return false;
                return perm.blocks.Where(b => b.IsNumbered()).Select(b => b.GetCategory())
                    .Union(new[] { tile.GetCategory() })
                    .Distinct()
                    .Count() < 2;
            });

        public static readonly Artifact ExtractionForceps = new ExtractionForcepsArtifact();

        public static readonly Artifact JadeMirror = new JadeMirrorArtifact();
        public static readonly Artifact JadeRing = new JadeRingArtifact();

        public static readonly Artifact ZhouPiBook = new ZhouPiBookArtifact();
        public static readonly Artifact IceAxe = new IceAxeArtifact();
        public static readonly Artifact NylonRope = new NylonRopeArtifact();

        public static readonly Artifact Shredder = new ShredderArtifact();
        public static readonly Artifact Censer = new CenserArtifact();

        public static readonly Artifact VoidTeapot = new VoidTeapotArtifact();
        public static readonly Artifact TrainTicket = new TrainTicketArtifact();
        public static readonly Artifact Xiaolongbao = new XiaolongbaoArtifact();
        public static readonly Artifact LuckySeven = new LuckySevenArtifact();

        public static readonly Artifact PirateChest = new PirateChestArtifact();

        public static readonly Artifact CakeExpert = new CakeExpertArtifact();

        public static readonly Artifact BambooSegment = new BambooSegmentArtifact();

        public static readonly Artifact RedWood = Artifact.CreateOnTileEffectArtifact("red_wood", Rarity.EPIC, (_, _, tile, lst) =>
            {
                if (tile.properties.font.GetRegName() == TileFont.RED.GetRegName()) lst.Add(ScoreEffect.MulFan(1.2f, RedWood));
            }).SetHighlightRequirement((tile, _) => tile.properties.font.GetRegName() == TileFont.RED.GetRegName())
            .SetPrerequisite(player => player.GetAllTiles().Any(tile => tile.properties.font.GetRegName() == TileFont.RED.GetRegName()));

        public static readonly Artifact DemonStatue = new DemonStatueArtifact();
        public static readonly Artifact AmethystAmulet = new AmethystAmuletArtifact();

        public static readonly Artifact SichuanLianPu = new SichuanLianPuArtifact();

        public static readonly Artifact GoldenD4Dice = new GoldenD4DiceArtifact();

        public static readonly Artifact BlueCrystalBall = new BlueCrystalBallArtifact();

        public static readonly Artifact PurpleCrystalBall = new PurpleCrystalBallArtifact();

        public static readonly Artifact MobiusRing = Artifact.CreateOnTileEffectArtifact("mobius_ring", Rarity.COMMON,
            (_, _, tile, lst) =>
            {
                if (tile.IsNumbered() && tile.GetOrder() % 2 == 0)
                {
                    lst.Add(ScoreEffect.AddFu(12, MobiusRing));
                }
            }).SetHighlightRequirement((tile, _) => tile.IsNumbered() && tile.GetOrder() % 2 == 0);

        public static readonly Artifact SkyBook = Artifact.CreateOnBlockEffectArtifact("sky_book", Rarity.COMMON,
            (player, perm, _, lst) =>
            {
                if (perm.ToTiles().All(t => !t.IsHonor(player)))
                {
                    lst.Add(ScoreEffect.AddFan(2, SkyBook));
                }
            }).SetHighlightRequirement((tile, _) => tile.IsNumbered());

        public static readonly Artifact GoldenDagger = new GoldenDaggerArtifact();

        //TODO: 需要改一下，改成变形之前
        public static readonly Artifact OpalDagger = Artifact.CreateOnTileEffectArtifact("opal_dagger", Rarity.RARE,
            (player, _, tile, lst) =>
            {
                if (tile.CompactWithMaterial(TileMaterial.Ore(), player))
                {
                    lst.Add(ScoreEffect.MulFan(2, OpalDagger));
                }
            })
            .SetHighlightRequirement((tile, player) => tile.CompactWithMaterial(TileMaterial.Ore(), player))
            .SetPrerequisite(player => player.GetAllTiles().Any(tile => tile.CompactWithMaterial(TileMaterial.Ore(), player)));

        public static readonly Artifact CopperDagger = new CopperDaggerArtifact();

        public static readonly Artifact AgateDagger = new AgateDaggerArtifact();

        public static readonly Artifact HatOfFortune = Artifact.CreateOnTileEffectArtifact("hat_of_fortune", Rarity.RARE,
            (_, _, tile, lst) =>
            {
                if (tile.GetOrder() > 4 && tile.GetCategory() == Tile.Category.Wan)
                {
                    lst.Add(ScoreEffect.AddFan(5, HatOfFortune));
                }
            }).SetHighlightRequirement((tile, _) => tile.GetCategory() == Tile.Category.Wan && tile.GetOrder() > 4);

        public static readonly Artifact AncientGoldCoin = new AncientGoldCoinArtifact();

        public static readonly Artifact MinerHat = new MinerHatArtifact()
            .SetHighlightRequirement((tile, player) => tile.CompactWithMaterial(TileMaterial.PLAIN, player));

        //刮风下雨，常伴左右
        public static readonly Artifact OilPaperUmbrella = new OilPaperUmbrellaArtifact();

        //再多好物事都装的下
        public static readonly Artifact ShoppingTrolley = new ShoppingTrolleyArtifact();

        public static readonly Artifact VermilionBirdFan = Artifact.CreateOnSelfEffectArtifact("vermilion_bird_fan", Rarity.RARE,
            (player, _, lst) =>
            {
                Artifact[] list = { Tanghulu, BambooWine, Mower, Xiaolongbao, FrogToy, HalfLucky47, UnluckyFour, Saw };
                int arc = list.Count(a => ((LevelingArtifact)a).Level == 0);
                int cnt = player.GetGadgets().Count(a => !a.IsConsumable() && a.uses == 0);

                for (int i = 0; i < arc; i++)
                {
                    lst.Add(ScoreEffect.MulFan(2f, VermilionBirdFan));
                }

                for (int i = 0; i < cnt; i++)
                {
                    lst.Add(ScoreEffect.MulFan(1.2f, VermilionBirdFan));
                }
            });

        public static readonly Artifact CollectorsAmulet = new CollectorsAmuletArtifact();

        public static readonly Jigsaw3Artifact Jigsaw2 = (Jigsaw3Artifact) new Jigsaw3Artifact()
            .SetHighlightRequirement((tile, _) => Jigsaw2.Level == 0 || (tile.GetOrder() == 2 && tile.IsNumbered()));
        public static readonly Jigsaw12Artifact Jigsaw12 = new();
        public static readonly Jigsaw19Artifact Jigsaw20 = new();

        public static readonly Artifact CopperStatue = Artifact.CreateOnTileEffectArtifact("copper_statue", Rarity.COMMON,
            (player, _, tile, lst) =>
            {
                if (!player.Selecting(tile)) return;
                if(tile.GetOrder() >= 7 && tile.GetCategory() == Tile.Category.Wan && tile.properties.material.GetRegName() != TileMaterial.COPPER.GetRegName())
                {
                    lst.Add(new TransformMaterialEffect(TileMaterial.COPPER, CopperStatue, tile, "effect_transform_copperize_name"));
                }
            })
            .SetHighlightRequirement((tile, _) => tile.GetOrder() >= 7 && tile.GetCategory() == Tile.Category.Wan);

        public static readonly Artifact SmoothCobblestone = Artifact.CreateOnBlockEffectArtifact("smooth_cobblestone", Rarity.COMMON,
            (player, _, block, lst) =>
            {
                if (block.IsAAA() && block.All(t => player.GetHandDeckCopy().Contains(t)))
                {
                    lst.Add(new IncreDiscardEffect("cobblestone_shining", SmoothCobblestone, 3));
                }
            });

        public static readonly Artifact MedusaAmulet = new MedusaAmuletArtifact();

        public static readonly Artifact StoneLion = Artifact.CreateOnBlockEffectArtifact("stone_lion", Rarity.COMMON,
            (_, _, block, lst) =>
            {
                if ((block.IsAAA()) && block.All(t => t.IsNumbered()))
                    lst.Add(ScoreEffect.AddFu(30, StoneLion));
            }).SetHighlightRequirement((tile, _) => tile.IsNumbered());

        public static readonly Artifact VoidStele = new VoidSteleArtifact();

        public static readonly Artifact WindVane = new WindVaneArtifact();

        public static readonly Artifact TuningBianzhong = Artifact.CreateOnSelfEffectArtifact("tuning_bianzhong", Rarity.RARE,
            (_, _, lst) =>
            {
                lst.Add(ScoreEffect.AddFan(8, TuningBianzhong));
            });

        public static readonly Artifact GuqinArtifact = new GuqinArtifact();
        public static readonly Artifact PipaArtifact = new PipaArtifact();
        public static readonly Artifact ErhuArtifact = new ErhuArtifact();
        public static readonly Artifact GuzhengArtifact = new GuzhengArtifact();

        public static readonly Artifact JokerArtifact = Artifact.CreateOnBlockEffectArtifact("joker", Rarity.COMMON,
            (player, perm, block, lst) =>
            {
                if (block.Any(a => !player.Selecting(a))) return;
                if (!block.IsAAA() || block.IsAAAA()) return;
                int amount = perm.JiangFulfillAll(t => t.IsSameCategory(block.tiles[0]))? 2 : 1;
                lst.Add(new EarnMoneyEffect(amount, JokerArtifact));
            });

        public static readonly Artifact AgateIdentification = Artifact.CreateOnSelfEffectArtifact("agate_identification", Rarity.RARE,
            (_, _, _) => {})
            .SetHighlightRequirement((tile, player) => tile.CompactWithMaterial(TileMaterial.Ore(), player));

        public static readonly Artifact MagnetiteSample = new MagnetiteSampleAritfact();

        public static readonly Artifact MalachiteVase = new MalachiteVaseArtifact()
            .SetHighlightRequirement((tile, player) => tile.CompactWithMaterial(TileMaterial.COPPER, player))
            .SetPrerequisite(player => player.GetAllTiles().Any(tile => tile.CompactWithMaterial(TileMaterial.COPPER, player)));

        public static readonly Artifact Toolbox = new ToolboxArtifact();

        public static readonly Artifact IvoryD3Dice = Artifact.CreateOnSelfEffectArtifact("ivory_d3_dice", Rarity.EPIC,
            (player, _, lst) =>
            {
                int num = player.GenerateRandomInt(3) + 1;
                if (player.GetArtifacts().Contains(LeadBlock))
                {
                    lst.Add(new TextEffect("effect_leaded_name", IvoryD3Dice));
                    num = 3;
                }
                lst.Add(ScoreEffect.MulFan(num, IvoryD3Dice));
            });
        
        public static readonly Artifact Diabolo = new DiaboloArtifact();

        public static readonly Artifact AncientScripture = Artifact.CreateOnBlockEffectArtifact("ancient_scripture", Rarity.RARE,
            (player, _, block, lst) =>
            {
                if (!block.IsAAA()) return;
                double amount = 20;
                if (block.IsAAAA()) amount *= 2;
                if (block.All(t => t.IsYaoJiu(player))) amount *= 2;
                lst.Add(ScoreEffect.AddFu(amount, AncientScripture));
            });

        public static readonly Artifact CopperLoudspeaker = new CopperLoudspeakerArtifact();
        
        public static readonly Artifact SilverLoudspeaker = new SilverLoudspeakerArtifact();
        
        public static readonly Artifact WoodenFish = new WoodenFishArtifact();
        
        public static readonly Artifact ClayBall = new ClayBallArtifact();
        
        public static readonly Artifact BambooWine = new BambooWineArtifact()
            .SetHighlightRequirement((tile, _) => tile.CompactWithCategory(Tile.Category.Suo));
        
        public static readonly Artifact LightningAmulet = new LightningAmuletArtifact();
        
        public static readonly Artifact LeaningTowerModel = new LeaningTowerModelArtifact();

        public static readonly Artifact Shuugi = Artifact.CreateOnTileEffectArtifact("shuugi", Rarity.RARE,
            (player, _, tile, lst) =>
            {
                if (!player.Selecting(tile)) return;
                if (tile.properties.font.GetRegName().Equals(TileFont.RED.GetRegName()))
                {
                    lst.Add(new EarnMoneyEffect(2, Shuugi));
                }
            })
            .SetHighlightRequirement((tile, _) => tile.properties.font.GetRegName() == TileFont.RED.GetRegName());

        public static readonly Artifact Ruler = new RulerArtifact();
        
        public static readonly Artifact Weight = Artifact.CreateOnSelfEffectArtifact("weight", Rarity.RARE,
            (_, perm, lst) =>
            {
                int num = perm.ToTiles().Where(t => t.CompactWithCategory(Tile.Category.Wan)).Select(t => t.GetOrder()).Sum();
                if (num > 60)
                {
                    lst.Add(ScoreEffect.MulFan(2, Weight));
                }
            })
            .SetHighlightRequirement((tile, _) => tile.CompactWithCategory(Tile.Category.Wan));

        public static readonly Artifact BabblingBook = new BabblingBookArtifact();
        
        public static readonly Artifact Sutra = new SutraArtifact();
        
        public static readonly Artifact BurningPact = new BurningPactArtifact();
        
        public static readonly Artifact TravelBag = new TravelBagArtifact();
        
        public static readonly Artifact GoldenScythe = new GoldenScytheArtifact();
        
        public static readonly Artifact LightningBall = new LightningBallArtifact();
        
        public static readonly Artifact IndigoPen = new IndigoPenArtifact();
        
        public static readonly Artifact SpringArm = new SpringArmArtifact();

        public static readonly Artifact MysteriousDice = new MysteriousDiceArtifact();
        
        //public readonly static Artifact JadeSeal = new JadeSealArtifact();

        //Artifacts related to Bamboo Deck
        
        public static readonly Artifact GoldenSeed = new GoldenSeedArtifact();
        
        public static readonly Artifact BambooShoot = new BambooShootArtifact();
        
        public static readonly Artifact Nunchaku = new NunchakuArtifact();
        
        public static readonly Artifact CinnabarPen = Artifact.CreateOnTileEffectArtifact("cinnabar_pen", Rarity.EPIC, (player, _, tile, effects) =>
        {
            if (!player.Selecting(tile)) return;
            if (tile.ContainsRed(player) && tile.properties.font.GetRegName() != TileFont.RED.GetRegName())
            {
                effects.Add(new TransformColorEffect(TileFont.RED, CinnabarPen, tile, "effect_dyed_name"));
            }
        }).SetHighlightRequirement((t,p) => t.ContainsRed(p));
        
        public static readonly Artifact IronHen = new IronHenArtifact();
        
        public static readonly ThreeDGlassesArtifact ThreeDGlasses = new ThreeDGlassesArtifact();
        
        public static readonly Artifact InkBottle = new InkBottleArtifact();
        
        public static readonly Artifact BloodyFilmRoll = new BloodyFilmRollArtifact();
        
        public static readonly Artifact Misericorde = new MisericordeArtifact();
        
        public static readonly Artifact PalmFan = Artifact.CreateOnSelfEffectArtifact("palm_fan", Rarity.COMMON,
            (player, perm, lst) =>
            {
                int amount = perm.GetYakus(player).Count(y => player.GetSkillSet().GetLevel(y) > 0 && YakuTester.InfoMap[y].GetYakuCategories().Contains(1));
                if(amount != 0)
                    lst.Add(new IncreDiscardEffect("palm_fan", PalmFan, amount));
            });
        
        public static readonly Artifact FourColorGem = new FourColorGemArtifact();
        
        public static readonly Artifact CrystalBookmark = new CrystalBookmarkArtifact();
        
        public static readonly Artifact MechanicalOrizulu = new MechanicalOrizuluArtifact()
            
            .SetHighlightRequirement((t, _) => t.IsNumbered() && (t.GetOrder() == 2 || t.GetOrder() == 5 || t.GetOrder() == 8));
        
        public static readonly Artifact Pruner = new PrunerArtifact();
        
        public static readonly Artifact UnluckyFour = new UnluckyFourArtifact().SetHighlightRequirement((t, _) => t.IsNumbered() && t.GetOrder() == 4);
        
        public static readonly Artifact BottomlessBag = new BottomlessBagArtifact();
        
        public static readonly Artifact Teppoudama = new ("teppoudama", Rarity.COMMON);
        
        public static readonly Artifact Observer = new ObserverArtifact();
        
        public static readonly Artifact KochSnowflake = new KochSnowflakeArtifact();
        
        public static readonly Artifact PaintBucket = new PaintBucketArtifact();

        public static readonly Artifact CopperLock = new CopperLockArtifact();

        public static readonly Artifact GoldenLock = new GoldenLockArtifact();

        public static readonly Artifact Magnifier = new MagnifierArtifact();

        public static readonly Artifact Dart = new DartArtifact();
        
        #endregion

        #region 瓷牌遗物


        public static readonly Artifact Heart5Wan = new Love5mArtifact()
            .SetHighlightRequirement((tile, _) => tile.CompactWithCategory(Tile.Category.Wan));

        public static readonly Artifact MysteriousCrate = new MysteriousCrateArtifact()
            .SetHighlightRequirement((tile, player) => tile.CompactWithMaterial(TileMaterial.MysteriousColorPorcelain(), player))
            .SetPrerequisite(p => p.GetAllTiles().Any(tile => tile.CompactWithMaterial(TileMaterial.MysteriousColorPorcelain(), p)));

        public static readonly Artifact PorcelainMirror = new PorcelainMirrorArtifact();
        
        public static readonly Artifact PorcelainScissor = new PorcelainScissorArtifact();
        
        public static readonly Artifact PorcelainSword = new PorcelainSwordArtifact();

        public static readonly Artifact LuoyangShovel = new LuoyangShovelArtifact();
        
        public static readonly Artifact PorcelainSpear = new PorcelainSpearArtifact();
        
        public static readonly Artifact PorcelainFish = new PorcelainFishArtifact()
            .SetPrerequisite(player => player.GetAllTiles()
                .Any(tile => tile.CompactWithMaterial(TileMaterial.PINK_PORCELAIN, player)));

        public static readonly Artifact BodhiSeed = new BodhiSeedArtifact()
            .SetPrerequisite(player => player.GetAllTiles()
                .Any(tile => tile.CompactWithMaterial(TileMaterial.BonePorcelain(), player)));

        public static readonly Artifact MysteriousScroll = new MysteriousScrollArtifact()
            .SetPrerequisite(player => player.GetAllTiles()
                .Any(tile => tile.IsYaoJiu(player)));
        

        #endregion

        #region 竹骨遗物
        
        public static readonly Artifact BountyList = new BountyListArtifact();
        
        public static readonly Artifact PrayerBeads = new PrayerBeadsArtifact();
        
        public static readonly Artifact TreasureMap = new TreasureMapArtifact();
        
        public static readonly Artifact BoneDragonBall = new BoneDragonBallArtifact();
        
        public static readonly Artifact GemDragonBall = new GemDragonBallArtifact();
        
        public static readonly Artifact Shamisen = new ShamisenInstrumentArtifact();
        
        public static readonly Artifact BoneDragonAmulet = new BoneDragonAmuletArtifact();
        
        public static readonly Artifact TotsukaBladeArtifact = new TotsukaBladeArtifact();
        
        public static readonly Artifact YasakaniMagatama = new YasakaniMagatamaArtifact();
        
        public static readonly Artifact YataMirror = new YataMirrorArtifact();
        
        public static readonly Artifact Geta = new GetaArtifact();
        
        public static readonly Artifact BambooTicket = new BambooTicketArtifact();
        
        public static readonly Artifact AntiDragonBook = new AntiDragonBookArtifact();

        #endregion

        #region 魔物遗物

        public static readonly Artifact VioletGoatHorn = new VioletGoatHornArtifact();
        
        public static readonly Artifact MaliciousSpray = new MaliciousSprayArtifact();
        
        public static readonly Artifact EssencePot = new EssencePotArtifact();
        
        public static readonly Artifact MiniTomb = new Artifact("mini_tomb", Rarity.COMMON)
            .SetHighlightRequirement((t, p) => p.DetermineMaterialCompactbility(t, TileMaterial.Ghost()))
            .SetPrerequisite(p => p.GetAllTiles().Any(t => p.DetermineMaterialCompactbility(t, TileMaterial.Taotie())));
        
        public static readonly Artifact GhostFulu = new GhostFuluArtifact();
        
        public static readonly Artifact MonsterBirthCertificate = new MonsterBirthCertificateArtifact();
        
        public static readonly Artifact MysteriousFleshBall = new MysteriousFleshBallArtifact();
        
        public static readonly Artifact CorruptedCrystalBall = new CorruptedCrystalBallArtifact();
        
        public static readonly Artifact SilverDogLeash = new Artifact("silver_dog_leash", Rarity.COMMON)
            .SetHighlightRequirement((t, p) => p.DetermineMaterialCompactbility(t, TileMaterial.Taotie()))
            .SetPrerequisite(p => p.GetAllTiles().Any(t => p.DetermineMaterialCompactbility(t, TileMaterial.Taotie())));
        
        public static readonly Artifact RainbowCheese = new RainbowCheeseArtifact();

        #endregion

        #region 五彩遗物
        
        public static readonly Artifact Honeysuckle = new HoneysuckleArtifact().SetHighlightRequirement((t, _) => t is FlowerTile);
        
        public static readonly Artifact Lantana = new LantanaArtifact().SetHighlightRequirement((t, _) => t is FlowerTile);
        
        public static readonly Artifact ForgetMeNot = new ForgetMeNotArtifact().SetHighlightRequirement((t, _) => t is FlowerTile);
        
        public static readonly Artifact Bellflower = new BellflowerArtifact().SetHighlightRequirement((t, _) => t is FlowerTile);
        
        public static readonly Artifact WaterLily = new WaterLilyArtifact().SetHighlightRequirement((t, _) => t is FlowerTile);
        
        public static readonly Artifact Fig = Artifact.CreateOnSelfEffectArtifact("fig", Rarity.COMMON,
            (player, _, lst) =>
            {
                if (player is not RainbowDeck.RainbowPlayer rainbowPlayer) return;
                if (!rainbowPlayer.PlayedFlowerTiles.Any()) lst.Add(ScoreEffect.AddFu(50, Fig));
            });
        
        public static readonly Artifact Daffodil = new DaffodilArtifact().SetHighlightRequirement((t, _) => t is FlowerTile);
        
        public static readonly Artifact ChinaRose = new ChinaRoseArtifact().SetHighlightRequirement((t, _) => t is FlowerTile);
        
        public static readonly Artifact Datura = new DaturaArtifact().SetHighlightRequirement((t, _) => t is FlowerTile);
        
        public static readonly Artifact Dandelion = new DandelionArtifact().SetHighlightRequirement((t, _) => t.GetCategory() == Tile.Category.Feng);

        #endregion

        #region 银河遗物
        
        public static readonly Artifact OuroborosNeckless = Artifact.CreateOnTileEffectArtifact("ouroboros_neckless", Rarity.COMMON, (p, perm, tile, lst) =>
        {
            if (!p.Selecting(tile)) return;
            int order = tile.GetOrder();
            if (order != 1 && order != 9) return;
            if (perm.blocks.Any(b => b.tiles.Any(t => t.GetOrder() == 1) && b.tiles.Any(t => t.GetOrder() == 9) && b.tiles.Any(t => t == tile)))
                lst.Add(new TransformColorEffect(TileFont.RED, OuroborosNeckless, tile, "effect_dyed_name"));
        });

        public static readonly Artifact StarRing = new StarRingArtifact();
        
        public static readonly Artifact LightPyramid = Artifact.CreateOnTileEffectArtifact("light_pyramid", Rarity.COMMON, (p, perm, t, lst) =>
        {
            if (!p.Selecting(t)) return;
            if (t.GetCategory() != Tile.Category.Feng) return;
            if(perm.ToTiles().Any(t2 => t2.GetOrder() == ((t.GetOrder() - 1 + 2) % 4) + 1))
            {
                lst.Add(new FractureEffect(LightPyramid, t));
            }
        });

        #endregion

        #region 锦囊遗物

        public static readonly Artifact HeatJade = new HeatJadeArtifact();
        public static readonly Artifact BlueBag = new BlueBagArtifact();
        public static readonly Artifact RedBag = new RedBagArtifact();
        public static readonly Artifact CloudFulu = new CloudFuluArtifact();
        public static readonly Artifact Drawstring = new DrawstringArtifact();
        public static readonly Artifact SpikyBag = new SpikyBagArtifact();
        public static readonly Artifact TeleportingOrizulu = new TeleportingOrizuluArtifact();
        public static readonly Artifact BagOfMarbles = new BagOfMarblesArtifact();
        public static readonly Artifact NeonBag = new NeonBagArtifact();
        public static readonly Artifact GemBackpack = new GemBackpackArtifact();
        public static readonly Artifact FirePatternPouch = new FirePatternPouchArtifact();

        #endregion

        #region 木牌遗物

        public static readonly Artifact RecyclingBin = new RecyclingBinArtifact();
        
        public static readonly Artifact DriftBottle = new DriftBottleArtifact();
        
        public static readonly Artifact GoldenThumbRing = new GoldenThumbRingArtifact();
        
        public static readonly Artifact WoodenTower = new WoodenTowerArtifact();
        
        public static readonly Artifact CopperThumbRing = new CopperThumbRingArtifact();
        
        public static readonly Artifact JadeThumbRing = new CrimsonThumbRingArtifact();
        
        public static readonly Artifact BambooBurin = new BambooBurinArtifact();
        
        public static readonly Artifact Saw = new SawArtifact();
        
        public static readonly Artifact WarpedFungus = new WarpedFungusArtifact();
        
        public static readonly Artifact FishingBasket = new FishingBasketArtifact();
        
        public static readonly Artifact BurntAmulet = new BurntAmuletArtifact();
        
        public static readonly Artifact LivelyBracelet = new EmeraldWoodBracelet();
        
        public static readonly Artifact BranchOfYggdrasil = new BranchOfYggdrasilArtifact();
        
        #endregion

        #region 可合成遗物

        public static readonly Artifact TricolorOrizulu = new TricolorOrizuluArtifact()
            .SetHighlightRequirement((t, p) => t.IsNumbered());

        public static readonly Artifact YinYangNunchaku = new YinYangNunchakuArtifact();
        
        public static readonly Artifact WildcardWindVane = new WildcardWindvaneArtifact();
        
        public static readonly Artifact ClimbingKit = new ClimbingKitArtifact();
        
        public static readonly BlackHoleArtifact BlackHole = new ();
        
        public static readonly Artifact HalfLucky47 = new HalfLucky47Artifact();
        
        public static readonly Artifact FallenCrystalBall = new FallenCrystalBallArtifact();
        
        public static readonly Artifact MalachiteDagger = new MalachiteDaggerArtifact();
        
        public static readonly Artifact GoldenLoudspeaker = new GoldenLoudspeakerArtifact();
        
        public static readonly Artifact PorcelainHalberd = new PorcelainHalberdArtifact();
        
        //public readonly static Artifact Shengsibu = new ShengsibuArtifact();
        
        public static readonly Artifact CheatingWeight = new CheatingWeightArtifact();
        
        public static readonly Artifact XiaoqianOrizulu = new XiaoqianOrizuluArtifact();
        
        public static readonly Artifact DaqianOrizulu = new DaqianOrizuluArtifact();
        
        public static readonly Artifact TrinityForce = new TrinityForceArtifact();
        
        public static readonly Artifact CopperLion = new CopperLionArtifact();
        
        public static readonly Artifact BigEncyclopedia = new BigEncyclopediaArtifact();
        
        public static readonly Artifact FiveSealingFulu = new FiveSealingFuluArtifact();
        
        #endregion

        #region 赤云遗物

        public static readonly Artifact BrocadeOrizulu = new BrocadeOrizuluArtifact();
        
        public static readonly Artifact BicolorFeather = new BicolorFeatherArtifact();
        
        public static readonly Artifact RustCrook = new RustCrookArtifact();
        
        public static readonly Artifact RustCopperRing = new RustCopperRingArtifact();
        
        public static readonly Artifact ScarletKaleidoscope = new ScarletKaleidoscopeArtifact();
        
        public static readonly Artifact ScarletHourglass = new ScarletHourglassArtifact();
        
        public static readonly Artifact RustChest = new RustChestArtifact();
        
        public static readonly ScarletCoreArtifact ScarletCore = new ScarletCoreArtifact();
        
        public static readonly Artifact DottledPig = Artifact.CreateOnBlockEffectArtifact("dottled_pig", Rarity.EPIC, (player, _, block, lst) =>
        {
            if(player is ScarletPlayer scarletPlayer)
            {
                if (scarletPlayer.IsCompatibleWithUnusedCategory(block.GetCategory()))
                {
                    lst.Add(ScoreEffect.MulFan(1.5D, DottledPig));
                }
            }
        });
        
        public static readonly Artifact RustAxe = new RustAxeArtifact();
        
        #endregion

        #region 阴阳遗物
        
        public static readonly Artifact SoulFlag = new SoulFlagArtifact();
        
        public static readonly Artifact SoulScythe = new SoulScytheArtifact();
        
        public static readonly Artifact YinYangMirror = new YinYangMirrorArtifact();
        
        public static readonly Artifact YinYangButterfly = new YinYangButterflyArtifact();
        
        public static readonly Artifact SoulBottle = new SoulBottleArtifact();
        
        public static readonly Artifact GoldenBell = new GoldenBellArtifact();
        
        public static readonly Artifact YinYangJadeFlute = new YinYangJadeFluteArtifact();
        
        #endregion

        #region 甜品遗物

        public static readonly Artifact BlueCandle = new BlueCandleArtifact();
        public static readonly Artifact Rosemary = new RosemaryArtifact();
        public static readonly Artifact BrownSugar = new BrownSugarArtifact();
        public static readonly Artifact GildedLionBowl = new GildedLionBowlArtifact();
        public static readonly Artifact IceBlade = new IceBladeArtifact();
        public static readonly Artifact MeteoriteKnife = new MeteoriteKnifeArtifact();
        public static readonly Artifact WaterPatternPouch = new WaterPatternPouchArtifact();
        public static readonly Artifact CakeKnife = new CakeKnifeArtifact();
        public static readonly Artifact LuckyCookie = new LuckyCookieArtifact();
        public static readonly Artifact Altar = new AltarArtifact();
        public static readonly Artifact TaotiePact = new TaotiePactArtifact();
        
        #endregion

        #region 葫芦麻将

        public static readonly Artifact PurpleGourd = new PurpleGourdArtifact();

        #endregion
        
        #region 圣旨麻将

        public static readonly Artifact BambooBook = new BambooBookArtifact();
        public static readonly Artifact WitherAmulet = new WitherAmuletArtifact();
        public static readonly Artifact ForbiddenAmulet = new ForbiddenAmuletArtifact();
        public static readonly Artifact StoneSword = new StoneSwordAmuletArtifact();
        public static readonly Artifact BattleDrum = new BattleDrumArtifact();
        public static readonly Artifact BicolorSilk = new BicolorSilkArtifact();
        public static readonly Artifact Hufu = new HufuArtifact();
        public static readonly Artifact CoinAmulet = new CoinAmuletArtifact();
        public static readonly Artifact UnknownAmulet = new UnknownAmuletArtifact();
        
        #endregion

        /// <summary>
        /// 遗物列表
        /// </summary>
        public static readonly List<Artifact> ArtifactList = new()
        {
            BambooPenContainer, SymbolBig, SymbolMid, SymbolSmall, BronzeStreetlamp, PeachWoodSword, TeaCup, CopperCoin, BankCard,
            BaseOrizuru, WhiteOrizuru, BlackOrizuru, RedOrizuru, BlueOrizuru, GreenOrizuru, GlitchedOrizuru, GoldenOrizuru, Kaleidoscope, SilverIngot,
            RolyPolyToy, WoodenCrutch, GoldIngot, FireAxe, OrigamiBear, Tanghulu, TricolorFlower, BloodFeather, Golden3Dots, Compass, PiggyBank, Encyclopedia,
            Mower, TrashCan, GoldenScissor, LibraryCard, SpringBoot, BicolorOrizulu, CopperMirror, CrystalMirror, TwinOrizulu, Brush, AllSeeingEye, FrogToy,
            PrincessToy, Leaf, BronzePocketWatch, CopperRing, SilverRing, LeadBlock, D10Dice, CopperIngot, ExtractionForceps, JadeMirror, JadeRing, ZhouPiBook,
            IceAxe, NylonRope, Shredder, Censer, VoidTeapot, TrainTicket, Xiaolongbao, LuckySeven, BambooSegment, CakeExpert, PirateChest
            , RedWood, DemonStatue, SichuanLianPu, OilPaperUmbrella, MobiusRing, SkyBook, GoldenD4Dice, HatOfFortune, AncientGoldCoin,
            GoldenDagger, OpalDagger, CopperDagger, AgateDagger, BlueCrystalBall, PurpleCrystalBall, MinerHat, ShoppingTrolley, VermilionBirdFan,
            CollectorsAmulet, Jigsaw2, Jigsaw12, Jigsaw20, CopperStatue, VoidStele, StoneLion, MedusaAmulet, SmoothCobblestone, WindVane,
            GuqinArtifact, TuningBianzhong, PipaArtifact, ErhuArtifact, GuzhengArtifact, JokerArtifact, AgateIdentification,
            MagnetiteSample, MalachiteVase, Toolbox, IvoryD3Dice, Heart5Wan, MysteriousCrate, PorcelainMirror, PorcelainScissor,
            CopperLoudspeaker, SilverLoudspeaker, GoldenLoudspeaker, PorcelainSword, LeaningTowerModel, LuoyangShovel, WoodenFish, ClayBall,
            BambooWine, BountyList, PrayerBeads, TreasureMap, BoneDragonBall, GemDragonBall, Shamisen, BoneDragonAmulet, TotsukaBladeArtifact,
            YasakaniMagatama, YataMirror, Shuugi, Ruler, Weight, BabblingBook, Sutra, BambooShoot, Nunchaku, CinnabarPen, LightningAmulet,
            VioletGoatHorn, MaliciousSpray, MiniTomb, GhostFulu, IronHen, MonsterBirthCertificate, ThreeDGlasses, InkBottle, BloodyFilmRoll,
            Misericorde, Honeysuckle, Lantana, ForgetMeNot, Daffodil, Fig, Bellflower, ChinaRose, WaterLily, PalmFan, FourColorGem, CrystalBookmark,
            MechanicalOrizulu, MysteriousFleshBall, CorruptedCrystalBall, Pruner, Geta, Datura, EssencePot, Dandelion, SilverDogLeash, RainbowCheese,
            StarRing, OuroborosNeckless, LightPyramid, UnluckyFour, BottomlessBag, HeatJade, BlueBag, RedBag, CloudFulu, Drawstring, TeleportingOrizulu,
            BagOfMarbles, NeonBag, GemBackpack, SpikyBag, Teppoudama, BambooTicket, Observer, AntiDragonBook, BurningPact, TravelBag, GoldenScythe,
            IndigoPen, LightningBall, PorcelainSpear, PorcelainFish, BodhiSeed, MysteriousScroll, Diabolo, AncientScripture, AmethystAmulet,
            SpringArm, RecyclingBin, DriftBottle, GoldenSeed, GoldenThumbRing, WoodenTower, CopperThumbRing, JadeThumbRing,
            BambooBurin, Saw, WarpedFungus, PaintBucket, FishingBasket, TricolorOrizulu, YinYangNunchaku, WildcardWindVane, ClimbingKit,
            BlackHole, HalfLucky47, FallenCrystalBall, MalachiteDagger, PorcelainHalberd, XiaoqianOrizulu, DaqianOrizulu, CopperLion,
            BigEncyclopedia, TrinityForce, BurntAmulet, LivelyBracelet, BranchOfYggdrasil, FiveSealingFulu, ScarletCore, BrocadeOrizulu, BicolorFeather,
            RustCrook, RustCopperRing, ScarletKaleidoscope, ScarletHourglass, RustChest, RustAxe, DottledPig, SoulFlag, SoulScythe, YinYangMirror, YinYangButterfly,
            SoulBottle, GoldenBell, YinYangJadeFlute, BlueCandle, Rosemary, BrownSugar, GildedLionBowl, IceBlade, MeteoriteKnife, WaterPatternPouch, CakeKnife,
            LuckyCookie, Altar, FirePatternPouch, TaotiePact, PurpleGourd, Dart, Magnifier, BambooBook, WitherAmulet, ForbiddenAmulet, StoneSword, BattleDrum, 
            BicolorSilk, Hufu, CoinAmulet, UnknownAmulet
        };

        public static readonly Dictionary<Artifact, int> ARTIFACT_SPRITE_ID_MAP = new()
        {
            { BambooPenContainer, 0 },
            { SymbolBig, 1 },
            { SymbolMid, 2 },
            { SymbolSmall, 3 },
            { BronzeStreetlamp , 4 },
            { PeachWoodSword, 5 },
            { TeaCup, 7 },
            { CopperCoin, 8 },
            { BankCard, 9 },
            { Kaleidoscope, 10 },
            { SilverIngot , 11 },
            { RolyPolyToy , 12 },
            { WoodenCrutch, 15 },
            { BaseOrizuru, 16 },
            { WhiteOrizuru, 17 },
            { BlackOrizuru , 18 },
            { RedOrizuru, 19 },
            { BlueOrizuru, 20 },
            { GreenOrizuru, 21 },
            { GlitchedOrizuru, 22 },
            { GoldIngot, 24 },
            { FireAxe, 30 },
            { OrigamiBear, 26 },
            { Tanghulu, 27 },
            { GoldenOrizuru, 23 },
            { TricolorFlower, 31 },
            { BloodFeather, 25 },
            { Golden3Dots, 14 },
            { Compass, 28 },
            { PiggyBank, 29 },
            { Encyclopedia, 36 },
            { Mower, 33 },
            { TrashCan, 34 },
            { SpringBoot, 35 },
            { GoldenScissor, 32 },
            { LibraryCard, 37 },
            { BicolorOrizulu, 41 },
            { CopperMirror, 44 },
            { CrystalMirror, 43 },
            { TwinOrizulu, 42 },
            { Brush, 55 },
            { AllSeeingEye, 48 },
            { FrogToy, 57 },
            { PrincessToy, 56 },
            { Leaf, 63 },
            { BronzePocketWatch, 62 },
            { CopperRing, 60 },
            { SilverRing, 61 },
            { LeadBlock, 46 },
            { D10Dice, 45 },
            { CopperIngot, 52 },
            { ZhouPiBook, 53},
            { ExtractionForceps, 54 },
            { JadeMirror, 58 },
            { JadeRing, 59 },
            { IceAxe, 64},
            { NylonRope, 76},
            { Shredder, 40 },
            { Censer, 38 },
            { VoidTeapot, 66 },
            { TrainTicket, 67 },
            { Xiaolongbao, 65 },
            { LuckySeven, 68 },
            { RedWood, 71 },
            { DemonStatue, 78 },
            { PirateChest, 79 },
            { CakeExpert, 70},
            { BambooSegment, 83 },
            { SichuanLianPu, 85 },
            { OilPaperUmbrella, 91},
            { GoldenD4Dice, 99 },
            { MobiusRing, 92 },
            { SkyBook, 72 },
            { HatOfFortune, 111 },
            { AncientGoldCoin, 109 },
            { GoldenDagger, 75 },
            { OpalDagger, 80 },
            { CopperDagger, 82 },
            { AgateDagger, 81 },
            { BlueCrystalBall, 96 },
            { PurpleCrystalBall, 95 },
            { MinerHat, 102 },
            { ShoppingTrolley, 108 },
            { VermilionBirdFan, 69 },
            { CollectorsAmulet, 103 },
            { Jigsaw2, 106 },
            { Jigsaw12, 104 },
            { Jigsaw20, 105 },
            { CopperStatue, 112 },
            { VoidStele, 115 },
            { SmoothCobblestone, 116 },
            { StoneLion, 114 },
            { MedusaAmulet, 113 },
            { WindVane, 120 },
            { GuqinArtifact, 124 },
            { TuningBianzhong, 123 },
            { PipaArtifact, 125 },
            { ErhuArtifact, 127 },
            { GuzhengArtifact, 126 },
            { JokerArtifact, 134 },
            { AgateIdentification, 132 },
            { MagnetiteSample, 131 },
            { MalachiteVase, 133 },
            { Toolbox, 135 },
            { IvoryD3Dice, 136 },
            { Heart5Wan, 143 },
            { MysteriousCrate, 145 },
            { PorcelainMirror, 146 },
            { PorcelainScissor, 148 },
            { CopperLoudspeaker, 137 },
            { SilverLoudspeaker, 138 },
            { GoldenLoudspeaker, 139 },
            { PorcelainSword, 144 },
            { AncientScripture, 212 },
            { LeaningTowerModel, 100 },
            { LuoyangShovel, 151 },
            { WoodenFish, 156 },
            { ClayBall, 155 },
            { BambooWine, 154 },
            { Shuugi, 168 },
            { Ruler, 140 },
            { Weight, 169 },
            { BabblingBook, 170 },
            { Sutra, 171 },
            { BambooShoot, 152 },
            { Nunchaku, 176 },
            { CinnabarPen, 177 },
            { LightningAmulet, 153 },
            { PalmFan, 198 },
            { FourColorGem, 208 },
            { CrystalBookmark, 199 },
            { MechanicalOrizulu, 200 },
            { Pruner, 205 },
            { Observer, 235 },
            { GoldenScythe, 238 },
            { IndigoPen, 239 },
            { LightningBall, 232 },

            { BountyList, 157 },
            { TreasureMap, 158 },
            { PrayerBeads, 159 },
            { BoneDragonBall, 162 },
            { GemDragonBall, 160 },
            { Shamisen, 128 },
            { BoneDragonAmulet, 163 },
            { TotsukaBladeArtifact, 165 },
            { YasakaniMagatama, 166 },
            { YataMirror, 167 },
            { Geta, 204 },
            { BambooTicket, 230 },
            { AntiDragonBook, 231 },

            { Honeysuckle, 189 },
            { Lantana, 192 },
            { ForgetMeNot, 190 },
            { Daffodil, 195 },
            { Fig, 194 },
            { ChinaRose, 196 },
            { Bellflower, 193 },
            { WaterLily, 191 },
            { Datura, 206 },
            { Dandelion, 207 },

            { VioletGoatHorn, 173 },
            { MaliciousSpray, 178 },
            { MiniTomb, 179 },
            { GhostFulu, 175 },
            { IronHen, 174 },
            { MonsterBirthCertificate, 183 },
            { MysteriousFleshBall, 201 },
            { CorruptedCrystalBall, 202 },
            { EssencePot, 209 },
            { SilverDogLeash, 210 },
            { RainbowCheese, 211 },
            { UnluckyFour, 216 },
            { BottomlessBag, 217 },
            { Teppoudama, 229 },

            { ThreeDGlasses, 185 },
            { InkBottle, 186 },
            { BloodyFilmRoll, 187 },
            { Misericorde, 197 },
            { BurningPact, 237 },
            { TravelBag, 74 },

            { StarRing, 214 },
            { OuroborosNeckless, 213 },
            { LightPyramid, 215 },

            { HeatJade, 219 },
            { BlueBag, 218 },
            { RedBag, 221 },
            { CloudFulu, 222 },
            { Drawstring, 223 },
            { TeleportingOrizulu, 220 },
            { SpikyBag, 224 },
            { BagOfMarbles, 225 },
            { NeonBag, 227 },
            { GemBackpack, 228 },

            { PorcelainFish, 147 },
            { PorcelainSpear, 130 },
            { BodhiSeed, 234 },
            { MysteriousScroll, 188 },
            { Diabolo, 240 },
            { AmethystAmulet, 236 },
            { SpringArm, 243 },
            { GoldenSeed, 260 },

            //Craftable Artifacts
            { TricolorOrizulu, 269 },
            { YinYangNunchaku, 277 },
            { WildcardWindVane, 278 },
            { ClimbingKit, 280 },
            { BlackHole, 121 },
            { HalfLucky47, 77 },
            { FallenCrystalBall, 281 },
            { MalachiteDagger, 282 },
            { PorcelainHalberd, 283 },
            { XiaoqianOrizulu, 285 },
            { DaqianOrizulu, 284 },
            { CopperLion, 286 },
            { BigEncyclopedia, 279 },
            { TrinityForce, 288 },
            { FiveSealingFulu, 289 },

            { RecyclingBin, 247 },
            { DriftBottle, 248 },
            { GoldenThumbRing, 255 },
            { WoodenTower, 254 },
            { CopperThumbRing, 249 },
            { JadeThumbRing, 259 },
            { BambooBurin, 256 },
            { Saw, 250 },
            { WarpedFungus, 258 },
            { PaintBucket, 332 },
            { FishingBasket, 257 },
            { BurntAmulet, 262 },
            { LivelyBracelet, 268 },
            { BranchOfYggdrasil, 264 },
            { CheatingWeight, 289 },

            { ScarletCore, 292 },
            { BrocadeOrizulu, 299},
            { BicolorFeather, 291},
            { RustCrook, 290},
            { RustCopperRing, 304},
            { ScarletKaleidoscope, 303},
            { ScarletHourglass, 296},
            { RustChest, 301},
            { RustAxe, 302},
            { DottledPig, 297},

            { SoulFlag, 305},
            { SoulScythe, 306},
            { YinYangMirror, 308},
            { YinYangButterfly, 307},
            { SoulBottle, 310},
            { GoldenBell, 313},
            { YinYangJadeFlute, 311},

            { BlueCandle, 320},
            { Rosemary, 321},
            { BrownSugar, 322},
            { GildedLionBowl, 323},
            { IceBlade, 324},
            { MeteoriteKnife, 325},
            { WaterPatternPouch, 329},
            { CakeKnife, 327},
            { LuckyCookie, 318},
            { Altar, 316},
            { FirePatternPouch, 330 },
            { TaotiePact, 326 },
            
            { CopperLock, 336 },
            
            { GoldenLock, 338 },
            { Magnifier, 339 },
            { PurpleGourd, 340 },
            { Dart, 341 },
            { BambooBook, 348 },
            { WitherAmulet, 349 },
            { ForbiddenAmulet, 350 },
            { StoneSword, 351 },
            { BattleDrum, 352 },
            { BicolorSilk, 353 },
            { Hufu, 354 },
            { CoinAmulet, 355 },
            { UnknownAmulet, 356 }
        };

        public static Dictionary<string, Artifact> NameToArtifactMap;

        public static Artifact GetArtifact(string name)
        {
            if (NameToArtifactMap != null && NameToArtifactMap.ContainsKey(name)) return NameToArtifactMap.GetValueOrDefault(name, null);
            
            NameToArtifactMap = new Dictionary<string, Artifact>();
            foreach (var artifact in ArtifactList)
            {
                NameToArtifactMap.Add(artifact.GetNameID(), artifact);
            }
            
            return NameToArtifactMap.GetValueOrDefault(name, null);
        }
    }
}
