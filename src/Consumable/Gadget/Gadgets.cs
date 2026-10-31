using System.Linq;

namespace Aotenjo
{
    public class Gadgets
    {
        public static Gadget Rice => new RiceGadget();
        public static Gadget Sticker => new StickerGadget();
        public static Gadget RedMarker => new RedMarkerGadget();
        public static Gadget Chisel => new ChiselGadget();
        public static Gadget Magnet => new MagnetGadget();
        public static Gadget InkBrush => new InkBrushGadget();
        public static Gadget Stimulant => new StimulantGadget();
        public static Gadget Whisk => new WhiskGadget();

        public static Gadget TaiChiScroll => new TaiChiScrollGadget();

        public static Gadget MiniHammer => new MiniHammerGadget();

        public static Gadget MiniBrush => new MiniBrushGadget();

        public static Gadget Tweezer => new TweezerGadget();

        public static Gadget Eraser => new EraserGadget();

        public static Gadget SnakeEye => new SnakesEyeGadget();

        public static Gadget SewingKit => new SewingKitGadget();

        public static Gadget Rook => new RookGadget();

        public static Gadget HairDryer => new HairDryerGadget();

        public static Gadget Camera => new CameraGadget();

        public static Gadget CloudGloves => new CloudGlovesGadget();

        public static Gadget CarrotStamp => new CarrotStampGadget();

        public static Gadget Whistle => new WhistleGadget();
        public static Gadget FishingRod => new FishingRodGadget();

        public static Gadget Weight => new WeightGadget();

        public static Gadget Plasticine => new PlasticineGadget();

        /// <summary>
        /// 按完整注册名创建仅用于说明的独立实例，不加入背包，也不执行获得或使用回调。
        /// 悬停链接还可引用奖励专属小道具和旧道具，因此不能仅用图鉴列表判断是否合法；
        /// 这些补充项只参与预览，不改变商店、图鉴和收集成就的范围。未知名称返回空值。
        /// </summary>
        public static Gadget GetPreview(string regName)
        {
            if (string.IsNullOrEmpty(regName)) return null;
            return regName switch
            {
                "order_mirror" => new OrderMirrorGadget(),
                "time_shard" => new TimeShardGadget(),
                "dandelion_seed_1" => new DandelionSeedGadget(1),
                "dandelion_seed_2" => new DandelionSeedGadget(2),
                "dandelion_seed_3" => new DandelionSeedGadget(3),
                "dandelion_seed_4" => new DandelionSeedGadget(4),
                "magic_brush_wan" => new MagicBrushGadget(Tile.Category.Wan),
                "magic_brush_bing" => new MagicBrushGadget(Tile.Category.Bing),
                "magic_brush_suo" => new MagicBrushGadget(Tile.Category.Suo),
                _ => GadgetCompleteList().FirstOrDefault(g => g.regName == regName)
            };
        }

        public static Gadget[] GadgetList(Player player, bool inShop, bool unlockAll = false) => new[]
        {
            Rice,
            RedMarker,
            Chisel,
            Sticker,
            Magnet,
            InkBrush,
            Stimulant,
            Whisk,
            TaiChiScroll,
            MiniHammer,
            MiniBrush,
            Tweezer,
            Eraser,
            SnakeEye,
            SewingKit,
            Rook,
            HairDryer,
            Camera,
            CloudGloves,
            CarrotStamp,
            Whistle,
            FishingRod,
            Weight,
            Plasticine
        }.Where(g => unlockAll || g.CanObtainBy(player, inShop)).ToArray();

        public static Gadget[] GadgetCompleteList() => new[]
        {
            Rice,
            RedMarker,
            Chisel,
            Sticker,
            Magnet,
            InkBrush,
            Stimulant,
            Whisk,
            TaiChiScroll,
            MiniHammer,
            MiniBrush,
            Tweezer,
            Eraser,
            SnakeEye,
            SewingKit,
            Rook,
            HairDryer,
            Camera,
            CloudGloves,
            CarrotStamp,
            Whistle,
            FishingRod,
            Weight,
            Plasticine
        };
    }
}
