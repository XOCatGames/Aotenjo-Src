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
            Weight
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
            Weight
        };
    }
}