using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    public static class Artifacts
    {

        #region 通用遗物
        
        //形似“外界”文字的小巧玩意儿，对部分灵韵有共鸣
        public static readonly ArtifactRegistration SymbolBig = Register("symbol_big", () => new SymbolBigArtifact());

        //形似“外界”文字的小巧玩意儿，对纯灵有共鸣
        public static readonly ArtifactRegistration SymbolMid = Register("symbol_mid", () => new SymbolMidArtifact());

        //形似“外界”文字的小巧玩意儿，对部分灵韵有共鸣
        public static readonly ArtifactRegistration SymbolSmall = Register("symbol_small", () => new SymbolSmallArtifact());

        //铜质的路灯, 似乎适合三指抓握
        public static readonly ArtifactRegistration BronzeStreetlamp = Register("bronze_street_lamp", () => new BronzeStreetlampArtifact());

        //五魁无所遁形
        public static readonly ArtifactRegistration PeachWoodSword = Register("peach_wood_sword", () => new PeachWoodSwordArtifact());
        
        //内有珍珠，西米露和仙草
        public static readonly ArtifactRegistration TeaCup = Register("tea_cup", () => new TeaCupArtifact());

        //空空如也的竹门笔筒
        public static readonly ArtifactRegistration BambooPenContainer = Register("bamboo_pen_container", () => new BambooPenContainerArtifact());

        //特制的钱币，仅能在周派城镇寻得
        public static readonly ArtifactRegistration CopperCoin = Register("ancient_copper_coin", () => new CopperCoinArtifact());

        //奇怪的卡片，似有金钱于内
        public static readonly ArtifactRegistration BankCard = Register("bank_card", () => new BankCardArtifact());

        //普普通通的纸鹤，用尺子折制
        public static readonly ArtifactRegistration BaseOrizuru = Register("orizulu", () => new BaseOrizuruArtifact());

        //纯白的纸鹤，似有一丝纯灵
        public static readonly ArtifactRegistration WhiteOrizuru = Register("white_orizulu", () => new WhiteOrizuruArtifact());

        //纯黑的纸鹤，略有一丝不详
        public static readonly ArtifactRegistration BlackOrizuru = Register("black_orizulu", () => new BlackOrizuruArtifact());

        //纯红的纸鹤，颇为喜庆
        public static readonly ArtifactRegistration RedOrizuru = Register("red_orizulu", () => new RedOrizuruArtifact());

        //蓝色的纸鹤，这种染料不多见
        public static readonly ArtifactRegistration BlueOrizuru = Register("blue_orizulu", () => new BlueOrizuruArtifact());

        //与草地颜色相仿的纸鹤，折痕崭新
        public static readonly ArtifactRegistration GreenOrizuru = Register("green_orizulu", () => new GreenOrizuruArtifact());

        //散发杂灵的纸鹤，需要特殊的载体释放力量
        public static readonly ArtifactRegistration GlitchedOrizuru = Register("glitched_orizulu", () => new GlitchedOrizulu()
            .SetHighlightRequirement((a, p) => a.IsHonor(p)));

        //啊，迷人的金色纸鹤
        public static readonly ArtifactRegistration GoldenOrizuru = Register("golden_orizulu", () => new GoldenOrizuluArtifact());

        //包罗万象，三种颜色在其中交织
        public static readonly ArtifactRegistration Kaleidoscope = Register("kaleidoscope", () => new KaleidoscopeArtifact());

        //万法宗制式千足银
        public static readonly ArtifactRegistration SilverIngot = Register("silver_ingot", () => new SilverIngotArtifact());

        //万法宗制式万足金
        public static readonly ArtifactRegistration GoldIngot = Register("gold_ingot", () => new GoldIngotArtifact());

        //怎么也推不倒
        public static readonly ArtifactRegistration RolyPolyToy = Register("roly_poly_toy", () => new RolyPolyToyArtifact());

        //老头和小孩专用
        public static readonly ArtifactRegistration WoodenCrutch = Register("wooden_crutch", () => new WoodenCrutchArtifact());

        //斧头帮的制式斧子，据说从不外传
        public static readonly ArtifactRegistration FireAxe = Register("fire_axe", () => new FireAxeArtifact());

        //精美的工艺品，眼神灵动
        public static readonly ArtifactRegistration OrigamiBear = Register("origami_bear", () => new OrigamiBearArtifact());

        //八颗葫芦一颗不少
        public static readonly ArtifactRegistration Tanghulu = Register("tanghulu", () => new TanghuluArtifact());

        //来自桃花源，一触碰就会变化颜色的花朵
        public static readonly ArtifactRegistration TricolorFlower = Register("tricolor_flower", () => new TricolorFlowerArtifact());

        //是怎样的异兽才会长出这样的羽毛？
        public static readonly ArtifactRegistration BloodFeather = Register("blood_feather", () => new BloodFeatherArtifact());

        //周派最后的遗产
        public static readonly ArtifactRegistration Golden3Dots = Register("golden_3s", () => new Golden3sArtifact()
            .SetHighlightRequirement((a, _) => a.CompatWithCategory(Tile.Category.Bing)));

        //一直指向同一个位置，那是哪里？
        public static readonly ArtifactRegistration Compass = Register("compass", () => new CompassArtifact());

        //可爱的陶瓷小猪
        public static readonly ArtifactRegistration PiggyBank = Register("piggy_bank", () => new PiggyBankArtifact()
            .SetHighlightRequirement((a, _) => a.CompatWithCategory(Tile.Category.Bing)));

        public static readonly ArtifactRegistration Encyclopedia = Register("encyclopedia", () => new EncyclopediaArtifact());

        public static readonly ArtifactRegistration Mower = Register("mower", () => new MowerArtifact());

        public static readonly ArtifactRegistration TrashCan = Register("trash_can", () => new TrashCanArtifact());

        public static readonly ArtifactRegistration GoldenScissor = Register("golden_scissor", () => new GoldenScissorArtifact());

        public static readonly ArtifactRegistration LibraryCard = Register("committee_license", () => new LibraryCardArtifact());

        public static readonly ArtifactRegistration SpringBoot = Register("spring_boot", () => new SpringBootArtifact());

        public static readonly ArtifactRegistration BicolorOrizulu = Register("bicolor_orizulu", () => new BicolorOrizuluArtifact());

        public static readonly ArtifactRegistration CopperMirror = Register("copper_mirror", () => new CopperMirrorArtifact());
        public static readonly ArtifactRegistration CrystalMirror = Register("crystal_mirror", () => new CrystalMirrorArtifact());
        public static readonly ArtifactRegistration TwinOrizulu = Register("twin_orizulu", () => new TwinOrizuluArtifact());
        public static readonly ArtifactRegistration Brush = Register("brush", () => new BrushArtifact());

        public static readonly ArtifactRegistration AllSeeingEye = Register("all_seeing_eye", () => new AllSeeingEyeArtifact());

        public static readonly ArtifactRegistration FrogToy = Register("frog_toy", () => new FrogToyArtifact());
        public static readonly ArtifactRegistration PrincessToy = Register("princess_toy", () => new PrincessToyArtifact());

        public static readonly ArtifactRegistration Leaf = Register("leaf", () => new LeafArtifact());

        public static readonly ArtifactRegistration BronzePocketWatch = Register("bronze_pocket_watch", () => new BronzePocketWatchArtifact());

        public static readonly ArtifactRegistration CopperRing = Register("copper_ring", () => new CopperRingArtifact());

        public static readonly ArtifactRegistration SilverRing = Register("silver_ring", () => new SilverRingArtifact());

        public static readonly ArtifactRegistration LeadBlock = Register("lead_block", () => new LeadBlockArtifact());

        public static readonly ArtifactRegistration D10Dice = Register("d10_dice", () => new D10DiceArtifact());

        public static readonly ArtifactRegistration CopperIngot = Register("copper_ingot", () => new CopperIngotArtifact());

        public static readonly ArtifactRegistration ExtractionForceps = Register("extraction_forceps", () => new ExtractionForcepsArtifact());

        public static readonly ArtifactRegistration JadeMirror = Register("jade_mirror", () => new JadeMirrorArtifact());
        public static readonly ArtifactRegistration JadeRing = Register("jade_ring", () => new JadeRingArtifact());

        public static readonly ArtifactRegistration ZhouPiBook = Register("zhou_pi_book", () => new ZhouPiBookArtifact());
        public static readonly ArtifactRegistration IceAxe = Register("ice_axe", () => new IceAxeArtifact());
        public static readonly ArtifactRegistration NylonRope = Register("nylon_rope", () => new NylonRopeArtifact());

        public static readonly ArtifactRegistration Shredder = Register("shredder", () => new ShredderArtifact());
        public static readonly ArtifactRegistration Censer = Register("censer", () => new CenserArtifact());

        public static readonly ArtifactRegistration VoidTeapot = Register("void_teapot", () => new VoidTeapotArtifact());
        public static readonly ArtifactRegistration TrainTicket = Register("train_ticket", () => new TrainTicketArtifact());
        public static readonly ArtifactRegistration Xiaolongbao = Register("xiaolongbao", () => new XiaolongbaoArtifact());
        public static readonly ArtifactRegistration LuckySeven = Register("lucky_seven", () => new LuckySevenArtifact());

        public static readonly ArtifactRegistration PirateChest = Register("pirate_chest", () => new PirateChestArtifact());

        public static readonly ArtifactRegistration CakeExpert = Register("cake_expert", () => new CakeExpertArtifact());

        public static readonly ArtifactRegistration BambooSegment = Register("bamboo_segment", () => new BambooSegmentArtifact());

        public static readonly ArtifactRegistration RedWood = Register("red_wood", () => new RedWoodArtifact());

        public static readonly ArtifactRegistration DemonStatue = Register("demon_statue", () => new DemonStatueArtifact());
        public static readonly ArtifactRegistration AmethystAmulet = Register("amethyst_amulet", () => new AmethystAmuletArtifact());

        public static readonly ArtifactRegistration SichuanLianPu = Register("sichuan_lian_pu", () => new SichuanLianPuArtifact());

        public static readonly ArtifactRegistration GoldenD4Dice = Register("golden_d4_dice", () => new GoldenD4DiceArtifact());

        public static readonly ArtifactRegistration BlueCrystalBall = Register("blue_crystal_ball", () => new BlueCrystalBallArtifact());

        public static readonly ArtifactRegistration PurpleCrystalBall = Register("purple_crystal_ball", () => new PurpleCrystalBallArtifact());

        public static readonly ArtifactRegistration MobiusRing = Register("mobius_ring", () => new MobiusRingArtifact());

        public static readonly ArtifactRegistration SkyBook = Register("sky_book", () => new SkyBookArtifact());

        public static readonly ArtifactRegistration GoldenDagger = Register("golden_dagger", () => new GoldenDaggerArtifact());

        //TODO: 需要改一下，改成变形之前
        public static readonly ArtifactRegistration OpalDagger = Register("opal_dagger", () => new OpalDaggerArtifact());

        public static readonly ArtifactRegistration CopperDagger = Register("copper_dagger", () => new CopperDaggerArtifact());

        public static readonly ArtifactRegistration AgateDagger = Register("crystal_dagger", () => new AgateDaggerArtifact()
            .SetMaterialGift(TileMaterial.Agate, 5));

        public static readonly ArtifactRegistration HatOfFortune = Register("hat_of_fortune", () => new HatOfFortuneArtifact());

        public static readonly ArtifactRegistration AncientGoldCoin = Register("ancient_gold_coin", () => new AncientGoldCoinArtifact());

        public static readonly ArtifactRegistration MinerHat = Register("miner_hat", () => new MinerHatArtifact()
            .SetHighlightRequirement((tile, player) => tile.CompatWithMaterial(TileMaterial.PLAIN, player)));

        //刮风下雨，常伴左右
        public static readonly ArtifactRegistration OilPaperUmbrella = Register("oil_paper_umbrella", () => new OilPaperUmbrellaArtifact());

        //再多好物事都装的下
        public static readonly ArtifactRegistration ShoppingTrolley = Register("shopping_trolley", () => new ShoppingTrolleyArtifact());

        public static readonly ArtifactRegistration VermilionBirdFan = Register("vermilion_bird_fan", () => new VermilionBirdFanArtifact());

        public static readonly ArtifactRegistration CollectorsAmulet = Register("collectors_amulet", () => new CollectorsAmuletArtifact());

        public static readonly ArtifactRegistration Jigsaw2 = Register("jigsaw_3", () =>
        {
            var artifact = new Jigsaw3Artifact();
            return artifact.SetHighlightRequirement((tile, _) => artifact.Level == 0 || (tile.GetOrder() == 2 && tile.IsNumbered()));
        });
        public static readonly ArtifactRegistration Jigsaw12 = Register("jigsaw_12", () => new Jigsaw12Artifact());
        public static readonly ArtifactRegistration Jigsaw20 = Register("jigsaw_19", () => new Jigsaw19Artifact());

        public static readonly ArtifactRegistration CopperStatue = Register("copper_statue", () => new CopperStatueArtifact());

        public static readonly ArtifactRegistration SmoothCobblestone = Register("smooth_cobblestone", () => new SmoothCobblestoneArtifact());

        public static readonly ArtifactRegistration MedusaAmulet = Register("medusa_amulet", () => new MedusaAmuletArtifact());

        public static readonly ArtifactRegistration StoneLion = Register("stone_lion", () => new StoneLionArtifact());

        public static readonly ArtifactRegistration VoidStele = Register("void_stele", () => new VoidSteleArtifact());

        public static readonly ArtifactRegistration WindVane = Register("wind_vane", () => new WindVaneArtifact());

        public static readonly ArtifactRegistration TuningBianzhong = Register("tuning_bianzhong", () => new TuningBianzhongArtifact());

        public static readonly ArtifactRegistration GuqinArtifact = Register("guqin", () => new GuqinArtifact());
        public static readonly ArtifactRegistration PipaArtifact = Register("pipa", () => new PipaArtifact());
        public static readonly ArtifactRegistration ErhuArtifact = Register("erhu", () => new ErhuArtifact());
        public static readonly ArtifactRegistration GuzhengArtifact = Register("guzheng", () => new GuzhengArtifact());

        public static readonly ArtifactRegistration JokerArtifact = Register("joker", () => new JokerArtifact());

        public static readonly ArtifactRegistration AgateIdentification = Register("agate_identification", () => new AgateIdentificationArtifact());

        public static readonly ArtifactRegistration MagnetiteSample = Register("magnetite_sample", () => new MagnetiteSampleAritfact());

        public static readonly ArtifactRegistration MalachiteVase = Register("malachite_vase", () => new MalachiteVaseArtifact()
            .SetHighlightRequirement((tile, player) => tile.CompatWithMaterial(TileMaterial.COPPER, player))
            .SetMaterialGift(() => TileMaterial.COPPER, 5));

        public static readonly ArtifactRegistration Toolbox = Register("toolbox", () => new ToolboxArtifact());

        public static readonly ArtifactRegistration IvoryD3Dice = Register("ivory_d3_dice", () => new IvoryD3DiceArtifact());
        
        public static readonly ArtifactRegistration Diabolo = Register("diabolo", () => new DiaboloArtifact());

        public static readonly ArtifactRegistration AncientScripture = Register("ancient_scripture", () => new AncientScriptureArtifact());

        public static readonly ArtifactRegistration CopperLoudspeaker = Register("copper_loudspeaker", () => new CopperLoudspeakerArtifact());
        
        public static readonly ArtifactRegistration SilverLoudspeaker = Register("silver_loudspeaker", () => new SilverLoudspeakerArtifact());
        
        public static readonly ArtifactRegistration WoodenFish = Register("wooden_fish", () => new WoodenFishArtifact());
        
        public static readonly ArtifactRegistration ClayBall = Register("clay_ball", () => new ClayBallArtifact());
        
        public static readonly ArtifactRegistration BambooWine = Register("bamboo_wine", () => new BambooWineArtifact()
            .SetHighlightRequirement((tile, _) => tile.CompatWithCategory(Tile.Category.Suo)));
        
        public static readonly ArtifactRegistration LightningAmulet = Register("lightning_amulet", () => new LightningAmuletArtifact());
        
        public static readonly ArtifactRegistration LeaningTowerModel = Register("leaning_tower_model", () => new LeaningTowerModelArtifact());

        public static readonly ArtifactRegistration Shuugi = Register("shuugi", () => new ShuugiArtifact());

        public static readonly ArtifactRegistration Ruler = Register("ruler", () => new RulerArtifact());
        
        public static readonly ArtifactRegistration Weight = Register("weight", () => new WeightArtifact());

        public static readonly ArtifactRegistration BabblingBook = Register("babbling_book", () => new BabblingBookArtifact());

        public static readonly ArtifactRegistration Sutra = Register("sutra", () => new SutraArtifact());
        
        public static readonly ArtifactRegistration BurningPact = Register("burning_pact", () => new BurningPactArtifact());
        
        public static readonly ArtifactRegistration TravelBag = Register("travel_bag", () => new TravelBagArtifact());
        
        public static readonly ArtifactRegistration GoldenScythe = Register("golden_scythe", () => new GoldenScytheArtifact());
        
        public static readonly ArtifactRegistration LightningBall = Register("lightning_ball", () => new LightningBallArtifact());
        
        public static readonly ArtifactRegistration IndigoPen = Register("indigo_pen", () => new IndigoPenArtifact());
        
        public static readonly ArtifactRegistration SpringArm = Register("spring_arm", () => new SpringArmArtifact());

        public static readonly ArtifactRegistration MysteriousDice = Register("mysterious_dice", () => new MysteriousDiceArtifact());
        
        //public readonly static Artifact JadeSeal = new JadeSealArtifact();

        //Artifacts related to Bamboo Deck
        
        public static readonly ArtifactRegistration GoldenSeed = Register("golden_seed", () => new GoldenSeedArtifact());
        
        public static readonly ArtifactRegistration BambooShoot = Register("bamboo_shoot", () => new BambooShootArtifact());
        
        public static readonly ArtifactRegistration Nunchaku = Register("chainsticks", () => new NunchakuArtifact());
        
        public static readonly ArtifactRegistration CinnabarPen = Register("cinnabar_pen", () => new CinnabarPenArtifact());
        
        public static readonly ArtifactRegistration IronHen = Register("iron_hen", () => new IronHenArtifact());
        
        public static readonly ArtifactRegistration ThreeDGlasses = Register("3d_glasses", () => new ThreeDGlassesArtifact());
        
        public static readonly ArtifactRegistration InkBottle = Register("ink_bottle", () => new InkBottleArtifact());
        
        public static readonly ArtifactRegistration BloodyFilmRoll = Register("bloody_film_roll", () => new BloodyFilmRollArtifact()
            .SetFontGift(() => TileFont.COLORLESS, 5));
        
        public static readonly ArtifactRegistration Misericorde = Register("misericorde", () => new MisericordeArtifact());
        
        public static readonly ArtifactRegistration PalmFan = Register("palm_fan", () => new PalmFanArtifact());
        
        public static readonly ArtifactRegistration FourColorGem = Register("four_color_gem", () => new FourColorGemArtifact());
        
        public static readonly ArtifactRegistration CrystalBookmark = Register("crystal_bookmark", () => new CrystalBookmarkArtifact());
        
        public static readonly ArtifactRegistration MechanicalOrizulu = Register("mechanical_orizulu", () => new MechanicalOrizuluArtifact()
            
            .SetHighlightRequirement((t, _) => t.IsNumbered() && (t.GetOrder() == 2 || t.GetOrder() == 5 || t.GetOrder() == 8)));
        
        public static readonly ArtifactRegistration Pruner = Register("pruner", () => new PrunerArtifact());
        
        public static readonly ArtifactRegistration UnluckyFour = Register("unlucky_four", () => new UnluckyFourArtifact().SetHighlightRequirement((t, _) => t.IsNumbered() && t.GetOrder() == 4));
        
        public static readonly ArtifactRegistration BottomlessBag = Register("bottomless_bag", () => new BottomlessBagArtifact());
        
        public static readonly ArtifactRegistration Teppoudama = Register("teppoudama", () => new ("teppoudama", Rarity.COMMON));
        
        public static readonly ArtifactRegistration Observer = Register("observer", () => new ObserverArtifact());
        
        public static readonly ArtifactRegistration KochSnowflake = Register("koch_snowflake", () => new KochSnowflakeArtifact());
        
        public static readonly ArtifactRegistration PaintBucket = Register("paint_bucket", () => new PaintBucketArtifact());

        public static readonly ArtifactRegistration CopperLock = Register("copper_lock", () => new CopperLockArtifact());

        public static readonly ArtifactRegistration SilverLock = Register("silver_lock", () => new SilverLockArtifact());

        public static readonly ArtifactRegistration GoldenLock = Register("golden_lock", () => new GoldenLockArtifact());

        public static readonly ArtifactRegistration Magnifier = Register("magnifier", () => new MagnifierArtifact());

        public static readonly ArtifactRegistration Dart = Register("dart", () => new DartArtifact());
        
        #endregion

        #region 瓷牌遗物


        public static readonly ArtifactRegistration Heart5Wan = Register("heart_5m", () => new Love5mArtifact()
            .SetHighlightRequirement((tile, _) => tile.CompatWithCategory(Tile.Category.Wan)));

        public static readonly ArtifactRegistration MysteriousCrate = Register("mysterious_cassette", () => new MysteriousCrateArtifact()
            .SetHighlightRequirement((tile, player) => tile.CompatWithMaterial(TileMaterial.MysteriousColorPorcelain(), player))
            .SetMaterialGift(TileMaterial.MysteriousColorPorcelain, 5));

        public static readonly ArtifactRegistration PorcelainMirror = Register("porcelain_mirror", () => new PorcelainMirrorArtifact());
        
        public static readonly ArtifactRegistration PorcelainScissor = Register("porcelain_scissor", () => new PorcelainScissorArtifact());
        
        public static readonly ArtifactRegistration PorcelainSword = Register("porcelain_sword", () => new PorcelainSwordArtifact());

        public static readonly ArtifactRegistration LuoyangShovel = Register("luoyang_shovel", () => new LuoyangShovelArtifact());
        
        public static readonly ArtifactRegistration PorcelainSpear = Register("porcelain_spear", () => new PorcelainSpearArtifact());
        
        public static readonly ArtifactRegistration PorcelainFish = Register("porcelain_fish", () => new PorcelainFishArtifact()
            .SetMaterialGift(() => TileMaterial.PINK_PORCELAIN, 5));

        public static readonly ArtifactRegistration BodhiSeed = Register("bodhi_seed", () => new BodhiSeedArtifact()
            .SetMaterialGift(TileMaterial.BonePorcelain, 5));

        public static readonly ArtifactRegistration MysteriousScroll = Register("mysterious_scroll", () => new MysteriousScrollArtifact()
            .SetMaterialGift(TileMaterial.MysteriousColorPorcelain, 5));
        

        #endregion

        #region 竹骨遗物
        
        public static readonly ArtifactRegistration BountyList = Register("bounty_list", () => new BountyListArtifact());
        
        public static readonly ArtifactRegistration PrayerBeads = Register("prayer_beads", () => new PrayerBeadsArtifact());
        
        public static readonly ArtifactRegistration TreasureMap = Register("treasure_map", () => new TreasureMapArtifact());
        
        public static readonly ArtifactRegistration BoneDragonBall = Register("bone_dragon_ball", () => new BoneDragonBallArtifact());
        
        public static readonly ArtifactRegistration GemDragonBall = Register("gem_dragon_ball", () => new GemDragonBallArtifact());
        
        public static readonly ArtifactRegistration Shamisen = Register("shamisen", () => new ShamisenInstrumentArtifact());
        
        public static readonly ArtifactRegistration BoneDragonAmulet = Register("bone_dragon_amulet", () => new BoneDragonAmuletArtifact());
        
        public static readonly ArtifactRegistration TotsukaBladeArtifact = Register("totsuka_blade", () => new TotsukaBladeArtifact());
        
        public static readonly ArtifactRegistration YasakaniMagatama = Register("yasakani_magatama", () => new YasakaniMagatamaArtifact());
        
        public static readonly ArtifactRegistration YataMirror = Register("yata_mirror", () => new YataMirrorArtifact());
        
        public static readonly ArtifactRegistration Geta = Register("geta", () => new GetaArtifact());
        
        public static readonly ArtifactRegistration BambooTicket = Register("bamboo_ticket", () => new BambooTicketArtifact());
        
        public static readonly ArtifactRegistration AntiDragonBook = Register("antidragon_book", () => new AntiDragonBookArtifact());

        #endregion

        #region 魔物遗物

        public static readonly ArtifactRegistration VioletGoatHorn = Register("violet_goat_horn", () => new VioletGoatHornArtifact());
        
        public static readonly ArtifactRegistration MaliciousSpray = Register("malicious_spray", () => new MaliciousSprayArtifact());
        
        public static readonly ArtifactRegistration EssencePot = Register("essence_pot", () => new EssencePotArtifact()
            .SetMaterialGift(TileMaterial.Taotie, 5));
        
        public static readonly ArtifactRegistration MiniTomb = Register("mini_tomb", () => new Artifact("mini_tomb", Rarity.COMMON)
            .SetHighlightRequirement((t, p) => p.DetermineMaterialCompatibility(t, TileMaterial.Ghost()))
            .SetMaterialGift(TileMaterial.Ghost, 5));
        
        public static readonly ArtifactRegistration GhostFulu = Register("ghost_fulu", () => new GhostFuluArtifact());
        
        public static readonly ArtifactRegistration MonsterBirthCertificate = Register("monster_birth_certificate", () => new MonsterBirthCertificateArtifact());
        
        public static readonly ArtifactRegistration MysteriousFleshBall = Register("mysterious_flesh_ball", () => new MysteriousFleshBallArtifact());
        
        public static readonly ArtifactRegistration CorruptedCrystalBall = Register("corrupted_crystal_ball", () => new CorruptedCrystalBallArtifact()
            .SetMaterialGift(TileMaterial.Nest, 5));
        
        public static readonly ArtifactRegistration SilverDogLeash = Register("silver_dog_leash", () => new Artifact("silver_dog_leash", Rarity.COMMON)
            .SetHighlightRequirement((t, p) => p.DetermineMaterialCompatibility(t, TileMaterial.Taotie()))
            .SetMaterialGift(TileMaterial.Taotie, 5));
        
        public static readonly ArtifactRegistration RainbowCheese = Register("rainbow_cheese", () => new RainbowCheeseArtifact()
            .SetMaterialGift(TileMaterial.GoldMouse, 5));

        #endregion

        #region 五彩遗物
        
        public static readonly ArtifactRegistration Honeysuckle = Register("honeysuckle", () => new HoneysuckleArtifact().SetHighlightRequirement((t, _) => t is FlowerTile));
        
        public static readonly ArtifactRegistration Lantana = Register("lantana", () => new LantanaArtifact().SetHighlightRequirement((t, _) => t is FlowerTile));
        
        public static readonly ArtifactRegistration ForgetMeNot = Register("forget_me_not", () => new ForgetMeNotArtifact().SetHighlightRequirement((t, _) => t is FlowerTile));
        
        public static readonly ArtifactRegistration Bellflower = Register("bellflower", () => new BellflowerArtifact().SetHighlightRequirement((t, _) => t is FlowerTile));
        
        public static readonly ArtifactRegistration WaterLily = Register("water_lily", () => new WaterLilyArtifact().SetHighlightRequirement((t, _) => t is FlowerTile));
        
        public static readonly ArtifactRegistration Fig = Register("fig", () => new FigArtifact());
        
        public static readonly ArtifactRegistration Daffodil = Register("daffodil", () => new DaffodilArtifact().SetHighlightRequirement((t, _) => t is FlowerTile));
        
        public static readonly ArtifactRegistration ChinaRose = Register("china_rose", () => new ChinaRoseArtifact().SetHighlightRequirement((t, _) => t is FlowerTile));
        
        public static readonly ArtifactRegistration Datura = Register("datura", () => new DaturaArtifact().SetHighlightRequirement((t, _) => t is FlowerTile));
        
        public static readonly ArtifactRegistration Dandelion = Register("dandelion", () => new DandelionArtifact().SetHighlightRequirement((t, _) => t.GetCategory() == Tile.Category.Feng));

        #endregion

        #region 银河遗物
        
        public static readonly ArtifactRegistration OuroborosNeckless = Register("ouroboros_neckless", () => new OuroborosNecklessArtifact());

        public static readonly ArtifactRegistration StarRing = Register("star_ring", () => new StarRingArtifact());
        
        public static readonly ArtifactRegistration LightPyramid = Register("light_pyramid", () => new LightPyramidArtifact());

        #endregion

        #region 锦囊遗物

        public static readonly ArtifactRegistration HeatJade = Register("heat_jade", () => new HeatJadeArtifact());
        public static readonly ArtifactRegistration BlueBag = Register("blue_bag", () => new BlueBagArtifact());
        public static readonly ArtifactRegistration RedBag = Register("red_bag", () => new RedBagArtifact());
        public static readonly ArtifactRegistration CloudFulu = Register("cloud_fulu", () => new CloudFuluArtifact());
        public static readonly ArtifactRegistration Drawstring = Register("drawstring", () => new DrawstringArtifact());
        public static readonly ArtifactRegistration SpikyBag = Register("spiky_bag", () => new SpikyBagArtifact());
        public static readonly ArtifactRegistration TeleportingOrizulu = Register("teleporting_orizulu", () => new TeleportingOrizuluArtifact());
        public static readonly ArtifactRegistration BagOfMarbles = Register("bag_of_marbles", () => new BagOfMarblesArtifact());
        public static readonly ArtifactRegistration NeonBag = Register("neon_bag", () => new NeonBagArtifact());
        public static readonly ArtifactRegistration GemBackpack = Register("gem_backpack", () => new GemBackpackArtifact());
        public static readonly ArtifactRegistration FirePatternPouch = Register("fire_pattern_pouch", () => new FirePatternPouchArtifact());

        #endregion

        #region 木牌遗物

        public static readonly ArtifactRegistration RecyclingBin = Register("recycling_bin", () => new RecyclingBinArtifact());
        
        public static readonly ArtifactRegistration DriftBottle = Register("drift_bottle", () => new DriftBottleArtifact());
        
        public static readonly ArtifactRegistration GoldenThumbRing = Register("golden_thumb_ring", () => new GoldenThumbRingArtifact());
        
        public static readonly ArtifactRegistration WoodenTower = Register("wooden_tower", () => new WoodenTowerArtifact());
        
        public static readonly ArtifactRegistration CopperThumbRing = Register("copper_thumb_ring", () => new CopperThumbRingArtifact());
        
        public static readonly ArtifactRegistration JadeThumbRing = Register("jade_thumb_ring", () => new CrimsonThumbRingArtifact());
        
        public static readonly ArtifactRegistration BambooBurin = Register("bamboo_burin", () => new BambooBurinArtifact());
        
        public static readonly ArtifactRegistration Saw = Register("saw", () => new SawArtifact());
        
        public static readonly ArtifactRegistration WarpedFungus = Register("warped_fungus", () => new WarpedFungusArtifact());
        
        public static readonly ArtifactRegistration FishingBasket = Register("fishing_basket", () => new FishingBasketArtifact());
        
        public static readonly ArtifactRegistration BurntAmulet = Register("burnt_amulet", () => new BurntAmuletArtifact());
        
        public static readonly ArtifactRegistration LivelyBracelet = Register("emerald_wood_bracelet", () => new EmeraldWoodBracelet());
        
        public static readonly ArtifactRegistration BranchOfYggdrasil = Register("branch_of_yggdrasil", () => new BranchOfYggdrasilArtifact());
        
        #endregion

        #region 可合成遗物

        public static readonly ArtifactRegistration TricolorOrizulu = Register("tricolor_orizulu", () => new TricolorOrizuluArtifact()
            .SetHighlightRequirement((t, p) => t.IsNumbered()));

        public static readonly ArtifactRegistration YinYangNunchaku = Register("yin_yang_nunchaku", () => new YinYangNunchakuArtifact());
        
        public static readonly ArtifactRegistration WildcardWindVane = Register("wildcard_windvane", () => new WildcardWindVaneArtifact());
        
        public static readonly ArtifactRegistration ClimbingKit = Register("climbing_kit", () => new ClimbingKitArtifact());
        
        public static readonly ArtifactRegistration BlackHole = Register("black_hole", () => new BlackHoleArtifact());
        
        public static readonly ArtifactRegistration HalfLucky47 = Register("half_lucky_47", () => new HalfLucky47Artifact());
        
        public static readonly ArtifactRegistration FallenCrystalBall = Register("fallen_crystal_ball", () => new FallenCrystalBallArtifact());
        
        public static readonly ArtifactRegistration MalachiteDagger = Register("malachite_dagger", () => new MalachiteDaggerArtifact());
        
        public static readonly ArtifactRegistration GoldenLoudspeaker = Register("golden_loudspeaker", () => new GoldenLoudspeakerArtifact());
        
        public static readonly ArtifactRegistration PorcelainHalberd = Register("porcelain_halberd", () => new PorcelainHalberdArtifact());
        
        //public readonly static Artifact Shengsibu = new ShengsibuArtifact();
        
        public static readonly ArtifactRegistration CheatingWeight = Register("cheating_weight", () => new CheatingWeightArtifact());
        
        public static readonly ArtifactRegistration XiaoqianOrizulu = Register("xiaoqian_orizulu", () => new XiaoqianOrizuluArtifact());
        
        public static readonly ArtifactRegistration DaqianOrizulu = Register("daqian_orizulu", () => new DaqianOrizuluArtifact());
        
        public static readonly ArtifactRegistration TrinityForce = Register("trinity_force", () => new TrinityForceArtifact());
        
        public static readonly ArtifactRegistration CopperLion = Register("copper_lion", () => new CopperLionArtifact());
        
        public static readonly ArtifactRegistration BigEncyclopedia = Register("big_encyclopedia", () => new BigEncyclopediaArtifact());
        
        public static readonly ArtifactRegistration FiveSealingFulu = Register("ghost_of_five_fulu", () => new FiveSealingFuluArtifact());
        
        #endregion

        #region 赤云遗物

        public static readonly ArtifactRegistration BrocadeOrizulu = Register("brocade_orizulu", () => new BrocadeOrizuluArtifact());
        
        public static readonly ArtifactRegistration BicolorFeather = Register("bicolor_feather", () => new BicolorFeatherArtifact());
        
        public static readonly ArtifactRegistration RustCrook = Register("rust_crook", () => new RustCrookArtifact());
        
        public static readonly ArtifactRegistration RustCopperRing = Register("rust_copper_ring", () => new RustCopperRingArtifact());
        
        public static readonly ArtifactRegistration ScarletKaleidoscope = Register("scarlet_kaleidoscope", () => new ScarletKaleidoscopeArtifact());
        
        public static readonly ArtifactRegistration ScarletHourglass = Register("scarlet_hourglass", () => new ScarletHourglassArtifact());
        
        public static readonly ArtifactRegistration RustChest = Register("rust_chest", () => new RustChestArtifact());
        
        public static readonly ArtifactRegistration ScarletCore = Register("scarlet_core", () => new ScarletCoreArtifact());
        
        public static readonly ArtifactRegistration DottledPig = Register("dottled_pig", () => new DottledPigArtifact());
        
        public static readonly ArtifactRegistration RustAxe = Register("rust_axe", () => new RustAxeArtifact());
        
        #endregion

        #region 阴阳遗物
        
        public static readonly ArtifactRegistration SoulFlag = Register("soul_flag", () => new SoulFlagArtifact());
        
        public static readonly ArtifactRegistration SoulScythe = Register("soul_scythe", () => new SoulScytheArtifact());
        
        public static readonly ArtifactRegistration YinYangMirror = Register("yin_yang_mirror", () => new YinYangMirrorArtifact());
        
        public static readonly ArtifactRegistration YinYangButterfly = Register("yin_yang_butterfly", () => new YinYangButterflyArtifact());
        
        public static readonly ArtifactRegistration SoulBottle = Register("soul_bottle", () => new SoulBottleArtifact());
        
        public static readonly ArtifactRegistration GoldenBell = Register("golden_bell", () => new GoldenBellArtifact());
        
        public static readonly ArtifactRegistration YinYangJadeFlute = Register("yin_yang_jade_flute", () => new YinYangJadeFluteArtifact());
        
        #endregion

        #region 甜品遗物

        public static readonly ArtifactRegistration BlueCandle = Register("blue_candle", () => new BlueCandleArtifact());
        public static readonly ArtifactRegistration Rosemary = Register("rosemary", () => new RosemaryArtifact());
        public static readonly ArtifactRegistration BrownSugar = Register("brown_sugar", () => new BrownSugarArtifact());
        public static readonly ArtifactRegistration GildedLionBowl = Register("gilded_lion_bowl", () => new GildedLionBowlArtifact());
        public static readonly ArtifactRegistration IceBlade = Register("ice_blade", () => new IceBladeArtifact());
        public static readonly ArtifactRegistration MeteoriteKnife = Register("meteorite_knife", () => new MeteoriteKnifeArtifact());
        public static readonly ArtifactRegistration WaterPatternPouch = Register("water_pattern_pouch", () => new WaterPatternPouchArtifact());
        public static readonly ArtifactRegistration CakeKnife = Register("cake_knife", () => new CakeKnifeArtifact());
        public static readonly ArtifactRegistration LuckyCookie = Register("lucky_cookie", () => new LuckyCookieArtifact());
        public static readonly ArtifactRegistration Altar = Register("altar", () => new AltarArtifact());
        public static readonly ArtifactRegistration TaotiePact = Register("taotie_pact", () => new TaotiePactArtifact());
        
        #endregion

        #region 葫芦遗物

        public static readonly ArtifactRegistration PurpleGourd = Register("purple_gourd", () => new PurpleGourdArtifact());

        #endregion
        
        #region 圣旨遗物

        public static readonly ArtifactRegistration BambooBook = Register("bamboo_book", () => new BambooBookArtifact());
        public static readonly ArtifactRegistration WitherAmulet = Register("wither_amulet", () => new WitherAmuletArtifact());
        public static readonly ArtifactRegistration ForbiddenAmulet = Register("forbidden_amulet", () => new ForbiddenAmuletArtifact());
        public static readonly ArtifactRegistration StoneSword = Register("stone_sword", () => new StoneSwordAmuletArtifact());
        public static readonly ArtifactRegistration BattleDrum = Register("battle_drum", () => new BattleDrumArtifact());
        public static readonly ArtifactRegistration BicolorSilk = Register("bicolor_silk", () => new BicolorSilkArtifact());
        public static readonly ArtifactRegistration Hufu = Register("hufu", () => new HufuArtifact());
        public static readonly ArtifactRegistration CoinAmulet = Register("coin_amulet", () => new CoinAmuletArtifact());
        public static readonly ArtifactRegistration UnknownAmulet = Register("unknown_amulet", () => new UnknownAmuletArtifact());
        
        #endregion

        #region 机械遗物

        public static readonly ArtifactRegistration TwistingDevice = Register("twisting_device", () => new TwistingDeviceArtifact());
        public static readonly ArtifactRegistration Gearbox = Register("gearbox", () => new GearboxArtifact());
        public static readonly ArtifactRegistration TinfoilHat = Register("tinfoil_hat", () => new TinfoilHatArtifact());
        public static readonly ArtifactRegistration ConveyorBelt = Register("conveyor_belt", () => new ConveyorBeltArtifact());
        public static readonly ArtifactRegistration ClassifiedBlueprint = Register("classified_blueprint", () => new ClassifiedBlueprintArtifact());
        public static readonly ArtifactRegistration PhotovoltaicPanel = Register("photovoltaic_panel", () => new PhotovoltaicPanelArtifact());
        public static readonly ArtifactRegistration NetworkSwitch = Register("network_switch", () => new NetworkSwitchArtifact());
        public static readonly ArtifactRegistration MolecularReconstructor = Register("molecular_reconstructor", () => new MolecularReconstructorArtifact());
        public static readonly ArtifactRegistration FiberOptic = Register("fiber_optic", () => new FiberOpticArtifact());
        public static readonly ArtifactRegistration Blackbody = Register("blackbody", () => new BlackbodyArtifact());
        public static readonly ArtifactRegistration TriangularLampPost = Register("triangular_lamp_post", () => new TriangularLampPostArtifact());
        public static readonly ArtifactRegistration GreenScreen = Register("green_screen", () => new GreenScreenArtifact());
        public static readonly ArtifactRegistration ScrapBin = Register("scrap_bin", () => new ScrapBinArtifact());
        public static readonly ArtifactRegistration StepperMotor = Register("stepper_motor", () => new StepperMotorArtifact());
        public static readonly ArtifactRegistration PieMagnet = Register("pie_magnet", () => new PieMagnetArtifact());
        public static readonly ArtifactRegistration OverclockModule = Register("overclock_module", () => new OverclockModuleArtifact());
        public static readonly ArtifactRegistration WorkshopManual = Register("workshop_manual", () => new WorkshopManualArtifact());
        public static readonly ArtifactRegistration Lever = Register("lever", () => new LeverArtifact());
        public static readonly ArtifactRegistration ThreeProngTerminalStrip = Register("three_prong_terminal_strip", () => new ThreeProngTerminalStripArtifact());
        public static readonly ArtifactRegistration Pinata = Register("pinata", () => new PinataArtifact());
        public static readonly ArtifactRegistration Tanggu = Register("tanggu", () => new TangguArtifact());
        public static readonly ArtifactRegistration MechanicalD9 = Register("mechanical_d9", () => new MechanicalD9Artifact());
        public static readonly ArtifactRegistration LongBearing = Register("long_bearing", () => new LongBearingArtifact());
        public static readonly ArtifactRegistration Ecosphere = Register("ecosphere", () => new EcosphereArtifact());
        public static readonly ArtifactRegistration MechanicalPouch = Register("mechanical_pouch", () => new MechanicalPouchArtifact());

        #endregion

        public static readonly ArtifactRegistration PairMagnet = Register("pair_magnet", () => new PairMagnetArtifact());
        public static readonly ArtifactRegistration Parasite = Register("parasite", () => new ParasiteArtifact());

        /// <summary>
        /// 遗物列表
        /// </summary>
        internal static readonly ArtifactRegistration[] BuiltinEntries = new ArtifactRegistration[]
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
            BicolorSilk, Hufu, CoinAmulet, UnknownAmulet, PairMagnet, CopperLock, SilverLock, GoldenLock,
            TwistingDevice, Gearbox, TinfoilHat, ConveyorBelt, ClassifiedBlueprint, PhotovoltaicPanel,
            NetworkSwitch, MolecularReconstructor, FiberOptic, Blackbody, TriangularLampPost, GreenScreen,
            ScrapBin, StepperMotor, PieMagnet, OverclockModule, WorkshopManual, Lever,
            ThreeProngTerminalStrip, MechanicalD9, LongBearing, Ecosphere, MechanicalPouch, Parasite, Pinata, Tanggu
        };

        // 旧数字编号独立冻结，未来图片重排不能改变历史和旧档身份。
        public static readonly IReadOnlyDictionary<string, int> LEGACY_NUMBER_ID_MAP =
            new System.Collections.ObjectModel.ReadOnlyDictionary<string, int>(new Dictionary<string, int>
            {
                { "bamboo_pen_container", 0 },
                { "symbol_big", 1 },
                { "symbol_mid", 2 },
                { "symbol_small", 3 },
                { "bronze_street_lamp", 4 },
                { "silver_ingot", 11 },
                { "roly_poly_toy", 12 },
                { "black_orizulu", 18 },
                { "peach_wood_sword", 5 },
                { "tea_cup", 7 },
                { "ancient_copper_coin", 8 },
                { "bank_card", 9 },
                { "kaleidoscope", 10 },
                { "wooden_crutch", 15 },
                { "orizulu", 16 },
                { "white_orizulu", 17 },
                { "red_orizulu", 19 },
                { "blue_orizulu", 20 },
                { "green_orizulu", 21 },
                { "glitched_orizulu", 22 },
                { "gold_ingot", 24 },
                { "fire_axe", 30 },
                { "origami_bear", 26 },
                { "tanghulu", 27 },
                { "golden_orizulu", 23 },
                { "tricolor_flower", 31 },
                { "blood_feather", 25 },
                { "golden_3s", 14 },
                { "compass", 28 },
                { "piggy_bank", 29 },
                { "encyclopedia", 36 },
                { "mower", 33 },
                { "trash_can", 34 },
                { "spring_boot", 35 },
                { "golden_scissor", 32 },
                { "committee_license", 37 },
                { "bicolor_orizulu", 41 },
                { "copper_mirror", 44 },
                { "crystal_mirror", 43 },
                { "twin_orizulu", 42 },
                { "brush", 55 },
                { "all_seeing_eye", 48 },
                { "frog_toy", 57 },
                { "princess_toy", 56 },
                { "leaf", 63 },
                { "bronze_pocket_watch", 62 },
                { "copper_ring", 60 },
                { "silver_ring", 61 },
                { "lead_block", 46 },
                { "d10_dice", 45 },
                { "copper_ingot", 52 },
                { "zhou_pi_book", 53 },
                { "extraction_forceps", 54 },
                { "jade_mirror", 58 },
                { "jade_ring", 59 },
                { "ice_axe", 64 },
                { "nylon_rope", 76 },
                { "shredder", 40 },
                { "censer", 38 },
                { "void_teapot", 66 },
                { "train_ticket", 67 },
                { "xiaolongbao", 65 },
                { "lucky_seven", 68 },
                { "red_wood", 71 },
                { "demon_statue", 78 },
                { "pirate_chest", 79 },
                { "cake_expert", 70 },
                { "bamboo_segment", 83 },
                { "sichuan_lian_pu", 85 },
                { "oil_paper_umbrella", 91 },
                { "golden_d4_dice", 99 },
                { "mobius_ring", 92 },
                { "sky_book", 72 },
                { "hat_of_fortune", 111 },
                { "ancient_gold_coin", 109 },
                { "golden_dagger", 75 },
                { "opal_dagger", 80 },
                { "copper_dagger", 82 },
                { "crystal_dagger", 81 },
                { "blue_crystal_ball", 96 },
                { "purple_crystal_ball", 95 },
                { "miner_hat", 102 },
                { "shopping_trolley", 108 },
                { "vermilion_bird_fan", 69 },
                { "collectors_amulet", 103 },
                { "jigsaw_3", 106 },
                { "jigsaw_12", 104 },
                { "jigsaw_19", 105 },
                { "copper_statue", 112 },
                { "void_stele", 115 },
                { "smooth_cobblestone", 116 },
                { "stone_lion", 114 },
                { "medusa_amulet", 113 },
                { "wind_vane", 120 },
                { "guqin", 124 },
                { "tuning_bianzhong", 123 },
                { "pipa", 125 },
                { "erhu", 127 },
                { "guzheng", 126 },
                { "joker", 134 },
                { "agate_identification", 132 },
                { "magnetite_sample", 131 },
                { "malachite_vase", 133 },
                { "toolbox", 135 },
                { "ivory_d3_dice", 136 },
                { "heart_5m", 143 },
                { "mysterious_cassette", 145 },
                { "porcelain_mirror", 146 },
                { "porcelain_scissor", 148 },
                { "copper_loudspeaker", 137 },
                { "silver_loudspeaker", 138 },
                { "golden_loudspeaker", 139 },
                { "porcelain_sword", 144 },
                { "ancient_scripture", 212 },
                { "leaning_tower_model", 100 },
                { "luoyang_shovel", 151 },
                { "wooden_fish", 156 },
                { "clay_ball", 155 },
                { "bamboo_wine", 154 },
                { "shuugi", 168 },
                { "ruler", 140 },
                { "weight", 169 },
                { "babbling_book", 170 },
                { "sutra", 171 },
                { "bamboo_shoot", 152 },
                { "chainsticks", 176 },
                { "cinnabar_pen", 177 },
                { "lightning_amulet", 153 },
                { "palm_fan", 198 },
                { "four_color_gem", 208 },
                { "crystal_bookmark", 199 },
                { "mechanical_orizulu", 200 },
                { "pruner", 205 },
                { "observer", 235 },
                { "golden_scythe", 238 },
                { "indigo_pen", 239 },
                { "lightning_ball", 232 },
                { "bounty_list", 157 },
                { "treasure_map", 158 },
                { "prayer_beads", 159 },
                { "bone_dragon_ball", 162 },
                { "gem_dragon_ball", 160 },
                { "shamisen", 128 },
                { "bone_dragon_amulet", 163 },
                { "totsuka_blade", 165 },
                { "yasakani_magatama", 166 },
                { "yata_mirror", 167 },
                { "geta", 204 },
                { "bamboo_ticket", 230 },
                { "antidragon_book", 231 },
                { "honeysuckle", 189 },
                { "lantana", 192 },
                { "forget_me_not", 190 },
                { "daffodil", 195 },
                { "fig", 194 },
                { "china_rose", 196 },
                { "bellflower", 193 },
                { "water_lily", 191 },
                { "datura", 206 },
                { "dandelion", 207 },
                { "violet_goat_horn", 173 },
                { "malicious_spray", 178 },
                { "mini_tomb", 179 },
                { "ghost_fulu", 175 },
                { "iron_hen", 174 },
                { "monster_birth_certificate", 183 },
                { "mysterious_flesh_ball", 201 },
                { "corrupted_crystal_ball", 202 },
                { "essence_pot", 209 },
                { "silver_dog_leash", 210 },
                { "rainbow_cheese", 211 },
                { "unlucky_four", 216 },
                { "bottomless_bag", 217 },
                { "teppoudama", 229 },
                { "3d_glasses", 185 },
                { "ink_bottle", 186 },
                { "bloody_film_roll", 187 },
                { "misericorde", 197 },
                { "burning_pact", 237 },
                { "travel_bag", 74 },
                { "star_ring", 214 },
                { "ouroboros_neckless", 213 },
                { "light_pyramid", 215 },
                { "heat_jade", 219 },
                { "blue_bag", 218 },
                { "red_bag", 221 },
                { "cloud_fulu", 222 },
                { "drawstring", 223 },
                { "teleporting_orizulu", 220 },
                { "spiky_bag", 224 },
                { "bag_of_marbles", 225 },
                { "neon_bag", 227 },
                { "gem_backpack", 228 },
                { "porcelain_fish", 147 },
                { "porcelain_spear", 130 },
                { "bodhi_seed", 234 },
                { "mysterious_scroll", 188 },
                { "diabolo", 240 },
                { "amethyst_amulet", 236 },
                { "spring_arm", 243 },
                { "golden_seed", 260 },
                { "tricolor_orizulu", 269 },
                { "yin_yang_nunchaku", 277 },
                { "wildcard_windvane", 278 },
                { "climbing_kit", 280 },
                { "black_hole", 121 },
                { "half_lucky_47", 77 },
                { "fallen_crystal_ball", 281 },
                { "malachite_dagger", 282 },
                { "porcelain_halberd", 283 },
                { "xiaoqian_orizulu", 285 },
                { "daqian_orizulu", 284 },
                { "copper_lion", 286 },
                { "big_encyclopedia", 279 },
                { "trinity_force", 288 },
                { "ghost_of_five_fulu", 289 },
                { "recycling_bin", 247 },
                { "drift_bottle", 248 },
                { "golden_thumb_ring", 255 },
                { "wooden_tower", 254 },
                { "copper_thumb_ring", 249 },
                { "jade_thumb_ring", 259 },
                { "bamboo_burin", 256 },
                { "saw", 250 },
                { "warped_fungus", 258 },
                { "paint_bucket", 332 },
                { "fishing_basket", 257 },
                { "burnt_amulet", 262 },
                { "emerald_wood_bracelet", 268 },
                { "branch_of_yggdrasil", 264 },
                { "cheating_weight", 289 },
                { "scarlet_core", 292 },
                { "brocade_orizulu", 299 },
                { "bicolor_feather", 291 },
                { "rust_crook", 290 },
                { "rust_copper_ring", 304 },
                { "scarlet_kaleidoscope", 303 },
                { "scarlet_hourglass", 296 },
                { "rust_chest", 301 },
                { "rust_axe", 302 },
                { "dottled_pig", 297 },
                { "soul_flag", 305 },
                { "soul_scythe", 306 },
                { "yin_yang_mirror", 308 },
                { "yin_yang_butterfly", 307 },
                { "soul_bottle", 310 },
                { "golden_bell", 313 },
                { "yin_yang_jade_flute", 311 },
                { "blue_candle", 320 },
                { "rosemary", 321 },
                { "brown_sugar", 322 },
                { "gilded_lion_bowl", 323 },
                { "ice_blade", 324 },
                { "meteorite_knife", 325 },
                { "water_pattern_pouch", 329 },
                { "cake_knife", 327 },
                { "lucky_cookie", 318 },
                { "altar", 316 },
                { "fire_pattern_pouch", 330 },
                { "taotie_pact", 326 },
                { "copper_lock", 336 },
                { "silver_lock", 337 },
                { "golden_lock", 338 },
                { "magnifier", 339 },
                { "purple_gourd", 340 },
                { "dart", 341 },
                { "bamboo_book", 348 },
                { "wither_amulet", 349 },
                { "forbidden_amulet", 350 },
                { "stone_sword", 351 },
                { "battle_drum", 352 },
                { "bicolor_silk", 353 },
                { "hufu", 354 },
                { "coin_amulet", 355 },
                { "unknown_amulet", 356 },
                { "pair_magnet", 357 },
                { "twisting_device", 358 },
                { "gearbox", 380 },
                { "tinfoil_hat", 359 },
                { "conveyor_belt", 360 },
                { "classified_blueprint", 361 },
                { "photovoltaic_panel", 362 },
                { "network_switch", 363 },
                { "molecular_reconstructor", 379 },
                { "fiber_optic", 378 },
                { "blackbody", 364 },
                { "triangular_lamp_post", 365 },
                { "green_screen", 366 },
                { "scrap_bin", 367 },
                { "stepper_motor", 368 },
                { "pie_magnet", 369 },
                { "overclock_module", 370 },
                { "workshop_manual", 371 },
                { "lever", 372 },
                { "three_prong_terminal_strip", 373 },
                { "mechanical_d9", 374 },
                { "pinata", 382 },
                { "tanggu", 383 },
                { "long_bearing", 375 },
                { "ecosphere", 376 },
                { "mechanical_pouch", 377 },
                { "parasite", 381 },
            });

        public static readonly IReadOnlyDictionary<string, int> ARTIFACT_SPRITE_ID_MAP =
            new System.Collections.ObjectModel.ReadOnlyDictionary<string, int>(new Dictionary<string, int>
            {
                { "bamboo_pen_container", 0 },
                { "symbol_big", 1 },
                { "symbol_mid", 2 },
                { "symbol_small", 3 },
                { "bronze_street_lamp", 4 },
                { "silver_ingot", 11 },
                { "roly_poly_toy", 12 },
                { "black_orizulu", 18 },
                { "peach_wood_sword", 5 },
                { "tea_cup", 7 },
                { "ancient_copper_coin", 8 },
                { "bank_card", 9 },
                { "kaleidoscope", 10 },
                { "wooden_crutch", 15 },
                { "orizulu", 16 },
                { "white_orizulu", 17 },
                { "red_orizulu", 19 },
                { "blue_orizulu", 20 },
                { "green_orizulu", 21 },
                { "glitched_orizulu", 22 },
                { "gold_ingot", 24 },
                { "fire_axe", 30 },
                { "origami_bear", 26 },
                { "tanghulu", 27 },
                { "golden_orizulu", 23 },
                { "tricolor_flower", 31 },
                { "blood_feather", 25 },
                { "golden_3s", 14 },
                { "compass", 28 },
                { "piggy_bank", 29 },
                { "encyclopedia", 36 },
                { "mower", 33 },
                { "trash_can", 34 },
                { "spring_boot", 35 },
                { "golden_scissor", 32 },
                { "committee_license", 37 },
                { "bicolor_orizulu", 41 },
                { "copper_mirror", 44 },
                { "crystal_mirror", 43 },
                { "twin_orizulu", 42 },
                { "brush", 55 },
                { "all_seeing_eye", 48 },
                { "frog_toy", 57 },
                { "princess_toy", 56 },
                { "leaf", 63 },
                { "bronze_pocket_watch", 62 },
                { "copper_ring", 60 },
                { "silver_ring", 61 },
                { "lead_block", 46 },
                { "d10_dice", 45 },
                { "copper_ingot", 52 },
                { "zhou_pi_book", 53 },
                { "extraction_forceps", 54 },
                { "jade_mirror", 58 },
                { "jade_ring", 59 },
                { "ice_axe", 64 },
                { "nylon_rope", 76 },
                { "shredder", 40 },
                { "censer", 38 },
                { "void_teapot", 66 },
                { "train_ticket", 67 },
                { "xiaolongbao", 65 },
                { "lucky_seven", 68 },
                { "red_wood", 71 },
                { "demon_statue", 78 },
                { "pirate_chest", 79 },
                { "cake_expert", 70 },
                { "bamboo_segment", 83 },
                { "sichuan_lian_pu", 85 },
                { "oil_paper_umbrella", 91 },
                { "golden_d4_dice", 99 },
                { "mobius_ring", 92 },
                { "sky_book", 72 },
                { "hat_of_fortune", 111 },
                { "ancient_gold_coin", 109 },
                { "golden_dagger", 75 },
                { "opal_dagger", 80 },
                { "copper_dagger", 82 },
                { "crystal_dagger", 81 },
                { "blue_crystal_ball", 96 },
                { "purple_crystal_ball", 95 },
                { "miner_hat", 102 },
                { "shopping_trolley", 108 },
                { "vermilion_bird_fan", 69 },
                { "collectors_amulet", 103 },
                { "jigsaw_3", 106 },
                { "jigsaw_12", 104 },
                { "jigsaw_19", 105 },
                { "copper_statue", 112 },
                { "void_stele", 115 },
                { "smooth_cobblestone", 116 },
                { "stone_lion", 114 },
                { "medusa_amulet", 113 },
                { "wind_vane", 120 },
                { "guqin", 124 },
                { "tuning_bianzhong", 123 },
                { "pipa", 125 },
                { "erhu", 127 },
                { "guzheng", 126 },
                { "joker", 134 },
                { "agate_identification", 132 },
                { "magnetite_sample", 131 },
                { "malachite_vase", 133 },
                { "toolbox", 135 },
                { "ivory_d3_dice", 136 },
                { "heart_5m", 143 },
                { "mysterious_cassette", 145 },
                { "porcelain_mirror", 146 },
                { "porcelain_scissor", 148 },
                { "copper_loudspeaker", 137 },
                { "silver_loudspeaker", 138 },
                { "golden_loudspeaker", 139 },
                { "porcelain_sword", 144 },
                { "ancient_scripture", 212 },
                { "leaning_tower_model", 100 },
                { "luoyang_shovel", 151 },
                { "wooden_fish", 156 },
                { "clay_ball", 155 },
                { "bamboo_wine", 154 },
                { "shuugi", 168 },
                { "ruler", 140 },
                { "weight", 169 },
                { "babbling_book", 170 },
                { "sutra", 171 },
                { "bamboo_shoot", 152 },
                { "chainsticks", 176 },
                { "cinnabar_pen", 177 },
                { "lightning_amulet", 153 },
                { "palm_fan", 198 },
                { "four_color_gem", 208 },
                { "crystal_bookmark", 199 },
                { "mechanical_orizulu", 200 },
                { "pruner", 205 },
                { "observer", 235 },
                { "golden_scythe", 238 },
                { "indigo_pen", 239 },
                { "lightning_ball", 232 },
                { "bounty_list", 157 },
                { "treasure_map", 158 },
                { "prayer_beads", 159 },
                { "bone_dragon_ball", 162 },
                { "gem_dragon_ball", 160 },
                { "shamisen", 128 },
                { "bone_dragon_amulet", 163 },
                { "totsuka_blade", 165 },
                { "yasakani_magatama", 166 },
                { "yata_mirror", 167 },
                { "geta", 204 },
                { "bamboo_ticket", 230 },
                { "antidragon_book", 231 },
                { "honeysuckle", 189 },
                { "lantana", 192 },
                { "forget_me_not", 190 },
                { "daffodil", 195 },
                { "fig", 194 },
                { "china_rose", 196 },
                { "bellflower", 193 },
                { "water_lily", 191 },
                { "datura", 206 },
                { "dandelion", 207 },
                { "violet_goat_horn", 173 },
                { "malicious_spray", 178 },
                { "mini_tomb", 179 },
                { "ghost_fulu", 175 },
                { "iron_hen", 174 },
                { "monster_birth_certificate", 183 },
                { "mysterious_flesh_ball", 201 },
                { "corrupted_crystal_ball", 202 },
                { "essence_pot", 209 },
                { "silver_dog_leash", 210 },
                { "rainbow_cheese", 211 },
                { "unlucky_four", 216 },
                { "bottomless_bag", 217 },
                { "teppoudama", 229 },
                { "3d_glasses", 185 },
                { "ink_bottle", 186 },
                { "bloody_film_roll", 187 },
                { "misericorde", 197 },
                { "burning_pact", 237 },
                { "travel_bag", 74 },
                { "star_ring", 214 },
                { "ouroboros_neckless", 213 },
                { "light_pyramid", 215 },
                { "heat_jade", 219 },
                { "blue_bag", 218 },
                { "red_bag", 221 },
                { "cloud_fulu", 222 },
                { "drawstring", 223 },
                { "teleporting_orizulu", 220 },
                { "spiky_bag", 224 },
                { "bag_of_marbles", 225 },
                { "neon_bag", 227 },
                { "gem_backpack", 228 },
                { "porcelain_fish", 147 },
                { "porcelain_spear", 130 },
                { "bodhi_seed", 234 },
                { "mysterious_scroll", 188 },
                { "diabolo", 240 },
                { "amethyst_amulet", 236 },
                { "spring_arm", 243 },
                { "golden_seed", 260 },
                { "tricolor_orizulu", 269 },
                { "yin_yang_nunchaku", 277 },
                { "wildcard_windvane", 278 },
                { "climbing_kit", 280 },
                { "black_hole", 121 },
                { "half_lucky_47", 77 },
                { "fallen_crystal_ball", 281 },
                { "malachite_dagger", 282 },
                { "porcelain_halberd", 283 },
                { "xiaoqian_orizulu", 285 },
                { "daqian_orizulu", 284 },
                { "copper_lion", 286 },
                { "big_encyclopedia", 279 },
                { "trinity_force", 288 },
                { "ghost_of_five_fulu", 289 },
                { "recycling_bin", 247 },
                { "drift_bottle", 248 },
                { "golden_thumb_ring", 255 },
                { "wooden_tower", 254 },
                { "copper_thumb_ring", 249 },
                { "jade_thumb_ring", 259 },
                { "bamboo_burin", 256 },
                { "saw", 250 },
                { "warped_fungus", 258 },
                { "paint_bucket", 332 },
                { "fishing_basket", 257 },
                { "burnt_amulet", 262 },
                { "emerald_wood_bracelet", 268 },
                { "branch_of_yggdrasil", 264 },
                { "cheating_weight", 289 },
                { "scarlet_core", 292 },
                { "brocade_orizulu", 299 },
                { "bicolor_feather", 291 },
                { "rust_crook", 290 },
                { "rust_copper_ring", 304 },
                { "scarlet_kaleidoscope", 303 },
                { "scarlet_hourglass", 296 },
                { "rust_chest", 301 },
                { "rust_axe", 302 },
                { "dottled_pig", 297 },
                { "soul_flag", 305 },
                { "soul_scythe", 306 },
                { "yin_yang_mirror", 308 },
                { "yin_yang_butterfly", 307 },
                { "soul_bottle", 310 },
                { "golden_bell", 313 },
                { "yin_yang_jade_flute", 311 },
                { "blue_candle", 320 },
                { "rosemary", 321 },
                { "brown_sugar", 322 },
                { "gilded_lion_bowl", 323 },
                { "ice_blade", 324 },
                { "meteorite_knife", 325 },
                { "water_pattern_pouch", 329 },
                { "cake_knife", 327 },
                { "lucky_cookie", 318 },
                { "altar", 316 },
                { "fire_pattern_pouch", 330 },
                { "taotie_pact", 326 },
                { "copper_lock", 336 },
                { "silver_lock", 337 },
                { "golden_lock", 338 },
                { "magnifier", 339 },
                { "purple_gourd", 340 },
                { "dart", 341 },
                { "bamboo_book", 348 },
                { "wither_amulet", 349 },
                { "forbidden_amulet", 350 },
                { "stone_sword", 351 },
                { "battle_drum", 352 },
                { "bicolor_silk", 353 },
                { "hufu", 354 },
                { "coin_amulet", 355 },
                { "unknown_amulet", 356 },
                { "pair_magnet", 357 },
                { "twisting_device", 358 },
                { "gearbox", 380 },
                { "tinfoil_hat", 359 },
                { "conveyor_belt", 360 },
                { "classified_blueprint", 361 },
                { "photovoltaic_panel", 362 },
                { "network_switch", 363 },
                { "molecular_reconstructor", 379 },
                { "fiber_optic", 378 },
                { "blackbody", 364 },
                { "triangular_lamp_post", 365 },
                { "green_screen", 366 },
                { "scrap_bin", 367 },
                { "stepper_motor", 368 },
                { "pie_magnet", 369 },
                { "overclock_module", 370 },
                { "workshop_manual", 371 },
                { "lever", 372 },
                { "three_prong_terminal_strip", 373 },
                { "mechanical_d9", 374 },
                // 图集位置与存档编号独立，堂鼓位于 382，皮纳塔位于 383。
                { "pinata", 383 },
                { "tanggu", 382 },
                { "long_bearing", 375 },
                { "ecosphere", 376 },
                { "mechanical_pouch", 377 },
                { "parasite", 381 },
            });

        private static ArtifactRegistration Register(string id, System.Func<Artifact> factory) =>
            new ArtifactRegistration(id, factory);
    }
}
